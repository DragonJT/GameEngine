
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Numerics;

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable IDE1006 // Naming Styles

partial class JsColor{
    public float r, g, b, a = 1.0f;

    static readonly Dictionary<string, (int r, int g, int b)> NamedColors = new()
    {
        { "black", (0, 0, 0) },
        { "white", (255, 255, 255) },
        { "red", (255, 0, 0) },
        { "green", (0, 128, 0) },
        { "blue", (0, 0, 255) },
        { "yellow", (255, 255, 0) },
        { "cyan", (0, 255, 255) },
        { "magenta", (255, 0, 255) },
    };

    public JsColor(string color)
    {
        int r = 0, g = 0, b = 0;

        // Check for hex format
        if (color.StartsWith("#"))
        {
            string hex = color.Substring(1);
            if (hex.Length == 3) // #RGB format
            {
                r = int.Parse(hex[0].ToString() + hex[0], NumberStyles.HexNumber);
                g = int.Parse(hex[1].ToString() + hex[1], NumberStyles.HexNumber);
                b = int.Parse(hex[2].ToString() + hex[2], NumberStyles.HexNumber);
            }
            else if (hex.Length == 6) // #RRGGBB format
            {
                r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            }
            else
            {
                throw new ArgumentException("Invalid hex color format");
            }
        }
        // Check for rgb() or rgba() format
        else if (color.StartsWith("rgb"))
        {
            var matches = Regex.Matches(color, @"\d+(\.\d+)?");
            if (matches.Count >= 3)
            {
                r = int.Parse(matches[0].Value);
                g = int.Parse(matches[1].Value);
                b = int.Parse(matches[2].Value);

                if (matches.Count == 4) // rgba format
                {
                    a = float.Parse(matches[3].Value, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                throw new ArgumentException("Invalid rgb or rgba color format");
            }
        }
        // Check for hsl() or hsla() format
        else if (color.StartsWith("hsl"))
        {
            var matches = MyRegex().Matches(color);
            if (matches.Count >= 3)
            {
                float h = float.Parse(matches[0].Value, CultureInfo.InvariantCulture);
                float s = float.Parse(matches[1].Value, CultureInfo.InvariantCulture) / 100;
                float l = float.Parse(matches[2].Value, CultureInfo.InvariantCulture) / 100;

                if (matches.Count == 4) // hsla format
                {
                    a = float.Parse(matches[3].Value, CultureInfo.InvariantCulture);
                }

                (r, g, b) = HslToRgb(h, s, l);
            }
            else
            {
                throw new ArgumentException("Invalid hsl or hsla color format");
            }
        }
        // Check for named colors
        else if (NamedColors.TryGetValue(color.ToLower(), out var namedColor))
        {
            r = namedColor.r;
            g = namedColor.g;
            b = namedColor.b;
        }
        else
        {
            throw new ArgumentException("Unsupported color format");
        }

        this.r = r / 255f;
        this.g = g / 255f;
        this.b = b / 255f;
    }

    private static (int r, int g, int b) HslToRgb(float h, float s, float l)
    {
        float c = (1 - System.Math.Abs(2 * l - 1)) * s;
        float x = c * (1 - System.Math.Abs((h / 60) % 2 - 1));
        float m = l - c / 2;

        float r = 0, g = 0, b = 0;
        if (h < 60) { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else if (h < 360) { r = c; g = 0; b = x; }

        return ((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
    }

    [GeneratedRegex(@"\d+(\.\d+)?")]
    private static partial Regex MyRegex();
}

static class ConvertJavascriptInput{
    public static string? KeyToString(int key, bool shift){
        if(key >= 32 && key < 128){
            if(!shift && char.IsAsciiLetterUpper((char)key)){
                return char.ToLower((char)key).ToString();
            }
            return ((char)key).ToString();
        }
        else{
            return key switch
            {
                GLFW.GLFW_KEY_LEFT => "ArrowLeft",
                GLFW.GLFW_KEY_RIGHT => "ArrowRight",
                GLFW.GLFW_KEY_UP => "ArrowUp",
                GLFW.GLFW_KEY_DOWN => "ArrowDown",
                GLFW.GLFW_KEY_ENTER => "Enter",
                GLFW.GLFW_KEY_BACKSPACE => "Backspace",
                GLFW.GLFW_KEY_CAPS_LOCK => "CapsLock",
                _ => null,
            };
        }
    }
}

public static class Input{
    public static float mousex = 0;
    public static float mousey = 0;
    static HashSet<string> keys = [];
    
    public static bool GetKey(string key){
        return keys.Contains(key);
    }

    internal static void KeyCallback(int key, int scancode, int action, int mods){
        bool shift = (mods & GLFW.GLFW_MOD_SHIFT) != 0;
        var jsKey = ConvertJavascriptInput.KeyToString(key, shift);
        if(jsKey != null){
            if(action == GLFW.GLFW_PRESS){
                keys.Add(jsKey);
            }
            else if(action == GLFW.GLFW_RELEASE){
                keys.Remove(jsKey);
            }
        }
    }
}

public static class Math{
    public const float PI = 3.14159265359f;

    public static float cos(float radians){
        return (float)System.Math.Cos(radians);
    }   

    public static float sin(float radians){
        return (float)System.Math.Sin(radians);
    }
}

public static class console{
    public static void log(string text){
        Console.WriteLine(text);
    }
}

public static class Graphics{    
    static List<Vector2> points = [];

    public static float GetWidth(){
        return Program.fontRenderer!.ScreenWidth;
    }

    public static float GetHeight(){
        return Program.fontRenderer!.ScreenHeight;
    }

    public static void Clear(string color){
        FillRect(0, 0, GetWidth(), GetHeight(), color);
    }

    public static void AddPoint(float x, float y){
        points.Add(new Vector2(x, y));
    }

    public static void Fill(string color){
        Program.fontRenderer!.FillPoly(points, new JsColor(color));
        points.Clear();
    }

    public static void FillText(float x, float y, string text, float fontSize, string color){
        Program.fontRenderer!.FillText(x, y, text, fontSize / 200f, new JsColor(color));
    }

    public static void FillRect(float x, float y, float width, float height, string color){
        Program.fontRenderer!.FillRect(x, y, width, height, new JsColor(color));
    }
}

class RoslynRunner
{
    public static Action<string>? keyup;
    public static Action<string>? keydown;
    public static Action<float,float>? mousemove;
    public static Action<int>? mousedown;
    public static Action<int>? mouseup;
    public static Action<float>? draw;

    static T? CreateDelegate<T>(Type type, string name) where T:Delegate{
        var method = type.GetMethod(name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);
        if(method != null){
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }
        return null;
    }

    public static void Run(string projectName, string csharpCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(csharpCode);
        var runtime = AppDomain.CurrentDomain.GetAssemblies().First(a=>a.GetName().Name == "System.Runtime");

        var compilation = CSharpCompilation.Create("DynamicAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
                MetadataReference.CreateFromFile(runtime.Location)
            )
            .AddSyntaxTrees(syntaxTree);

        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => 
                    diagnostic.IsWarningAsError || 
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    Console.Error.WriteLine(diagnostic.ToString());
                }
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());
                var type = assembly.GetType(projectName)!;

                keydown = CreateDelegate<Action<string>>(type, "KeyDown");
                keyup = CreateDelegate<Action<string>>(type, "KeyUp");
                mousemove = CreateDelegate<Action<float,float>>(type, "MouseMove");
                mousedown = CreateDelegate<Action<int>>(type, "MouseDown");
                mouseup = CreateDelegate<Action<int>>(type, "MouseUp");
                draw = CreateDelegate<Action<float>>(type, "Draw");
            }
        }
    }
}


static class Program{
    public static Buffer memory = new (5000);
    public static IntPtr window;
    public static IntPtr monitor;
    public static FontRenderer? fontRenderer;

