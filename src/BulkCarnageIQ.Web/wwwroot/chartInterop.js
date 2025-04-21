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
                backgroundColor: 'rgba(54, 162, 235, 0.5)',
                borderColor: 'rgba(54, 162, 235, 1)',
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
