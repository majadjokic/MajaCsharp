using MajaCsharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using System.Globalization;
using System.Text.Json;



namespace MajaCsharp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<EmployeeSummary> Employees { get; set; } = new();

        
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        

        public async Task OnGetAsync()
        {
            string key = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
            var url = $"https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={key}";
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);



            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var employeesEntities = JsonSerializer.Deserialize<List<EmployeeEntity>>(json) ?? new();

                Employees = employeesEntities
                    .GroupBy(e => e.EmployeeName)
                    .Select(g => 
                    new EmployeeSummary
                    {
                        
                        Name = g.Key,
                        TotalHours= Math.Round(g.Sum(ee => Math.Abs((ee.EndTimeUtc - ee.StarTimeUtc).TotalHours)))
                    })
                    .OrderByDescending(e => e.TotalHours)
                    .ToList();

                var plotModel = new PlotModel { Title = "Employee Time Distribution" };
                var pieSeries = new PieSeries();

                foreach (var emp in Employees)
                {
                    if (emp.Name == null)
                    {
                        pieSeries.Slices.Add(new PieSlice("Unknown", emp.TotalHours));
                    }
                    else
                    {
                        pieSeries.Slices.Add(new PieSlice(emp.Name, emp.TotalHours));
                    }
                }

                plotModel.Series.Add(pieSeries);

                var exporter = new PngExporter { Width = 600, Height = 600};

                var chartPath = Path.Combine("wwwroot", "images", "chart.png");
                Directory.CreateDirectory(Path.GetDirectoryName(chartPath));
                using var stream = System.IO.File.Create(chartPath);
                exporter.Export(plotModel, stream);



            }
        }
    }
}








