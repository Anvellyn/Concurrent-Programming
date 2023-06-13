using NUnit.Framework;
using Logika;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaneTest
{
    public class LogicAPITest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CollisionTest()
        {
            LogicAPI Logicapi = LogicAPI.CreateLayer();
            Logicapi.AddBallsAndStart(1);
            Assert.AreEqual(Logicapi.getBallRadius(1), 12);
        }
    }
}