using NUnit.Framework;
using Logika;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaneTest
{
    public class CollisionControllerTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CollisionTest()
        {
            ; CollisionControler CC = new CollisionControler(1, 1, 5, 5, 10, 10);
            Assert.IsTrue(CC.IsCollision(21, 1, 10, true));
            Assert.IsTrue(CC.IsCollision(20, 1, 10, false));
            Assert.IsFalse(CC.IsCollision(26, 1, 10, true));
            Assert.IsFalse(CC.IsCollision(22, 1, 10, false));
        }

        [Test]
        public void XYBoundaryTest()
        {
            CollisionControler CC = new CollisionControler(1, 1, 3, 3, 10, 10);
            Assert.IsTrue(CC.IsTouchingBoundariesX(3));
            Assert.IsTrue(CC.IsTouchingBoundariesY(3));
            Assert.IsFalse(CC.IsTouchingBoundariesX(5));
            Assert.IsFalse(CC.IsTouchingBoundariesY(5));
        }
    }
}