window.downloadCsv = (fileName, csvContent) => {
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
};

window.speakEn = (text) => {
    if (!text || !('speechSynthesis' in window)) return false;
    try {
        window.speechSynthesis.cancel();
        const u = new SpeechSynthesisUtterance(text);
        u.lang = 'en-US';
        u.rate = 0.95;
        u.pitch = 1;
        const voices = window.speechSynthesis.getVoices();
        const enVoice = voices.find(v => /^en(-|_|$)/i.test(v.lang));
        if (enVoice) u.voice = enVoice;
        window.speechSynthesis.speak(u);
        return true;
    } catch {
        return false;
    }
};

window.copyTextSync = (text) => {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.setAttribute('readonly', '');
    textarea.style.position = 'fixed';
    textarea.style.top = '0';
    textarea.style.left = '0';
    textarea.style.width = '1px';
    textarea.style.height = '1px';
    textarea.style.opacity = '0';
    document.body.appendChild(textarea);
    textarea.focus();
    textarea.select();
    textarea.setSelectionRange(0, text.length);

    let ok = false;
    try {
        ok = document.execCommand('copy');
    } catch {
        ok = false;
    }
    document.body.removeChild(textarea);

    if (navigator.clipboard && window.isSecureContext) {
        try { navigator.clipboard.writeText(text); } catch { /* ignore */ }
    }
    return ok;
};

window.copyPromptFromButton = (btn) => {
    const target = btn.closest('.help-section')?.querySelector('.help-prompt');
    const text = target ? target.innerText : '';
    const ok = window.copyTextSync(text);
    if (ok) {
        const original = btn.dataset.originalText || btn.textContent;
        btn.dataset.originalText = original;
        btn.textContent = 'Copied!';
        clearTimeout(btn._copyTimer);
        btn._copyTimer = setTimeout(() => { btn.textContent = original; }, 1500);
    }
    return ok;
};
