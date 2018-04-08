﻿using Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TCPApplication;
using TCPApplication.Fakes;

namespace EchoApp.Tests
{
    [TestClass]
    public class ApplicationTest
    {
        [TestInitialize]
        public void Intialize()
        {
            
        }
        
        [TestMethod]
        public void AppFactory_GetEchoApp_ReturnsCorrectMode()
        {
           //Arrange
            var clientApp = new StubIEchoApp();
            var serverApp = new StubIEchoApp();

            var appFacory = new EchoAppFactory(clientApp,serverApp);

            //Act
            var factoryClient = appFacory.GetEchoApp(RunMode.Client);
            var factoryServer = appFacory.GetEchoApp(RunMode.Server);

            //Assert
            Assert.AreEqual(factoryClient, clientApp);
            Assert.AreEqual(factoryServer, serverApp);
            Assert.AreNotEqual(factoryServer, clientApp);
        }
       
        /// TODO Decouple program and test
    }
}
