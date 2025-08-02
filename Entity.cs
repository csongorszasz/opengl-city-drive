using System.Numerics;
using Silk.NET.OpenGL;


namespace projekt;

public class Entity
{
    public Model Model;
    public Transform Transform;

    public Entity(ref Model model, float Scale)
    {
        Model = model;
        Transform = new Transform();
        Transform.Scale = Scale;
    }
    
    public Entity(ref Model model, float Scale, Vector3 Position, Vector3 Rotation)
    {
        Model = model;
        
        (var bboxMin, var bboxMax) = CalculateBoundingBox();
        Transform = new Transform(
            Position, 
            Scale, 
            Quaternion.CreateFromYawPitchRoll(MathHelper.DegreesToRadians(Rotation.X), MathHelper.DegreesToRadians(Rotation.Y), MathHelper.DegreesToRadians(Rotation.Z)),
            bboxMin,
            bboxMax
            );
    }
    
    private (Vector3 min, Vector3 max) CalculateBoundingBox()
    {
        return Model.CalculateBoundingBox();
    }
    
    public void Draw(GL gl, Matrix4x4 view, Matrix4x4 projection, ref Shader shader, Vector3 cameraPosition)
    {
        shader.Use();
        // TODO: set texture uniforms
        shader.SetUniform("uModel", Transform.ModelMatrix);
        shader.SetUniform("uView", view);
        shader.SetUniform("uProjection", projection);
        shader.SetUniform("lightColor", new Vector3(GameManager.LightIntensity, GameManager.LightIntensity, GameManager.LightIntensity));
        shader.SetUniform("lightPos", new Vector3(GameManager.SkyboxScale * -0, GameManager.SkyboxScale * 8000f, GameManager.SkyboxScale * -0f));
        shader.SetUniform("viewPos", cameraPosition);
        Model.Draw(ref shader);
    }
    
    public void Dispose()
    {
        Model.Dispose();
    }
}