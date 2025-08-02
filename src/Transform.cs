using System.Numerics;

namespace projekt
{
    public class Transform
    {
        //A transform abstraction.
        //For a transform we need to have a position, a scale, and a rotation,
        //depending on what application you are creating, the type for these may vary.

        //Here we have chosen a vec3 for position, float for scale and quaternion for rotation,
        //as that is the most normal to go with.
        //Another example could have been vec3, vec3, vec4, so the rotation is an axis angle instead of a quaternion

        public Transform()
        {
            Position = new Vector3(0, 0, 0);
            Scale = 1f;
            Rotation = Quaternion.Identity;
            _bBoxMinOrigin = new Vector3(0, 0, 0);
            _bBoxMaxOrigin = new Vector3(0, 0, 0);
        }
        
        public Transform(Vector3 position, float scale, Quaternion rotation)
        {
            Position = position;
            Scale = scale;
            Rotation = rotation;
            _bBoxMinOrigin = new Vector3(0, 0, 0);
            _bBoxMaxOrigin = new Vector3(0, 0, 0);
        }
        
        public Transform(Vector3 position, float scale, Quaternion rotation, Vector3 bBoxMin, Vector3 bBoxMax)
        {
            Position = position;
            Scale = scale;
            Rotation = rotation;
            _bBoxMinOrigin = bBoxMin;
            _bBoxMaxOrigin = bBoxMax;
        }

        private readonly Vector3 _bBoxMinOrigin;
        private readonly Vector3 _bBoxMaxOrigin;

        // TODO: Rotate bboxes with car
        public Vector3 BBoxMin => _bBoxMinOrigin * Scale + Position;
        public Vector3 BBoxMax => _bBoxMaxOrigin * Scale + Position;
       
        public Vector3 Position { get; set; }

        public float Scale { get; set; }

        public Quaternion Rotation { get; set; }
        
        public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);
        
        public Matrix4x4 ModelMatrix => Matrix4x4.Identity * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);
    }
}
