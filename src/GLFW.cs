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
    public static (int x, int y) GetWindowSize(){
        var w = Program.memory.Allocate(4);
        var h = Program.memory.Allocate(4);
        GLFW.glfwGetWindowSize(Program.window, w, h);
        return (Marshal.PtrToStructure<int>(w), Marshal.PtrToStructure<int>(h));
    }

    public static (float x, float y) GetMonitorContentScale(){
        var xscale = Program.memory.Allocate(4);
        var yscale = Program.memory.Allocate(4);
        GLFW.glfwGetMonitorContentScale(Program.monitor, xscale, yscale);
        return (Marshal.PtrToStructure<float>(xscale), Marshal.PtrToStructure<float>(yscale));
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

    public static (float x, float y) GetCursorPosition(){
        var xptr = Program.memory.Allocate(8);
        var yptr = Program.memory.Allocate(8);
        GLFW.glfwGetCursorPos(Program.window, xptr, yptr);
        return((float)Marshal.PtrToStructure<double>(xptr), (float)Marshal.PtrToStructure<double>(yptr));
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
    public int TexWidth => dynamicTextureRed.width;
    public int TexHeight => dynamicTextureRed.height;
    Renderer renderer;
    DynamicTextureRed dynamicTextureRed;
    public int vertexID;
    public float screenWidth, screenHeight;

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

    public int AddVertex(float x, float y, float uvx, float uvy, JsColor color){
        renderer.vertices.AddFloatArray([x, y, color.r, color.g, color.b, color.a, uvx, uvy]);
        vertexID++;
        return vertexID - 1;
    }

    public void AddTriangles(int vertexID, int length){
        for(var i=2;i<length;i++){
            renderer.indices.AddIntArray([vertexID, vertexID + i - 1, vertexID + i]);
        }
    }

    void CalculateSize(){
        var windowSize = GLFWHelper.GetWindowSize();
        var monitorScale = GLFWHelper.GetMonitorContentScale();
        screenWidth = windowSize.x / monitorScale.x;
        screenHeight = windowSize.y / monitorScale.y;
    }

    public void Draw(){
        GL.glEnable(GL.GL_BLEND); 
        GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA); 
        renderer.UseShader();
        renderer.UpdateData();
        CalculateSize();
        renderer.SetMatrix("projection", Matrix4x4.CreateOrthographicOffCenter(0, screenWidth, screenHeight, 0, -1, 1));
        renderer.SetVertexAttribPointers([2,4,2]);
        dynamicTextureRed.Bind();
        renderer.Draw();
        renderer.ClearData();
        vertexID = 0;
    }
}

class CharacterData(char c, float uvMinX, float uvMinY, float uvMaxX, float uvMaxY, FontData.GlyphData glyphData){
    public char c = c;
    public float uvMinX = uvMinX;
    public float uvMinY = uvMinY;
    public float uvMaxX = uvMaxX;
    public float uvMaxY = uvMaxY;
    public FontData.GlyphData glyphData = glyphData;
}

