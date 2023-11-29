function openStreamInNewWindow(bytesBase64, fileName) {
    var mimeType = "application/pdf";
    var data = window.atob(bytesBase64);
    var bytes = new Uint8Array(data.length);
    for (var i = 0; i < data.length; i++) {
        bytes[i] = data.charCodeAt(i);
    }
    const blob = new Blob([bytes.buffer], { type: mimeType });
    const url = URL.createObjectURL(blob);

    const newWindow = window.open(url, '_blank');
    newWindow.document.write(`<html><head><title>${fileName}</title></head><body><embed width="100%" height="100%" src="${url}" type="${mimeType}"></body></html>`);
    newWindow.document.close();
}

//function saveAsFile(filename, bytesBase64) {
//    if (navigator.msSaveBlob) {
//        console.log("GOOD");
//        //Download document in Edge browser
//        var data = window.atob(bytesBase64);
//        var bytes = new Uint8Array(data.length);
//        for (var i = 0; i < data.length; i++) {
//            bytes[i] = data.charCodeAt(i);
//        }
//        var blob = new Blob([bytes.buffer], { type: "application/octet-stream" });
//        navigator.msSaveBlob(blob, filename);
//    }
//}

function saveAsFile(filename, bytesBase64) {
    var data = window.atob(bytesBase64);
    var arrayBuffer = new ArrayBuffer(data.length);
    var uint8Array = new Uint8Array(arrayBuffer);

    for (var i = 0; i < data.length; i++) {
        uint8Array[i] = data.charCodeAt(i);
    }

    var blob = new Blob([arrayBuffer], { type: "application/octet-stream" });

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        // Internet Explorer
        window.navigator.msSaveOrOpenBlob(blob, filename);
    } else {
        // Other browsers
        var objectUrl = window.URL.createObjectURL(blob);
        var link = document.createElement('a');
        link.href = objectUrl;
        link.download = filename;

        // Simule un clic sur le lien pour déclencher le téléchargement
        document.body.appendChild(link);
        link.click();

        // Nettoie l'élément 'a' après le téléchargement
        document.body.removeChild(link);
        window.URL.revokeObjectURL(objectUrl);
    }
}


function openFilesInput() {
    document.getElementById("ChampPiecesJointes");
}