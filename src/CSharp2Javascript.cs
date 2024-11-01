using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class CSharpToJavaScriptVisitor : CSharpSyntaxVisitor<string>
{
    static string GetInitializerWhenNull(string typeName){
        return typeName switch
        {
            "bool" => "false",
            "float" => "0",
            "int" => "0",
            "double" => "0",
            "long" => "0",
            "string" => "''",
            "char" => "''",
            _ => throw new Exception("Unexpected type"),
        };
    }

    public override string VisitCompilationUnit(CompilationUnitSyntax compilationUnitSyntax)
    {
        var jsBuilder = new StringBuilder();
        var classes = compilationUnitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>();
        foreach(var c in classes){
            jsBuilder.AppendLine(Visit(c));
        }

        foreach(var c in classes){
            foreach(var f in c.Members.OfType<FieldDeclarationSyntax>()){
                if(IsFieldStatic(f)){
                    var className = c.Identifier.Text;
                    foreach(var v in f.Declaration.Variables){
                        jsBuilder.Append(className+"."+v.Identifier.Text);
                        if(v.Initializer!=null){
                            jsBuilder.AppendLine("="+Visit(v.Initializer.Value)+";");
                        }
                        else{
                            var type = f.Declaration.Type.ToString();
                            jsBuilder.AppendLine("="+GetInitializerWhenNull(type)+";");
                        }
                    }
                }
            }
        }

        return jsBuilder.ToString();
    }

    public override string VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var jsBuilder = new StringBuilder();
        jsBuilder.AppendLine($"class {node.Identifier.Text} {{");

        foreach (var method in node.Members.OfType<MethodDeclarationSyntax>())
        {
            jsBuilder.Append(Visit(method));
        }
        jsBuilder.AppendLine("}");
        return jsBuilder.ToString();
    }

    static bool IsFieldStatic(FieldDeclarationSyntax fieldDeclaration)
    {
        return fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    static bool IsMethodStatic(MethodDeclarationSyntax methodDeclaration)
    {
        return methodDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    public override string VisitBlock(BlockSyntax node)
    {
        var jsBuilder = new StringBuilder();
        foreach (var statement in node.Statements)
        {
            jsBuilder.AppendLine(Visit(statement));
        }
        return jsBuilder.ToString();
    }

    public override string VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var jsBuilder = new StringBuilder();
        if(IsMethodStatic(node))
        {
            jsBuilder.Append("static ");
        }
        var methodName = node.Identifier.Text;
        jsBuilder.Append($"{methodName}(");
        var parameters = string.Join(", ", node.ParameterList.Parameters.Select(p => p.Identifier.Text));
        jsBuilder.Append(parameters);
        jsBuilder.AppendLine(") {");
        jsBuilder.Append(Visit(node.Body));
        jsBuilder.AppendLine("}");
        return jsBuilder.ToString();
    }

    public override string VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.Token.Value is float floatValue)
        {
            return floatValue.ToString();
        }
        return node.Token.Text;
    }

    public override string VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        return Visit(node.Expression) + ";";
    }

    public override string VisitArgument(ArgumentSyntax argumentSyntax)
    {
        return Visit(argumentSyntax.Expression)!;
    }

    public override string VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var expression = Visit(node.Expression);
        var args = string.Join(", ", node.ArgumentList.Arguments.Select(Visit));
        return $"{expression}({args})";
    }

    static T? GetNodeInParent<T>(SyntaxNode? node) where T:SyntaxNode
    {
        while (node != null)
        {
            if (node is T classDeclaration)
            {
                return classDeclaration; 
            }
            node = node.Parent;
        }
        return null;
    }

    public override string VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
    {
        var jsBuilder = new StringBuilder();
        jsBuilder.Append('`');

        foreach (var content in node.Contents)
        {
            if (content is InterpolatedStringTextSyntax text)
            {
                jsBuilder.Append(text.TextToken.ValueText);
            }
            else if (content is InterpolationSyntax interpolation)
            {
                jsBuilder.Append("${" + Visit(interpolation.Expression) + "}");
            }
        }
        jsBuilder.Append('`');
        return jsBuilder.ToString();
    }

    public override string VisitIdentifierName(IdentifierNameSyntax node)
    {
        var currentClass = GetNodeInParent<ClassDeclarationSyntax>(node)!;
        var currentMethod = GetNodeInParent<MethodDeclarationSyntax>(node)!;
        var nodeString = node.ToString();
        foreach(var p in currentMethod.ParameterList.Parameters){
            if(p.Identifier.Text == nodeString){
                return p.Identifier.Text;
            }
        }
        foreach(var v in currentMethod.DescendantNodes().OfType<VariableDeclaratorSyntax>()){
            if(v.Identifier.Text == nodeString){
                return v.Identifier.Text;
            }
        }
        foreach(var f in currentClass.DescendantNodes().OfType<FieldDeclarationSyntax>()){
            foreach(var v in f.Declaration.Variables){
                if(v.Identifier.Text == nodeString){
                    if(IsFieldStatic(f)){
                        return currentClass.Identifier.Text+"."+v.Identifier.Text;
                    }
                    else{
                        return "this."+v.Identifier.Text;
                    }
                }
            }
        }
        foreach(var m in currentClass.DescendantNodes().OfType<MethodDeclarationSyntax>()){
            if(m.Identifier.Text == nodeString){
                if(IsMethodStatic(m)){
                    return currentClass.Identifier.Text+"."+m.Identifier.Text;
                }
                else{
                    return "this."+m.Identifier.Text;
                }
            }
        }
        foreach(var c in GetNodeInParent<CompilationUnitSyntax>(node)!.DescendantNodes().OfType<ClassDeclarationSyntax>()){
            if(c.Identifier.Text == nodeString){
                return c.Identifier.Text;
            }
        }
        string[] javascriptClassNames = ["Input", "Graphics", "console", "Math"];
        if(javascriptClassNames.Contains(nodeString)){
            return nodeString;
        }
        throw new Exception("Cant find variable declaration: "+node.ToString());
    }

    public override string VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        var jsBuilder = new StringBuilder();
        var type = node.Declaration.Type.ToString();
        foreach (var variable in node.Declaration.Variables)
        {
            jsBuilder.Append("var ");
            jsBuilder.Append(variable.Identifier.Text);

            if (variable.Initializer != null)
            {
                jsBuilder.Append(" = ");
                jsBuilder.Append(Visit(variable.Initializer.Value));
            }

            jsBuilder.Append(';'); 
        }
        return jsBuilder.ToString();
    }

    public override string VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        var jsBuilder = new StringBuilder();
        var typeName = node.Type.ToString();

        if (typeName == "List" || typeName == "List<int>" || typeName == "List<string>")
        {
            jsBuilder.Append("[]");
        }
        else if (typeName == "Dictionary" || typeName == "Dictionary<string, int>")
        {
            jsBuilder.Append("new Map()");
        }
        else
        {
            jsBuilder.Append("new ");
            jsBuilder.Append(typeName);
            jsBuilder.Append('(');

            if (node.ArgumentList != null)
            {
                var arguments = string.Join(", ", node.ArgumentList.Arguments.Select(arg => Visit(arg.Expression)));
                jsBuilder.Append(arguments);
            }

            jsBuilder.Append(')');
        }

        return jsBuilder.ToString();
    }

    public override string VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
    {
        string operand = Visit(node.Operand)!;

        switch (node.Kind())
        {
            case SyntaxKind.PostIncrementExpression:
                return $"{operand}++"; 
            case SyntaxKind.PostDecrementExpression:
                return $"{operand}--"; 
            default:
                throw new InvalidOperationException("Unexpected postfix unary expression.");
        }
    }

    public override string VisitIfStatement(IfStatementSyntax node)
    {
        var jsBuilder = new StringBuilder();
        string condition = Visit(node.Condition)!;
        string statement = Visit(node.Statement)!;

        jsBuilder.AppendLine($"if ({condition}) {{");
        jsBuilder.Append(statement);

        if (node.Else != null)
        {
            jsBuilder.AppendLine("} else {");
            jsBuilder.Append(Visit(node.Else.Statement));
            jsBuilder.Append('}');
        }
        else
        {
            jsBuilder.Append('}');
        }

        return jsBuilder.ToString();
    }

    public override string VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        string left = Visit(node.Left)!;
        string right = Visit(node.Right)!;

        string operatorString = node.Kind() switch
        {
            SyntaxKind.SimpleAssignmentExpression => "=",
            SyntaxKind.AddAssignmentExpression => "+=",
            SyntaxKind.SubtractAssignmentExpression => "-=",
            SyntaxKind.MultiplyAssignmentExpression => "*=",
            SyntaxKind.DivideAssignmentExpression => "/=",
            SyntaxKind.ModuloAssignmentExpression => "%=",
            _ => throw new InvalidOperationException("Unsupported assignment expression.")
        };

        return $"{left} {operatorString} {right}";
    }

    public override string VisitCastExpression(CastExpressionSyntax node)
    {
        return Visit(node.Expression)!;
    }

    public override string VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        string left = Visit(node.Left)!;
        string right = Visit(node.Right)!;
        string operatorString = node.Kind() switch
        {
            SyntaxKind.EqualsExpression => "==",
            SyntaxKind.NotEqualsExpression => "!=",
            SyntaxKind.GreaterThanExpression => ">",
            SyntaxKind.GreaterThanOrEqualExpression => ">=",
            SyntaxKind.LessThanExpression => "<",
            SyntaxKind.LessThanOrEqualExpression => "<=",
            SyntaxKind.MultiplyExpression => "*",
            SyntaxKind.AddExpression => "+",
            SyntaxKind.SubtractExpression => "-",
            SyntaxKind.DivideExpression => "/",
            _ => throw new InvalidOperationException("Unsupported binary expression: "+node.Kind())
        };
        return $"{left} {operatorString} {right}";
    }

    public override string? VisitElementAccessExpression(ElementAccessExpressionSyntax node)
    {
        var argument = node.ArgumentList.Arguments[0].Expression;
        var expression = Visit(node.Expression)!;
        if(argument is RangeExpressionSyntax rangeExpressionSyntax){
            return RangeExpression(expression, rangeExpressionSyntax);
        }
        return $"{expression}[{argument}]";
    }

    public override string VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var name = node.Name.ToString();
        if(name == "Length"){
            name = "length";
        }
        return $"{Visit(node.Expression)}.{name}";
    }

    static string RangeExpression(string array, RangeExpressionSyntax node)
    {
        var start = node.LeftOperand;
        var end = node.RightOperand;

        string? startJs = ConvertRangeOperandToJs(array, start!, isStart: true);
        string? endJs = ConvertRangeOperandToJs(array, end!, isStart: false);

        string jsRangeExpression;
        if (startJs == null && endJs == null)
        {
            jsRangeExpression = $"{array}.slice(0)"; 
        }
        else if (startJs == null)
        {
            jsRangeExpression = $"{array}.slice(0, {endJs})";
        }
        else if (endJs == null)
        {
            jsRangeExpression = $"{array}.slice({startJs})";
        }
        else
        {
            jsRangeExpression = $"{array}.slice({startJs}, {endJs})";
        }
        return jsRangeExpression;
    }

    static string? ConvertRangeOperandToJs(string array, ExpressionSyntax operand, bool isStart)
    {
        if (operand == null)
        {
            return null;
        }
        if (operand is PrefixUnaryExpressionSyntax unary && unary.OperatorToken.IsKind(SyntaxKind.CaretToken))
        {
            var index = unary.Operand.ToString();
            return $"{array}.length - {index}";
        }
        return operand.ToString();
    }

    public override string VisitReturnStatement(ReturnStatementSyntax node)
    {
        var jsBuilder = new StringBuilder();
        var method = GetNodeInParent<MethodDeclarationSyntax>(node)!;
        if (method.ReturnType is TupleTypeSyntax tupleType)
        {
            if (node.Expression is TupleExpressionSyntax tupleExpressionSyntax)
            {
                jsBuilder.Append("return {");
                var arguments = tupleExpressionSyntax.Arguments;
                var elements = tupleType.Elements;

                for(var i = 0;i < arguments.Count; i++){
                    var expression = Visit(arguments[i].Expression);
                    var name = elements[i].Identifier.Text;
                    jsBuilder.Append($"{name}:{expression}");

                    if(i<arguments.Count-1){
                        jsBuilder.Append(", ");
                    }
                }
                jsBuilder.Append('}');
                return jsBuilder.ToString();
            }
            else
            {
                throw new Exception("return statement doesnt have tuple");
            }
        }
        else{
            throw new Exception("Expecting tuple to method returntype to have tuple");
        }
    }

    public override string VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
    {
        var operatorKind = node.OperatorToken.Kind();
        var operand = Visit(node.Operand);
        return operatorKind switch
        {
            SyntaxKind.PlusPlusToken => $"++{operand}",
            SyntaxKind.MinusMinusToken => $"--{operand}",
            SyntaxKind.ExclamationToken => $"!{operand}",
            SyntaxKind.PlusToken => $"+{operand}",
            SyntaxKind.MinusToken => $"-{operand}",
            SyntaxKind.TildeToken => $"~{operand}",
            _ => throw new Exception("operatorkind not expected: " + operatorKind),
        };
    }

    public override string? DefaultVisit(SyntaxNode node)
    {
        throw new Exception(node.GetType().FullName);
    }
}

