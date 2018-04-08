
using Application;
using Application.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EchoApp.Tests
{
    [TestClass]
    public class ApplicationTest
    {
        private IEchoApp app;
        [TestInitialize]
        public void Intialize()
        {
            
            this.app = new StubIEchoApp
            {
                StartRunSettings = settings => { }
            };

        }

        [TestMethod]
        public void TestMethod1()
        {

        }
        /**
         *  Start called once
         */

         // send message

        ///
    }
}
