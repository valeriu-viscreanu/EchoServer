using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpDrivers.Fakes;

namespace Logic.Tests
{
    [TestClass]
    public class ClientTests
    {
        private readonly RunSettings runSettings = new RunSettings
        {
            ServerList = new List<string>()
            {
                "192.168.56.13:213",
                "192.168.56.1:55001"
            }
        };
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Client_Start_CallsConnectOnStart()
        {
            //arrange
            var calledConnect = false;
            var tcpManager = new StubITCPManager()
            {
                ConnectString = endpoints =>
                {
                    calledConnect = true;
                    return true;
                },

            };

            var client = new Client(tcpManager);
            //act
            client.Start(runSettings);

            //assert
            Assert.IsTrue(calledConnect);
        }

        [TestMethod]
        public void Client_Start_TriesToReconnectToNewServerIfItFailes()
        {
            //arrange
            var tries = 0;
            var tcpManager = new StubITCPManager
            {
                ConnectString = endPointString =>
                {
                    if (tries == 0)
                    {
                        tries++;
                        return false;
                    }

                    return true;
                }

            };
            var client = new Client(tcpManager);

            //act
            client.Start(this.runSettings);
            //assert
            // no excetion
            Assert.IsTrue(tries > 0);
        }

        [TestMethod]
        public void Client_Start_CallsClosefCanNotConnect()
        {
            //arrange
            var calledStopTrue = false;
            var tcpManager = new StubITCPManager()
            {
                ConnectString = endPoint => false,
                Close = () => { calledStopTrue = true; }

            };
            var client = new Client(tcpManager);

            //act
            client.Start(runSettings);

            //assert
            Assert.IsTrue(calledStopTrue);
        }

        [TestMethod]
        public void Client_Stop_ClosesManagerOnStop()
        {
            //arrange
            var calledCloseManager = false;
            var tcpManager = new StubITCPManager()
            {
                Close = () => calledCloseManager = true

            };

            var client = new Client(tcpManager);
            //act
            client.Stop();

            //assert
            Assert.IsTrue(calledCloseManager);
        }
        [TestMethod]
        public void Client_SendMessage_CallsConnectIfDisconnected()
        {
            //arrange
            bool calledConnect;
            var tcpManager = new StubITCPManager()
            {
                IsClientConnectedGet = () => false,
                SendMessageString = message => true,
                ConnectString = message =>
                {
                    calledConnect = true;
                    return true;
                }
            };

            var client = new Client(tcpManager);
            client.Start(this.runSettings);

            //act
            calledConnect = false;
            var sentMessage = client.SendMessage("Test");

            //assert
            Assert.IsTrue(calledConnect);
            Assert.IsTrue(sentMessage);
        }
    }
}