    static void KeyCallback(IntPtr window, int key, int scancode, int action, int mods){
        Input.KeyCallback(key, scancode, action, mods);
        bool shift = (mods & GLFW.GLFW_MOD_SHIFT) != 0;
        var jsKey = ConvertJavascriptInput.KeyToString(key, shift);
        if(jsKey!=null){ 
            if(action == GLFW.GLFW_PRESS || action == GLFW.GLFW_REPEAT){
                RoslynRunner.keydown?.Invoke(jsKey);
            }
            else{
                RoslynRunner.keyup?.Invoke(jsKey);
            }
        }
    }

    static void CursorPosCallback(IntPtr window, double xpos, double ypos){
        var scale = GLFWHelper.GetMonitorContentScale();
        Input.mousex = ((float)xpos)/scale.x;
        Input.mousey = ((float)ypos)/scale.y;
        RoslynRunner.mousemove?.Invoke(Input.mousex, Input.mousey);
    }

    static void MouseButtonCallback(IntPtr window, int button, int action, int mods){
        if(action == GLFW.GLFW_PRESS || action == GLFW.GLFW_RELEASE){
            RoslynRunner.mousedown?.Invoke(button-1);
        }
        else{
            RoslynRunner.mouseup?.Invoke(button-1);
        }
    }

    static void Main(){
        string projectName = "Asteroids";
        if(GLFW.glfwInit() == 0){
            throw new Exception("Can't init glfw");
        }
        monitor = GLFW.glfwGetPrimaryMonitor();
        window = GLFW.glfwCreateWindow(2000,1400,projectName, IntPtr.Zero, IntPtr.Zero);
        GLFW.glfwMakeContextCurrent(window);
        GL.Init(GLFW.glfwGetProcAddress);

        GLFWHelper.SetKeyCallback(KeyCallback);
        GLFWHelper.SetCursorPosCallback(CursorPosCallback);
        GLFWHelper.SetMouseButtonCallback(MouseButtonCallback);
        var mousePosition = GLFWHelper.GetCursorPosition();
        CursorPosCallback(window, mousePosition.x, mousePosition.y);

        var code = File.ReadAllText("Projects/"+projectName+".cs");
        RoslynRunner.Run(projectName, code);
        CSharp2Javascript.Run(projectName, code);
        fontRenderer = new FontRenderer("Fonts/Roboto-Medium.ttf", 2048, 0.1f);
        System.Diagnostics.Stopwatch sw = new ();
        sw.Start();

        while(GLFW.glfwWindowShouldClose(window) == 0){
            sw.Stop();
            var deltaTime = (float)sw.Elapsed.TotalSeconds;
            sw.Reset();
            sw.Start();
            memory.size = 0;
            GL.glClearColor(0.1f,0.1f,0.1f,1);
            GL.glClear(GL.GL_COLOR_BUFFER_BIT);
            var windowSize = GLFWHelper.GetWindowSize();
            GL.glViewport(0,0,windowSize.x, windowSize.y);

            RoslynRunner.draw?.Invoke(deltaTime);
            fontRenderer.Draw();
            
            GLFW.glfwSwapBuffers(window);
            GLFW.glfwPollEvents();
        }
    }
}