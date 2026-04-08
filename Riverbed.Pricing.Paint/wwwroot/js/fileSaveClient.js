// Extended File Save Functionality for Blazor
(function () {
    window.FileSaveExtended = {
        downloadFile: function (filename, data, mimeType) {
            if (!mimeType) {
                mimeType = 'application/octet-stream';
            }

            let bytes;
            if (typeof data === 'string') {
                bytes = new TextEncoder().encode(data);
            } else if (data instanceof Uint8Array) {
                bytes = data;
            } else if (Array.isArray(data)) {
                bytes = new Uint8Array(data);
            } else {
                console.error('Unsupported data type for file download');
                return;
            }

            const blob = new Blob([bytes], { type: mimeType });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            link.style.display = 'none';

            document.body.appendChild(link);
            link.click();

            setTimeout(function () {
                document.body.removeChild(link);
                URL.revokeObjectURL(url);
            }, 100);
        }
    };
})();