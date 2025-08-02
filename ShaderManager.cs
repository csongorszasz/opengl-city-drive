using Silk.NET.OpenGL;

namespace projekt;

public class ShaderManager
{
    private GL _gl;

    public Shader FlatShader;
    public Shader LightingShader1;
    public Shader LightingShaderMaterials;

    public ShaderManager(GL gl)
    {
        _gl = gl;
        
        FlatShader = new Shader(_gl, "shader.vert", "flat.frag");
        LightingShader1 = new Shader(_gl, "shader.vert", "lighting.frag");
        // LightingShaderMaterials = new Shader(_gl, "shader.vert", "lighting_materialsfrag");
    }

    public void Dispose()
    {
        FlatShader.Dispose();
        LightingShader1.Dispose();
        // LightingShaderMaterials.Dispose();
    }
}