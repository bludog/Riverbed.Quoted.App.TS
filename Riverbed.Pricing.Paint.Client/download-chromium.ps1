$chromiumUrl = "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win_x64%2F1495859%2Fchrome-win.zip?generation=1754107546001986&alt=media"
$destinationFolder = "Chromium"
$zipFile = "chromium.zip"

# Create destination folder if it doesn't exist
if (!(Test-Path $destinationFolder)) {
    New-Item -ItemType Directory -Path $destinationFolder | Out-Null
}

# Download Chromium zip
Write-Host "Downloading Chromium from $chromiumUrl..."
Invoke-WebRequest -Uri $chromiumUrl -OutFile $zipFile

# Extract zip to destination folder
Write-Host "Extracting Chromium..."
Expand-Archive -Path $zipFile -DestinationPath $destinationFolder -Force

# Move chrome.exe to the root of the Chromium folder
$chromeExe = Join-Path $destinationFolder "chrome-win64\chrome.exe"
if (Test-Path $chromeExe) {
    Move-Item -Path $chromeExe -Destination $destinationFolder -Force
}

# Clean up
Remove-Item $zipFile
Remove-Item -Recurse -Force (Join-Path $destinationFolder "chrome-win64")

Write-Host "Chromium downloaded and ready at $destinationFolder\chrome.exe"