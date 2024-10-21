using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

static class Compiler
{
    static List<WasmInstruction> CompileExpression(ExpressionSyntax expression){
        if(expression is LiteralExpressionSyntax literalExpressionSyntax){
            if(literalExpressionSyntax.Kind() == SyntaxKind.NumericLiteralExpression) {
                if(literalExpressionSyntax.Token.Value is int intValue){
                    return [new (Opcode.i32_const, intValue)];
                }
                else{
                    throw new Exception("Is not int");
                }
            }
            else{
                throw new Exception("Is not numericLiteralExpression");
            } 
        }
        else if(expression is InvocationExpressionSyntax invocationExpressionSyntax){
            List<WasmInstruction> argInstructions = [];
            foreach(var arg in invocationExpressionSyntax.ArgumentList.Arguments){
                argInstructions.AddRange(CompileExpression(arg.Expression));
            }
            if (invocationExpressionSyntax.Expression is IdentifierNameSyntax identifierName){
                return [..argInstructions, new (Opcode.call, identifierName.ToString())];
            }
            else{
                throw new Exception("Expecting identifiername syntax");
            }
        }
        Console.WriteLine(expression.GetType());
        return [];   
    }

    static List<WasmInstruction> CompileStatement(StatementSyntax statement){
        if(statement is ReturnStatementSyntax returnStatementSyntax){
            var instructions = new List<WasmInstruction>();
            if(returnStatementSyntax.Expression != null){
                instructions.AddRange(CompileExpression(returnStatementSyntax.Expression));
            }
            instructions.Add(new WasmInstruction(Opcode.ret));
            return instructions;
        }
        else if(statement is ExpressionStatementSyntax expressionStatementSyntax){
            return CompileExpression(expressionStatementSyntax.Expression);
        }
        throw new Exception("Error");
    }

    public static void Run(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        var method = root.DescendantNodes()
                         .OfType<MethodDeclarationSyntax>()
                         .FirstOrDefault();
        
        var body = method!.Body;
        var wasmFunction = new WasmFunction(true, Valtype.I32, "Run", []);
        foreach(var statement in body!.Statements){
            wasmFunction.instructions.AddRange(CompileStatement(statement));
        }
        var wasmEmitter = new WasmEmitter("Game/index.html");
        wasmEmitter.wasmImportFunctions.Add(new (Valtype.Void, "ConsoleLog", [new (Valtype.I32, "i")], @"console.log(i);"));
        wasmEmitter.wasmFunctions.Add(wasmFunction);
        wasmEmitter.EmitHtml();
    }
}