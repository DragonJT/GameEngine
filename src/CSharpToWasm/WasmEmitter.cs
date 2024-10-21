
enum Opcode
{
    block = 0x02,
    loop = 0x03,
    br = 0x0c,
    br_if = 0x0d,
    end = 0x0b,
    ret = 0x0f,
    call = 0x10,
    drop = 0x1a,
    get_local = 0x20,
    set_local = 0x21,
    i32_store_8 = 0x3a,
    i32_const = 0x41,
    f32_const = 0x43,
    i32_eqz = 0x45,
    i32_eq = 0x46,
    f32_eq = 0x5b,
    f32_lt = 0x5d,
    f32_gt = 0x5e,
    i32_and = 0x71,
    f32_add = 0x92,
    f32_sub = 0x93,
    f32_mul = 0x94,
    f32_div = 0x95,
    f32_neg = 0x8c,
    i32_trunc_f32_s = 0xa8,
    f32_load = 0x2a,
    f32_store = 0x38,
    i32_mul = 0x6c,
    i32_add = 0x6a,
    i32_sub = 0x6b,
    i32_div_s = 0x6d,
    f32_convert_i32_s = 0xb2,
    i32_lt_s = 0x48,
    i32_gt_s = 0x4a,
    @if = 0x04,
}

enum SectionType
{
    Custom = 0,
    Type = 1,
    Import = 2,
    Func = 3,
    Table = 4,
    Memory = 5,
    Global = 6,
    Export = 7,
    Start = 8,
    Element = 9,
    Code = 10,
    Data = 11
}

enum Valtype
{
    Void = -1,
    I32 = 0x7f,
    F32 = 0x7d
}

enum ExportType
{
    Func = 0x00,
    Table = 0x01,
    Mem = 0x02,
    Global = 0x03
}

// https://webassembly.github.io/spec/core/binary/types.html#binary-blocktype
// https://github.com/WebAssembly/design/blob/main/BinaryEncoding.md#value_type
enum Blocktype
{
    @void = 0x40,
    i32 = 0x7f,
}

class WasmVariable(Valtype valtype, string name){
    public Valtype valtype = valtype;
    public string name = name;
    public int id = -1;
}

class WasmInstruction(Opcode opcode, object? data = null){
    public Opcode opcode = opcode;
    public object? data = data;
}

class WasmImportFunction(Valtype returnType, string name, WasmVariable[] parameters, string javascript){
    public string name = name;
    public Valtype returnType = returnType;
    public WasmVariable[] parameters = parameters;
    public int id = -1;
    public string javascript = javascript;
}

class WasmFunction(bool export, Valtype returnType, string name, WasmVariable[] parameters){
    public List<WasmInstruction> instructions = [];
    public Valtype returnType = returnType;
    public WasmVariable[] parameters = parameters;
    public bool export = export;
    public string name = name;
    public int id = -1;
}

static class WasmHelper
{
    public const byte emptyArray = 0x0;
    public const byte functionType = 0x60;

    public static byte[] MagicModuleHeader => [0x00, 0x61, 0x73, 0x6d];

    public static byte[] ModuleVersion => [0x01, 0x00, 0x00, 0x00];

    public static byte[] Ieee754(float value)
    {
        return BitConverter.GetBytes(value);
    }

    public static byte[] SignedLEB128(int value)
    {
        List<byte> bytes = [];
        bool more = true;

        while (more)
        {
            byte chunk = (byte)(value & 0x7fL); // extract a 7-bit chunk
            value >>= 7;

            bool signBitSet = (chunk & 0x40) != 0; // sign bit is the msb of a 7-bit byte, so 0x40
            more = !((value == 0 && !signBitSet) || (value == -1 && signBitSet));
            if (more) { chunk |= 0x80; } // set msb marker that more bytes are coming

            bytes.Add(chunk);
        }
        return bytes.ToArray();
    }

    public static byte[] UnsignedLEB128(uint value)
    {
        List<byte> bytes = [];
        do
        {
            byte byteValue = (byte)(value & 0x7F); // Extract 7 bits
            value >>= 7; // Shift right by 7 bits

            if (value != 0)
                byteValue |= 0x80; // Set the high bit to indicate more bytes

            bytes.Add(byteValue);
        }
        while (value != 0);
        return bytes.ToArray();
    }

    public static byte[] String(string value)
    {
        List<byte> bytes = [.. UnsignedLEB128((uint)value.Length)];
        foreach (var v in value)
        {
            bytes.Add((byte)v);
        }
        return bytes.ToArray();
    }

    public static byte[] Vector(params byte[][] vector)
    {
        return [..UnsignedLEB128((uint)vector.Length), ..vector.SelectMany(b=>b).ToArray()];
    }

    public static byte[] Vector(params byte[] vector)
    {
        return [..UnsignedLEB128((uint)vector.Length), ..vector];
    }