class FontRenderer{
    byte[] bytes;
    int texWidth;
    int texHeight;
    FontData fontData;
    float fontScale;
    Dictionary<char, CharacterData> characterData = [];
    DynamicTextureRenderer2D dynamicTextureRenderer2D;
    public float ScreenWidth => dynamicTextureRenderer2D.screenWidth;
    public float ScreenHeight => dynamicTextureRenderer2D.screenHeight;
    public int VertexID => dynamicTextureRenderer2D.vertexID;

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
        texWidth = textureSize;
        texHeight = textureSize;
        bytes = new byte[texWidth * texHeight];
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
        bytes[x + y * texWidth] = value;
    }

    byte GetPixel(int x, int y){
        return bytes[x + y * texWidth];
    }

    static Vector2 Lerp(Vector2 a, Vector2 b, float amount){
        return new Vector2(float.Lerp(a.X, b.X, amount), float.Lerp(a.Y, b.Y, amount));
    }

    static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float amount){
        return Lerp(Lerp(a, b, amount), Lerp(b, c, amount), amount);
    }

    void FillCharacterOntoTexture(ref int posX, ref int posY, char c){
        if(fontData.TryGetGlyph(c, out FontData.GlyphData glyphData)){
            var minY = (int)(posY + glyphData.MinY * fontScale);
            var minX = (int)(posX + glyphData.MinX * fontScale);
            var maxY = (int)(posY + glyphData.MaxY * fontScale);
            var maxX = (int)(posX + glyphData.MaxX * fontScale);
            if(minX < 0 || maxX >= texWidth || minY < 0 || maxY >= texHeight){
                posX = 0;
                posY += (int)LineHeight(1);
                minY = (int)(posY + glyphData.MinY * fontScale);
                minX = (int)(posX + glyphData.MinX * fontScale);
                maxY = (int)(posY + glyphData.MaxY * fontScale);
                maxX = (int)(posX + glyphData.MaxX * fontScale);
            }
            characterData.Add(c, new CharacterData(c, minX/(float)texWidth, minY/(float)texHeight, maxX/(float)texWidth, maxY/(float)texHeight, glyphData));
            List<Vector2[]> contours = GlythHelper.CreateContoursWithImpliedPoints(glyphData, fontScale);
            for(var ci = 0; ci< contours.Count; ci++){
                var contour = contours[ci];
                for(var i = 0;i < contour.Length-2; i+=2){
                    var dist = (contour[i] - contour[i+2]).Length();
                    int resolution = (int)(dist * 2);
                    for(var ti=0;ti<=resolution;ti++){
                        var point = Bezier(contour[i], contour[i+1], contour[i+2], ti/(float)resolution);
                        var xi = (int)point.X;
                        var yi = (int)point.Y;
                        if(contour[i].Y < contour[i+2].Y){
                            if(GetPixel(posX + xi, posY + yi) == 0){
                                SetPixel(posX + xi, posY + yi, 254);
                            }
                        }
                        else{
                            SetPixel(posX + xi, posY + yi, 253);
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

    public void FillPoly(List<Vector2> poly, JsColor color){
        var uvx = 0.5f/texWidth;
        var uvy = 0.5f/texHeight;
        var vertexID = dynamicTextureRenderer2D.vertexID;
        foreach(var p in poly){
            dynamicTextureRenderer2D.AddVertex(p.X, p.Y, uvx, uvy, color);
        }
        dynamicTextureRenderer2D.AddTriangles(vertexID, poly.Count);
    }

    public void FillRect(float x, float y, float width, float height, JsColor color){
        var uvx = 0.5f/texWidth;
        var uvy = 0.5f/texHeight;
        var vertexID = dynamicTextureRenderer2D.vertexID;
        dynamicTextureRenderer2D.AddVertex(x, y, uvx, uvy, color);
        dynamicTextureRenderer2D.AddVertex(x+width, y, uvx, uvy, color);
        dynamicTextureRenderer2D.AddVertex(x+width, y+height, uvx, uvy, color);
        dynamicTextureRenderer2D.AddVertex(x, y+height, uvx, uvy, color);
        dynamicTextureRenderer2D.AddTriangles(vertexID, 4);
    }

    public void StrokeRect(float x, float y, float width, float height, JsColor color, float border){
        FillRect(x, y, width, border, color);
        FillRect(x, y, border, height, color);
        FillRect(x, y + height - border, width, border, color);
        FillRect(x + width - border, y, border, height, color);
    }

    public float FillCharacter(float x, float y, char c, float characterScale, JsColor color){
        if(c == ' '){
            return FontHeight(characterScale) * 0.5f;
        }
        else if(characterData.TryGetValue(c, out CharacterData? character)){
            var minX = character.glyphData.MinX * fontScale * characterScale;
            var minY = character.glyphData.MinY * fontScale * characterScale;
            var w = character.glyphData.Width * fontScale * characterScale;
            var h = character.glyphData.Height * fontScale * characterScale;
            var fontHeight = FontHeight(characterScale);

            var vertexID = dynamicTextureRenderer2D.vertexID;
            dynamicTextureRenderer2D.AddVertex(x + minX, y + fontHeight - minY, character.uvMinX, character.uvMinY, color);
            dynamicTextureRenderer2D.AddVertex(x + minX + w, y + fontHeight - minY, character.uvMaxX, character.uvMinY, color);
            dynamicTextureRenderer2D.AddVertex(x + minX + w, y + fontHeight - minY - h, character.uvMaxX, character.uvMaxY, color);
            dynamicTextureRenderer2D.AddVertex(x + minX, y + fontHeight - minY - h, character.uvMinX, character.uvMaxY, color);
            dynamicTextureRenderer2D.AddTriangles(vertexID, 4);

            return character.glyphData.AdvanceWidth * fontScale * characterScale;
        }
        return 0;
    }

    public float FillText(float x, float y, string text, float characterScale, JsColor color){
        var deltax = 0f;
        foreach(var c in text){
            deltax += FillCharacter(x + deltax, y, c, characterScale, color);
        }
        return deltax;
    }

    public void Draw(){
        dynamicTextureRenderer2D.Draw();
    }
}
