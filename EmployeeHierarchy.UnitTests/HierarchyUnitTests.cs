using System;
using System.Collections.Generic;
using EmployeeHierarchy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmployeeHierarchy.UnitTests
{
    [TestClass]
    public class HierarchyUnitTests
    {
        private Employees employees;
        private Employee _rootEmployee;

      
        string csvData = $"Emp1,Emp2,2000{Environment.NewLine}" +
                         $"Emp3,Emp4,4000{Environment.NewLine}" +
                         $"Emp2,Emp14,6000{Environment.NewLine}" +
                         $"Emp5,Emp6,6000{Environment.NewLine}" +
                         $"Emp7,Emp8,Kes8000{Environment.NewLine}" +
                         $"CEO,,200000{Environment.NewLine}" +
                         $"Emp9,Emp10,10000";

        string csvData2 = $"Emp1,Emp2,2000{Environment.NewLine}" +
                          $"Emp3,Emp4,4000{Environment.NewLine}" +
                          $"Emp5,Emp6,6000{Environment.NewLine}" +
                          $"Emp2,Emp1,Kes8000{Environment.NewLine}" +
                          $"Emp1,Emp2,Kes8000{Environment.NewLine}" +
                          $"CEO,,200000{Environment.NewLine}" +
                          $"CEO-2,,10000";

        public HierarchyUnitTests()
        {
            employees = new Employees(csvData);
            _rootEmployee = employees.RootEmployee;
        }



        [TestMethod]
        public void TestSalaryBudgetForManager()
        {
            // Arrange
            string managerid = "CEO";
            long expected = 0;

            // Act 
            long actual = employees.CalculateSalaryBudget(managerid);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestThereIsOneCeo()
        {
            // Arrange 
            bool expected = true;

            // Act ...
            bool actual = employees.CheckThereIsOneCeo(employees.EmployeesMetadata);

            // Assert ...
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestEveryManagerIsAnEmployee()
        {
            // Arrange ...
            bool expected = false;
            List<Employee> employeesList = this.employees.EmployeesMetadata;

            // Act ...
            bool actual = this.employees.EveryManagerIsAnEmployee(employeesList);

            // Assert ...
            Assert.AreEqual(expected, actual);
            Assert.IsNotNull(employees);
        }

        [TestMethod]
        public void TestNoManagerialCircularReference()
        {
            // Arrange ...
            bool expected = true;
            List<Employee> employeesList = this.employees.EmployeesMetadata;

            // Act ...
            bool actual = this.employees.NoManagerialCircularReference(employeesList);

            // Assert ...
            Assert.AreEqual(expected, actual);
        }

    }
}
