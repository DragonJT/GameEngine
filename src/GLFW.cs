using System.Runtime.InteropServices;
using SebText.FontLoading;
using System.Numerics;

static class Kernel32{
    [DllImport("kernel32.dll")]
    public static extern void RtlZeroMemory(IntPtr dst, UIntPtr length);
}

delegate void CursorPosCallbackDelegate(IntPtr window, double xpos, double ypos);
delegate void MouseButtonCallbackDelegate(IntPtr window, int button, int action, int mods);
delegate void KeyCallbackDelegate(IntPtr window, int key, int scancode, int action, int mods);
delegate void CharCallbackDelegate(IntPtr window, uint codepoint);

static class GLFW{
    private const string DllFilePath = @"glfw3.dll";

    public const int GLFW_OPENGL_API = 0x00030001;
    public const int GLFW_OPENGL_ES_API = 0x00030002;

    public const int GLFW_KEY_SPACE = 32;
    public const int GLFW_KEY_APOSTROPHE = 39;  /* ' */
    public const int GLFW_KEY_COMMA = 44;  /* , */
    public const int GLFW_KEY_MINUS = 45;  /* - */
    public const int GLFW_KEY_PERIOD = 46;  /* . */
    public const int GLFW_KEY_SLASH = 47;  /* / */
    public const int GLFW_KEY_0 = 48;
    public const int GLFW_KEY_1 = 49;
    public const int GLFW_KEY_2 = 50;
    public const int GLFW_KEY_3 = 51;
    public const int GLFW_KEY_4 = 52;
    public const int GLFW_KEY_5 = 53;
    public const int GLFW_KEY_6 = 54;
    public const int GLFW_KEY_7 = 55;
    public const int GLFW_KEY_8 = 56;
    public const int GLFW_KEY_9 = 57;
    public const int GLFW_KEY_SEMICOLON = 59;  /* ; */
    public const int GLFW_KEY_EQUAL = 61;  /* = */
    public const int GLFW_KEY_A = 65;
    public const int GLFW_KEY_B = 66;
    public const int GLFW_KEY_C = 67;
    public const int GLFW_KEY_D = 68;
    public const int GLFW_KEY_E = 69;
    public const int GLFW_KEY_F = 70;
    public const int GLFW_KEY_G = 71;
    public const int GLFW_KEY_H = 72;
    public const int GLFW_KEY_I = 73;
    public const int GLFW_KEY_J = 74;
    public const int GLFW_KEY_K = 75;
    public const int GLFW_KEY_L = 76;
    public const int GLFW_KEY_M = 77;
    public const int GLFW_KEY_N = 78;
    public const int GLFW_KEY_O = 79;
    public const int GLFW_KEY_P = 80;
    public const int GLFW_KEY_Q = 81;
    public const int GLFW_KEY_R = 82;
    public const int GLFW_KEY_S = 83;
    public const int GLFW_KEY_T = 84;
    public const int GLFW_KEY_U = 85;
    public const int GLFW_KEY_V = 86;
    public const int GLFW_KEY_W = 87;
    public const int GLFW_KEY_X = 88;
    public const int GLFW_KEY_Y = 89;
    public const int GLFW_KEY_Z = 90;
    public const int GLFW_KEY_LEFT_BRACKET = 91;  /* [ */
    public const int GLFW_KEY_BACKSLASH = 92;  /* \ */
    public const int GLFW_KEY_RIGHT_BRACKET = 93;  /* ] */
    public const int GLFW_KEY_GRAVE_ACCENT = 96;  /* ` */
    public const int GLFW_KEY_WORLD_1 = 161; /* non-US #1 */
    public const int GLFW_KEY_WORLD_2 = 162; /* non-US #2 */

