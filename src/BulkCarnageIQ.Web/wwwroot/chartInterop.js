window.chartInstances = {};

window.renderBarChart = (canvasId, chartData) => {
    console.log("renderBarChart called with:", chartData);
    const ctx = document.getElementById(canvasId);

    if (window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
    }

    const newChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: chartData.labels,
            datasets: [{
                label: chartData.datasetLabel,
                data: chartData.values,
                backgroundColor: 'rgba(217, 35, 15, 0.5)',
                borderColor: '#d9230f',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    window.chartInstances[canvasId] = newChart;
};

window.renderLineChart = (chartId, title, labels, data, dataLabel, lineColor, xLabel, yLabel) => {
    const ctx = document.getElementById(chartId).getContext('2d');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: dataLabel,
                data: data,
                borderColor: lineColor,
                fill: false,
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: title
                }
            },
            scales: {
                x: { title: { display: true, text: xLabel } },
                y: { title: { display: true, text: yLabel } }
            }
        }
    });
};
