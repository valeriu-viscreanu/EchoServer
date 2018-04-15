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
            var tcpmanager = new StubTCPManager();
            var clientApp = new StubClient(tcpmanager);
            var serverApp = new StubServer(tcpmanager);

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
