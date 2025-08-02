using System.Numerics;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace projekt;

public class GameManager
{
    private GL _gl;
    private IWindow _window;
    private IKeyboard _primaryKeyboard;
    private ModelManager _modelManager;
    private ShaderManager _shaderManager;
    
    public static Random random;

    public static float SkyboxScale = 1f;
    public static float LightIntensity = 0.1f;
    
    public bool HumansMoving = true;
    public int HumanSpawnArea = 100;
    public Vector2 HumanSpawnCorner1 = new Vector2(-35, -200);
    public Vector2 HumanSpawnCorner2 = new Vector2(35, 200);
    public int HumanCount = 100;
    public float HumanScale = 1f;
    
    public float CarScale = 0.1f;
    public float CarAcceleration = 0.05f;
    public float CarMaxSpeed = 100f;

    public bool IsDriving = true;
    public bool IsCarBeingControlled => Car.IsAccelerating || Car.IsBraking || Car.IsTurningLeft || Car.IsTurningRight;

    public Vector2 CityCorner1; // x, y
    public Vector2 CityCorner2; // x, y

    public bool IsSceneResetting;
    
    // Entities
    public Skybox Skybox;
    public Car Car;
    public List<Human> Humans;
    public City City;

    public List<Human> OldHumans;
    
    // Camera
    public Camera Camera;
    
    public GameManager(GL gl, IWindow window, IKeyboard primaryKeyboard, ModelManager modelManager, ShaderManager shaderManager)
    {
        random = new Random();
        _gl = gl;
        _window = window;
        _primaryKeyboard = primaryKeyboard;
        _modelManager = modelManager;
        _shaderManager = shaderManager;
        
        SetUpCamera();
        LoadScene1();
    }
    
    private void SetUpCamera()
    {
        var size = _window.FramebufferSize;
        Camera = new Camera(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY, (float)size.X / size.Y, 
            new CameraView[]
            {
                new CarTPView(),
                new CarRearView(),
                new CarFrontLeftWheelView(),
                new CarFrontRightWheelView(),
                new CarFPView()
            });
    }

    public void SetTPView()
    {
        Camera.CurrentView = Camera.Views[0];
    }
    
    public void SetRearView()
    {
        Camera.CurrentView = Camera.Views[1];
    }
    
    public void SetFrontLeftWheelView()
    {
        Camera.CurrentView = Camera.Views[2];
    }
    
    public void SetFrontRightWheelView()
    {
        Camera.CurrentView = Camera.Views[3];
    }
    
    public void SetFPView()
    {
        Camera.CurrentView = Camera.Views[4];
    }
    
    public void LoadScene1()
    {
        /* Create entities */
        LightIntensity = 0.1f;
        
        
        Skybox = new Skybox(ref _modelManager.SkyboxModel, SkyboxScale,new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0f, 0f, 0f));
        Car = new Car(ref _modelManager.CarModel, CarScale,new Vector3(0f, 5.7f, -300.0f), new Vector3(0f, 0f, 0f), CarAcceleration, CarMaxSpeed);
             
        Humans = new List<Human>();
        for (int i = 0; i < HumanCount; i++)
        {
            float posX = random.Next((int)HumanSpawnCorner1.X, (int)HumanSpawnCorner2.X);
            float posZ = random.Next((int)HumanSpawnCorner1.Y, (int)HumanSpawnCorner2.Y);
            float rotX = random.Next(360);
            Humans.Add(new Human(ref _modelManager.HumanModel, HumanScale,new Vector3(posX, 7f, posZ), new Vector3(rotX, 0f, 0f)));
        }

        City = new City(ref _modelManager.CityModel, 0.1f);
     
