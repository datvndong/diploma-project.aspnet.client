const barOptions = {
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: true
        }
      }]
    },
    legend: {
      display: false
    },
    elements: {
      point: {
        radius: 0
      }
    }
};

const doughnutPieOptions = {
    responsive: true,
    animation: {
      animateScale: true,
      animateRotate: true
    }
};

function randomRGBA() {
    const o = Math.round
    const r = Math.random
    const s = 255;
    return 'rgba(' + o(r() * s) + ',' + o(r() * s) + ',' + o(r() * s) + ',';
}

function randomDoughnutOrPie() {
    return Math.random() >= 0.5 ? 'doughnut' : 'pie';
}

function createChart(container, element, isBarChart) {
	const id = element.key;
	const label = element.label;
		
	// Create div
	let str = '<div class="col-lg-6 grid-margin stretch-card"><div class="card"><div class="card-body"><h4 class="card-title">';
	str += typeof label === 'undefined' ? 'Checboxes Bar Chart' : element.label;
	str += '</h4><canvas id="' + id + '" style="height:';
	str += isBarChart ? 230 : 250;
	str += 'px"></canvas></div></div></div>';
	
	container.append(str);
	
	// Create content
	if ($('#' + id).length) {
		const chartCanvas = $('#' + id).get(0).getContext("2d");
	    // This will get the first returned node in the jQuery collection.
		new Chart(chartCanvas, {
			type: isBarChart ? 'bar' : randomDoughnutOrPie(),
			data: isBarChart ? createBarData(element) : createDoughnutPieData(element),
			options: isBarChart ? barOptions : doughnutPieOptions
	    });
	}
}

function createDoughnutPieData(element) {
	const doughnutPieData = {};
	
	// Create datasets & labels
	const datasets = [];
	const data = [];
	const backgroundColor = [];
	const borderColor = [];
	const labels = [];
	
	const chartDatas = element.data;
	chartDatas.forEach(obj => {
		data.push(obj.count);
		let rgba = randomRGBA();
		backgroundColor.push(rgba + '0.3)');
		borderColor.push(rgba + '1)');
		labels.push(obj.label);
	});
	
	// Add datasets to result obj
	const datasetsObj = {};
	datasetsObj.data = data;
	datasetsObj.backgroundColor = backgroundColor;
	datasetsObj.borderColor = borderColor;
	datasets.push(datasetsObj);
	
	doughnutPieData.datasets = datasets;
	
	// Add labels to result obj
	doughnutPieData.labels = labels;
	
	// return chart data
	return doughnutPieData;
}

function createBarData(element) {
	const barData = {};
	
	// Create datasets & labels
	const labels = [];
	const datasets = [];
	const label = '# of Votes';
	const data = [];
	const backgroundColor = [];
	const borderColor = [];
	const borderWidth = 1;
	
	let chartDatas = element.data;
	if (typeof chartDatas === 'undefined') {
		// chart about checkbox
		chartDatas = element;
	}
	chartDatas.forEach(obj => {
		labels.push(obj.label);
		data.push(obj.count);
		let rgba = randomRGBA();
		backgroundColor.push(rgba + '0.2)');
		borderColor.push(rgba + '1)');
	});
	
	// Add labels to result obj
	barData.labels = labels;
	
	// Add datasets to result obj
	const datasetsObj = {};
	datasetsObj.label = label;
	datasetsObj.data = data;
	datasetsObj.backgroundColor = backgroundColor;
	datasetsObj.borderColor = borderColor;
	datasetsObj.borderWidth = borderWidth;
	datasets.push(datasetsObj);
	
	barData.datasets = datasets;
	
	// return chart data
	return barData;
}