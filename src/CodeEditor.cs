class CodeEditor{
    string code;
    int cursor = 0;
    float fontSize = 0.2f;
    string filePath;

    public CodeEditor(string filePath){
        Program.charCallback += CharCallback;
        Program.keyCallback += KeyCallback;
        Program.draw += Draw;
        this.filePath = filePath;
        code = File.ReadAllText(filePath);
    }

    void Insert(string value){
        code = code[0..cursor] + value + code[cursor..];
        cursor+=value.Length;
    }

    void Delete(){
        if(code.Length > 0){
            code = code[0..(cursor-1)] + code[cursor..];
            cursor--;
        }
    }

    void CharCallback(char c){
        Insert(c.ToString());
    }

    int FindStartOfLine(int cursor){
        for(var i=cursor-1;i>=0;i--){
            if(code[i] == '\n'){
                return i+1;
            }
        }
        return 0;
    }

    string FindWhitespaceAtStartOfLine(int cursor){
        var i = FindStartOfLine(cursor);
        string result = "";
        while(i<code.Length && (code[i]=='\t' || code[i] == ' ')){
            result += code[i];
            i++;
        }
        return result;
    }

    void KeyCallback(int key, int scancode, int action, int mods){
        if(action == GLFW.GLFW_PRESS || action == GLFW.GLFW_REPEAT){
            if(key == GLFW.GLFW_KEY_BACKSPACE){
                Delete();
            }
            else if(key == GLFW.GLFW_KEY_ENTER){
                Insert('\n' + FindWhitespaceAtStartOfLine(cursor));
            }
            else if(key == GLFW.GLFW_KEY_TAB){
                Insert('\t'.ToString());
            }
            else if(key == GLFW.GLFW_KEY_LEFT){
                if(cursor > 0){
                    cursor--;
                }
            }
            else if(key == GLFW.GLFW_KEY_RIGHT){
                if(cursor < code.Length){
                    cursor++;
                }
            }
        }
        if(action == GLFW.GLFW_PRESS && (mods|GLFW.GLFW_MOD_CONTROL) != 0){
            if(key == GLFW.GLFW_KEY_S){
                File.WriteAllText(filePath, code);
            }
            if(key == GLFW.GLFW_KEY_R){
                Compiler.Run(code);
            }
        }
    }

    Vector2 GetPositionAtIndex(FontRenderer renderer, int index){
        var x = 0f;
        var y = 0f;
        for(var i=0;i<index;i++){
            if(code[i] == '\n'){
                x = 0f;
                y+=renderer.LineHeight(fontSize);
            }
            else if(code[i] == '\t'){
                x+=renderer.MeasureCharacter(' ', fontSize) * 4;
            }
            else{
                x+=renderer.MeasureCharacter(code[i], fontSize);
            }
        }
        return new Vector2(x,y);
    }

    void Draw(FontRenderer renderer){
        var x = 0f;
        var y = 0f;
        for(var i=0;i<code.Length;i++){
            if(code[i] == '\n'){
                x = 0f;
                y+=renderer.LineHeight(fontSize);
            }
            else if(code[i] == '\t'){
                x+=renderer.MeasureCharacter(' ', fontSize) * 4;
            }
            else{
                x+=renderer.DrawCharacter(new Vector2(x,y), code[i], fontSize, Color.White);
            }
        }
        var cursorPosition = GetPositionAtIndex(renderer, cursor);
        renderer.DrawRect(new Rect(cursorPosition.x, cursorPosition.y, 2, renderer.FontHeight(fontSize)), Color.White);
    }
}