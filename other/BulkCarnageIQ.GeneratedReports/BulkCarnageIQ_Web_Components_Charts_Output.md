# Directory: Components\Charts

## File: LineChart.razor

```C#
@using Microsoft.JSInterop
@inject IJSRuntime JS

<div>
    <canvas id="@ChartId" width="400" height="200"></canvas>
</div>

@code {
    [Parameter] public string ChartId { get; set; } = $"chart-{Guid.NewGuid()}";
    [Parameter] public string Title { get; set; } = "Line Chart";
    [Parameter] public List<string> Labels { get; set; } = new();
    [Parameter] public List<float> Data { get; set; } = new();
    [Parameter] public string DataLabel { get; set; } = "Data";
    [Parameter] public string LineColor { get; set; } = "rgba(75, 192, 192, 1)";
    [Parameter] public string xLabel { get; set; } = "X-Axis";
    [Parameter] public string yLabel { get; set; } = "Y-Axis";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("renderLineChart", ChartId, Title, Labels, Data, DataLabel, LineColor, xLabel, yLabel);
        }
    }
}

```

