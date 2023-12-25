using EmployeeNetProject.DTO;

namespace EmployeeNetProject.Interface
{
    public interface IGetEmployeesModel
    {
        public  Task<List<EmployeeForShow>> GetEmployees();
    }
}
