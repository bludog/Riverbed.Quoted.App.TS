using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Reports.Utils
{
    public class FileSaveHelper
    {
        private readonly IJSRuntime _jsRuntime;

        public FileSaveHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SaveFileAsync(string fileName, byte[] fileContent, string mimeType)
        {
            var base64 = Convert.ToBase64String(fileContent);
            await _jsRuntime.InvokeVoidAsync("saveFile", fileName, base64, mimeType);
        }
    }
}
