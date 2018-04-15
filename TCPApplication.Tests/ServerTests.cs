using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpDrivers.Fakes;

namespace TCPApplication.Tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void Server_SendMessage_ReturnsFalseIfException()
        {
            //arrange
            var tcpManager = new StubITcpServerManager
            {
                SendMessageIEnumerableOfTcpClientActionOfStringString = (sender, messgeHandler, message) => { throw new Exception(); }
            };
            var server = new Server(tcpManager);

            //act
           var result = server.SendMessage("test message");
            //assert
            // no excetion
            Assert.IsFalse(result);
        }
    }
}
