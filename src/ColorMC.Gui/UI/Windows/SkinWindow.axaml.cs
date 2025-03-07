using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Threading;
using ColorMC.Gui.SkinModel;
using ColorMC.Gui.UIBinding;
using ColorMC.Gui.Utils.LaunchSetting;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace ColorMC.Gui.UI.Windows;

public partial class SkinWindow : Window
{
    public SkinWindow()
    {
        InitializeComponent();

        this.Init();
        Icon = App.Icon;
        Rectangle1.MakeResizeDrag(this);

        ComboBox1.Items = UserBinding.GetSkinType();

        Skin.SetWindow(this);

        Text1.Text = Skin.Info;

        ComboBox2.Items = BaseBinding.GetSkinRotateName();

        ComboBox1.SelectionChanged += ComboBox1_SelectionChanged;
        ComboBox2.SelectionChanged += ComboBox2_SelectionChanged;
        Button1.Click += Button1_Click;
        Button2.Click += Button2_Click;
        Button3.Click += Button3_Click;
        Button4.Click += Button4_Click;
        Button5.Click += Button5_Click;
        CheckBox1.Click += CheckBox1_Click;
        CheckBox2.Click += CheckBox2_Click;
        CheckBox3.Click += CheckBox3_Click;

        Opened += SkinWindow_Opened;
        Closed += SkinWindow_Closed;

        App.PicUpdate += Update;
        App.SkinLoad += App_SkinLoad;

        Slider1.PropertyChanged += Slider1_PropertyChanged;
        Slider2.PropertyChanged += Slider2_PropertyChanged;
        Slider3.PropertyChanged += Slider3_PropertyChanged;

        ComboBox2.SelectedIndex = 0;

        Check();
        Update();
    }

    private void CheckBox3_Click(object? sender, RoutedEventArgs e)
    {
        Skin.SetAnimation(CheckBox3.IsChecked == true);
    }

    private void CheckBox2_Click(object? sender, RoutedEventArgs e)
    {
        Skin.SetCapeDisplay(CheckBox2.IsChecked == true);
    }

    private void Button5_Click(object? sender, RoutedEventArgs e)
    {
        switch (ComboBox2.SelectedIndex)
        {
            case 0:
                Skin.ArmRotate.X = 0;
                Skin.ArmRotate.Y = 0;
                Skin.ArmRotate.Z = 0;
                break;
            case 1:
                Skin.LegRotate.X = 0;
                Skin.LegRotate.Y = 0;
                Skin.LegRotate.Z = 0;
                break;
            case 2:
                Skin.HeadRotate.X = 0;
                Skin.HeadRotate.Y = 0;
                Skin.HeadRotate.Z = 0;
                break;
            default:
                return;
        }
        Slider1.Value = 0;
        Slider2.Value = 0;
        Slider3.Value = 0;
        Skin.InvalidateVisual();
    }