    /* Function keys */
    public const int GLFW_KEY_ESCAPE = 256;
    public const int GLFW_KEY_ENTER = 257;
    public const int GLFW_KEY_TAB = 258;
    public const int GLFW_KEY_BACKSPACE = 259;
    public const int GLFW_KEY_INSERT = 260;
    public const int GLFW_KEY_DELETE = 261;
    public const int GLFW_KEY_RIGHT = 262;
    public const int GLFW_KEY_LEFT = 263;
    public const int GLFW_KEY_DOWN = 264;
    public const int GLFW_KEY_UP = 265;
    public const int GLFW_KEY_PAGE_UP = 266;
    public const int GLFW_KEY_PAGE_DOWN = 267;
    public const int GLFW_KEY_HOME = 268;
    public const int GLFW_KEY_END = 269;
    public const int GLFW_KEY_CAPS_LOCK = 280;
    public const int GLFW_KEY_SCROLL_LOCK = 281;
    public const int GLFW_KEY_NUM_LOCK = 282;
    public const int GLFW_KEY_PRINT_SCREEN = 283;
    public const int GLFW_KEY_PAUSE = 284;
    public const int GLFW_KEY_F1 = 290;
    public const int GLFW_KEY_F2 = 291;
    public const int GLFW_KEY_F3 = 292;
    public const int GLFW_KEY_F4 = 293;
    public const int GLFW_KEY_F5 = 294;
    public const int GLFW_KEY_F6 = 295;
    public const int GLFW_KEY_F7 = 296;
    public const int GLFW_KEY_F8 = 297;
    public const int GLFW_KEY_F9 = 298;
    public const int GLFW_KEY_F10 = 299;
    public const int GLFW_KEY_F11 = 300;
    public const int GLFW_KEY_F12 = 301;
    public const int GLFW_KEY_F13 = 302;
    public const int GLFW_KEY_F14 = 303;
    public const int GLFW_KEY_F15 = 304;
    public const int GLFW_KEY_F16 = 305;
    public const int GLFW_KEY_F17 = 306;
    public const int GLFW_KEY_F18 = 307;
    public const int GLFW_KEY_F19 = 308;
    public const int GLFW_KEY_F20 = 309;
    public const int GLFW_KEY_F21 = 310;
    public const int GLFW_KEY_F22 = 311;
    public const int GLFW_KEY_F23 = 312;
    public const int GLFW_KEY_F24 = 313;
    public const int GLFW_KEY_F25 = 314;
    public const int GLFW_KEY_KP_0 = 320;
    public const int GLFW_KEY_KP_1 = 321;
    public const int GLFW_KEY_KP_2 = 322;
    public const int GLFW_KEY_KP_3 = 323;
    public const int GLFW_KEY_KP_4 = 324;
    public const int GLFW_KEY_KP_5 = 325;
    public const int GLFW_KEY_KP_6 = 326;
    public const int GLFW_KEY_KP_7 = 327;
    public const int GLFW_KEY_KP_8 = 328;
    public const int GLFW_KEY_KP_9 = 329;
    public const int GLFW_KEY_KP_DECIMAL = 330;
    public const int GLFW_KEY_KP_DIVIDE = 331;
    public const int GLFW_KEY_KP_MULTIPLY = 332;
    public const int GLFW_KEY_KP_SUBTRACT = 333;
    public const int GLFW_KEY_KP_ADD = 334;
    public const int GLFW_KEY_KP_ENTER = 335;
    public const int GLFW_KEY_KP_EQUAL = 336;
    public const int GLFW_KEY_LEFT_SHIFT = 340;
    public const int GLFW_KEY_LEFT_CONTROL = 341;
    public const int GLFW_KEY_LEFT_ALT = 342;
    public const int GLFW_KEY_LEFT_SUPER = 343;
    public const int GLFW_KEY_RIGHT_SHIFT = 344;
    public const int GLFW_KEY_RIGHT_CONTROL = 345;
    public const int GLFW_KEY_RIGHT_ALT = 346;
    public const int GLFW_KEY_RIGHT_SUPER = 347;
    public const int GLFW_KEY_MENU = 348;

    public const int GLFW_MOD_SHIFT = 0x0001;
    public const int GLFW_MOD_CONTROL = 0x0002;

    public const int GLFW_RELEASE = 0;
    public const int GLFW_PRESS = 1;
    public const int GLFW_REPEAT = 2;
    public const int GLFW_MOUSE_BUTTON_1 = 0;
    public const int GLFW_MOUSE_BUTTON_2 = 1;
    public const int GLFW_MOUSE_BUTTON_LEFT = GLFW_MOUSE_BUTTON_1;
    public const int GLFW_MOUSE_BUTTON_RIGHT = GLFW_MOUSE_BUTTON_2;

