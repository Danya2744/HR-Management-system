using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HR_department;
using System.Windows;

namespace UnitTestProject2
{
    [TestClass]
    public class EmployeeTests
    {
        private Add_staff _addStaff;
        private Add_user _addUser;

        [TestInitialize]
        public void TestInitialize()
        {
            _addStaff = new Add_staff();
            _addUser = new Add_user(12);
        }

        [TestMethod]
        public void ValidateEmployeeInputs_AllValidData_ReturnsTrue()
        {
            string lastName = "Иванов";
            string firstName = "Иван";
            string middleName = "Иванович";
            DateTime birthDate = new DateTime(1990, 1, 1);
            string phone = "1234567890";
            string education = "Высшее";
            DateTime hireDate = DateTime.Today;
            var position = new Add_staff.Position { Id = 1, Name = "Разработчик" };
            var department = new Add_staff.Department { Id = 1, Name = "IT" };

            bool result = _addStaff.ValidateEmployeeInputs(
                lastName, firstName, middleName, birthDate, phone,
                education, hireDate, position, department);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateEmployeeInputs_EmptyLastName()
        {
            string lastName = "";
            string firstName = "Иван";

            bool result = _addStaff.ValidateEmployeeInputs(
                lastName, firstName, null, DateTime.Now.AddYears(-20),
                "1234567890", "Высшее", DateTime.Today,
                new Add_staff.Position(), new Add_staff.Department());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateEmployeeInputs_InvalidPhone()
        {
            string phone = "123";
            bool result = _addStaff.ValidateEmployeeInputs(
                "Иванов", "Иван", null, DateTime.Now.AddYears(-20),
                phone, "Высшее", DateTime.Today,
                new Add_staff.Position(), new Add_staff.Department());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateEmployeeInputs_UnderageEmployee()
        {
            DateTime birthDate = DateTime.Today.AddYears(-17); 
            bool result = _addStaff.ValidateEmployeeInputs(
                "Иванов", "Иван", null, birthDate,
                "1234567890", "Высшее", DateTime.Today,
                new Add_staff.Position(), new Add_staff.Department());

            Assert.IsFalse(result);
        }
    }

    [TestClass]
    public class UserTests
    {
        private Add_user _addUser;

        [TestInitialize]
        public void TestInitialize()
        {
            _addUser = new Add_user(12);
        }

        [TestMethod]
        public void ValidateUserInputs_AllValidData()
        {
            string login = "testuser";
            string password = "password123";
            var status = new { Id = 1, Name = "Admin" };
            bool result = _addUser.ValidateUserInputs(login, password, status);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateUserInputs_EmptyLogin()
        {
            string login = "";
            string password = "password123";
            var status = new { Id = 1, Name = "Admin" };

            bool result = _addUser.ValidateUserInputs(login, password, status);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateUserInputs_ShortPassword()
        {
            string login = "testuser";
            string password = "123";
            var status = new { Id = 1, Name = "Admin" };
            bool result = _addUser.ValidateUserInputs(login, password, status);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateUserInputs_LongPassword_ReturnsFalse()
        {
            string login = "testuser";
            string password = "thispasswordistoolong1234567890";
            var status = new { Id = 1, Name = "Admin" };
            bool result = _addUser.ValidateUserInputs(login, password, status);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateUserInputs_NoStatus_ReturnsFalse()
        {
            string login = "testuser";
            string password = "password123";
            object status = null;
            bool result = _addUser.ValidateUserInputs(login, password, status);

            Assert.IsFalse(result);
        }
    }
}
