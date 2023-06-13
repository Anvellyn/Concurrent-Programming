using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dane
{
	public class Ball : IObservable<int>
	{
		public int id { get; }
		public double positionX { get; private set; }
		public double positionY { get; private set; }
		public double shiftX { get; set; }
		public double shiftY { get; set; }
		public int Radius { get; } = 12;
		public double Mass { get; } = 10;
		internal readonly IList<IObserver<int>> observers;

		private Task BallThread;

		Random rng = new Random();

		public double generateRandomDouble(double min, double max)
		{
			return rng.NextDouble() * (max - min) + min;
		}

		public Ball(int id)
		{
			this.id = id;
			observers = new List<IObserver<int>>();

			this.positionX = generateRandomDouble(1, 500);
			this.positionY = generateRandomDouble(1, 500);

			this.shiftX = generateRandomDouble(3, 5);
			this.shiftY = generateRandomDouble(3, 5);
		}
		public void StartMoving()
		{
			this.BallThread = new Task(MoveBall);
			BallThread.Start();
		}

        //przesuwa pi³kê na planszy, zmieniaj¹c jej po³o¿enie
        public void MoveBall()
		{
			while (true)
			{
				changeBallPosition();

				foreach (var observer in observers.ToList())
				{
					if (observer != null)
					{
						observer.OnNext(id);
					}
				}
				System.Threading.Thread.Sleep(25);
			}
		}

		public void changeBallPosition()
		{
			positionX += shiftX;
			positionY += shiftY;
		}

		public IDisposable Subscribe(IObserver<int> observer)
        { //nowy obserwator zostanie dodany do listy observers
            if (!observers.Contains(observer))
				observers.Add(observer);
			return new Unsubscriber(observers, observer);
		}

		private class Unsubscriber : IDisposable
		{
			private IList<IObserver<int>> _observers;
			private IObserver<int> _observer; 

			public Unsubscriber
			(IList<IObserver<int>> observers, IObserver<int> observer)
			{
				_observers = observers;
				_observer = observer;
			}

			public void Dispose()
			{
				if (_observer != null && _observers.Contains(_observer))
					_observers.Remove(_observer);
			}
		}
	}
}