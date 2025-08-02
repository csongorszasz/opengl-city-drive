using Silk.NET.OpenGL;

namespace projekt;

public class ModelManager
{
    private GL _gl;
    
    // models
    public Model SkyboxModel;
    public Model CarModel;
    public Model HumanModel;
    public Model CityModel;
    
    public ModelManager(GL gl)
    {
        _gl = gl;
        
        /* Load the models */
            
        // Load skybox
        SkyboxModel = new Model(gl, "Resources/final/skybox_blue_cloudy/skybox_blue_cloudy.obj");

        // Load car
        CarModel = new Model(gl, "Resources/final/cybertruck/cybertruck.obj");
        
        // Load human
        HumanModel = new Model(gl, "Resources/final/gentleman/gentleman.obj");
        
        // Load city
        CityModel = new Model(gl, "Resources/final/city/city.obj");
    }

    public void Dispose()
    {
        SkyboxModel.Dispose();
        CarModel.Dispose();
        HumanModel.Dispose();
        CityModel.Dispose();
    }
}