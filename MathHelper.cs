using System;

namespace projekt
{
    public static class MathHelper
    {
        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }
        
        public static (double yaw, double pitch, double roll) QuaternionToEulerAngles(double w, double x, double y, double z)
        {
            // Yaw (ψ) - Rotation around the y-axis
            double yaw = Math.Atan2(2.0 * (x * y + w * z), 1.0 - 2.0 * (y * y + z * z));

            // Pitch (θ) - Rotation around the x-axis
            double pitch = Math.Asin(2.0 * (w * y - z * x));

            // Roll (φ) - Rotation around the z-axis
            double roll = Math.Atan2(2.0 * (w * x + y * z), 1.0 - 2.0 * (x * x + y * y));

            return (yaw, pitch, roll);
        }
    }
}