    private void Slider3_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "Value")
        {
            switch (ComboBox2.SelectedIndex)
            {
                case 0:
                    Skin.ArmRotate.Z = (float)Slider2.Value;
                    break;
                case 1:
                    Skin.LegRotate.Z = (float)Slider3.Value;
                    break;
                case 2:
                    Skin.HeadRotate.Z = (float)Slider3.Value;
                    break;
                default:
                    return;
            }
            Skin.InvalidateVisual();
        }
    }

    private void Slider2_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "Value")
        {
            switch (ComboBox2.SelectedIndex)
            {
                case 0:
                    Skin.ArmRotate.Y = (float)Slider2.Value;
                    break;
                case 1:
                    Skin.LegRotate.Y = (float)Slider2.Value;
                    break;
                case 2:
                    Skin.HeadRotate.Y = (float)Slider2.Value;
                    break;
                default:
                    return;
            }
            Skin.InvalidateVisual();
        }
    }

    private void Slider1_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "Value")
        {
            switch (ComboBox2.SelectedIndex)
            {
                case 0:
                    Skin.ArmRotate.X = (float)Slider1.Value;
                    break;
                case 1:
                    Skin.LegRotate.X = (float)Slider1.Value;
                    break;
                case 2:
                    Skin.HeadRotate.X = (float)Slider1.Value;
                    break;
                default:
                    return;
            }
            Skin.InvalidateVisual();
        }
    }

    private void ComboBox2_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Vector3 rotate;
        switch (ComboBox2.SelectedIndex)
        {
            case 0:
                rotate = Skin.ArmRotate;
                Slider1.Minimum = 0;
                Slider2.Minimum = -360;
                Slider3.Minimum = 0;
                Slider3.IsEnabled = false;
                break;
            case 1:
                rotate = Skin.LegRotate;
                Slider1.Minimum = 0;
                Slider2.Minimum = -360;
                Slider3.Minimum = 0;
                Slider3.IsEnabled = false;
                break;
            case 2:
                rotate = Skin.HeadRotate;
                Slider3.IsEnabled = true;
                Slider1.Minimum = -360;
                Slider2.Minimum = -360;
                Slider3.Minimum = -360;
                break;
            default:
                return;
        }

        Slider1.Value = rotate.X;
        Slider2.Value = rotate.Y;
        Slider3.Value = rotate.Z;
    }

    private void CheckBox1_Click(object? sender, RoutedEventArgs e)
    {
        Skin.SetTopDisplay(CheckBox1.IsChecked == true);
    }

    private async void Button4_Click(object? sender, RoutedEventArgs e)
    {
        var res = await BaseBinding.OpSave(this, "保存皮肤", ".png", "skin.png");
        if (res != null)
        {
            await UserBinding.SkinImage.SaveAsPngAsync(res.GetPath());
            Info2.Show("已保存");
        }
    }

    private void SkinWindow_Opened(object? sender, EventArgs e)
    {
        var temp = Matrix.CreateRotation(Math.PI);
        Skin.RenderTransform = new ImmutableTransform(temp);
    }

    private void App_SkinLoad()
    {
        Skin.ChangeSkin();

        Dispatcher.UIThread.Post(Check);
    }

    private void Button3_Click(object? sender, RoutedEventArgs e)
    {
        UserBinding.EditSkin(this);
    }

    private void Button2_Click(object? sender, RoutedEventArgs e)
    {
        Skin.Reset();
    }

    private void Check()
    {
        if (Skin.HaveSkin)
        {
            Button4.IsEnabled = true;
        }
        else
        {
            Button4.IsEnabled = false;
        }
    }

    private void SkinWindow_Closed(object? sender, EventArgs e)
    {
        App.PicUpdate -= Update;

        App.SkinWindow = null;

        Skin.SetWindow(null);
        Skin.Close();
    }

    private void ComboBox1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ComboBox1.SelectedIndex == (int)Skin.steveModelType)
            return;
        if (ComboBox1.SelectedIndex == (int)SkinType.Unkonw)
        {
            Info.Show(Localizer.Instance["SkinWindow.Info1"]);
            ComboBox1.SelectedIndex = (int)Skin.steveModelType;
            return;
        }
        Skin.ChangeType(ComboBox1.SelectedIndex);
    }

    private async void Button1_Click(object? sender, RoutedEventArgs e)
    {
        await UserBinding.LoadSkin();
    }

    public void Update()
    {
        App.Update(this, Image_Back, Grid1);

        Skin.InvalidateVisual();
    }
}

public class SkinRender : Control
{
    class SkinAnimation : IDisposable
    {
        private Thread thread;
        private bool run;
        private bool start;
        private Semaphore semaphore = new(0, 2);
        private int frame = 0;
        private SkinRender Render;

        public Vector3 Arm;
        public Vector3 Leg;
        public Vector3 Head;

        public SkinAnimation(SkinRender render)
        {
            Render = render;
            thread = new(Tick)
            {
                Name = "ColorMC-SkinAnimation"
            };
            run = true;
            thread.Start();

            Arm.X = 40;
        }

        public void Dispose()
        {
            run = false;
            thread.Join();
        }

        public void Start()
        {
            start = true;
            semaphore.Release();
        }

        public void Pause()
        {
            start = false;
        }

        private void Tick()
        {
            while (run)
            {
                semaphore.WaitOne();
                while (start)
                {
                    frame++;
                    if (frame > 120)
                    {
                        frame = 0;
                    }

                    if (frame <= 60)
                    {
                        //0 360
                        //-180 180
                        Arm.Y = (frame * 6) - 180;
                        //0 180
                        //90 -90
                        Leg.Y = 90 - (frame * 3);
                        //-30 30
                        if (Render.steveModelType == SkinType.NewSlim)
                        {
                            Head.Z = 0;
                            Head.X = frame - 30;
                        }
                        else
                        {
                            Head.X = 0;
                            Head.Z = frame - 30;
                        }
                    }
                    else
                    {
                        //360 720
                        //180 -180
                        Arm.Y = 540 - (frame * 6);
                        //180 360
                        //-90 90
                        Leg.Y = (frame * 3) - 270;
                        //30 -30
                        if (Render.steveModelType == SkinType.NewSlim)
                        {
                            Head.Z = 0;
                            Head.X = 90 - frame;
                        }
                        else
                        {
                            Head.X = 0;
                            Head.Z = 90 - frame;
                        }
                    }

                    Dispatcher.UIThread.InvokeAsync(Render.InvalidateVisual).Wait();

                    Thread.Sleep(10);
                }
            }
        }
    }

