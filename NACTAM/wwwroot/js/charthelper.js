/**
 * example data
 */
// var transactions = [[12, "2021-01-01", "2022-04-01"], [8,	"2021-01-01", "2023-04-04"], [10, "2022-08-15", "2023-04-04"]];

const taxFreeColor = "#21306c";
const taxPayingColor = "#3259ca";
const hoverColor = "#5279aa";




/**
 * function to create a datapoint for the chart.js dataset
 * @param {number} x1 - starting x position
 * @param {number} y1 - starting y position
 * @param {number} x2 - ending	 x position
 * @param {number} y2 - ending	 y position
 * @param (string) color - color
 * @param (number) i - index of the element
 *
 * author: Tuan Bui
 */
var datapoint = (x1, y1, x2, y2, color, i = 0) => ({
	type: 'box',
	xMin: x1,
	xMax: x2,
	yMin: y1,
	yMax: y2,
	backgroundColor: color,
	borderColor: color,
	usual: color,
	borderWidth: 1,
	label: {
		enabled: false,
		backgroundColor: 'black',
		color: '#ffffff',
		drawTime: 'afterDatasetsDraw',
		content: [`Menge: ${y2 - y1}`, `gekauft: ${new Date(x1).toLocaleDateString()}`, `bis: ${new Date(x2).toLocaleDateString()}`]
	},
	enter({ element }, event) {
		if (i != -1) {
			element.backgroundColor = hoverColor;
			element.label.options.display = true;
			return true;
		}
	},
	leave({ element }, event) {
		if (i != -1) {
			element.backgroundColor = element.usual;
			element.label.options.display = false;
			return true;
		}
	}
});

var yearAnnotation = (year, val, col) => datapoint(new Date(year.toString()), 0, addYear(new Date(year.toString())), val, col, -1);

/**
 * adds another year to a time
 * @param (Date) x - date
 *
 * author: Tuan Bui
 */
function addYear(x){
	x.setFullYear(x.getFullYear() + 1);
	return x;
}

/**
 * generates the annotation boxes from transactions
 * @param (Transaction[]) ts - list of safe after buy transactions
 *
 * author: Tuan Bui
 */
function transactionsToData(ts){
	let dataset = [];
	let y = 0;

	for (let index = 0; index < ts.length; index ++) {
		let i = ts[index];
		let year1 = addYear(new Date(i[1]));
		let minDate = Math.min(new Date(i[2]), year1);
		let endDate = new Date(i[2]);
		dataset.push(datapoint(minDate, y, endDate, y + i[0], taxFreeColor, index));
		dataset.push(datapoint(new Date(i[1]), y, minDate, y + i[0], taxPayingColor, index));
		y += i[0];
	}
	dataset.unshift(yearAnnotation(year, y, "#00000023"));
	return dataset;
}

/**
 * Function to create diagrams like given in the example
 *
 * @param {string} id - canvas id in DOM
 * @param {[number, string, string][]} - list of transactions with start and end date
 * (you should use `AfterSaleBuyTransactionViewModel` for this)
 * @param (string | Date | number) minDate - minimum date, optional
 * @param (string | Date | number) maxDate - maximum date, optional
 *
 * author: Tuan Bui
 */
function newWaterflowChart(id, transactions, minDate, maxDate) {
	const annotationData = transactionsToData(transactions);
	const config = {
		type: "scatter",
		data: {
			labels: ["steuerfrei", "steuerpflichtig"],
			datasets: [
				{
					label: "steuerfrei",
					backgroundColor: taxFreeColor
				},
				{
					label: "steuerpflichtig",
					backgroundColor: taxPayingColor
				}
			]
		},
		options: {
			indexAxis: 'y',
			scales: {
				x: {
					type: 'time',
					beginAtZero: false,
					time: {
						unit: 'month',
						displayFormats: {
							day: 'D MMM yyyy'
						}
					},
					min: minDate,
					max: maxDate,
					display: true,
					text: "Zeit"
				},
				y: {
					categorySpacing: 0,
					reverse: true,
					display: true,
					text: "gehandelte Menge",
					title: {
						display: true,
						text: "gehandelte Menge"
					},
					min: 0,
					max: annotationData[0].yMax
				}
			},

			plugins: {
				annotation: {
					annotations: annotationData
				}
			}
		}
	};

	return new Chart(id, config)
}
