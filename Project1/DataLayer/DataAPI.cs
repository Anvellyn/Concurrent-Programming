using System;
using System.Collections.Generic;
using System.Threading;

namespace Dane
{
    public abstract class DaneAbstractAPI : IObserver<int>, IObservable<int>
    {
        public abstract double getBallPositionX(int ballId);
        public abstract double getBallPositionY(int ballId);
        public abstract int getBallRadius(int ballId);
        public abstract double getBallSpeedX(int ballId);
        public abstract double getBallSpeedY(int ballId);
        public abstract double getBallMass(int ballId);
        public abstract int getBoardSize();
        public abstract void setBallSpeed(int ballId, double speedX, double speedY);
        public abstract void createBalls(int ballsAmount);
        public abstract int getBallsAmount();
        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(int value);
        public abstract IDisposable Subscribe(IObserver<int> observer);
        public static DaneAbstractAPI CreateDataApi()
        {
            return new DaneApi();
        }
    }

    public class DaneApi : DaneAbstractAPI
    {
        private BallRepository ballRepository;
        private IDisposable unsubscriber;
        static object _lock = new object(); //zabezpieczenie przed równoczesnym dostêpem do listy obserwatorów
        private IList<IObserver<int>> observers;
        private Barrier barrier; //s³u¿y do zsynchronizowania w¹tków w momencie tworzenia nowych kul

        public DaneApi()
        {
            this.ballRepository = new BallRepository();

            observers = new List<IObserver<int>>();
        }

        public override double getBallPositionX(int ballId)
        {
            return this.ballRepository.getBall(ballId).positionX;
        }

        public override double getBallPositionY(int ballId)
        {
            return this.ballRepository.getBall(ballId).positionY;
        }

        public override int getBoardSize()
        {
            return ballRepository.boardSize;
        }

        public override double getBallMass(int ballId)
        {
            return this.ballRepository.getBall(ballId).Mass;
        }

        public override int getBallRadius(int ballId)
        {
            return this.ballRepository.getBall(ballId).Radius;
        }

        public override double getBallSpeedX(int ballId)
        {
            return this.ballRepository.getBall(ballId).shiftX;
        }

        public override double getBallSpeedY(int ballId)
        {
            return this.ballRepository.getBall(ballId).shiftY;
        }

        public override void setBallSpeed(int ballId, double speedX, double speedY)
        {
            this.ballRepository.getBall(ballId).shiftX = speedX;
            this.ballRepository.getBall(ballId).shiftY = speedY;
        }

        public override int getBallsAmount()
        {
            return ballRepository.balls.Count;
        }

        public override void createBalls(int ballsAmount)
        {
            barrier = new Barrier(ballsAmount);
            ballRepository.createBalls(ballsAmount);

            foreach (var ball in ballRepository.balls)
            {
                Subscribe(ball); //dodaje pi³kê jako obserwatora do listy obserwatorów
                ball.StartMoving();
            }
            //kiedy nowa kulka jest tworzona, jej w¹tek zaczyna siê poruszaæ i na koñcu wywo³uje metodê OnNext(int value) obiektu eventObservable
        }

        #region observer

        public virtual void Subscribe(IObservable<int> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public override void OnCompleted()
        {
            Unsubscribe();
        }

        public override void OnError(Exception error)
        {
            throw error;
        }

        public override void OnNext(int value)
        {
            barrier.SignalAndWait();
            //barrier.SignalAndWait() s³u¿y do zablokowania w¹tku do momentu, a¿ wszystkie w¹tki tworz¹ce kule do³¹cz¹ do punktu synchronizacji,
            //co zapewnia, ¿e ka¿da kula zostanie utworzona i gotowa do poruszania siê przed rozpoczêciem gry
            foreach (var observer in observers)
            {
                observer.OnNext(value);
            }

        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        #endregion

        #region provider

        public override IDisposable Subscribe(IObserver<int> observer)
        {
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

        #endregion
    }
}