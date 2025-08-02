using System.Numerics;
using Timer = System.Timers.Timer;

namespace projekt;

public class Human : Entity
{
    public Timer timer;
    public static Random Random = new Random();
    
    public Human(ref Model model, float Scale, Vector3 Position, Vector3 Rotation) : base(ref model, Scale, Position, Rotation)
    {
        timer = new Timer(Random.Next(1, 5) * 1000);
        timer.Elapsed += (sender, args) =>
        {
            Transform.Rotation = Quaternion.CreateFromYawPitchRoll(Random.Next(-180, 180), 0, 0);
            timer.Interval = Random.Next(1, 5) * 1000;
            timer.Start();
        };
        timer.Start();
    }
    
    public void Update(float deltaTime)
    {
        this.Transform.Position += this.Transform.Forward * deltaTime * 10;
    }
}