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
            return runExp(ast as Exp);
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
            else
            {
                throw new Exception("BUG");
            }
        }
    }
}
