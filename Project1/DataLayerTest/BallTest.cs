using NUnit.Framework;
using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaneTest
{
    public class BallTest
    {
        [SetUp]
        public void Setup()
        {
            Ball Ball = new Ball(1);
            double pX = Ball.positionX;
            double pY = Ball.positionY;
            Ball.changeBallPosition();
            Assert.AreEqual(Ball.positionX, pX + Ball.shiftX);
            Assert.AreEqual(Ball.positionY, pY + Ball.shiftY);
        }

        [Test]
        public void PositionTest()
        {
            Ball Ball = new Ball(1);
            Assert.IsTrue(Ball.positionX <= 500 && Ball.positionX >= 1);
            Assert.IsTrue(Ball.positionY <= 500 && Ball.positionY >= 1);
            Assert.IsTrue(Ball.shiftX <= 5 && Ball.shiftX >= 3);
            Assert.IsTrue(Ball.shiftY <= 5 && Ball.shiftY >= 3);
        }
    }
}