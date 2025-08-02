using System.Numerics;

namespace projekt;

public class Car : Entity
{
    public static float maxSteeringAngle = MathHelper.DegreesToRadians(30f);
    
    public static bool IsAccelerating = false;
    public static bool IsBraking = false;
    public static bool IsTurningLeft = false;
    public static bool IsTurningRight = false;
    
    public Car(ref Model model, float Scale, Vector3 Position, Vector3 Rotation, float Acceleration, float MaxSpeed) : base(ref model, Scale, Position, Rotation)
    {
        this.Acceleration = Acceleration;
        this.MaxSpeed = MaxSpeed;
        Velocity = Vector3.Zero;
    }

    private void Accelerate(float amount)
    {
        if (Speed < MaxSpeed)
        {
            Velocity += Transform.Forward * Acceleration * amount;
        }
        if (Speed > MaxSpeed)
        {
            Velocity = Vector3.Normalize(Velocity) * MaxSpeed;
        }
    }

    public void Accelerate()
    {
        Accelerate(2f);
        if (IsAccelerating)
            return;
        IsAccelerating = true;
        IsBraking = false;
    }
    
    private void Brake(float amount)
    {
        Velocity *= 1 - amount;
    }

    public void Brake()
    {
        Brake(0.01f);
        if (IsBraking)
            return;
        IsBraking = true;
        IsAccelerating = false;
    }
    
    private void Turn(float yawAmount)
    {
        Brake(0.01f);
        if (Speed > 0.5f)
        {
            float newYaw = (float) Math.Clamp((double) yawAmount, -maxSteeringAngle, maxSteeringAngle);
            Transform.Rotation = Quaternion.Multiply(Transform.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(newYaw)));
        }
    }
    
    public void TurnLeft()
    {
        Turn(1f);
        if (IsTurningLeft)
            return;
        IsTurningLeft = true;
        IsTurningRight = false;
    }
    
    public void TurnRight()
    {
        Turn(-1f);
        if (IsTurningRight)
            return;
        IsTurningRight = true;
        IsTurningLeft = false;
    }
    
    public void Update(float deltaTime)
    {
        // Update the car's position and direction
        Transform.Position += Velocity * deltaTime;
        // Engine break
        if (!IsAccelerating && !IsBraking)
            Brake(0.001f);
        
        if (Speed < 0.001f)
        {
            Velocity = Vector3.Zero;
        }
    }
    
    public float Acceleration;
    public float MaxSpeed;
    public Vector3 Velocity;
    public float Speed => Velocity.Length(); 
}