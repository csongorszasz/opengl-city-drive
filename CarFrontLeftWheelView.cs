using System.Numerics;

namespace projekt;

public class CarFrontLeftWheelView : CameraView
{
    public CarFrontLeftWheelView()
    {
        SetOffset(new Vector3(0.85f, 0.35f, 0.3f));
        SetBackView(false);
        SetLerpAmount(0.95f);
    }
}