/*
 *
 * js for pdf generation for the tax evaluation
 *
 * rewrite of the old pdf generation
 *
 * author: Tuan Bui
*/

function round(x){
	return (Math.round((x + Number.EPSILON) * 100) / 100).toFixed(2)
}

function initDoc(doc){
	doc.addFont('Helvetica', 'Helvetica', 'normal');
	doc.addFont('Helvetica', 'Helvetica', 'bold');
}

function generateHeader(doc){
	doc.setTextColor('#000000');
	doc.setFont("Helvetica", "bold");
	doc.setFontSize(28);
	let title = "Steuerbericht " + year;
	doc.text(title, 40, 50);
	doc.setFont("Helvetica", "normal");
	doc.setFontSize(11);
	doc.setTextColor('#5E5E5E');
	doc.text(`Vom 01.01.${year} bis 31.12.${year}`, 40, 75);

	doc.setTextColor('#000000');
	doc.setFontSize(11);
	let n = doc.internal.getNumberOfPages();
	doc.text(`Seite ${n}`, 510, 810);
	doc.setTextColor('#5E5E5E');
	doc.text(`generiert am: ${new Date().toString()}`, 40, 810);
	doc.setTextColor('#000000');
}

function generateTitlePage(doc){
	generateHeader(doc);
	doc.line(40, 110, 555, 110, 'F');
	doc.setFont("Helvetica", "bold");
	//NACTAM
	doc.setTextColor('#007bff');
	doc.text("NACTAM ©", 480, 100);
	doc.addImage(document.querySelector("#hidden-logo"), "png", 480, 10, 62, 73, undefined, "NONE");
	doc.setFont("Helvetica", "normal");
	doc.setTextColor('#000000');
	doc.text("Persönliche Daten", 40, 135);
	doc.setTextColor('#5E5E5E');
	doc.line(40, 140, 160, 140, 'F');
	doc.text("Name: " + username, 40, 155);
	doc.text("Email: " + email, 40, 170);
	doc.text("Telefonnummer: " + phonenumber, 40, 185);
	let yvals = [200, 215];
	if (address != "k.A. k.A.")
		doc.text("Adresse: " + address, 40, yvals.shift());
	if (city != "k.A. k.A.")
		doc.text("Ort: " + city, 40, yvals.shift());
	doc.setTextColor('#000000');

	doc.setFont("Helvetica", "normal");

	generateMainData(doc, 250);
}

