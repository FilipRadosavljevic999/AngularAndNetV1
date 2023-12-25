using EmployeeNetProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using EmployeeNetProject.Interface;

namespace EmployeeNetProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IGetEmployeesModel _getEmployeesModel;

        public HomeController(ILogger<HomeController> logger, IGetEmployeesModel getEmployeesModel)
        {
            _logger = logger;
            _getEmployeesModel = getEmployeesModel;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var employe = await _getEmployeesModel.GetEmployees();
                return View(employe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo je do greške u pri u Index metodi.");
                return View(ex);
            }
           
        }

        public async Task<IActionResult> DownloadChartPng()
        {
            try
            {
                var groupedByEmployee = await _getEmployeesModel.GetEmployees();
                groupedByEmployee.ForEach(e => e.EmployeeName = e.EmployeeName ?? "Other");
                using (Bitmap bmp = new Bitmap(800, 800))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    // Nacrtajte pie chart
                    float startAngle = 0;
                    float totalHours = groupedByEmployee.Sum(emp => (float)emp.TotalHoursWorked);
                    float radius = 200;
                    foreach (var employeeGroup in groupedByEmployee)
                    {
                        float sweepAngle = (float)(360 * (employeeGroup.TotalHoursWorked / totalHours));
                        using (Brush brush = new SolidBrush(GetRandomColor()))
                        {
                            g.FillPie(brush, new Rectangle(50, 50, 400, 400), startAngle, sweepAngle);
                            float midAngle = startAngle + sweepAngle / 2;
                            float textX = (float)(radius * Math.Cos(midAngle * Math.PI / 180) + 250);
                            float textY = (float)(radius * Math.Sin(midAngle * Math.PI / 180) + 250);

                            g.DrawString($"{(employeeGroup.TotalHoursWorked / totalHours) * 100:F2}%", new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, textX, textY);
                        }

                        startAngle += sweepAngle;
                    }
                    var legendBrush = new SolidBrush(Color.Black);
                    float legendX = 600;
                    float legendY = 50;
                    float offset = 20;
                    foreach (var employeeGroup in groupedByEmployee)
                    {
                        g.FillRectangle(new SolidBrush(GetRandomColor()), legendX, legendY, 10, 10);
                        g.DrawString($"{employeeGroup.EmployeeName}", new Font(FontFamily.GenericSansSerif, 8), legendBrush, legendX + 15, legendY);
                        legendY += offset;
                    }

                    var memoryStream = new MemoryStream();
                    bmp.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Position = 0;

                    return File(memoryStream.ToArray(), "image/png", "pie_chart.png");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo je do greške u pri u DOWNLOADPNG metodi.");
                return null;
            }
          
            
        }
        private Color GetRandomColor()
        {
            Random rnd = new Random();
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}