using NUnit.Framework;
using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaneTest
{
    public class RepoTest
    {
        [SetUp]
        public void Setup()
        {
            BallRepository BallRepository = new BallRepository();
            BallRepository.createBalls(2);
            Assert.AreEqual(BallRepository.balls.Count, 2);
            Assert.AreEqual(BallRepository.balls[0].id, 1);
            Assert.AreEqual(BallRepository.balls[1].id, 2);
        }

        [Test]
        public void getBallsTest()
        {
            BallRepository BallRepository = new BallRepository();
            BallRepository.createBalls(2);
            Assert.AreEqual(BallRepository.getBall(1), BallRepository.balls[0]);
            Assert.AreEqual(BallRepository.getBall(2), BallRepository.balls[1]);
        }
    }
}