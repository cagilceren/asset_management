for (const [key, value] of Object.entries(soldTransactions)) {
	let value2 = unsoldTransactions[key];
	let data = value.map(x => [x.SoldAmount, x.BuyDate.substring(0,10), x.SellDate.substring(0,10)]).concat(
		value2.map(x => [x.RemainingAmount, x.Date.substring(0, 10), new Date().toISOString().substring(0,10)]));
	let minDate = data.map(x => x[1]).sort()[0];
	newWaterflowChart(`custom-chart-${key}`, data, minDate);
	pieChartHelper(`pie-chart-${key}`, [taxed[key], untaxed[key]], minDate);
}

function pieChart(id, data, labels = ["steuerpflichtig", "steuerfrei"]) {
	return new Chart(id, { type: 'doughnut', data: {
		labels: labels,
		datasets: data
	} });
}

function pieChartHelper(id, data) {
	return pieChart(id, [{ data: data,  backgroundColor: ['#3259ca', '#21306C']}])
}

pieChartHelper("pie-chart", [totalValues[1].Profits, totalValues[0].Profits - totalValues[1].Profits]);

let toDisplayArray = values => [values.Profits, -values.Losses, values.Fees, values.Income, values.Expenditures];

let barChart = new Chart("bar-chart", {
	type: 'bar',
	data: {
		labels: ['Gewinne', 'Verluste', 'Gebühren', 'Einkommen', 'Ausgaben'],
		datasets: [{ data: toDisplayArray(totalValues[0]), backgroundColor: ["#4e73df", "#e74a3b", "#f6c23e"] } ]
	},
	options: {
		scales: {
			y: {
				beginAtZero: true
			}
		},
		plugins: {
			legend: {
				display: false
			}
		}
	}
});

let taxedDataBarChart = false;
document.getElementById("switch-bar-chart").addEventListener("input", function(ev) {
	taxedDataBarChart = !taxedDataBarChart;
	if (taxedDataBarChart)
		barChart.data.datasets[0].data = toDisplayArray(totalValues[1]);
	else
		barChart.data.datasets[0].data = toDisplayArray(totalValues[0]);
	barChart.update();
})
