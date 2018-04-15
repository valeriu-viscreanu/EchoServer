using Logic;
using Logic.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpDrivers.Fakes;

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
            var clientManager = new StubITcpClientManager();
            var serverManager = new StubITcpServerManager();
            var clientApp = new StubClient(clientManager);
            var serverApp = new StubServer(serverManager);

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