    class Win : GameWindow
    {
        public Win() : base(new(), new()
        {
            StartVisible = false
        })
        {

        }
    }

    public record VAOItem
    {
        public int VertexBufferObject;
        public int IndexBufferObject;
        public int VertexArrayObject;
    }

    public record ModelVAO
    {
        public VAOItem Head = new();
        public VAOItem Body = new();
        public VAOItem LeftArm = new();
        public VAOItem RightArm = new();
        public VAOItem LeftLeg = new();
        public VAOItem RightLeg = new();
        public VAOItem Cape = new();
    }

    private string VertexShaderSource =>
@"
#version 330 core

layout (location = 0) in vec3 a_position;
layout (location = 1) in vec2 a_texCoord;
layout (location = 2) in vec3 a_normal;

uniform mat4 model;
uniform mat4 projection;
uniform mat4 view;
uniform mat4 self;

out vec3 normalIn;
out vec2 texIn;
out vec3 fragPosIn;

void main()
{
    texIn = a_texCoord;
	
    mat4 temp = view * model * self;

    fragPosIn = vec3(temp * vec4(a_position, 1.0f));
    normalIn = mat3(transpose(inverse(temp))) * a_normal;

	gl_Position = projection * temp * vec4(a_position, 1.0);
}
";

    private string FragmentShaderSource =>
@"
#version 330 core
uniform sampler2D texture0;

uniform vec3 lightColor;

in vec3 fragPosIn;
in vec3 normalIn;
in vec2 texIn;

out vec4 FragColor;

void main()
{
    float ambientStrength = 0.1f;
    vec3 ambient = ambientStrength * lightColor;
 
    vec3 norm = normalize(normalIn);
    vec3 lightDir = normalize(-fragPosIn);
    float diff = max(dot(norm, lightDir), 0.0f);
    vec3 diffuse = diff * lightColor;
 
    vec3 result = (ambient + diffuse);

    FragColor = texture(texture0, texIn) * vec4(result, 1.0f);
}
";

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private struct Vertex
    {
        public Vector3 Position;
        public Vector2 UV;
        public Vector3 Normal;
    }

    public bool HaveSkin { get; private set; } = false;
    private bool HaveCape = false;
    private bool SwitchModel = false;
    private bool SwitchSkin = false;
    private bool TopDisplay = true;
    private bool CapeDisplay = true;
    private bool Animation = true;

    private float Dis = 1;
    private Vector2 RotXY;
    private Vector2 DiffXY;

    private Vector2 XY;
    private Vector2 SaveXY;
    private Vector2 LastXY;

    private int texture;
    private int texture1;

    public SkinType steveModelType { get; private set; }
    private int steveModelDrawOrder;

    private int _vertexShader;
    private int _fragmentShader;
    private int _shaderProgram;

    public Vector3 ArmRotate;
    public Vector3 LegRotate;
    public Vector3 HeadRotate;

    private Vector2 LastSize;
    private WriteableBitmap bitmap;

    public string Info = "";

    private readonly ModelVAO NormalVAO = new();
    private readonly ModelVAO TopVAO = new();

    private SkinWindow Window;
    private Win Window1;
    private SkinAnimation skina;

    public SkinRender()
    {
        Window1 = new();

        skina = new(this);
        skina.Start();

        Init();
    }

    public void SetWindow(SkinWindow window)
    {
        Window = window;
    }

    public void Init()
    {
        GL.ClearColor(0, 0, 0, 1);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.CullFace(CullFaceMode.Back);

        CheckError();

        Info = $"Renderer: {GL.GetString(StringName.Renderer)} Version: {GL.GetString(StringName.Version)}";

        _vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_vertexShader, VertexShaderSource);
        CompileShader(_vertexShader);