    public const int GLFW_CLIENT_API = 0x00022001;
    public const int GLFW_CONTEXT_VERSION_MAJOR = 0x00022002;
    public const int GLFW_CONTEXT_VERSION_MINOR = 0x00022003;

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr glfwGetPrimaryMonitor();

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwGetMonitorContentScale(IntPtr monitor, IntPtr xscale, IntPtr yscale);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static int glfwInit();

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr glfwCreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwMakeContextCurrent(IntPtr window);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static int glfwWindowShouldClose(IntPtr window);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwPollEvents();

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr glfwGetProcAddress (string procname);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwWindowHint (int hint, int value);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwSwapBuffers (IntPtr window);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwGetWindowSize(IntPtr window, IntPtr width, IntPtr height);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwSetMouseButtonCallback(IntPtr window, IntPtr mouseButtonCallback);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwSetCursorPosCallback(IntPtr window, IntPtr cursorPosCallback);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwSetCharCallback(IntPtr window, IntPtr charCallback);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwGetCursorPos(IntPtr window, IntPtr xpos, IntPtr ypos);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwSetKeyCallback(IntPtr window, IntPtr keyCallback);

    [DllImport(DllFilePath , CallingConvention = CallingConvention.Cdecl)]
    public extern static void glfwGetKey(IntPtr window, int key);
}

static class GLFWHelper {
    public static Vector2i GetWindowSize(){
        var w = Program.memory.Allocate(4);
        var h = Program.memory.Allocate(4);
        GLFW.glfwGetWindowSize(Program.window, w, h);
        return new Vector2i(Marshal.PtrToStructure<int>(w), Marshal.PtrToStructure<int>(h));
    }

    public static Vector2 GetMonitorContentScale(){
        var xscale = Program.memory.Allocate(4);
        var yscale = Program.memory.Allocate(4);
        GLFW.glfwGetMonitorContentScale(Program.monitor, xscale, yscale);
        return new Vector2(Marshal.PtrToStructure<float>(xscale), Marshal.PtrToStructure<float>(yscale));
    }

    public static void SetMouseButtonCallback(MouseButtonCallbackDelegate mouseButtonCallbackDelegate){
        var ptr = Marshal.GetFunctionPointerForDelegate(mouseButtonCallbackDelegate);
        GLFW.glfwSetMouseButtonCallback(Program.window, ptr);
    }

    public static void SetCursorPosCallback(CursorPosCallbackDelegate cursorPosCallbackDelegate){
        var ptr = Marshal.GetFunctionPointerForDelegate(cursorPosCallbackDelegate);
        GLFW.glfwSetCursorPosCallback(Program.window, ptr);
    }

    public static void SetKeyCallback(KeyCallbackDelegate keyCallbackDelegate){
        var ptr = Marshal.GetFunctionPointerForDelegate(keyCallbackDelegate);
        GLFW.glfwSetKeyCallback(Program.window, ptr);
    }

    public static void SetCharCallback(CharCallbackDelegate charCallbackDelegate){
        var ptr = Marshal.GetFunctionPointerForDelegate(charCallbackDelegate);
        GLFW.glfwSetCharCallback(Program.window, ptr);
    }

    public static Vector2 GetCursorPosition(){
        var xptr = Program.memory.Allocate(8);
        var yptr = Program.memory.Allocate(8);
        GLFW.glfwGetCursorPos(Program.window, xptr, yptr);
        return new Vector2((float)Marshal.PtrToStructure<double>(xptr), (float)Marshal.PtrToStructure<double>(yptr));
    }
}

class Shader{
    public uint program;

    static uint CreateShader(string source, uint type){
        var shader = GL.glCreateShader(type);
        GL.glShaderSource(shader, 1, Program.memory.AddStringArray([source]), 0);
        GL.glCompileShader(shader);
        var success = Program.memory.Allocate(4);
        GL.glGetShaderiv(shader, GL.GL_COMPILE_STATUS, success);
        if(Marshal.PtrToStructure<int>(success) == 0){
            var infoLog = Program.memory.Allocate(512);
            GL.glGetShaderInfoLog(shader, 512, 0, infoLog);
            throw new Exception(Marshal.PtrToStringAnsi(infoLog));
        }
        return shader;
    }

