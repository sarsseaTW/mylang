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
        //Dictionary<string, float> symbol_dict = new Dictionary<string, float>();
        public float Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装


            Console.WriteLine("Interpreter.Run");// *-- ast.ToString() => BinOp;
            Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, true));
            Parser _parser = new Parser();
            _parser.symbol_dict[_parser.symbol_str] = Run_exp(ast as Exp);
            return _parser.symbol_dict[_parser.symbol_str];//as 強制轉型
        }
        float Run_exp(Exp exp)
        {
            Console.WriteLine("Run_exp.Run");// *-- ast.ToString() => BinOp;
            Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(exp, true) + "\n");
            Console.WriteLine("----------------------------------------------------------------");
            if (exp is BinOp)//  [ '+' , '-' , '*' , '/']
            {
                var _binOp = exp as BinOp;

                float exp_LHS = Run_exp(_binOp.Lhs);
                float exp_RHS = Run_exp(_binOp.Rhs);

                switch (_binOp.Operator)
                {
                    case BinOpType.Add:
                        return exp_LHS + exp_RHS;
                    case BinOpType.Sub:
                        return exp_LHS - exp_RHS;
                    case BinOpType.Multiply:
                        return exp_LHS * exp_RHS;
                    case BinOpType.Divide:
                        return exp_LHS / exp_RHS;
                }
            }
            else if(exp is Number)
            {
                var num = exp as Number;
                return num.Value;
            }
            //else if (exp is Symbol)
            //{
            //    var num = exp as Symbol;
            //    float found;
            //    if (symbol_dict.TryGetValue(num.Value, out found))
            //    {
            //        return found;
            //    }
            //    else
            //    {
            //        Console.WriteLine(num.Value + " = ");
            //        var _input = Console.ReadLine();
            //        var n = Convert.ToSingle(_input);
            //        symbol_dict[num.Value] = n;
            //        return n;
            //    }
            //}
            //else if (exp is Let)
            //{
            //    var num = exp as Let;

            //    return 1;
            //}
            return 0;
        }
    }
}