        /* Setup info */
        IsSceneResetting = false;
        CityCorner1 = new Vector2(964, 557);
        CityCorner2 = new Vector2(-964, -989);
    }
    
    public void Update(double deltaTime)
    {
        if (CheckWinCondition())
        {
            LightIntensity = 0.6f;
            Console.WriteLine("YOU WON!");
            ResetScene();
            return;
        }
        
        HandleInput(deltaTime);
        UpdateEntities(deltaTime);
        HandleCollisions();
        if (IsSceneResetting)
            return;
        Camera.Update(this);
        
        // Console.WriteLine($"Car Position: {Car.Transform.Position}");
    }
    
    private bool CheckWinCondition()
    {
        if (Car.Transform.Position.X > CityCorner1.X || Car.Transform.Position.X < CityCorner2.X ||
            Car.Transform.Position.Z > CityCorner1.Y || Car.Transform.Position.Z < CityCorner2.Y)
        {
            return true;
        }
        return false;
    }

    public static async Task InvokeMethodWithDelay(int delayMilliseconds, Action methodToInvoke)
    {
        await Task.Delay(delayMilliseconds);
        methodToInvoke?.Invoke();
    }
    
    private async void ResetScene()
    {
        IsSceneResetting = true;
        OldHumans = Humans;
        await InvokeMethodWithDelay(3000, () =>
        {
            LoadScene1();
            SetUpCamera();
        });
        IsSceneResetting = false;
    }
    
    private void HandleInput(double deltaTime)
    {
        var moveSpeed = 5.0f * (float) deltaTime;
        if (!IsDriving)
        {
            if (_primaryKeyboard.IsKeyPressed(Key.W))
            {
                //Move forwards
                Camera.Position += moveSpeed * Camera.Front;
            }

            if (_primaryKeyboard.IsKeyPressed(Key.S))
            {
                //Move backwards
                Camera.Position -= moveSpeed * Camera.Front;
            }

            if (_primaryKeyboard.IsKeyPressed(Key.A))
            {
                //Move left
                Camera.Position -= Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            }

            if (_primaryKeyboard.IsKeyPressed(Key.D))
            {
                //Move right
                Camera.Position += Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            }
        }
        else
        {
            // acceleration, braking
            if (_primaryKeyboard.IsKeyPressed(Key.Up))
            {
                Car.Accelerate();
            }
            else if (_primaryKeyboard.IsKeyPressed(Key.Down))
            {
                Car.Brake();
            }
            else
            {
                Car.IsAccelerating = false;
                Car.IsBraking = false;
            }
            
            // turning
            if (_primaryKeyboard.IsKeyPressed(Key.Left))
            {
                Car.TurnLeft();
            }
            else if (_primaryKeyboard.IsKeyPressed(Key.Right))
            {
                Car.TurnRight();
            }
            else
            {
                Car.IsTurningLeft = false;
                Car.IsTurningRight = false;
            }
        }
    }
    
    private void UpdateEntities(double deltaTime)
    {
        Car.Update((float) deltaTime);
            
        for (int i = 0; i < Humans.Count; i++)
        {
            if (HumansMoving)
            {
                Humans[i].Update((float)deltaTime);
            }
        }
    }
    
    private void HandleCollisions()
    {
        for (int i = 0; i < Humans.Count; i++)
        {
            if (Collision.CheckAABBCollision(Car.Transform, Humans[i].Transform))
            {
                ToggleHumanMovement();
                Humans[i].Transform.Rotation = Quaternion.CreateFromYawPitchRoll(random.Next(-180, 180), 90, 0);
                Console.WriteLine($"YOU LOST! Collision with Human nr.{i}");
                ResetScene();
                ToggleHumanMovement();
            }
        }
    }

    private void ToggleHumanMovement()
    {
        HumansMoving = !HumansMoving;
        foreach (var human in Humans)
        {
            human.timer.Enabled = HumansMoving;
        }
    }
    
    public void Render(double deltaTime)
    {
        var view = Camera.GetViewMatrix();
        var projection = Camera.GetProjectionMatrix();
            
        Skybox.Draw(_gl, view, projection, ref _shaderManager.LightingShader1, Camera.Position);
        
        Car.Draw(_gl, view, projection, ref _shaderManager.LightingShader1, Camera.Position);
        // Drawer.DrawBoundingBox(_gl, Matrix4x4.Identity, view, projection, Car.Transform.BBoxMin, Car.Transform.BBoxMax);

        if (!IsSceneResetting)
        {
            foreach (var human in Humans)
            {
                human.Draw(_gl, view, projection, ref _shaderManager.LightingShader1, Camera.Position);
                // Drawer.DrawBoundingBox(_gl, Matrix4x4.Identity, view, projection, human.Transform.BBoxMin, human.Transform.BBoxMax);
            }
        }
        else
        {
            foreach (var human in OldHumans)
            {
                human.Draw(_gl, view, projection, ref _shaderManager.LightingShader1, Camera.Position);
            }
        }

        City.Draw(_gl, view, projection, ref _shaderManager.LightingShader1, Camera.Position);
    }
    
    public void Dispose()
    {
        Skybox.Dispose();
        Car.Dispose();
        foreach (var human in Humans)
        {
            human.Dispose();
        }
        City.Dispose();
    }
}