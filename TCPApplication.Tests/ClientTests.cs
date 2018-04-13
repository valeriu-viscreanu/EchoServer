using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Logic.Fakes;
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
                ConnectIPEndPoint = point => { calledConnect = true; },
                CreateIpEndPointsIEnumerableOfString = ipSring => new List<IPEndPoint> { new IPEndPoint(IPAddress.Parse("192.168.56.13"), 134) }
                
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
            var tcpManager = new StubITCPManager()
            {
                ConnectIPEndPoint = endPoint =>
                {
                    if (tries == 0)
                    {
                        tries++;
                        throw new SocketException();
                    }
                },
                CreateIpEndPointsIEnumerableOfString = ipSring => new List<IPEndPoint>
                {
                    new IPEndPoint(IPAddress.Parse("192.168.56.13"), 134),
                    new IPEndPoint(IPAddress.Parse("192.168.56.12"), 134),
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
            var threwExceptionUpwards = false;
            var tcpManager = new StubITCPManager()
            {
                ConnectIPEndPoint = endPoint =>
                {
                        throw new SocketException();
                },
                Close = () => { calledStopTrue = true; },
                CreateIpEndPointsIEnumerableOfString = ipSring => new List<IPEndPoint>
                {
                    new IPEndPoint(IPAddress.Parse("192.168.56.13"), 134),
                    new IPEndPoint(IPAddress.Parse("192.168.56.12"), 134),
                }

            };
            var client = new Client(tcpManager);

            //act
            try
            {

                client.Start(this.runSettings);
            }
            catch (Exception)
            {
                threwExceptionUpwards = true;
            }
            //assert
            Assert.IsTrue(calledStopTrue);
            Assert.IsTrue(threwExceptionUpwards);
        }

        [TestMethod]
        public void Client_Stop_ClosesManagerOnStop()
        {
            //arrange
            var calledCloseManager = false;
            var tcpManager = new StubITCPManager()
            {
                Close = () => calledCloseManager = true,
                CreateIpEndPointsIEnumerableOfString = ipSring => new List<IPEndPoint>() { new IPEndPoint(IPAddress.Parse("192.168.56.13"), 134) }

            };

            var client = new Client(tcpManager);
            //act
            client.Stop();

            //assert
            Assert.IsTrue(calledCloseManager);
        }

        //TODO TEST invalid ip in 
    }
}