        _fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_fragmentShader, FragmentShaderSource);
        CompileShader(_vertexShader);

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, _vertexShader);
        GL.AttachShader(_shaderProgram, _fragmentShader);
        LinkProgram(_shaderProgram);

        InitVAO(NormalVAO);
        InitVAO(TopVAO);

        texture = GL.GenTexture();
        texture1 = GL.GenTexture();

        CheckError();

        PointerWheelChanged += OpenGlPageControl_PointerWheelChanged;
        PointerPressed += OpenGlPageControl_PointerPressed;
        PointerReleased += OpenGlPageControl_PointerReleased;
        PointerMoved += OpenGlPageControl_PointerMoved;

        LoadSkin();
    }

    private static void InitVAOItem(VAOItem item)
    {
        int[] temp1 = new int[2];
        GL.GenBuffers(2, temp1);
        item.VertexBufferObject = temp1[0];
        item.IndexBufferObject = temp1[1];
    }

    private static void InitVAO(ModelVAO vao)
    {
        int[] temp1 = new int[7];

        GL.GenVertexArrays(7, temp1);

        vao.Head.VertexArrayObject = temp1[0];
        vao.Body.VertexArrayObject = temp1[1];
        vao.LeftArm.VertexArrayObject = temp1[2];
        vao.RightArm.VertexArrayObject = temp1[3];
        vao.LeftLeg.VertexArrayObject = temp1[4];
        vao.RightLeg.VertexArrayObject = temp1[5];
        vao.Cape.VertexArrayObject = temp1[6];

        InitVAOItem(vao.Head);
        InitVAOItem(vao.Body);
        InitVAOItem(vao.LeftArm);
        InitVAOItem(vao.RightArm);
        InitVAOItem(vao.LeftLeg);
        InitVAOItem(vao.RightLeg);
        InitVAOItem(vao.Cape);
    }

    public void ChangeType(int index)
    {
        steveModelType = (SkinType)index;

        SwitchModel = true;

        InvalidateVisual();
    }

    public void ChangeSkin()
    {
        SwitchSkin = true;

        InvalidateVisual();
    }

    public void Reset()
    {
        Dis = 1;
        RotXY.X = 0;
        RotXY.Y = 0;
        DiffXY.X = 0;
        DiffXY.Y = 0;
        XY.X = 0;
        XY.Y = 0;
        SaveXY.X = 0;
        SaveXY.Y = 0;
        LastXY.X = 0;
        LastXY.Y = 0;

        InvalidateVisual();
    }

    private static void LoadTex(Image<Rgba32> image, int tex)
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, tex);

        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear
        );
        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest
        );
        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge
        );
        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge
        );

        var pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width,
           image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private void LoadSkin()
    {
        if (UserBinding.SkinImage == null)
        {
            HaveSkin = false;
            Dispatcher.UIThread.Post(() =>
            {
                Window.Label1.IsVisible = true;
                Window.Label1.Content = Localizer.Instance["SkinWindow.Info2"];
            });
            return;
        }

        steveModelType = SkinUtil.GetTextType(UserBinding.SkinImage);
        if (steveModelType == SkinType.Unkonw)
        {
            HaveSkin = false;
            Dispatcher.UIThread.Post(() =>
            {
                Window.Label1.IsVisible = true;
                Window.Label1.Content = Localizer.Instance["SkinWindow.Info3"];
            });
            return;
        }
        LoadTex(UserBinding.SkinImage, texture);
        if (UserBinding.CapeIamge != null)
        {
            LoadTex(UserBinding.CapeIamge, texture1);
            HaveCape = true;
        }
        else
        {
            HaveCape = false;
        }

        CheckError();

        LoadModel();

        HaveSkin = true;
        Dispatcher.UIThread.Post(() =>
        {
            Window.ComboBox1.SelectedIndex = (int)steveModelType;
            Window.Label1.IsVisible = false;
        });
    }

    private unsafe void PutVAO(VAOItem vao, ModelItem model, float[] uv)
    {
        float[] vertices =
        {
            0.0f,  0.0f, -1.0f,
            0.0f,  0.0f, -1.0f,
            0.0f,  0.0f, -1.0f,
            0.0f,  0.0f, -1.0f,

            0.0f,  0.0f,  1.0f,
            0.0f,  0.0f,  1.0f,
            0.0f,  0.0f,  1.0f,
            0.0f,  0.0f,  1.0f,

            -1.0f,  0.0f,  0.0f,
            -1.0f,  0.0f,  0.0f,
            -1.0f,  0.0f,  0.0f,
            -1.0f,  0.0f,  0.0f,

            1.0f,  0.0f,  0.0f,
            1.0f,  0.0f,  0.0f,
            1.0f,  0.0f,  0.0f,
            1.0f,  0.0f,  0.0f,

            0.0f,  1.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  1.0f,  0.0f,

            0.0f, -1.0f,  0.0f,
            0.0f, -1.0f,  0.0f,
            0.0f, -1.0f,  0.0f,
            0.0f, -1.0f,  0.0f,
        };

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(vao.VertexArrayObject);

        int a_Position = GL.GetAttribLocation(_shaderProgram, "a_position");
        int a_texCoord = GL.GetAttribLocation(_shaderProgram, "a_texCoord");
        int a_normal = GL.GetAttribLocation(_shaderProgram, "a_normal");

        GL.DisableVertexAttribArray((uint)a_Position);
        GL.DisableVertexAttribArray((uint)a_texCoord);
        GL.DisableVertexAttribArray((uint)a_normal);

        int size = model.Model.Length / 3;

        var points = new Vertex[size];

        for (var primitive = 0; primitive < size; primitive++)
        {
            var srci = primitive * 3;
            var srci1 = primitive * 2;
            points[primitive] = new Vertex
            {
                Position = new(model.Model[srci], model.Model[srci + 1], model.Model[srci + 2]),
                UV = new(uv[srci1], uv[srci1 + 1]),
                Normal = new(vertices[srci], vertices[srci + 1], vertices[srci + 2])
            };
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, vao.VertexBufferObject);
        var vertexSize = Marshal.SizeOf<Vertex>();
        fixed (void* pdata = points)
            GL.BufferData(BufferTarget.ArrayBuffer, points.Length * vertexSize,
                new IntPtr(pdata), BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, vao.IndexBufferObject);
        fixed (void* pdata = model.Point)
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                model.Point.Length * sizeof(ushort), new IntPtr(pdata), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer((uint)a_Position, 3, VertexAttribPointerType.Float,
            false, 8 * sizeof(float), 0);
        GL.VertexAttribPointer((uint)a_texCoord, 2, VertexAttribPointerType.Float,
            false, 8 * sizeof(float), 3 * sizeof(float));
        GL.VertexAttribPointer((uint)a_normal, 3, VertexAttribPointerType.Float,
            false, 8 * sizeof(float), 5 * sizeof(float));

        GL.EnableVertexAttribArray((uint)a_Position);
        GL.EnableVertexAttribArray((uint)a_texCoord);
        GL.EnableVertexAttribArray((uint)a_normal);

        GL.BindVertexArray(0);

        CheckError();
    }

    private unsafe void LoadModel()
    {
        var steve = new Steve3DModel();
        var stevetex = new Steve3DTexture();

        var normal = steve.GetSteve(steveModelType);
        var top = steve.GetSteveTop(steveModelType);
        var tex = stevetex.GetSteveTexture(steveModelType);
        var textop = stevetex.GetSteveTextureTop(steveModelType);

        steveModelDrawOrder = normal.Head.Point.Length;

        PutVAO(NormalVAO.Head, normal.Head, tex.Head);
        PutVAO(NormalVAO.Body, normal.Body, tex.Body);
        PutVAO(NormalVAO.LeftArm, normal.LeftArm, tex.LeftArm);
        PutVAO(NormalVAO.RightArm, normal.RightArm, tex.RightArm);
        PutVAO(NormalVAO.LeftLeg, normal.LeftLeg, tex.LeftLeg);
        PutVAO(NormalVAO.RightLeg, normal.RightLeg, tex.RightLeg);
        PutVAO(NormalVAO.Cape, normal.Cape, tex.Cape);

        PutVAO(TopVAO.Head, top.Head, textop.Head);
        if (steveModelType != SkinType.Old)
        {
            PutVAO(TopVAO.Head, top.Head, textop.Head);
            PutVAO(TopVAO.Body, top.Body, textop.Body);
            PutVAO(TopVAO.LeftArm, top.LeftArm, textop.LeftArm);
            PutVAO(TopVAO.RightArm, top.RightArm, textop.RightArm);
            PutVAO(TopVAO.LeftLeg, top.LeftLeg, textop.LeftLeg);
            PutVAO(TopVAO.RightLeg, top.RightLeg, textop.RightLeg);
        }
    }

    private void OpenGlPageControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!HaveSkin)
            return;

        var po = e.GetCurrentPoint(this);
        var pos = e.GetPosition(this);

        if (po.Properties.IsLeftButtonPressed)
        {
            DiffXY.X = (float)pos.X - RotXY.Y;
            DiffXY.Y = -(float)pos.Y + RotXY.X;
        }
        else if (po.Properties.IsRightButtonPressed)
        {
            LastXY.X = (float)pos.X;
            LastXY.Y = (float)pos.Y;
        }

        InvalidateVisual();
    }

    private void OpenGlPageControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!HaveSkin)
            return;

        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            SaveXY.X = XY.X;
            SaveXY.Y = XY.Y;
        }

        InvalidateVisual();
    }

    private void OpenGlPageControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!HaveSkin)
            return;

        var po = e.GetCurrentPoint(this);

        if (po.Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(this);
            RotXY.Y = (float)point.X - DiffXY.X;
            RotXY.X = (float)point.Y + DiffXY.Y;
        }
        else if (po.Properties.IsRightButtonPressed)
        {
            var point = e.GetPosition(this);
            XY.X = (-(LastXY.X - (float)point.X) / 100) + SaveXY.X;
            XY.Y = ((LastXY.Y - (float)point.Y) / 100) + SaveXY.Y;
        }

        InvalidateVisual();
    }

    private void OpenGlPageControl_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!HaveSkin)
            return;

        if (e.Delta.Y > 0)
        {
            Dis += 0.1f;
        }
        else if (e.Delta.Y < 0)
        {
            Dis -= 0.1f;
        }

        InvalidateVisual();
    }

    private void DrawCape()
    {
        if (HaveCape && CapeDisplay)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture1);

            var modelLoc = GL.GetUniformLocation(_shaderProgram, "self");

            var model = Matrix4.CreateTranslation(0, -2f * CubeC.Value, -CubeC.Value * 0.1f) *
               Matrix4.CreateRotationX((float)(10.8 * Math.PI / 180)) *
               Matrix4.CreateTranslation(0, 1.6f * CubeC.Value,
               -CubeC.Value * 0.5f);
            GL.UniformMatrix4(modelLoc, false, ref model);

            GL.BindVertexArray(NormalVAO.Cape.VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }

    private void DrawNormal()
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        var modelLoc = GL.GetUniformLocation(_shaderProgram, "self");
        var model = Matrix4.Identity;
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(NormalVAO.Body.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
            DrawElementsType.UnsignedShort, IntPtr.Zero);

        model = Matrix4.CreateTranslation(0, CubeC.Value, 0) *
               Matrix4.CreateRotationZ((Animation ? skina.Head.X : HeadRotate.X) / 360) *
               Matrix4.CreateRotationX((Animation ? skina.Head.Y : HeadRotate.Y) / 360) *
               Matrix4.CreateRotationY((Animation ? skina.Head.Z : HeadRotate.Z) / 360) *
               Matrix4.CreateTranslation(0, CubeC.Value * 1.5f, 0);
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(NormalVAO.Head.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                 DrawElementsType.UnsignedShort, IntPtr.Zero);

        if (steveModelType == SkinType.NewSlim)
        {
            model = Matrix4.CreateTranslation(CubeC.Value / 2, -(1.375f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? skina.Arm.X : ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? skina.Arm.Y : ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (1.375f * CubeC.Value) - (CubeC.Value / 2), 1.375f * CubeC.Value, 0);
        }
        else
        {
            model = Matrix4.CreateTranslation(CubeC.Value / 2, -(1.5f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? skina.Arm.X : ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? skina.Arm.Y : ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (1.5f * CubeC.Value) - (CubeC.Value / 2), 1.5f * CubeC.Value, 0);
        }
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(NormalVAO.LeftArm.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

        if (steveModelType == SkinType.NewSlim)
        {
            model = Matrix4.CreateTranslation(-CubeC.Value / 2, -(1.375f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? -skina.Arm.X : -ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? -skina.Arm.Y : -ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (-1.375f * CubeC.Value) + (CubeC.Value / 2), 1.375f * CubeC.Value, 0);
        }
        else
        {
            model = Matrix4.CreateTranslation(-CubeC.Value / 2, -(1.5f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? -skina.Arm.X : -ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? -skina.Arm.Y : -ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (-1.5f * CubeC.Value) + (CubeC.Value / 2), 1.5f * CubeC.Value, 0);
        }
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(NormalVAO.RightArm.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                 DrawElementsType.UnsignedShort, IntPtr.Zero);

        model = Matrix4.CreateTranslation(0, -1.5f * CubeC.Value, 0) *
               Matrix4.CreateRotationZ((Animation ? skina.Leg.X : LegRotate.X) / 360) *
               Matrix4.CreateRotationX((Animation ? skina.Leg.Y : LegRotate.Y) / 360) *
               Matrix4.CreateTranslation(CubeC.Value * 0.5f, -CubeC.Value * 1.5f, 0);
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(NormalVAO.LeftLeg.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

        model = Matrix4.CreateTranslation(0, -1.5f * CubeC.Value, 0) *
               Matrix4.CreateRotationZ((Animation ? -skina.Leg.X : -LegRotate.X) / 360) *
               Matrix4.CreateRotationX((Animation ? -skina.Leg.Y : -LegRotate.Y) / 360) *
               Matrix4.CreateTranslation(-CubeC.Value * 0.5f, -CubeC.Value * 1.5f, 0);
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(NormalVAO.RightLeg.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private void DrawTop()
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        var modelLoc = GL.GetUniformLocation(_shaderProgram, "self");
        var model = Matrix4.Identity;
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(TopVAO.Body.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                 DrawElementsType.UnsignedShort, IntPtr.Zero);

        model = Matrix4.CreateTranslation(0, CubeC.Value, 0) *
               Matrix4.CreateRotationZ((Animation ? skina.Head.X : HeadRotate.X) / 360) *
               Matrix4.CreateRotationX((Animation ? skina.Head.Y : HeadRotate.Y) / 360) *
               Matrix4.CreateRotationY((Animation ? skina.Head.Z : HeadRotate.Z) / 360) *
               Matrix4.CreateTranslation(0, CubeC.Value * 1.5f, 0);
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(TopVAO.Head.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                 DrawElementsType.UnsignedShort, IntPtr.Zero);

        if (steveModelType == SkinType.NewSlim)
        {
            model = Matrix4.CreateTranslation(CubeC.Value / 2, -(1.375f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? skina.Arm.X : ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? skina.Arm.Y : ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (1.375f * CubeC.Value) - (CubeC.Value / 2), 1.375f * CubeC.Value, 0);
        }
        else
        {
            model = Matrix4.CreateTranslation(CubeC.Value / 2, -(1.5f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? skina.Arm.X : ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? skina.Arm.Y : ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (1.5f * CubeC.Value) - (CubeC.Value / 2), 1.5f * CubeC.Value, 0);
        }
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(TopVAO.LeftArm.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

        if (steveModelType == SkinType.NewSlim)
        {
            model = Matrix4.CreateTranslation(-CubeC.Value / 2, -(1.375f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? -skina.Arm.X : -ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? -skina.Arm.Y : -ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (-1.375f * CubeC.Value) + (CubeC.Value / 2), 1.375f * CubeC.Value, 0);
        }
        else
        {
            model = Matrix4.CreateTranslation(-CubeC.Value / 2, -(1.5f * CubeC.Value), 0) *
                Matrix4.CreateRotationZ((Animation ? -skina.Arm.X : -ArmRotate.X) / 360) *
                Matrix4.CreateRotationX((Animation ? -skina.Arm.Y : -ArmRotate.Y) / 360) *
                Matrix4.CreateTranslation(
                    (-1.5f * CubeC.Value) + (CubeC.Value / 2), 1.5f * CubeC.Value, 0);
        }
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(TopVAO.RightArm.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

        model = Matrix4.CreateTranslation(0, -1.5f * CubeC.Value, 0) *
               Matrix4.CreateRotationZ((Animation ? skina.Leg.X : LegRotate.X) / 360) *
               Matrix4.CreateRotationX((Animation ? skina.Leg.Y : LegRotate.Y) / 360) *
               Matrix4.CreateTranslation(CubeC.Value * 0.5f, -CubeC.Value * 1.5f, 0);
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(TopVAO.LeftLeg.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                 DrawElementsType.UnsignedShort, IntPtr.Zero);

        model = Matrix4.CreateTranslation(0, -1.5f * CubeC.Value, 0) *
               Matrix4.CreateRotationZ((Animation ? -skina.Leg.X : -LegRotate.X) / 360) *
               Matrix4.CreateRotationX((Animation ? -skina.Leg.Y : -LegRotate.Y) / 360) *
               Matrix4.CreateTranslation(-CubeC.Value * 0.5f, -CubeC.Value * 1.5f, 0);
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(TopVAO.RightLeg.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, steveModelDrawOrder,
                DrawElementsType.UnsignedShort, IntPtr.Zero);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (SwitchSkin)
        {
            LoadSkin();
            SwitchSkin = false;
        }
        if (SwitchModel)
        {
            LoadModel();
            SwitchModel = false;
        }

        if (!HaveSkin)
            return;

        int x = (int)Bounds.Width;
        int y = (int)Bounds.Height;

        var screen = Window.PlatformImpl?.Screen.ScreenFromWindow(Window.PlatformImpl);
        if (screen != null)
        {
            x = (int)(Bounds.Width * screen.Scaling);
            y = (int)(Bounds.Height * screen.Scaling);
        }

        if (LastSize.X != x || LastSize.Y != y)
        {
            LastSize = new(x, y);
            bitmap?.Dispose();
            bitmap = new WriteableBitmap(new PixelSize(x, y), new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Rgba8888, AlphaFormat.Premul);
        }

        if (Window1.Size.X != x || Window1.Size.Y != y)
        {
            Window1.Size = new(x, y);
        }

        GL.Viewport(0, 0, x, y);

        if (App.BackBitmap != null)
        {
            GL.ClearColor(0, 0, 0, 0.2f);
        }
        else
        {
            GL.ClearColor(0, 0, 0, 1);
        }

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        CheckError();
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.UseProgram(_shaderProgram);
        CheckError();

        var viewLoc = GL.GetUniformLocation(_shaderProgram, "view");
        var projectionLoc = GL.GetUniformLocation(_shaderProgram, "projection");
        var modelLoc = GL.GetUniformLocation(_shaderProgram, "model");

        var lightColorLoc = GL.GetUniformLocation(_shaderProgram, "lightColor");

        var projection = Matrix4.CreatePerspectiveFieldOfView(
            (float)(Math.PI / 4), (float)(Bounds.Width / Bounds.Height),
            0.001f, 1000);

        var view = Matrix4.LookAt(new(0, 0, 7), new(), new(0, 1, 0));

        var model = Matrix4.CreateRotationX(RotXY.X / 360)
                    * Matrix4.CreateRotationY(RotXY.Y / 360)
                    * Matrix4.CreateTranslation(new(XY.X, XY.Y, 0))
                    * Matrix4.CreateScale(Dis);

        GL.Uniform3(lightColorLoc, 1.0f, 1.0f, 1.0f);       //光源：默认为白色

        GL.UniformMatrix4(viewLoc, false, ref view);
        GL.UniformMatrix4(modelLoc, false, ref model);
        GL.UniformMatrix4(projectionLoc, false, ref projection);

        CheckError();

        DrawNormal();

        DrawCape();

        if (TopDisplay)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthMask(false);

            DrawTop();

            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
        }

        using (var fb = bitmap.Lock())
        {
            GL.ReadPixels(0, 0, x, y, PixelFormat.Rgba, PixelType.UnsignedByte, fb.Address);
        }

        context.DrawImage(bitmap, new(0, 0, x, y), Bounds);

        CheckError();
    }

    private static void CompileShader(int shader)
    {
        // Try to compile the shader
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }

    private static void LinkProgram(int program)
    {
        // We link the program
        GL.LinkProgram(program);

        // Check for linking errors
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
            GL.GetProgramInfoLog(program, out var error);
            App.ShowError("着色程序错误", new Exception(error));
        }
    }

    private static void CheckError()
    {
        ErrorCode err;
        while ((err = GL.GetError()) != ErrorCode.NoError)
        {
            Console.WriteLine(err);
        }
    }

    private static void DeleteVAOItem(VAOItem item)
    {
        GL.DeleteBuffer(item.VertexBufferObject);
        GL.DeleteBuffer(item.IndexBufferObject);
    }

    private static void DeleteVAO(ModelVAO vao)
    {
        GL.DeleteVertexArray(vao.Head.VertexArrayObject);
        GL.DeleteVertexArray(vao.Body.VertexArrayObject);
        GL.DeleteVertexArray(vao.LeftArm.VertexArrayObject);
        GL.DeleteVertexArray(vao.RightArm.VertexArrayObject);
        GL.DeleteVertexArray(vao.LeftLeg.VertexArrayObject);
        GL.DeleteVertexArray(vao.RightLeg.VertexArrayObject);

        DeleteVAOItem(vao.Head);
        DeleteVAOItem(vao.Body);
        DeleteVAOItem(vao.LeftArm);
        DeleteVAOItem(vao.RightArm);
        DeleteVAOItem(vao.LeftLeg);
        DeleteVAOItem(vao.RightLeg);
    }

    public void Close()
    {
        Window1.Close();
        bitmap?.Dispose();

        // Unbind everything
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        // Delete all resources.
        DeleteVAO(NormalVAO);
        DeleteVAO(TopVAO);

        GL.DeleteProgram(_shaderProgram);
        GL.DeleteShader(_fragmentShader);
        GL.DeleteShader(_vertexShader);

        Window1.Dispose();
    }

    public void SetTopDisplay(bool value)
    {
        TopDisplay = value;

        InvalidateVisual();
    }

    public void SetCapeDisplay(bool value)
    {
        CapeDisplay = value;

        InvalidateVisual();
    }

    public void SetAnimation(bool value)
    {
        Animation = value;
        if (value)
        {
            skina.Start();
        }
        else
        {
            skina.Pause();
        }

        InvalidateVisual();
    }
}
