using System.Numerics;

namespace projekt;

public class CameraView
{
    public Vector3 Offset { get; private set; }
    
    public bool BackView { get; private set; }
    
    public float LerpAmount { get; private set; }
    
    public CameraView()
    {
        Offset = new Vector3(0, 0, 0);
        BackView = false;
        LerpAmount = 0.1f;        
    }

    public void SetOffset(Vector3 offset)
    {
        Offset = offset;
    }
    
    public void SetBackView(bool backView)
    {
        BackView = backView;
    }
    
    public void SetLerpAmount(float lerpAmount)
    {
        LerpAmount = lerpAmount;
    }
    
    public void IncreaseDistance(float distance)
    {
        Offset = Offset with { Z = distance };
    }
    
    public void IncreaseHeight(float height)
    {
        Offset = Offset with { Y = height };
    }
    
    public void IncreaseHorizontalDistance(float distance)
    {
        Offset = Offset with { X = distance };
    }
    
    public void DecreaseDistance(float distance)
    {
        Offset = Offset with { Z = -distance };
    }
    
    public void DecreaseHeight(float height)
    {
        Offset = Offset with { Y = -height };
    }
    
    public void DecreaseHorizontalDistance(float distance)
    {
        Offset = Offset with { X = -distance };
    }

    public Vector3 GetDesiredPosition(Transform carTransform)
    {
        // Calculate the camera's position relative to the car's position and rotation
        Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(carTransform.Rotation);
        Vector3 relativePosition = Vector3.Transform(Offset, rotationMatrix);
        return carTransform.Position + relativePosition;
    }
    
    public Vector3 GetDesiredFront(Transform carTransform)
    {
        // Calculate the desired look-at direction based on the car's rotation
        Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(carTransform.Rotation);
        Vector3 desiredFront = Vector3.Transform(new Vector3(0, 0, (BackView ? -1 : 1)), rotationMatrix);
        return desiredFront;
    }
}