    public Shader(string vertexSource, string fragmentSource){
        var vertexShader = CreateShader(vertexSource, GL.GL_VERTEX_SHADER);
        var fragmentShader = CreateShader(fragmentSource, GL.GL_FRAGMENT_SHADER);
        program = GL.glCreateProgram();
        GL.glAttachShader(program, vertexShader);
        GL.glAttachShader(program, fragmentShader);
        GL.glLinkProgram(program);
        var success = Program.memory.Allocate(4);
        GL.glGetProgramiv(program, GL.GL_LINK_STATUS, success);
        if(Marshal.PtrToStructure<int>(success) == 0) {
            var infoLog = Program.memory.Allocate(512);
            GL.glGetProgramInfoLog(program, 512, 0, infoLog);
            throw new Exception(Marshal.PtrToStringAnsi(infoLog));
        }
        GL.glDeleteShader(vertexShader);
        GL.glDeleteShader(fragmentShader);  
    }

    public void Use(){
        GL.glUseProgram(program);
    }
}

class Buffer(int maxSize){
    public readonly IntPtr ptr = Marshal.AllocHGlobal(maxSize);
    public readonly int maxSize = maxSize;
    public int size = 0;

    public void SetAllDataToZero(){
        Kernel32.RtlZeroMemory(ptr, (uint)maxSize);
    }

    public byte[] GetBytes(int id, int size){
        byte[] managedArray = new byte[size];
        Marshal.Copy(ptr + id, managedArray, 0, size);
        return managedArray;
    }

    public void SetBytes(int id, byte[] bytes){
        Marshal.Copy(bytes, 0, ptr + id, bytes.Length);
    }

    public IntPtr AddBytes(byte[] bytes){
        var ptr = Allocate(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        return ptr;
    }

    public IntPtr AddFloatArray(float[] array){
        return AddBytes(array.SelectMany(BitConverter.GetBytes).ToArray());
    }

    public IntPtr AddString(string @string){
        byte[] bytes = @string.Select(c=>(byte)c).ToArray();
        return AddBytes(bytes);
    }

    public IntPtr AddStringArray(string[] strings){
        IntPtr[] ptrs = strings.Select(AddString).ToArray();
        return AddBytes(ptrs.SelectMany(i=>BitConverter.GetBytes(i)).ToArray());
    }

    public IntPtr AddIntArray(int[] array){
        return AddBytes(array.SelectMany(BitConverter.GetBytes).ToArray());
    }

    public IntPtr AddUintArray(uint[] array){
        return AddBytes(array.SelectMany(BitConverter.GetBytes).ToArray());
    }

    public IntPtr Allocate(int size){
        if(this.size + size > maxSize){
            throw new Exception("Buffer not large enough...");
        }
        var ptr = this.ptr + this.size;
        this.size += size;
        return ptr;
    }

    public void FreeAll(){
        Marshal.FreeHGlobal(ptr);
    }
}

static class GLHelper{
    public static uint GenVertexArray(){
        var ptr = Program.memory.Allocate(4);
        GL.glGenVertexArrays(1, ptr);
        return Marshal.PtrToStructure<uint>(ptr);
    }

    public static uint GenBuffer(){
        var ptr = Program.memory.Allocate(4);
        GL.glGenBuffers(1, ptr);
        return Marshal.PtrToStructure<uint>(ptr);
    }

    public static uint GenTexture(){
        var ptr = Program.memory.Allocate(4);
        GL.glGenTextures(1, ptr);
        return Marshal.PtrToStructure<uint>(ptr);
    }

    public static void UniformMatrix4fv(Shader shader, string name, Matrix4x4 matrix){
        var namePtr = Program.memory.AddString(name+'\0');
        var location = GL.glGetUniformLocation(shader.program, namePtr);
        var matrixPtr = Program.memory.AddFloatArray([
            matrix.M11, matrix.M12, matrix.M13, matrix.M14, 
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34, 
            matrix.M41, matrix.M42, matrix.M43, matrix.M44,
        ]);
        GL.glUniformMatrix4fv(location, 1, 0, matrixPtr);
    }
}

struct Rect(float x, float y, float width, float height){
    public float x = x;
    public float y = y;
    public float width = width;
    public float height = height;
    public Vector2 Center => new (x + width*0.5f, y + height*0.5f);

