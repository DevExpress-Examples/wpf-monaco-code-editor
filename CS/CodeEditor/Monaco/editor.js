require.config({ paths: { vs: 'vs' } });

require(['vs/editor/editor.main'], function () {

    /* ==============================
       Message Contracts
    ============================== */

    const Incoming = {
        SetText: "SetText",
        SetReadOnly: "SetReadOnly",
        MarkAsSaved: "MarkAsSaved",
        GetLanguages: "GetLanguages",
        SetLanguage: "SetLanguage",
        UpdateOption: "UpdateOption",
        RegisterTheme: "RegisterTheme",
        SetTheme: "SetTheme",
        RegisterLanguage: "RegisterLanguage"
    };

    const Outgoing = {
        TextChanged: "TextChanged",
        IsDirtyChanged: "IsDirtyChanged",
        EditorReady: "EditorReady",
        Languages: "Languages"
    };

    function send(type, payload) {
        window.chrome.webview.postMessage({ type, payload });
    }

    /* ==============================
       Editor Initialization
    ============================== */

    const editor = monaco.editor.create(
        document.getElementById('container'),
        {
            value: '',
            language: 'csharp',
            theme: 'vs-dark'
        }
    );

    //the model with wich the editor works
    const model = editor.getModel();
    let baselineVersion = model.getAlternativeVersionId();
    let lastDirtyState = false;


    // === ResizeObserver ===
    const container = document.getElementById('container');

    const resizeObserver = new ResizeObserver(() => {
        editor.layout();
    });

    resizeObserver.observe(container);

    /* =====================================
       Internal State Logic
    ===================================== */

    function updateDirtyState() {
        // inform the wpf part that the content was modified
        const currentVersion = model.getAlternativeVersionId();
        const isDirty = currentVersion !== baselineVersion;

        if (isDirty !== lastDirtyState) {

            lastDirtyState = isDirty;

            send(Outgoing.IsDirtyChanged, isDirty);
        }
    }

    let changeTimeout;

    function updateTextChanged() {
        clearTimeout(changeTimeout);

        changeTimeout = setTimeout(() => {
            send(Outgoing.TextChanged, editor.getValue());
        }, 150);

    }

    /* =====================================
       Incoming Handlers (Dispatcher)
    ===================================== */

    function handleSetText(text) {
        if (editor.getValue() !== text) {
            editor.setValue(text);
            //update scroll positions to the most left and top position
            editor.setScrollTop(0);
            editor.setScrollLeft(0);

            //Consider the new text as new and set the IsDirty flag to False.
            baselineVersion = model.getAlternativeVersionId();
            //inform WPF that IsModified has changed
            updateDirtyState()
        }
    }

    function handleSetReadOnly(value) {
        editor.updateOptions({ readOnly: value });
    }

    function handleMarkAsSaved() {
        editor.pushUndoStop();
        baselineVersion = model.getAlternativeVersionId();
        updateDirtyState();
    }

    function handleGetLanguages() {
        const langs = monaco.languages.getLanguages().map(l => l.id);;
        send(Outgoing.Languages, langs);
    }

    function handleSetLanguage(id) {
        if (!model || !id)
            return;

        if (model.getLanguageId() === id)
            return;

        monaco.editor.setModelLanguage(model, id);
    }

    function handleUpdateOption(payload) {
        if (!editor || !payload?.option)
            return;

        editor.updateOptions({
            [payload.option]: payload.value
        });
    }

    function handleRegisterTheme(payload) {
        monaco.editor.defineTheme(payload.name, {
            base: payload.base,
            inherit: payload.inherit,
            rules: payload.rules,
            colors: payload.colors ?? {}
        });
    }

    function handleSetTheme(themeName) {
        monaco.editor.setTheme(themeName);
    }

    function evaluateJsObjectLiteral(code, errorMessage) {
        try {
            return new Function(
                `"use strict"; return (${code});`
            )();
        }
        catch (e) {
            console.error(errorMessage, e);
            throw e;
        }
    }

    function handleRegisterLanguage(payload) {

        if (!payload?.id)
            return;

        const id = payload.id;

        const exists = monaco.languages
            .getLanguages()
            .some(l => l.id === id);

        if (!exists) {
            monaco.languages.register({ id });
        }

        if (payload.monarch) {
            const definition = evaluateJsObjectLiteral(
                payload.monarch,
                "Failed to evaluate Monarch"
            );

            monaco.languages.setMonarchTokensProvider(id, definition);
        }

        if (payload.configuration) {
            const config = evaluateJsObjectLiteral(
                payload.configuration,
                "Failed to evaluate configuration"
            );

            monaco.languages.setLanguageConfiguration(id, config);
        }
    }

    /*===================================================*/


    const handlers = {
        [Incoming.SetText]: handleSetText,
        [Incoming.SetReadOnly]: handleSetReadOnly,
        [Incoming.MarkAsSaved]: handleMarkAsSaved,
        [Incoming.GetLanguages]: handleGetLanguages,
        [Incoming.SetLanguage]: handleSetLanguage,
        [Incoming.UpdateOption]: handleUpdateOption,
        [Incoming.RegisterTheme]: handleRegisterTheme,
        [Incoming.SetTheme]: handleSetTheme,
        [Incoming.RegisterLanguage]: handleRegisterLanguage
    };

    model.onDidChangeContent(() => {
        updateTextChanged();
        updateDirtyState();
    });

    //listen to updates from the WPF side
    window.chrome.webview.addEventListener('message', event => {
        const msg = event.data;
        const handler = handlers[msg.type];
        if (handler) handler(msg.payload);
    });

    /*===================================================*/
    //notify that the editor has been initialized (requestAnimationFrame is required, because otherwise the editor is not ready for SetText)
    send(Outgoing.EditorReady);
    /*===================================================*/
});