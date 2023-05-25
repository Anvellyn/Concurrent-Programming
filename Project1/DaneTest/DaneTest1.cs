using NUnit.Framework;
using Dane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaneTest
{
	public class DataTest
	{
		[SetUp]
		public void Setup()
		{
			DaneAbstractAPI api = DaneAbstractAPI.CreateDataApi();
			api.createBalls(3);
			Assert.AreEqual(api.getBallsAmount(), 3);
		}

		[Test]
		public void SpeedSetterTest()
		{
			DaneAbstractAPI api = DaneAbstractAPI.CreateDataApi();
			api.createBalls(1);
			api.setBallSpeed(1, 2, 2);
			Assert.AreEqual(api.getBallSpeedX(1), 2);
			Assert.AreEqual(api.getBallSpeedY(1), 2);
		}
	}
}