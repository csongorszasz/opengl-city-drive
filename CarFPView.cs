using System.Numerics;

namespace projekt;

public class CarFPView : CameraView
{
    public CarFPView()
    {
        SetOffset(new Vector3(0.25f, 0.95f, 0.2f));
        SetBackView(false);
        SetLerpAmount(0.85f);
    }
}