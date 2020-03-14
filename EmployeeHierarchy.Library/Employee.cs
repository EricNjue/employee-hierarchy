using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmployeeHierarchy.Library
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string ManagerId { get; set; }
        public long EmployeeSalary { get; set; }

        public bool IsCeo => string.IsNullOrEmpty(ManagerId);

        public List<Employee> Employees { get; } = new List<Employee>();

        public Employee(string managerId)
        {
            ManagerId = managerId;
        }

        public bool JuniorStaffAdded(string employeeId)
        {
            var staff = Employees.FirstOrDefault(f => f.EmployeeId == employeeId);

            return !(staff is null);
        }

        public void IsManagerOf(Employee e)
        {
            Employees.Add(e);
        }

        public override string ToString()
        {
            return ManagerId;
        }
    }
}
