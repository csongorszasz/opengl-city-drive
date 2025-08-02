using System.Numerics;
using Silk.NET.OpenGL;

namespace projekt;

public class Skybox : Entity
{
    public Skybox(ref Model model, float Scale, Vector3 Position, Vector3 Rotation) : base(ref model, Scale, Position, Rotation)
    { }
}