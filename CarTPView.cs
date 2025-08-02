using System.Numerics;

namespace projekt;

public class CarTPView : CameraView
{
    public CarTPView()
    {
        SetOffset(new Vector3(0, 1.5f, -5f));
        SetBackView(false);
        SetLerpAmount(0.25f);
    }
}