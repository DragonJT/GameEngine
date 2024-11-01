

static class Asteroids
{
    static float y = 0;
    static string text = "";
    static float angle;

    static void KeyDown(string key){
        if(key.Length == 1){
            text += (char)key[0];
        }
        else if(key == "Backspace"){
            if(text.Length > 0){
                text = text[0..^1];
            }
        }
    }

    static (float x, float y) RotatePoint(float x, float y, float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Math.PI / 180f;
        float rotatedX = x * Math.cos(angleInRadians) - y * Math.sin(angleInRadians);
        float rotatedY = x * Math.sin(angleInRadians) + y * Math.cos(angleInRadians);
        return (rotatedX, rotatedY);
    }

    static void DrawTriangle(float x, float y, float radius, float angle, string color){
        var p1 = RotatePoint(-radius, radius, angle);
        var p2 = RotatePoint(0, -radius, angle);
        var p3 = RotatePoint(radius, radius, angle);
        Graphics.AddPoint(p1.x + x, p1.y + y);
        Graphics.AddPoint(p2.x + x, p2.y + y);
        Graphics.AddPoint(p3.x + x, p3.y + y);
        Graphics.Fill(color);
    }

    static void Draw(float deltaTime)
    {
        Graphics.Clear("rgb(180,180,180)");
        Graphics.FillText(100,100,"HelloWorld",60,"cyan");
        Graphics.FillText(50,200,text,40,"green");
        Graphics.FillText(100,y,"BOO",90,"blue");
        Graphics.FillRect(Input.mousex, Input.mousey, 10, 10, "yellow");
        DrawTriangle(Graphics.GetWidth() / 2, Graphics.GetHeight() / 2, 20, angle, "magenta");
        y+=Graphics.GetHeight()*deltaTime*0.5f;
        angle+=180 * deltaTime;
        if(y > Graphics.GetHeight()){
            y = 0;
        }
    }
}