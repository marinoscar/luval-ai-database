window.disableEl = function (elId) {
    var el = document.getElementById(elId);
    el.setAttribute('disabled', 'disabled');
}

window.plotChart = function (elId, jsonConfig) {
    try {
        var config = JSON.parse(jsonConfig);
        var chart = new Chart(document.getElementById(elId), config);
    } catch (error) {
        console.error(error);
        alert(error);
    }
}