const relationshipCtx = document.getElementById("relationshipCtx");
const currencyCtx = document.getElementById("currencyCtx");

/// <summary>
/// Currency chart, at beginning with everything 0
/// Authornames: Marco Lembert
/// </summary>
var currencyChart = new Chart(currencyCtx, {
    type: "line",
    data: {
        labels: generateLast12DaysLabels(),
        datasets: [{
            label: 'Währung',
            data: [0, 0, 0,0, 0, 0,0, 0, 0,0, 0, 0],
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
        }]
    }
})

/// <summary>
/// Method for removing error message after possible coingecko error
/// Authornames: Marco Lembert
/// </summary>
function dismissError() {
    var errorDiv = document.querySelector(".alert.alert-danger");
    if (errorDiv) {
        errorDiv.style.display = "none"; // Verberge die Fehlermeldung
    }
}

/// <summary>
/// Generates last 12 dates, used for currencychart e.g 1.1, 1.2,...
/// Authornames: Marco Lembert
/// </summary>
function generateLast12DaysLabels() {
    const labels = [];
    const today = new Date();
  
    for (let i = 11; i >= 0; i--) {
      const date = new Date(today);
      date.setDate(today.getDate() - i);
      const day = date.getDate();
      const month = date.getMonth() + 1;
      const formattedDate = `${day < 10 ? '0' : ''}${day}.${month < 10 ? '0' : ''}${month}`;
      labels.push(formattedDate);
    }
  
    return labels;
  }
  

/// <summary>
/// This method is called after selecting currency to show data into chart
/// This method gets data from controller and updates canvas
/// Authornames: Marco Lembert
/// </summary>
/// <param name="value">Currency name</param>
function updateCanvas(value) {
    currencyChart.data.datasets[0].label = value;

    // get chart data
    $.ajax({
        url: '/Transaction/CurrencyChart/',
        type: "GET",
        data: {currency: value},
        success: function(data) {
            console.log(data)
            currencyChart.data.datasets[0].data = data;

            currencyChart.update();
        },
        error: function(error) {
            
        }
    })
}

/// <summary>
/// This is needed for searching input from assets table
/// Authornames: Marco Lembert
/// </summary>
$(document).ready(function () {
    $('#assets-table').DataTable({
        "paging": false,
        "scrollY": "500px",
        "scrollCollapse": true,
        initComplete: function () {

        }
    });
});
