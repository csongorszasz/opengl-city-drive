using System.Numerics;

namespace projekt;

public class CarRearView : CameraView
{
    public CarRearView()
    {
        SetOffset(new Vector3(0, 1.5f, 7f));
        SetBackView(true);
        SetLerpAmount(0.25f);
    }
}