    public static Rect CreateFromStartEnd(Vector2 start, Vector2 end){
        var center = (start + end) * 0.5f;
        var size = new Vector2(MathF.Abs(end.x - start.x), MathF.Abs(end.y - start.y));
        return CreateFromCenterSize(center, size);
    }

    public static Rect CreateFromCenterSize(Vector2 center, Vector2 size){
        return new Rect(center.x - size.x*0.5f, center.y - size.y*0.5f, size.x, size.y);
    }

    public static bool Intersect(Rect a, Rect b){
        var aCenter = a.Center;
        var bCenter = b.Center;
        Vector2 dist = aCenter - bCenter;
        return MathF.Abs(dist.x) < (a.width + b.width) * 0.5f && MathF.Abs(dist.y) < (a.height + b.height) * 0.5f;
    }

    public Rect Translate(Vector2 translate){
        return new Rect(x + translate.x, y + translate.y, width, height);
    }
}

struct Color(float r, float g, float b, float a = 1){
    public float r = r;
    public float g = g;
    public float b = b;
    public float a = a;

    public static Color RandomColor(Random random){
        return new Color(random.NextSingle(), random.NextSingle(), random.NextSingle());
    }

    public static Color Blue => new (0,0,1);
    public static Color Red => new (1,0,0);
    public static Color Yellow => new (1,1,0);
    public static Color Orange => new (1,0.5f,0);
    public static Color Green => new (0,1,0);
    public static Color Black => new (0,0,0);
    public static Color White => new (1,1,1);
    public static Color Magenta => new (1,0,1);
    
    public override string ToString() {
        return "("+r+","+g+","+b+","+a+")";
    }
}
 
struct Vector2(float x, float y){
    public float x = x;
    public float y = y;
    
    public static Vector2 Zero => new (0,0);

    public float Length(){
        return MathF.Sqrt(x*x + y*y);
    }

    public Vector2 Normalized(){
        var length = Length();
        return new Vector2(x/length, y/length);
    }

    public static Vector2 operator -(Vector2 a, Vector2 b){
        return new Vector2(a.x - b.x, a.y - b.y);
    }

    public static Vector2 operator +(Vector2 a, Vector2 b){
        return new Vector2(a.x + b.x, a.y + b.y);
    }

    public static Vector2 operator *(Vector2 v, float f){
        return new Vector2(v.x * f, v.y * f);
    }

    public static Vector2 operator *(float f, Vector2 v){
        return new Vector2(v.x * f, v.y * f);
    }

    public static Vector2 operator /(Vector2 v, float f){
        return new Vector2(v.x / f, v.y / f);
    }

    public static Vector2 operator -(Vector2 v){
        return new Vector2(-v.x, -v.y);
    }

    public static Vector2 Direction(Vector2 a, Vector2 b){
        return (b-a).Normalized();
    }

    public Vector2 PerpendicularClockwise(){
        return new Vector2(y, -x);
    }

    public Vector2 PerpendicularCounterClockwise(){
        return new Vector2(-y, x);
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float amount){
        return new Vector2(float.Lerp(a.x, b.x, amount), float.Lerp(a.y, b.y, amount));
    }

    public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float amount){
        return Lerp(Lerp(a,b,amount), Lerp(b,c,amount), amount);
    }

    public static float Dot(Vector2 a, Vector2 b){
        return a.x * b.x + a.y * b.y;
    }

    public override string ToString(){
        return "("+x+","+y+")";
    }
}

struct Vector2i(int x, int y){
    public int x = x;
    public int y = y;
}

class DynamicTextureRed {
    uint id;
    public readonly int width;
    public readonly int height;
    Buffer data;

    public DynamicTextureRed(int width, int height){
        id = GLHelper.GenTexture();
        this.width = width;
        this.height = height;
        data = new Buffer(width * height);
        data.SetAllDataToZero();
    }