static class CSharp2Javascript{
    public static void Run(string projectName, string csharpCode){
        SyntaxTree tree = CSharpSyntaxTree.ParseText(csharpCode);
        SyntaxNode root = tree.GetRoot();

        var visitor = new CSharpToJavaScriptVisitor();

        var jsBuilder = new StringBuilder();
        jsBuilder.AppendLine(@"
function CreateCanvas(width,height){
    var canvas = document.createElement('canvas');
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    document.body.appendChild(canvas);
    document.body.style.overflow = 'hidden';
    document.body.style.margin = '0px';
    addEventListener('resize', ()=>{
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    });
    return canvas.getContext('2d');
}

class Input{
}

class Graphics{
    static Clear(color){
        ctx.fillStyle = color;
        ctx.fillRect(0,0,ctx.canvas.width,ctx.canvas.height);
    }

    static FillRect(x,y,width,height,color){
        ctx.fillStyle = color;
        ctx.fillRect(x,y,width,height);
    }

    static FillText(x,y,text,fontSize,color){
        ctx.fillStyle = color;
        ctx.font = fontSize+'px Arial';
        ctx.fillText(text,x,y+fontSize);
    }

    static AddPoint(x, y){
        Graphics.points.push({x,y});
    }

    static Fill(color){
        ctx.beginPath();
        for(var p of Graphics.points){
            ctx.lineTo(p.x, p.y);
        }
        ctx.fillStyle = color;
        ctx.fill();
        Graphics.points = [];
    }

    static GetWidth(){
        return ctx.canvas.width;
    }

    static GetHeight(){
        return ctx.canvas.height;
    }
}

var ctx = CreateCanvas(800,600);
Graphics.points = [];

var lastUpdate = Date.now();
        ");

    jsBuilder.AppendLine(visitor.Visit(root));
    jsBuilder.AppendLine($@"function KeyDown(e){{
    {projectName}.KeyDown(e.key);
}}
addEventListener('keydown', KeyDown);");
    jsBuilder.AppendLine($@"function MouseMove(e){{
    Input.mousex = e.clientX;
    Input.mousey = e.clientY;
    if({projectName}.MouseMove){{
        {projectName}.MouseMove(e.clientX, e.clientY);
    }}
}}
addEventListener('mousemove', MouseMove);");
    jsBuilder.AppendLine($@"function Draw(){{
    var now = Date.now();
    if({projectName}.Draw){{
        {projectName}.Draw((now - lastUpdate)/1000);
    }}
    lastUpdate = now;
    requestAnimationFrame(Draw);
}}
Draw();");
        
        var indexHTML = $@"<!doctype html>
<html>
  <head>
    <title>{projectName}</title>
  </head>
  <body>
    <script>
"+jsBuilder.ToString()+@"</script>
  </body>
</html>";
        File.WriteAllText("Web/index.html",indexHTML);
    }
}