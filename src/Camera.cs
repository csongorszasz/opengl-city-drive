using System;
using System.Numerics;

namespace projekt
{
    public class Camera
    {
        public CameraView[] Views;
        public CameraView CurrentView;
        public Vector3 Position { get; set; }
        public Vector3 Front { get; set; }

        public Vector3 Up { get; private set; }
        public float AspectRatio { get; set; }

        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; }

        private float Zoom = 45f;

        public Camera(Vector3 position, Vector3 front, Vector3 up, float aspectRatio, CameraView[] views)
        {
            Position = position;
            AspectRatio = aspectRatio;
            Front = front;
            Up = up;
            Views = views;
            CurrentView = Views[0];
        }

        private void ResetYawPitch()
        {
            Yaw = -90f;
            Pitch = 0f;
        }
        
        public void ModifyZoom(float zoomAmount)
        {
            //We don't want to be able to zoom in too close or too far away so clamp to these values
            Zoom = Math.Clamp(Zoom - zoomAmount, 1.0f, 45f);
        }

        public void ModifyDirection(float xOffset, float yOffset, GameManager gameManager)
        {
            Yaw += xOffset;
            Pitch -= yOffset;

            //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
            Pitch = Math.Clamp(Pitch, -89f, 89f);

            var cameraDirection = Vector3.Zero;
            cameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            cameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            cameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            
            Front = -Vector3.Normalize(cameraDirection);
            Front = Front with { Y = -Front.Y };
            // Front = Vector3.Transform(Front, Matrix4x4.CreateFromQuaternion(gameManager.Car.Transform.Rotation));
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom), AspectRatio, 0.1f, 20000.0f);
        }
        
        public void FollowObject(Transform target, bool onlyPosition)
        {
            Vector3 desiredPosition = CurrentView.GetDesiredPosition(target);
            Vector3 forward = CurrentView.GetDesiredFront(target);
            
            Position = Vector3.Lerp(Position, desiredPosition, CurrentView.LerpAmount);

            if (onlyPosition && CurrentView != Views[1])
                return;
            
            Front = forward;
            Up = Vector3.UnitY;
        }

        public void NextCarView()
        {
            CurrentView = Views[(Array.IndexOf(Views, CurrentView) + 1) % Views.Length];
        }

        public void Update(GameManager gameManager)
        {
            if (gameManager.IsDriving)
            {
                if (gameManager.IsCarBeingControlled)
                {
                    FollowObject(gameManager.Car.Transform, false);
                    ResetYawPitch();
                }
                else
                {
                    FollowObject(gameManager.Car.Transform, true);
                }
            }
        }
    }
}