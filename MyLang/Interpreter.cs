using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    public class Env
    {
        Dictionary<string, float> variables_ = new Dictionary<string, float>();

        public Env()
        {

        }

        public float Get(string name)
        {
            return variables_[name];
        }

        public void Set(string name, float val)
        {
            variables_[name] = val;
        }
    }

    public class Interpreter
    {
        Env env_ = new Env();

        public Interpreter()
        {
        }

        public void Run(Ast.Ast ast)
        {
            runProgram((Ast.Program)ast);
        }

        public void runProgram(Ast.Program prog)
        {
            foreach( var stat in prog.Statements)
            {
                if (stat is PrintStatement) {
                    var s = (PrintStatement)stat;
                    float value = runExp(s.Exp);
                    Console.WriteLine(value);
                }
                else if( stat is AssignStatement)
                {
                    var s = (AssignStatement)stat;
                    float value = runExp(s.Exp);
                    env_.Set(((Ast.Symbol)s.Variable).Name, value);
                }
            }
        }

        float runExp(Exp exp)
        {
            if( exp is BinOp)
            {
                var binop = exp as BinOp;
                float lhsValue = runExp(binop.Lhs);
                float rhsValue = runExp(binop.Rhs);
                switch( binop.Operator)
                {
                    case BinOpType.Add:
                        return lhsValue + rhsValue;
                    case BinOpType.Sub:
                        return lhsValue - rhsValue;
                    case BinOpType.Multiply:
                        return lhsValue * rhsValue;
                    case BinOpType.Divide:                        ;
                        return lhsValue / rhsValue;
                    default:
                        throw new Exception($"Unkonwn operator {binop.Operator}");
                }
            }
            else if( exp is Number)
            {
                var number = exp as Number;
                return number.Value;
            }
            else if (exp is Symbol)
            {
                var symbol = exp as Symbol;
                return env_.Get(symbol.Name);
            }
            else
            {
                throw new Exception("BUG");
            }
        }
    }
}
