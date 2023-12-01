/**
 * PDF generation with jspdf and 
 * chart generation with Chart.js
 * 
 * @author Mervan Kilic
 */

var jpt;

var downloadButton = document.getElementById("generate_pdf");
var attrprofit = profit;
var attrloss = loss;
var attrfee = fee;
var attrpfs = profitfromsales;
var attryear = year;
var attrcc = cryptocurrency;
var attrtaxfreeprofit = taxfreeprofit;
var attrtaxfreeloss = taxfreeloss;
var attrtaxfreefee = taxfreefee;
var attrtaxliableprofit = taxliableprofit;
var attrtaxliableloss = taxliableloss;
var attrtaxliablefee = taxliablefee;
var attrdate = creationdate;
var attrtaxation = taxationcount;
var attrpdfprofit = pdfprofit;
var attrpdfloss = pdfloss;
var attrpdffee = pdffee; 
var attrmining = mining;
var attrstaking = staking;
var attrsalelimit = salelimit;
var attrotherslimit = otherslimit;

function showErrorBanner(){
	let el = document.querySelector('#error-banner-1');
	el.style.transform = "scaleY(1)";
	el.style.height = "auto";

}


//PDF generation
downloadButton.addEventListener("click", async () => {

	if (attryear === "0"){
		showErrorBanner();
	}
	//create pdf
	var doc = new jsPDF('p', 'pt', 'a4');
	doc.addFont('Helvetica', 'Helvetica', 'normal');
	doc.addFont('Helvetica', 'Helvetica', 'bold');
	//title
	doc.setFont("Helvetica", "bold");
	doc.setFontSize(28);
	var title = "Steuerbericht " + attryear;
	doc.text(title, 40, 50);

	//date
	doc.setFont("Helvetica", "normal");
	doc.setFontSize(11);
	doc.setTextColor('#5E5E5E');
	const date = "Vom 01.01." + attryear + " bis 31.12." + attryear;
	doc.text(date, 40, 75);

	doc.line(40, 110, 555, 110, 'F');

	//rects
	doc.setFillColor(152, 251, 152);
	doc.roundedRect(40, 120, 255, 60, 6, 6, 'F');
	doc.setFillColor(240, 128, 128);
	doc.roundedRect(300, 120, 255, 60, 6, 6, 'F');
	doc.roundedRect(40, 185, 255, 60, 6, 6, 'F');
	var num = parseInt(attrpfs);
	if(num >= 0) {
		doc.setFillColor(152, 251, 152);
	}
	doc.roundedRect(300, 185, 255, 60, 6, 6, 'F');

	//rect content
	doc.setTextColor('#000000');
	doc.text("Gewinne", 50, 135);
	doc.text("Verluste", 310, 135);
	doc.text("Gebühren", 50, 200);
	doc.text("Gesamtgewinne aus Verkäufen", 310, 200);

	//rect data
	doc.setFont("Helvetica", "bold");
	doc.setFontSize(15);
	doc.text(attrpdfprofit + " €", 50, 160);
	doc.text(attrpdfloss + " €", 310, 160);
	doc.text(attrpdffee + " €", 50, 225);
	doc.text(attrpfs + " €", 310, 225);

	//table headlines
	const hline1 = "Steuerpflichtige und steuerfreie Veräußerungen nach §23 EStG";
	doc.text(hline1, 40, 320); 

	const hline2 = "Erträge aus Staking und Mining"
	doc.text(hline2, 40, 430);

	const hline3 = "Restliche Freibeträge bei Verkäufen und sonstigen Leistungen"
	doc.text(hline3, 40, 540);

	
	//table with data
	var columns1 = [
		{title: "Kategorie", dataKey: "col2"},
		{title: "Gewinne", dataKey: "col3"}, 
		{title: "Verluste", dataKey: "col4"},
		{title: "Gebühren", dataKey: "col5"}
	];
	var rows1 = [
		{
			"col2": "steuerfrei",
			"col3": attrtaxfreeprofit + " €",
			"col4": attrtaxfreeloss + " €",
			"col5": attrtaxfreefee + " €"
		}, 
		{
			"col2": "steuerpflichtig",
			"col3": attrtaxliableprofit + " €",
			"col4": attrtaxliableloss + " €",
			"col5": attrtaxliablefee + " €"
		}
	];

	doc.autoTable(columns1, rows1, {
			styles: {
				fillColor: [100,100,100],
				lineColor: 240, 
				lineWidth: 1,
			},
			columnStyles: {
				col2: {fillColor: false},
				col3: {fillColor: false},
				col4: {fillColor: false},
				col5: {fillColor: false},		 
			},
			margin: {top: 330} 
	});

	var columns2 = [
		{title: "Kategorie", dataKey: "col6"},
		{title: "Betrag", dataKey: "col7"},
	];
	var rows2 = [
		{
			"col6": "Mining",
			"col7": mining + " €",
		},
		{
			"col6": "Staking",
			"col7": staking + " €",
		}
	];
	doc.autoTable(columns2, rows2, {
		styles: {
			fillColor: [100,100,100],
			lineColor: 240, 
			lineWidth: 1,
		},
		columnStyles: {
			col6: {fillColor: false},
			col7: {fillColor: false},	 
		},
		margin: {top: 330},
		startY: 440
	}); 

	var columns3 = [
		{title: "Kategorie", dataKey: "col8"},
		{title: "Restlicher Freibetrag", dataKey: "col9"}
	];

	var rows3 = [
		{
			"col8": "Verkäufe",
			"col9": attrsalelimit + " €",
		},
		{
			"col8": "sonstige Leistungen",
			"col9": attrotherslimit + " €", 
		}
	];

	doc.autoTable(columns3, rows3, {
		styles: {
			fillColor: [100,100,100],
			lineColor: 240, 
			lineWidth: 1,
		},
		columnStyles: {
			col8: {fillColor: false},
			col9: {fillColor: false},
		},
		margin: {top: 330},
		startY: 550
	}); 

	doc.setFont("Helvetica", "bold");
	//NACTAM 
	doc.setTextColor('#007bff');
	doc.text("NACTAM ©", 480, 100);
	doc.addImage(document.querySelector("#hidden-logo"), "png", 480, 10, 62, 73, undefined, "NONE");

	doc.setFont("Helvetica", "normal");
	//site number
	doc.setTextColor('#000000');
	doc.setFontSize(11);
	doc.text("Seite 1", 510, 810);
	
	//date of creation
	doc.setTextColor('#5E5E5E');
	doc.text("DE GMT+2\t" + attrdate, 300, 810);

	//save pdf
	if (attryear != "0" && attrcc != " ") {
		var pdfname = "Steuerbericht" + attryear +".pdf";
		doc.save(pdfname);
	}
});



//Bar chart
var ctx1 = document.getElementById('barChart');
var myChart1 = new Chart(ctx1, {
	type: 'bar',
	data: {
		labels: [''],
		datasets: [{
			label: 'Gewinne', 
			data: [parseInt(profit)],
			backgroundColor: '#4e73df',
			stack: 'Stack 0',
		}, 
		{ 
			label: 'Verluste',
			data: [parseInt(loss)],
			backgroundColor: '#e74a3b',
			stack: 'Stack 1',
		},
		{
			label: 'Gebühren', 
			data: [parseInt(fee)],
			backgroundColor: '#f6c23e',
			stack: 'Stack 2',
		}]
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
					display: false,
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

//Pie chart
var ctx2 = document.getElementById('pieChart');
var myChart2 = new Chart(ctx2, {
  type: 'doughnut',
  data: {
    labels: ['steuerpflichtig', 'steuerfrei'],
    datasets: [{
      label: 'Transactions',
      data: [attrtaxation[0], attrtaxation[1]],
      backgroundColor: ['#3259ca', '#21306C']
    }]
  }
});

newWaterflowChart('customChart', transactions, '2022-12-02', '2023-12-02')

