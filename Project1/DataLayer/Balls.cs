using System;
using System.Collections.Generic;
using System.Text;

namespace Dane
{
    public class BallRepository
    {
        public List<Ball> balls { get; set; }
        public int boardSize { get; set; } = 555;

        public BallRepository()
        {
            balls = new List<Ball>();
        }

        public void createBalls(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                balls.Add(new Ball(i + 1));
            }
        }

        public Ball getBall(int id)
        {
            return balls[id - 1];
        }
    }
}