    public void UpdateData(byte[] bytes){
        data.SetBytes(0, bytes);
        GL.glBindTexture(GL.GL_TEXTURE_2D, id);

        GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_REPEAT);	
        GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_REPEAT);
        GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_NEAREST);
        GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_NEAREST);

        GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RED, width, height, 0, GL.GL_RED, GL.GL_UNSIGNED_BYTE, data.ptr);
    }

    public void Bind(){
        GL.glBindTexture(GL.GL_TEXTURE_2D, id);
    }
}

class Renderer(Shader shader, Buffer vertices, Buffer indices){
    readonly uint vao = GLHelper.GenVertexArray();
    readonly uint vbo = GLHelper.GenBuffer();
    readonly uint ebo = GLHelper.GenBuffer();
    public readonly Buffer vertices = vertices, indices = indices;
    readonly Shader shader = shader;

    public void UpdateData(){
        GL.glBindVertexArray(vao);

        GL.glBindBuffer(GL.GL_ARRAY_BUFFER, vbo);
        GL.glBufferData(GL.GL_ARRAY_BUFFER, vertices.size, vertices.ptr, GL.GL_DYNAMIC_DRAW);

        GL.glBindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, ebo);
        GL.glBufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indices.size, indices.ptr, GL.GL_DYNAMIC_DRAW);
    }

    public void SetVertexAttribPointers(int[] vertexAttribPointerSizes){
        GL.glBindVertexArray(vao);
        var stride = 0;
        foreach(var v in vertexAttribPointerSizes){
            stride+=v;
        }
        uint id = 0;
        var offset = 0;
        foreach(var v in vertexAttribPointerSizes){
            GL.glVertexAttribPointer(id, v, GL.GL_FLOAT, GL.GL_FALSE, stride * sizeof(float), offset * sizeof(float));
            GL.glEnableVertexAttribArray(id);
            offset += v;
            id++;
        }
    }

    public void UseShader(){
        shader.Use();
    }

    public void SetMatrix(string name, Matrix4x4 matrix){
        GLHelper.UniformMatrix4fv(shader, name, matrix);
    }

    public void Draw(){
        GL.glBindVertexArray(vao);
        GL.glDrawElements(GL.GL_TRIANGLES, indices.size / sizeof(uint), GL.GL_UNSIGNED_INT, 0);
    }

    public void ClearData(){
        vertices.size = 0;
        indices.size = 0;
    }
}

class DynamicTextureRenderer2D {
    public int Width => dynamicTextureRed.width;
    public int Height => dynamicTextureRed.height;
    Renderer renderer;
    DynamicTextureRed dynamicTextureRed;
    uint vertexID;
    public Vector2 Size;

    public DynamicTextureRenderer2D(int width, int height){
        string vertexSource = @"#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec4 aColor;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 projection;

out vec4 color;
out vec2 TexCoord;

void main()
{
    gl_Position = projection * vec4(aPos, 0.0, 1.0);
    color = aColor;
    TexCoord = aTexCoord;
}  "+"\0";

        string fragmentSource = @"#version 330 core
out vec4 FragColor;
  
in vec4 color;
in vec2 TexCoord;

uniform sampler2D ourTexture;

void main()
{
    float c = texture(ourTexture, TexCoord).r;
    FragColor = vec4(c, c, c, c) * color;
} "+"\0";
        var shader = new Shader(vertexSource, fragmentSource);
        var vertices = new Buffer(sizeof(float) * 100000);
        var indices = new Buffer(sizeof(uint) * 30000);
        dynamicTextureRed = new DynamicTextureRed(width, height);
        renderer = new Renderer(shader, vertices, indices);
        CalculateSize();
    }

    public void UpdateTexture(byte[] bytes){
        dynamicTextureRed.UpdateData(bytes);
    }

    public void DrawShape(Vector2[] points, Vector2[] uvs, Color color){
        for(uint i=2;i<points.Length;i++){
            renderer.indices.AddUintArray([vertexID, vertexID+i-1, vertexID+i]);
        }
        for(var i=0;i<points.Length;i++){
            renderer.vertices.AddFloatArray([points[i].x, points[i].y, color.r, color.g, color.b, color.a, uvs[i].x, uvs[i].y]);
            vertexID++;
        }
    }

