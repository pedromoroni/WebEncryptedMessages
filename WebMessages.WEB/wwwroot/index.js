window.idb = {
    setItem: async (storeName, keyName, value) => {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open("MyKeyDb", 1);

            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: "KeyName" });
                }
            };

            request.onsuccess = (event) => {
                const db = event.target.result;
                const tx = db.transaction(storeName, "readwrite");
                const store = tx.objectStore(storeName);
                store.put({ KeyName: keyName, Value: value });
                tx.oncomplete = () => resolve(true);
                tx.onerror = (e) => reject(e);
            };

            request.onerror = (e) => reject(e);
        });
    },

    getItem: async (storeName, keyName) => {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open("MyKeyDb", 1);

            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: "KeyName" });
                }
            };

            request.onsuccess = (event) => {
                const db = event.target.result;
                const tx = db.transaction(storeName, "readonly");
                const store = tx.objectStore(storeName);
                const getRequest = store.get(keyName);

                getRequest.onsuccess = () => resolve(getRequest.result?.Value ?? null);
                getRequest.onerror = (e) => reject(e);
            };

            request.onerror = (e) => reject(e);
        });
    }
};

window.deviceInfo = {
    getDeviceName: () => {
        return navigator.userAgent;
    }
};
