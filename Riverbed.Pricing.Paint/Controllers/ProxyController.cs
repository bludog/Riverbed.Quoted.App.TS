using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Riverbed.Pricing.Paint.Controllers
{
    [ApiController]
    [Route("api/proxy")] 
    public class ProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /api/proxy/pdf?url={encodedUrl}
        [HttpGet("pdf")]
        public async Task<IActionResult> ProxyPdf([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Missing url parameter.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return BadRequest("Invalid URL.");

            if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only http/https URLs are allowed.");
            }

            try
            {
                var http = _httpClientFactory.CreateClient();

                HttpResponseMessage? result = null;

                if (IsOneDriveShare(uri))
                {
                    // Try to expand short link and build direct download URL
                    var expanded = await ExpandOneDriveShareAsync(http, uri) ?? uri;
                    var dl = BuildOneDriveDownloadUriFromLive(expanded);
                    if (dl != null)
                    {
                        result = await TryFetchAsync(http, dl);
                    }

                    // Fallback to API shares endpoint if needed
                    if (result == null || !result.IsSuccessStatusCode)
                    {
                        var direct = BuildOneDriveDirectContentUri(uri);
                        if (direct != null)
                        {
                            result = await TryFetchAsync(http, direct);
                        }
                    }
                }
                else if (IsGoogleDriveShare(uri))
                {
                    // First, try using the provided converter to a direct download link
                    try
                    {
                        var directLink = ToDirectGoogleDownloadLink(uri.ToString());
                        result = await TryFetchAsync(http, new Uri(directLink));
                    }
                    catch
                    {
                        // ignore and try fallback below
                    }

                    // Fallback to drive.usercontent direct endpoint using extracted fileId
                    if (result == null || !result.IsSuccessStatusCode)
                    {
                        var fileId = TryGetGoogleDriveFileId(uri);
                        if (!string.IsNullOrWhiteSpace(fileId))
                        {
                            var googleDirect = BuildGoogleDriveDirectContentUri(fileId!);
                            result = await TryFetchAsync(http, googleDirect);
                        }
                    }
                }

                // Fallback: try original URL
                result ??= await TryFetchAsync(http, uri);
                if (result == null)
                {
                    return BadRequest("Failed to fetch remote resource (TLS/connection).");
                }

                if (!result.IsSuccessStatusCode)
                {
                    return StatusCode((int)result.StatusCode, $"Upstream returned {(int)result.StatusCode}");
                }

                // Buffer content
                var bytes = await result.Content.ReadAsByteArrayAsync();
                var upstreamContentType = result.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                Response.Headers["X-Proxy-Upstream-ContentType"] = upstreamContentType;

                // Only serve as PDF if it actually looks like a PDF
                if (!LooksLikePdf(bytes))
                {
                    return File(bytes, upstreamContentType);
                }

                // It's a PDF. Normalize content type to application/pdf
                Response.Headers["Content-Disposition"] = "inline; filename=document.pdf";
                return File(bytes, "application/pdf");
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Failed to fetch remote PDF: {ex.Message}");
            }
        }

        private static async Task<HttpResponseMessage?> TryFetchAsync(HttpClient http, Uri uri)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                req.Headers.UserAgent.ParseAdd("RiverbedProxy/1.0");
                req.Headers.Accept.ParseAdd("application/pdf,application/octet-stream;q=0.9,*/*;q=0.8");
                var res = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                return res;
            }
            catch
            {
                if (string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var builder = new UriBuilder(uri) { Scheme = Uri.UriSchemeHttp, Port = -1 };
                        var httpUri = builder.Uri;
                        var req2 = new HttpRequestMessage(HttpMethod.Get, httpUri);
                        req2.Headers.UserAgent.ParseAdd("RiverbedProxy/1.0");
                        req2.Headers.Accept.ParseAdd("application/pdf,application/octet-stream;q=0.9,*/*;q=0.8");
                        var res2 = await http.SendAsync(req2, HttpCompletionOption.ResponseHeadersRead);
                        return res2;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        private static bool IsOneDriveShare(Uri uri)
        {
            var host = uri.Host.ToLowerInvariant();
            return host == "1drv.ms" || host == "onedrive.live.com";
        }

        private static Uri? BuildOneDriveDirectContentUri(Uri shareUri)
        {
            try
            {
                var original = shareUri.GetLeftPart(UriPartial.Path) + shareUri.Query; // drop fragment
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(original))
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
                var shareId = $"u!{base64}";
                var api = $"https://api.onedrive.com/v1.0/shares/{shareId}/root/content";
                return new Uri(api);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<Uri?> ExpandOneDriveShareAsync(HttpClient http, Uri shareUri)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, shareUri);
                req.Headers.UserAgent.ParseAdd("RiverbedProxy/1.0");
                using var res = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                return res.RequestMessage?.RequestUri; // final URI after redirects
            }
            catch
            {
                return null;
            }
        }

        private static Uri? BuildOneDriveDownloadUriFromLive(Uri finalUri)
        {
            if (!string.Equals(finalUri.Host, "onedrive.live.com", StringComparison.OrdinalIgnoreCase))
                return null;

            var query = System.Web.HttpUtility.ParseQueryString(finalUri.Query);
            var resid = query.Get("resid");
            var authkey = query.Get("authkey");
            if (string.IsNullOrWhiteSpace(resid) || string.IsNullOrWhiteSpace(authkey))
                return null;

            var download = $"https://onedrive.live.com/download?resid={Uri.EscapeDataString(resid)}&authkey={Uri.EscapeDataString(authkey)}&em=2";
            return new Uri(download);
        }

        // Google Drive helpers
        private static bool IsGoogleDriveShare(Uri uri)
        {
            var h = uri.Host.ToLowerInvariant();
            return h.Contains("drive.google.com") || h.Contains("docs.google.com");
        }

        private static string? TryGetGoogleDriveFileId(Uri uri)
        {
            // Patterns: /file/d/{id}/, open?id=, uc?id=
            var path = uri.AbsolutePath.Trim('/');
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var idx = Array.IndexOf(segments, "d");
            if (segments.Length >= 3 && segments[0] == "file" && idx == 1)
            {
                return segments[2];
            }
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var id = query.Get("id");
            if (!string.IsNullOrWhiteSpace(id)) return id;
            return null;
        }
        
        public static string ToDirectGoogleDownloadLink(string googleDriveUrl)
        {
            if (string.IsNullOrWhiteSpace(googleDriveUrl))
                throw new ArgumentException("URL cannot be null or empty.", nameof(googleDriveUrl));

            // Example format: https://drive.google.com/file/d/FILE_ID/view?usp=sharing
            var parts = googleDriveUrl.Split(new[] { "/d/", "/view" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new FormatException("Invalid Google Drive URL format.");

            string fileId = parts[1].Split('/')[0]; // Extracts the file ID safely

            return $"https://drive.google.com/uc?export=download&id={fileId}";
        }
        

        private static Uri BuildGoogleDriveDirectContentUri(string fileId)
        {
            // drive.usercontent endpoint serves raw bytes for public files
            var url = $"https://drive.usercontent.google.com/uc?id={Uri.EscapeDataString(fileId)}&export=download";
            return new Uri(url);
        }

        private static bool LooksLikePdf(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5) return false;
            var maxScan = Math.Min(bytes.Length, 1024);
            for (int i = 0; i < maxScan - 4; i++)
            {
                if (bytes[i] == (byte)'%' && bytes[i + 1] == (byte)'P' && bytes[i + 2] == (byte)'D' && bytes[i + 3] == (byte)'F' && bytes[i + 4] == (byte)'-')
                {
                    // Optionally check for %%EOF near the end
                    var tailScan = Math.Min(bytes.Length, 2048);
                    for (int j = bytes.Length - 6; j >= bytes.Length - tailScan && j >= 0; j--)
                    {
                        if (bytes[j] == (byte)'%' && bytes[j + 1] == (byte)'%' && bytes[j + 2] == (byte)'E' && bytes[j + 3] == (byte)'O' && bytes[j + 4] == (byte)'F')
                        {
                            return true;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
