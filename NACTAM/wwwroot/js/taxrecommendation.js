/**
 * js code to create chart.js and datatables in TaxRecommendationView. 
 * @author Cagil Ceren Aslan
 */


$(document).ready(function(){
	const backgroundColors = ["#9a71f6", "#e83e8c", "#f6c23e", "#20c9a6"];
	const hoverBackgroundColors = ["#cda5ff", "#ff6fa8", "#ffd966", "#83f6d8"];
	const saleProfitCtx = document.getElementById("saleProfit");
	const othersProfitCtx = document.getElementById("othersProfit");

	// saleProfit Pie Chart
	new Chart(saleProfitCtx, {
		type: "doughnut",
		data: {
			labels: ["sonstige Verkäufe", "Kryptoverkäufe", isInFreeLimitSale ? "Frei" : "Überschritten"],
			datasets: [
				{
					label: 'EUR',
					data: saleProfitChartData,
					backgroundColor: [ "#9a71f6" , "#f6c23e", isInFreeLimitSale ? "#20c9a6" : "#e83e8c"],
					hoverBackgroundColor: ["#cda5ff", "#ffd966", isInFreeLimitSale ? "#83f6d8" : "#ff6fa8"],
					hoverBorderColor: "rgba(234, 236, 244, 1)",
				},
			],
		},
		options: {
			locale: 'de-DE',
			maintainAspectRatio: false,
			legend: {
				display: false,
			},
			cutoutPercentage: 80
		},
	});

	// othersProfit Pie Chart
	new Chart(othersProfitCtx, {
		type: "doughnut",
		data: {
			labels: ["sonstige Leistungen", "Mining", "Staking", isInFreeLimitOther ? "Frei" : "Überschritten"],
			datasets: [
				{
					label: 'EUR',
					data: othersProfitChartData,
					backgroundColor: ["#9a71f6", "#f6c23e", "#4e73df", isInFreeLimitOther ? "#20c9a6" : "#e83e8c"],
					hoverBackgroundColor: ["#cda5ff", "#ffd966", "#8fa5ff",isInFreeLimitOther ? "#83f6d8" : "#ff6fa8"],
					hoverBorderColor: "rgba(234, 236, 244, 1)",
				},
			],
		},
		options: {
			locale: 'de-DE',
			maintainAspectRatio: false,
			legend: {
				display: false,
			},
			cutoutPercentage: 80,
		},
	});

	// tax-recommendation-table

	var table = $("#tax-recommendation-table").DataTable({
		paging: false,
		select: "single",
		columns: [
			{
				className: "details-control",
				orderable: false,
				data: null,
				defaultContent: "",
				render: function () {
					return '<i class="fa fa-plus-square" aria-hidden="true"></i>';
				},
				width: "15px",
			},
			{ data: "Asset" },
			{ data: "Verkaufbare Coinmenge" },
			{ data: "Tipps" },
		],
		order: [[1, "asc"]],
	});

	$('#tax-recommendation-table tbody').on('click', 'td.details-control', function () {
		var tr = $(this).closest('tr');
		var tdi = tr.find("i.fa");
		var row = table.row(tr);
		var hiddenDivId = "#transactions" + table.row(tr).id();

		if (row.child.isShown()) {
			// This row is already open - close it
			row.child.hide();
			tr.removeClass('shown');
			tdi.first().removeClass('fa-minus-square');
			tdi.first().addClass('fa-plus-square');
		}
		else {
			// Open this row
			row.child($(hiddenDivId).clone().removeClass("hidden")).show();
			tr.addClass('shown');
			tdi.first().removeClass('fa-plus-square');
			tdi.first().addClass('fa-minus-square');
		}
	});

	table.on("user-select", function (e, dt, type, cell, originalEvent) {
		if ($(cell.node()).hasClass("details-control")) {
			e.preventDefault();
		}
	});

	// assets Chart Data
	assetsChartDataSetList.forEach(chartData => {
		var chartName = document.getElementById(chartData.chartName);
		new Chart(chartName, {
			type: "doughnut",
			data: {
				labels: chartData.labels,
				datasets: [
					{
						label: 'Coins',
						data: chartData.dataSets[0].data,
						backgroundColor: backgroundColors,
						hoverBackgroundColor: hoverBackgroundColors,
						hoverBorderColor: "rgba(234, 236, 244, 1)",
					},
				],
			},
			options: {
				locale: 'de-DE',
				maintainAspectRatio: false,
				legend: {
					display: false,
				},
				cutoutPercentage: 80,
			},
		});
	});
})

