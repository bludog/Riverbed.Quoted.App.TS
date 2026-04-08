// Minimal + reliable: inserts an image URL at the current (or last) cursor
window.MudExQuill = {
    attach(el) {
        // Find the Quill instance MudEx attaches under a parent via __quill
        let host = el;
        for (let i = 0; i < 5 && host && !host.__quill; i++) host = host.parentElement;
        const quill = host && host.__quill;
        if (!quill) {
            console.warn("MudExQuill.attach: no __quill instance found.");
            return;
        }
        // Track the last selection so we can insert even if focus briefly shifts
        quill.on("selection-change", range => { el._lastRange = range; });
    },

    insertImageAtCursor(el, url) {
        let host = el;
        for (let i = 0; i < 5 && host && !host.__quill; i++) host = host.parentElement;
        const quill = host && host.__quill;
        if (!quill) {
            console.warn("MudExQuill.insertImageAtCursor: no __quill instance found.");
            return;
        }
        if (!/^https?:\/\//i.test(url)) {
            alert("Enter a valid http/https image URL.");
            return;
        }
        // Prefer current selection; fall back to last known; else append at end
        let range = quill.getSelection(true) || el._lastRange || { index: quill.getLength(), length: 0 };
        quill.insertEmbed(range.index, "image", url, "user");
        quill.setSelection(range.index + 1, 0, "user");
    }
};
