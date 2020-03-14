using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeHierarchy.Library
{
    public class Employees
    {
        public Employee RootEmployee { get; set; }
        public List<Employee> EmployeesMetadata { get; set; }

        public Employees(string csvString)
        {
            // Check if any data is provided ...
            if (string.IsNullOrEmpty(csvString))
                throw new Exception("No data provided.");

            // Validating the string ...

            // get the employees rows ...
            var employeesRows = csvString.Split(Environment.NewLine);

            if (employeesRows.Length < 1)
                throw new Exception("Invalid data provided.");

            EmployeesMetadata = new List<Employee>();
            foreach (var employeesRow in employeesRows)
            {
                var currentEmployeeData = employeesRow.Split(',');

                // each row must have 3 cols .... even though some may be null ...
                if (currentEmployeeData.Length != 3)
                {
                    // ignore the record and proceed ...
                    continue;
                }

                // extract the data ...
                var employeeId = currentEmployeeData[0];
                var managerId = currentEmployeeData[1];
                var salaryIsValid = long.TryParse(currentEmployeeData[2], out var employeeSalary);

                if (!salaryIsValid)
                {
                    // ignore the record and proceed ...
                    continue;
                }

                EmployeesMetadata.Add(new Employee(managerId)
                {
                    EmployeeId = employeeId,
                    EmployeeSalary = employeeSalary,
                    ManagerId = managerId
                });
            }


            // We now have valid employees with us ... data wise at least ...
            BuildEmployeeHierarchy(EmployeesMetadata);

            // Check There is only one CEO, i.e. only one employee with no manager.
            var oneCeoExists = CheckThereIsOneCeo(EmployeesMetadata);

        }

        public bool EveryManagerIsAnEmployee(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                if (string.IsNullOrEmpty(employee.ManagerId)) continue;
                bool value = ManagerIsAnEmployee(employees, employee.ManagerId);

                if (value != false)
                {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }

        private bool ManagerIsAnEmployee(List<Employee> employees, string managerId)
        {
            //check this ID exists in the employee column ...
            var employee = employees.FirstOrDefault(f => f.EmployeeId == managerId);

            return employee != null;
        }

        public long CalculateSalaryBudget(string managerId)
        {
            var employee = Search(RootEmployee, managerId);

            if (employee is null)
                return 0;

            var budget = employee.EmployeeSalary;
            foreach (var t in employee.Employees)
            {
                employee = t;
                budget += employee.EmployeeSalary;
                CalculateSalaryBudget(employee.ManagerId);
            }

            return budget;
        }

        public bool CheckThereIsOneCeo(List<Employee> employees)
        {
            var ceos = employees.Where(w => string.IsNullOrEmpty(w.ManagerId)).ToList();

            if (ceos.Count == 0)
                return false;

            return ceos.Count == 1;
        }

        public Employee Search(Employee root, string managerId)
        {
            if (managerId == root.ManagerId)
                return root;

            Employee managerFound = null;
            foreach (var t in root.Employees)
            {
                managerFound = Search(t, managerId);
                if (managerFound != null)
                    break;
            }
            return managerFound;
        }

        public bool NoManagerialCircularReference(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                bool value = CircularReferenceExists(employees, employee);

                if (value == true)
                    return false;
                continue;
            }
            return true;
        }

        private bool CircularReferenceExists(List<Employee> employees, Employee employee)
        {
            var value = employees.FirstOrDefault(f => f.EmployeeId == employee.ManagerId &&
                                                      f.ManagerId == employee.EmployeeId);

            return !(value is null);
        }


        private Employee BuildEmployeeHierarchy(IEnumerable<Employee> employeesMetadata)
        {
            var managerEmployees = new Dictionary<string, Employee>(); // will hold manager and their juniors ...
            foreach (var metadata in employeesMetadata)
            {
                var id = string.IsNullOrEmpty(metadata.ManagerId) ? metadata.EmployeeId : metadata.ManagerId;
                // check if the manager had been added ...
                if (managerEmployees.ContainsKey(id))
                {
                    // check if junior staff was added ...
                    var manager = managerEmployees[id];

                    if (manager.JuniorStaffAdded(metadata.EmployeeId)) continue;
                    var emp = new Employee(id)
                    {
                        EmployeeId = metadata.EmployeeId,
                        EmployeeSalary = metadata.EmployeeSalary
                    };

                    if (emp.IsCeo)
                        RootEmployee = emp;

                    manager.IsManagerOf(emp);
                }
                else
                {
                    var manager = new Employee(id)
                    {
                        EmployeeId = metadata.EmployeeId,
                        EmployeeSalary = metadata.EmployeeSalary,
                        ManagerId = id
                    };

                    manager.IsManagerOf(new Employee(metadata.ManagerId)
                    {
                        EmployeeId = metadata.EmployeeId,
                        EmployeeSalary = metadata.EmployeeSalary,
                        ManagerId = id
                    });



                    if (manager.IsCeo)
                        RootEmployee = manager;

                    managerEmployees.Add(id, manager);
                }
            }

            return RootEmployee;

        }
    }
}
