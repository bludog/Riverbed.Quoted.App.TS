function resolveQuill(host) {
    return host?.querySelector?.('.ql-editor')?.__quill ?? null;
}

export async function waitForQuill(host, timeoutMs = 10000) { // give it more time
    const start = Date.now();
    while (!resolveQuill(host)) {
        await new Promise(r => setTimeout(r, 50));
        if (Date.now() - start > timeoutMs) return false;
    }
    return true;
}

export function getSelectionIndex(host) {
    const q = resolveQuill(host);
    const r = q?.getSelection();
    return (r && typeof r.index === 'number') ? r.index : null;
}

export function getDocEndIndex(host) {
    const q = resolveQuill(host);
    if (!q) return 0;
    const len = q.getLength();
    return Number.isFinite(len) ? Math.max(0, len - 1) : 0;
}

export function setSelectionIndex(host, index) {
    const q = resolveQuill(host);
    if (!q) return;
    const i = Number.isFinite(index) ? index : 0;
    q.setSelection(i, 0, 'silent');
}
