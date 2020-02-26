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
        public Dictionary<string, float> symbol_dict = new Dictionary<string, float>();
        public string symbol_str = "";
        public void Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装
            Console.WriteLine(new AstDisplayer().BuildString(ast, false));
            Console.WriteLine("Interpreter.Run");// *-- ast.ToString() => BinOp;
            
            if (ast is PrintStatement P)
            {
                Console.WriteLine(Run_exp(P.Exp));
            }
            if (ast is LetStatement L)
            {
                symbol_str = L.Variable.Value.ToString();
                symbol_dict[symbol_str] = Run_exp(L.Exp);
                Console.WriteLine(symbol_dict[symbol_str]);
            }
            if(ast is Ast.Program pg)
            {
                Console.WriteLine("ast -> statement[]");
                function_statement(pg.Statements);
            }
        }
        void function_statement(Statement[] f_st)
        {
            Console.WriteLine("statement[] -> functionStatement");
            function_Body(f_st[0]);
        }
        void function_Body(Statement f_bd)
        {
            
        }
        float Run_exp(Exp exp)
        {
            //Console.WriteLine("Run_exp.Run");// *-- ast.ToString() => BinOp;
            //Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(exp, true) + "\n");
            //Console.WriteLine("----------------------------------------------------------------");
            if (exp is BinOp)//  [ '+' , '-' , '*' , '/']
            {
                var _binOp = exp as BinOp;

                float exp_LHS = Run_exp(_binOp.Lhs);
                float exp_RHS = Run_exp(_binOp.Rhs);

                switch (_binOp.Operator)
                {
                    case BinOpType.Add:
                        var sum = exp_LHS + exp_RHS;
                        symbol_dict[symbol_str] = sum;
                        return symbol_dict[symbol_str];
                    case BinOpType.Sub:
                        var sum2 = exp_LHS - exp_RHS;
                        symbol_dict[symbol_str] = sum2;
                        return symbol_dict[symbol_str];
                    case BinOpType.Multiply:
                        var sum3 = exp_LHS * exp_RHS;
                        symbol_dict[symbol_str] = sum3;
                        return symbol_dict[symbol_str];
                    case BinOpType.Divide:
                        var sum4 = exp_LHS / exp_RHS;
                        symbol_dict[symbol_str] = sum4;
                        return symbol_dict[symbol_str];
                    case BinOpType.Equal:
                        symbol_dict[symbol_str] = exp_RHS;
                        return symbol_dict[symbol_str];
                }
            }
            else if(exp is Number)
            {
                var num = exp as Number;
                return num.Value;
            }
            else if (exp is Symbol)
            {
                var num = exp as Symbol;
                float found;
                if (symbol_dict.TryGetValue(num.Value, out found))
                {
                    return found;
                }
                else
                {
                    symbol_str = num.Value;
                    symbol_dict[symbol_str] = 0;
                    return symbol_dict[symbol_str];
                }
            }
            return 0;
        }
    }
}
