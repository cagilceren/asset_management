/**
 * js code to create chart.js in Dashboard. 
 * @author Cagil Ceren Aslan
 */

var ctx = document.getElementById("finance-overview");
new Chart(ctx, {
	type: 'bar',
	data: {
		labels: ["Jan", "Feb", "März", "Apr", "Mai", "Jun", "Jul", "Aug", "Sept", "Okt", "Nov", "Dez"],
		datasets: [{
			label: "Einkommen",
			backgroundColor: "#a6b9ef",
			hoverBackgroundColor: "#b8c7f2",
			borderColor: "#a6b9ef",
			stack: 'Stack 0',
			data: incomeData,
		},
		{
			label: "Ausgaben",
			backgroundColor: "#4e73df",
			hoverBackgroundColor: "#5f81e2",
			borderColor: "#4e73df",
			stack: 'Stack 1',
			data: expensesData,
		},
		{
			label: "Nettoeinkommen",
			backgroundColor: "#27396f",
			hoverBackgroundColor: "#36509c",
			borderColor: "#36509c",
			stack: 'Stack 2',
			data: netEarningsData,
		}],
	},
	options: {
		responsive: true,
		locale: 'de-DE',
		maintainAspectRatio: false,
		plugins: {
			tooltip: {
				callbacks: {
					label: function(context) {
						let label = context.dataset.label || '';

						if (label) {
							label += ': ';
						}
						if (context.parsed.y !== null) {
							label += new Intl.NumberFormat('de-DE', { style: 'currency', currency: 'EUR' }).format(context.parsed.y);
						}
						return label;
					}
				}
			}
		},
		layout: {
			padding: {
			left: 10,
			right: 25,
			top: 25,
			bottom: 0
			}
		},
		scales: {
			x: {
				time: {
					unit: 'month'
				},
				grid: {
					color: "rgb(234, 236, 244)",
					zeroLineColor: "rgb(234, 236, 244)",
				},
				ticks: {
					maxTicksLimit: 24
				},
				maxBarThickness: 25,
			},
			y: {
				stacked: true,
				ticks: {
					min: 0,
					max: 15000,
					padding: 10,
					maxTicksLimit: 5,
					callback: function(data) {
						return data + " €";
					},
				},
				grid: {
					color: "rgb(234, 236, 244)",
					zeroLineColor: "rgb(234, 236, 244)",
					maxTicksLimit: 20,
				}
			},
		},
	}
});