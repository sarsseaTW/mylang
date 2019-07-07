using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLang
{
    namespace Ast
    {
        /// <summary>
        /// AST(Abstract Syntax Tree)のベースクラス
        /// </summary>
        public abstract class Ast
        {
            public abstract Tuple<string, Ast[]> GetDisplayInfo();
        }

        /// <summary>
        /// 式(Expression) のベースクラス
        /// </summary>
        public abstract class Exp : Ast
        {
        }

        public enum BinOpType
        {
            Add,
            Sub,
            Multiply,
            Divide,
        }

        /// <summary>
        /// 二項演算子(Binary Operator)
        /// </summary>
        public class BinOp : Exp
        {
            public readonly BinOpType Operator;
            public readonly Exp Lhs;
            public readonly Exp Rhs;
            public BinOp(BinOpType op, Exp lhs, Exp rhs)
            {
                Operator = op;
                Lhs = lhs;
                Rhs = rhs;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Operator.ToString(), new Ast[] { Lhs, Rhs });
            }
        }

        public class Number : Exp
        {
            public readonly float Value;
            public Number(float value)
            {
                Value = value;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Value.ToString(), (Ast[])null);
            }
        }

        /// <summary>
        /// ASTを文字列表現に変換するクラス
        /// </summary>
        public class AstDisplayer
        {
            List<Tuple<int, string>> list_;

            public AstDisplayer()
            {

            }

            public string BuildString(Ast ast, bool prettyPrint = true)
            {
                list_ = new List<Tuple<int, string>>();
                build(0, ast);
                if( prettyPrint)
                {
                    return string.Join("\n", list_.Select(s => new string(' ', s.Item1 * 2) + s.Item2).ToArray());
                }
                else
                {
                    return string.Join(" ", list_.Select(s => s.Item2).ToArray());
                }
            }

            void build(int level, Ast ast)
            {
                var displayInfo = ast.GetDisplayInfo();
                if (displayInfo.Item2 == null)
                {
                    add(level, displayInfo.Item1);
                }
                else
                {
                    add(level, displayInfo.Item1 + "(");
                    foreach( var child in displayInfo.Item2)
                    {
                        build(level + 1, child);
                    }
                    add(level, ")");
                }
            }

            void add(int level, string text)
            {
                list_.Add(Tuple.Create(level, text));
            }
        }
    }

}
