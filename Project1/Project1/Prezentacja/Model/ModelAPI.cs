using Logika;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Model
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        int Diameter { get; }
    }

    public class BallChaneEventArgs : EventArgs //przechowuje informacje o zmianie stanu obiektu reprezentuj¹cego pi³kê
    {
        public IBall Ball { get; set; }
    }

    public abstract class ModelAPI : IObservable<IBall>
    {
        public static ModelAPI CreateApi()
        {
            return new ModelBall();
        }

        public abstract void AddBallsAndStart(int ballsAmount);

        #region IObservable

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        #endregion IObservable

        internal class ModelBall : ModelAPI
        {
            private LogicAPI logicApi;
            public event EventHandler<BallChaneEventArgs> BallChanged;
            //deklaracja zdarzenia (event), które pozwala na subskrybowanie i powiadamianie zainteresowanych obiektów o zmianach w obiektach typu BallInModel
            //publiczny punkt dostêpowy do zdarzeñ zmian wartoœci obiektów typu IBall

            private IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
            //reprezentuje Ÿród³o zdarzeñ, które mog¹ byæ subskrybowane przez zewnêtrzne obiekty implementuj¹ce interfejs IObserver<IBall>
            //prywatne pole u¿ywane wewnêtrznie do implementacji logiki reakcji na te zmiany w klasie ModelBall

            private List<BallInModel> Balls = new List<BallInModel>();

            public ModelBall()
            {
                logicApi = logicApi ?? LogicAPI.CreateLayer();
                IDisposable observer = logicApi.Subscribe<int>(x => Balls[x - 1].Move(logicApi.getBallPositionX(x), logicApi.getBallPositionY(x)));
                eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
            }

            public override void AddBallsAndStart(int ballsAmount)
            {
                logicApi.AddBallsAndStart(ballsAmount);
                for (int i = 1; i <= ballsAmount; i++)
                {
                    BallInModel newBall = new BallInModel(logicApi.getBallPositionX(i), logicApi.getBallPositionY(i), logicApi.getBallRadius(i));
                    Balls.Add(newBall);
                }

                foreach (BallInModel ball in Balls)
                {
                    BallChanged?.Invoke(this, new BallChaneEventArgs() { Ball = ball });
                }

            }

            public override IDisposable Subscribe(IObserver<IBall> observer)
            {
                return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
                //Kiedy nast¹pi zdarzenie "BallChanged" (zmiana pi³ki), to metoda Subscribe wywo³a metodê OnNext na obserwatorze (tym, który subskrybuje). 
                //Metoda OnNext przeka¿e obiekt IBall zawarty w BallChaneEventArgs. Jeœli wyst¹pi b³¹d, metoda Subscribe wywo³a OnError, a jeœli strumieñ zdarzeñ 
                //zakoñczy siê, wywo³a metodê OnCompleted
            }
        }
    }
}