    void CalculateSize(){
        var windowSize = GLFWHelper.GetWindowSize();
        var monitorScale = GLFWHelper.GetMonitorContentScale();
        Size = new Vector2(windowSize.x / monitorScale.x, windowSize.y / monitorScale.y);
    }

    public void Draw(){
        GL.glEnable(GL.GL_BLEND); 
        GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA); 
        renderer.UseShader();
        renderer.UpdateData();
        CalculateSize();
        renderer.SetMatrix("projection", Matrix4x4.CreateOrthographicOffCenter(0, Size.x, Size.y, 0, -1, 1));
        renderer.SetVertexAttribPointers([2,4,2]);
        dynamicTextureRed.Bind();
        renderer.Draw();
        renderer.ClearData();
        vertexID = 0;
    }
}

class CharacterData(char c, Vector2 uvMin, Vector2 uvMax, FontData.GlyphData glyphData){
    public char c = c;
    public Vector2 uvMin = uvMin;
    public Vector2 uvMax = uvMax;
    public FontData.GlyphData glyphData = glyphData;
}

class FontRenderer{
    byte[] bytes;
    int width;
    int height;
    FontData fontData;
    float fontScale;
    Dictionary<char, CharacterData> characterData = [];
    DynamicTextureRenderer2D dynamicTextureRenderer2D;
    public Vector2 Size => dynamicTextureRenderer2D.Size;

    public float FontHeight(float characterScale){
        return 1800 * fontScale * characterScale;
    }

    public float LineHeight(float characterScale){
        return 2200 * fontScale * characterScale;
    }

    public float MeasureCharacter(char c, float characterScale){
        if(c == ' '){
            return FontHeight(characterScale) * 0.5f;
        }
        else if(characterData.TryGetValue(c, out CharacterData? character)){
            return character.glyphData.AdvanceWidth * fontScale * characterScale;
        }
        return 0;
    }

    public float MeasureText(string text, float characterScale){
        var x = 0f;
        foreach(var c in text){
            x += MeasureCharacter(c, characterScale);
        }
        return x;
    }

    public FontRenderer(string pathToFont, int textureSize, float fontScale){
        fontData = FontParser.Parse(pathToFont);
        dynamicTextureRenderer2D = new DynamicTextureRenderer2D(textureSize, textureSize);
        width = textureSize;
        height = textureSize;
        bytes = new byte[width * height];
        this.fontScale = fontScale;
        int x = 0;
        int y = (int)FontHeight(1);
        var str = "";
        for(var i=33;i<128;i++){
            str+=(char)i;
        }
        SetPixel(0,0,255);
        FillTextOntoTexture(ref x, ref y, str);
        dynamicTextureRenderer2D.UpdateTexture(bytes);
    }

    void SetPixel(int x, int y, byte value){
        bytes[x + y * width] = value;
    }

    byte GetPixel(int x, int y){
        return bytes[x + y * width];
    }

