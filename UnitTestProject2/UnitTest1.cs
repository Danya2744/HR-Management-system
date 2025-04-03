using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HR_department;

namespace HRDepartmentTests
{
    [TestClass]
    public class AuthorizationTests
    {
        private MainWindow _mainWindow;

        [TestInitialize]
        public void TestInitialize()
        {
            _mainWindow = new MainWindow();
        }

        [TestMethod]
        public void Auth_SuccessfulAdminLogin()
        {
            string username = "petrova_o";
            string password = "manager456";
            bool result = _mainWindow.Auth(username, password);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Auth_SuccessfulEmployeeLogin()
        {
            string username = "sidorov_a";
            string password = "devpass789";
            bool result = _mainWindow.Auth(username, password);

            Assert.IsTrue(result);
        }
        [TestMethod]
        public void Auth_EmptyUsername()
        {
            string username = "";
            string password = "manager456";
            bool result = _mainWindow.Auth(username, password);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Auth_EmptyPassword()
        {
            string username = "petrova_o";
            string password = "";
            bool result = _mainWindow.Auth(username, password);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Auth_InvalidCredentials()
        {
            string username = "none";
            string password = "wrongpassword";
            bool result = _mainWindow.Auth(username, password);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsCaptchaValid_CorrectCaptcha_ReturnsTrue()
        {
            _mainWindow.GenerateCaptcha();
            string correctCaptcha = _mainWindow.GetCaptchaText();

            bool result = _mainWindow.IsCaptchaValid(correctCaptcha);

            Assert.IsTrue(result, "Капча должна быть верной");
        }

        [TestMethod]
        public void IsCaptchaValid_IncorrectCaptcha_ReturnsFalse()
        {
            _mainWindow.GenerateCaptcha();
            string incorrectCaptcha = "WRONG";

            bool result = _mainWindow.IsCaptchaValid(incorrectCaptcha);

            Assert.IsFalse(result, "Капча не должна совпадать");
        }
    }
}