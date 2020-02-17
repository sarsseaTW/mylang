using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    public class Interpreter
    {
        public Interpreter()
        {
        }

        public float Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装


            Console.WriteLine("Interpreter.Run");// *-- ast.ToString() => BinOp;
            Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, false));
            return 0;
        }
        float Run_exp(Exp exp)
        {

            return 0;
        }
    }
}