function generateMainData(doc, y){
	doc.setTextColor('#000000');
	doc.setFillColor(totalValues[1].Profits > -totalValues[1].Losses ? "#d0ffd1" : "#ffd0d0");
	doc.roundedRect(134, y, 101, 50, 5, 5, 'F');
	doc.text("Bilanz", 144, y + 20);
	doc.text("Gewinn - Verlust", 144, y + 35);
	doc.setFillColor("#d0ffd1");
	doc.roundedRect(247, y, 101, 50, 5, 5, 'F');
	doc.text("Profit (Verkauf)", 257, y + 20);
	doc.text("(andere)", 257, y + 35);
	doc.setFillColor("#ffd0d0");
	doc.roundedRect(361, y, 101, 50, 5, 5, 'F');
	doc.text("Ausgaben", 371, y + 20);
	doc.setFillColor("#ffd0d0");
	doc.roundedRect(474, y, 101, 50, 5, 5, 'F');
	doc.text("Gebühren", 484, y + 20);
	doc.setFillColor("#d0eeff");
	doc.roundedRect(134, y + 200, 140, 50, 5, 5, 'F');
	doc.text("Verkäufe", 144, y + 220);
	doc.setFillColor("#d0eeff");
	doc.roundedRect(288, y + 200, 140, 50, 5, 5, 'F');
	doc.text("Mining", 298, y + 220);
	doc.setFillColor("#d0eeff");
	doc.roundedRect(443, y + 200, 140, 50, 5, 5, 'F');
	doc.text("Staking", 453, y + 220);

	doc.setFont("Helvetica", "normal");
	doc.text("steuerfrei", 40, y + 70);
	doc.text(round(totalValues[0].Profits + totalValues[0].Losses - (totalValues[1].Profits + totalValues[1].Losses)) + "€", 144, y + 70);
	doc.text(("" + round(totalValues[0].Profits-totalValues[0].Profits) + "€ - " + round(-totalValues[0].Losses+totalValues[1].Losses)) + "€", 144, y +85);
	doc.text(round(totalValues[0].Income - totalValues[1].Income) + "€", 257, y +70);
	doc.text(round(totalValues[0].ProfitsFromOther - totalValues[1].ProfitsFromOther) + "€", 257, y +85);
	doc.text(round(totalValues[0].Expenditures - totalValues[1].Expenditures) + "€", 371, y +70);
	doc.text(round(totalValues[0].Fees - totalValues[1].Fees) + "€", 484, y +70);
	doc.text("steuerpflichtig", 40, y +110);
	doc.text(round(totalValues[1].Profits + totalValues[1].Losses) + "€", 144, y +110);
	doc.text(("" + round(totalValues[1].Profits) + "€ - " + round(-totalValues[1].Losses)) + "€", 144, y +125);
	doc.text(round(totalValues[1].Income) + "€", 257, y +110);
	doc.text(round(totalValues[1].ProfitsFromOther) + "€", 257, y +125);
	doc.text(round(totalValues[1].Expenditures) + "€", 371, y +110);
	doc.text(round(totalValues[1].Fees) + "€", 484, y +110);
	doc.text("gesamt", 40, y +150);
	doc.text(round(totalValues[0].Profits + totalValues[0].Losses) + "€", 144, y + 150);
	doc.text(("" + round(totalValues[0].Profits) + "€ - " + round(-totalValues[0].Losses)) + "€", 144, y +165);
	doc.text(round(totalValues[0].Income) + "€", 257, y +150);
	doc.text(round(totalValues[0].ProfitsFromOther) + "€", 257, y +165);
	doc.text(round(totalValues[0].Expenditures) + "€", 371, y +150);
	doc.text(round(totalValues[0].Fees) + "€", 484, y +150);
	doc.text("Erträge", 40, y + 270);
	doc.text(round(totalValues[1].ProfitsFromSelling - totalValues[1].Losses) + "€", 144, y +270);
	doc.text(round(totalValues[1].ProfitsFromMining) + "€", 298, y +270);
	doc.text(round(totalValues[1].ProfitsFromStaking) + "€", 453, y +270);
	doc.text("Freibeträge", 40, y + 295);
	doc.text(round(totalValues[1].RemainingTaxFreeSales) + "€", 144, y +295);
	doc.text(round(totalValues[1].RemainingTaxFreeOther) + "€", 298, y +295);
	doc.setTextColor('#5E5E5E');
	doc.setFontSize(8);
	doc.text("(§22 No.3 EStG)", 40, y + 80);
	doc.text("(§23 EStG)", 40, y + 120);
}



function generateCurrencyPageHeader(doc, currency){
	generateHeader(doc);
	doc.line(40, 90, 555, 90, 'F');
	doc.setTextColor('#007bff');
	doc.text("NACTAM ©", 480, 80);
	doc.setFontSize(16);
	doc.text(currency, 40, 120);
}
function generateCurrencyPage(doc, currency){
	doc.addPage();
	generateCurrencyPageHeader(doc, currency);
	let canvas = document.getElementById("custom-chart-" + currency);
	let imgData = canvas.toDataURL('image/png');

	doc.addImage(imgData, 'PNG', 40, 140, 500, 500 / canvas.width * canvas.height);
	doc.autoTable({html: "#table-" + currency, margin: {top:  500 / canvas.width * canvas.height + 200 }});
}

function generatePDF(pdfname = "Steuerbericht" + year + ".pdf"){
	let doc = new jsPDF('p', 'pt', 'a4');
	generateTitlePage(doc);
	for (const [key, value] of Object.entries(soldTransactions)) {
		if (value.length != 0)
			generateCurrencyPage(doc, key);
	}


	doc.save(pdfname);
}


let triggerButton = document.getElementById("generate-pdf");
triggerButton.addEventListener("click", ev => generatePDF() );