    void FillCharacterOntoTexture(ref int posX, ref int posY, char c){
        if(fontData.TryGetGlyph(c, out FontData.GlyphData glyphData)){
            var minY = (int)(posY + glyphData.MinY * fontScale);
            var minX = (int)(posX + glyphData.MinX * fontScale);
            var maxY = (int)(posY + glyphData.MaxY * fontScale);
            var maxX = (int)(posX + glyphData.MaxX * fontScale);
            if(minX < 0 || maxX >= width || minY < 0 || maxY >= height){
                posX = 0;
                posY += (int)LineHeight(1);
                minY = (int)(posY + glyphData.MinY * fontScale);
                minX = (int)(posX + glyphData.MinX * fontScale);
                maxY = (int)(posY + glyphData.MaxY * fontScale);
                maxX = (int)(posX + glyphData.MaxX * fontScale);
            }
            var uvMin = new Vector2(minX/(float)width, minY/(float)height);
            var uvMax = new Vector2(maxX/(float)width, maxY/(float)height);
            characterData.Add(c, new CharacterData(c, uvMin, uvMax, glyphData));
            List<Vector2[]> contours = GlythHelper.CreateContoursWithImpliedPoints(glyphData, fontScale);
            for(var ci = 0; ci< contours.Count; ci++){
                var contour = contours[ci];
                for(var i = 0;i < contour.Length-2; i+=2){
                    var dist = (contour[i] - contour[i+2]).Length();
                    int resolution = (int)(dist * 2);
                    for(var ti=0;ti<=resolution;ti++){
                        var point = Vector2.Bezier(contour[i], contour[i+1], contour[i+2], ti/(float)resolution);
                        var pointI = new Vector2i((int)point.x, (int)point.y);
                        if(contour[i].y < contour[i+2].y){
                            if(GetPixel(posX + pointI.x, posY + pointI.y) == 0){
                                SetPixel(posX + pointI.x, posY + pointI.y, 254);
                            }
                        }
                        else{
                            SetPixel(posX + pointI.x, posY + pointI.y, 253);
                        }
                    }
                }
            }
            for(var y=minY;y<maxY;y++){
                bool draw = false;
                for(var x=minX;x<maxX;x++){
                    if(GetPixel(x,y) == 254){
                        draw = true;
                    }
                    else if(GetPixel(x,y) == 253){
                        draw = false;
                    }
                    else if(GetPixel(x, y-1) == 255){
                        SetPixel(x,y,255);
                    }
                    else if(draw){
                        SetPixel(x,y,255);
                    }
                }
            }
            posX += (int)(glyphData.AdvanceWidth * fontScale);
        }
    }

    void FillTextOntoTexture(ref int x, ref int y, string text){
        for(var i=0;i<text.Length;i++){
            FillCharacterOntoTexture(ref x,ref y, text[i]);
        }
    }

    public void FillRect(Rect rect, Color color){
        Vector2[] points = [
            new Vector2(rect.x, rect.y),
            new Vector2(rect.x + rect.width, rect.y),
            new Vector2(rect.x + rect.width, rect.y + rect.height),
            new Vector2(rect.x, rect.y + rect.height)];
        Vector2 uv = new Vector2(0.5f/width,0.5f/height);
        Vector2[] uvs = [uv, uv, uv, uv];
        dynamicTextureRenderer2D.DrawShape(points, uvs, color);
    }

    public void StrokeRect(Rect rect, Color color, float border){
        FillRect(new Rect(rect.x, rect.y, rect.width, border), color);
        FillRect(new Rect(rect.x, rect.y, border, height), color);
        FillRect(new Rect(rect.x, rect.y + rect.height - border, width, border), color);
        FillRect(new Rect(rect.x + rect.width - border, rect.y, border, height), color);
    }

    public float FillCharacter(Vector2 position, char c, float characterScale, Color color){
        if(c == ' '){
            return FontHeight(characterScale) * 0.5f;
        }
        else if(characterData.TryGetValue(c, out CharacterData? character)){
            var minX = character.glyphData.MinX * fontScale * characterScale;
            var minY = character.glyphData.MinY * fontScale * characterScale;
            var w = character.glyphData.Width * fontScale * characterScale;
            var h = character.glyphData.Height * fontScale * characterScale;
            var fontHeight = FontHeight(characterScale);

            Vector2[] points = [
                new Vector2(position.x + minX, position.y + fontHeight - minY), 
                new Vector2(position.x + minX + w, position.y + fontHeight - minY), 
                new Vector2(position.x + minX + w, position.y + fontHeight - minY - h), 
                new Vector2(position.x + minX, position.y + fontHeight - minY - h)];
            Vector2[] uvs = [
                character.uvMin, 
                new Vector2(character.uvMax.x, character.uvMin.y), 
                character.uvMax,
                new Vector2(character.uvMin.x, character.uvMax.y)];
            dynamicTextureRenderer2D.DrawShape(points, uvs, color);
            return character.glyphData.AdvanceWidth * fontScale * characterScale;
        }
        return 0;
    }

    public float FillText(Vector2 position, string text, float characterScale, Color color){
        var x = 0f;
        foreach(var c in text){
            x += FillCharacter(new Vector2(position.x + x, position.y), c, characterScale, color);
        }
        return x;
    }

    public void Draw(){
        dynamicTextureRenderer2D.Draw();
    }
}