    public static byte[] Local(uint count, Valtype valtype)
    {
        return [..UnsignedLEB128(count), (byte)valtype];
    }

    public static byte[] Section(SectionType section, byte[][] bytes)
    {
        return [(byte)section, .. Vector(Vector(bytes))];
    }

    public static byte[] Return(Valtype type)
    {
        if (type == Valtype.Void)
        {
            return [emptyArray];
        }
        else
        {
            return Vector((byte)type);
        }
    }
}

class WasmEmitter(string filePath){
    public List<WasmImportFunction> wasmImportFunctions = [];
    public List<WasmFunction> wasmFunctions = [];
    string filePath = filePath;

    List<byte> EmitFunctionBody(WasmFunction f){
        List<byte> wasm = [];
        foreach(var i in f.instructions){
            if(i.opcode == Opcode.i32_const){
                wasm.AddRange([(byte)i.opcode, ..WasmHelper.SignedLEB128((int)i.data!)]);
            }
            else if(i.opcode == Opcode.call){
                var funcName = (string)i.data!;
                var func = wasmImportFunctions.First(f=>f.name == funcName);
                wasm.AddRange([(byte)Opcode.call, ..WasmHelper.UnsignedLEB128((uint)func.id)]);
            }
            else{
                wasm.Add((byte)i.opcode);
            }
        }
        wasm.Add((byte)Opcode.end);
        return wasm;
    }

    byte[] Emit(){
        var id = 0;
        foreach(var f in wasmImportFunctions){
            f.id = id;
            id++;
        }
        foreach(var f in wasmFunctions){
            f.id = id;
            id++;
        }

        List<byte[]> codeSection = [];

        foreach(var f in wasmFunctions){
            codeSection.Add(WasmHelper.Vector([WasmHelper.emptyArray, ..EmitFunctionBody(f)]));
        }

        List<byte[]> importSection = [];
        foreach(var f in wasmImportFunctions){
            importSection.Add([
                ..WasmHelper.String("env"), 
                ..WasmHelper.String(f.name),
                (byte)ExportType.Func, 
                ..WasmHelper.UnsignedLEB128((uint)f.id)]);
        }

        List<byte[]> typeSection = [];
        foreach(var f in wasmImportFunctions){
            typeSection.Add([
                WasmHelper.functionType, 
                ..WasmHelper.Vector(f.parameters.Select(p=>(byte)p.valtype).ToArray()), 
                ..WasmHelper.Return(f.returnType)]);
        }
        foreach(var f in wasmFunctions){
            typeSection.Add([
                WasmHelper.functionType,
                ..WasmHelper.Vector(f.parameters.Select(p=>(byte)p.valtype).ToArray()),
                ..WasmHelper.Return(f.returnType)]);
        }

        List<byte[]> funcSection = [];
        foreach(var f in wasmFunctions){
            funcSection.Add(WasmHelper.UnsignedLEB128((uint)f.id));
        }

        List<byte[]> exportSection = [];
        foreach(var f in wasmFunctions.Where(f=>f.export)){
            exportSection.Add([..WasmHelper.String(f.name), (byte)ExportType.Func, ..WasmHelper.UnsignedLEB128((uint)f.id)]);
        }
        byte[] wasm = [
            .. WasmHelper.MagicModuleHeader,
            .. WasmHelper.ModuleVersion,
            .. WasmHelper.Section(SectionType.Type, [..typeSection]),
            .. WasmHelper.Section(SectionType.Import, [.. importSection]),
            .. WasmHelper.Section(SectionType.Func, [..funcSection]),
            .. WasmHelper.Section(SectionType.Export, [..exportSection]),
            .. WasmHelper.Section(SectionType.Code, [..codeSection])];
        return wasm;
    }

    string EmitJavascriptParameters(WasmVariable[] variables){
        string result = "(";
        for(var i=0;i<variables.Length;i++){
            result+=variables[i].name;
            if(i<variables.Length-1){
                result+=",";
            }
        }
        return result + ")";
    }

    public void EmitHtml(){
        var importString = "";
        foreach(var f in wasmImportFunctions){
            importString += "imports.env."+f.name+" = function"+EmitJavascriptParameters(f.parameters)+"{\n";
            importString += f.javascript;
            importString += "}\n";
        }

        var wasm = Emit();
        string wasmString = string.Join(",", wasm.Select(b => "0x" + b.ToString("X2")));
        var html = @"
<!DOCTYPE html>
<html>
<head>
  <title>WebAssembly Example</title>
</head>
<body>
  <script>
const wasmBytecode = new Uint8Array([
" + wasmString +
@"]);
var imports = {};
var variables = [];
imports.env = {};
" +
importString
+ @"
WebAssembly.instantiate(wasmBytecode, imports)
  .then(module => {
    console.log(module.instance.exports.Run());
  })
  .catch(error => {
    console.error('Error:', error);
  });
  </script>
</body>
</html>";
        File.WriteAllText(filePath, html);
    }
}
