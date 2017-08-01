using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GiftServer.Exceptions;
using GiftServer.Data;
namespace GiftServerTester
{
    [TestClass]
    public class EventTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidEventException))]
        public void Create_Null()
        {
            Event e = new Event(1, null);
        }
    }
}
