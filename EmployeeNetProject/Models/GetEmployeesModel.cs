using EmployeeNetProject.DTO;
using EmployeeNetProject.Interface;
using EmployeeNetProject.Utility;
using Newtonsoft.Json;

namespace EmployeeNetProject.Models
{
    public class GetEmployeesModel: IGetEmployeesModel
    {
        public async Task<List<EmployeeForShow>> GetEmployees()
        {
            List<EmployeeDto> employees = new List<EmployeeDto>();
            HttpClient _client = new HttpClient();
            
            HttpResponseMessage response = await _client.GetAsync(BaseURL.Url); 

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                employees = JsonConvert.DeserializeObject<List<EmployeeDto>>(jsonString);
            }
            else
            {
                // Upravljanje greškom ako dođe do problema sa API-jem
                return null;
            }

            // Grupisanje podataka po imenima zaposlenika i izračunavanje ukupnog radnog vremena
            var groupedByEmployee = employees
                .GroupBy(entry => entry.EmployeeName)
                .Select(group => new EmployeeForShow
                {
                    EmployeeName = group.Key,
                    TotalHoursWorked = Math.Ceiling(group.Sum(entry => (entry.EndTimeUtc - entry.StarTimeUtc).TotalHours))
                }).OrderByDescending(x=>x.TotalHoursWorked).ToList();
            return groupedByEmployee;
        }
    }
}
