mergeInto(LibraryManager.library, {
    onUnityLoaded: function () {
        if (typeof window.onUnityLoaded === "function") {
            window.onUnityLoaded();
        }
    }
});