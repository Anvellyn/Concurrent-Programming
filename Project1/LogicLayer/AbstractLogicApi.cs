using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Dane;

namespace Logika
{
    public abstract class LogicAPI : IObserver<int>, IObservable<int>
    {
        public abstract void AddBallsAndStart(int BallsAmount);
        public abstract double getBallPositionX(int ballId);
        public abstract double getBallPositionY(int ballId);
        public abstract int getBallRadius(int ballId);

        public abstract IDisposable Subscribe(IObserver<int> observer);
        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(int value);


        //przekazuje siê argument typu DaneAbstractAPI, który jest wstrzykiwany do konstruktora klasy BusinessLogic, która dziedziczy po klasie LogicAPI, dependency injection
        public static LogicAPI CreateLayer(DaneAbstractAPI data = default(DaneAbstractAPI))
        {
            return new BusinessLogic(data == null ? DaneAbstractAPI.CreateDataApi() : data);
        }

        public class BallChaneEventArgs : EventArgs
        {
            public int ballId { get; set; }
        }

        private class BusinessLogic : LogicAPI, IObservable<int>
        {
            private readonly DaneAbstractAPI dataAPI;
            private IDisposable unsubscriber;
            static object _lock = new object();
            private IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
            public event EventHandler<BallChaneEventArgs> BallChanged;

            public BusinessLogic(DaneAbstractAPI dataAPI)
            {
                eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
                this.dataAPI = dataAPI;
                Subscribe(dataAPI);
            }

            public override double getBallPositionX(int ballId)
            {
                return this.dataAPI.getBallPositionX(ballId);
            }

            public override double getBallPositionY(int ballId)
            {
                return this.dataAPI.getBallPositionY(ballId);
            }

            public override int getBallRadius(int ballId)
            {
                return this.dataAPI.getBallRadius(ballId);
            }


            public override void AddBallsAndStart(int BallsAmount)
            {
                dataAPI.createBalls(BallsAmount);
            }

            #region observer

            public virtual void Subscribe(IObservable<int> provider)
            {
                if (provider != null)
                    unsubscriber = provider.Subscribe(this);
            }


            //OnNext jest wywo³ywana przez obserwowan¹ klasê, gdy pi³ka zostanie zaktualizowana
            //metoda zawiera logikê wykrywania kolizji miêdzy pi³kami i reagowania na nie
            //OnNext jest wywo³ywana przez obserwowan¹ klasê DaneAbstractAPI, gdy zmieni siê po³o¿enie pi³ki
            public override void OnNext(int value)
            {
                Monitor.Enter(_lock); //blokuje sekcjê krytyczn¹, zapobiega równoczesnym modyfikacjom zasobów przez wiele w¹tków
                try
                {
                    CollisionControler collisionControler = new CollisionControler(dataAPI.getBallPositionX(value), dataAPI.getBallPositionY(value), dataAPI.getBallSpeedX(value), dataAPI.getBallSpeedY(value), dataAPI.getBallRadius(value), 10);

                    for (int i = 1; i <= dataAPI.getBallsAmount(); i++)
                    {
                        if (value != i)
                        {
                            double otherBallX = dataAPI.getBallPositionX(i);
                            double otherBallY = dataAPI.getBallPositionY(i);
                            double otherBallSpeedX = dataAPI.getBallSpeedX(i);
                            double otherBallSpeedY = dataAPI.getBallSpeedY(i);
                            int otherBallRadius = dataAPI.getBallRadius(i);
                            double otherBallMass = dataAPI.getBallMass(i);

                            //Ten fragment kodu sprawdza kolizje miêdzy dwoma pi³kami w grze. Gdy pi³ka "value" koliduje z pi³k¹ "i" (inn¹ pi³k¹),
                            //to program wywo³uje metodê "IsCollision", aby sprawdziæ, czy w nastêpnym kroku pi³ki siê zderz¹. Jeœli tak, to program
                            //wywo³uje metodê "ImpulseSpeed" z kontrolera kolizji, aby uzyskaæ now¹ prêdkoœæ dla pi³ek po zderzeniu.
                            if (collisionControler.IsCollision(otherBallX + otherBallSpeedX, otherBallY + otherBallSpeedY, otherBallRadius, true))
                            {
                                if (!collisionControler.IsCollision(otherBallX, otherBallY, otherBallRadius, false))
                                {
                                    System.Diagnostics.Trace.WriteLine("Ball " + value + " hit ball " + i);

                                    Vector2[] newVelocity = collisionControler.ImpulseSpeed(otherBallX, otherBallY, otherBallSpeedX, otherBallSpeedY, otherBallMass);

                                    dataAPI.setBallSpeed(value, newVelocity[0].X, newVelocity[0].Y);
                                    dataAPI.setBallSpeed(i, newVelocity[1].Y, newVelocity[1].Y);
                                }
                            }
                        }
                    }

                    int boardSize = dataAPI.getBoardSize();

                    if (collisionControler.IsTouchingBoundariesX(boardSize))
                    {
                        dataAPI.setBallSpeed(value, -dataAPI.getBallSpeedX(value), dataAPI.getBallSpeedY(value));
                    }

                    if (collisionControler.IsTouchingBoundariesY(boardSize))
                    {
                        dataAPI.setBallSpeed(value, dataAPI.getBallSpeedX(value), -dataAPI.getBallSpeedY(value));
                    }
                    BallChanged?.Invoke(this, new BallChaneEventArgs() { ballId = value });
                }
                catch (SynchronizationLockException exception)
                {
                    throw new Exception("Checking collision synchronization lock not working", exception);
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }

            public override void OnCompleted()
            {
                Unsubscribe();
            }

            public override void OnError(Exception error)
            {
                throw error;
            }

            public virtual void Unsubscribe()
            {
                unsubscriber.Dispose();
            }

            #endregion

            #region observable
            public override IDisposable Subscribe(IObserver<int> observer)
            {
                return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.ballId), ex => observer.OnError(ex), () => observer.OnCompleted());
            }
            #endregion

        }
    }
}