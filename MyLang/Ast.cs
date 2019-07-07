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
            /// <summary>
            /// 文字列表現を作成するための情報を取得する.
            /// 
            /// string は 文字列でのそのASTの種類を表す
            /// Ast[] は、子供のASTを返す。子供を取らないASTの場合は、nullが入る。
            /// </summary>
            /// <returns>文字列表現のための情報</returns>
            public abstract Tuple<string, Ast[]> GetDisplayInfo();
        }

        #region Expression

        /// <summary>
        /// 式(Expression) のベースクラス
        /// </summary>
        public abstract class Exp : Ast { }

        /// <summary>
        /// ２項演算子の種類
        /// </summary>
        public enum BinOpType
        {
            Add, // +
            Sub, // -
            Multiply, // *
            Divide, // /
        }

        /// <summary>
        /// 二項演算子(Binary Operator)を表すAST
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

        /// <summary>
        /// 数値を表すAST
        /// </summary>
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
        /// 変数を表すAST
        /// </summary>
        public class Symbol : Exp
        {
            public readonly string Name;
            public Symbol(string name)
            {
                Name = name;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Name, (Ast[])null);
            }
        }
        #endregion

        #region Statement
        /// <summary>
        /// 文(Satement) のベースクラス
        /// </summary>
        public class Program : Ast
        {
            public readonly Statement[] Statements;

            public Program(IList<Statement> statements)
            {
                Statements = statements.ToArray();
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Program", Statements.Select(s=>(Ast)s).ToArray());
            }
        }

        /// <summary>
        /// 文(Satement) のベースクラス
        /// </summary>
        public abstract class Statement : Ast { }

        public class AssignStatement : Statement
        {
            public readonly Exp Variable;
            public readonly Exp Exp;
            public AssignStatement(Exp variable, Exp exp)
            {
                Variable = variable;
                Exp = exp;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("let", new Ast[] { Variable, Exp });
            }

        }

        public class PrintStatement : Statement
        {
            public readonly Exp Exp;
            public PrintStatement(Exp exp)
            {
                Exp = exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("print", new Ast[] { Exp });
            }
        }

        public class ReturnStatement : Statement
        {
            public readonly Exp Exp;
            public ReturnStatement(Exp exp)
            {
                Exp = exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("return", new Ast[] { Exp });
            }
        }

        public class FunctionStatement : Statement
        {
            public readonly Symbol Name;
            public readonly Symbol[] Parameters;
            public readonly Statement[] Body;
            public FunctionStatement(Symbol name, IList<Symbol> parameters, IList<Statement> body)
            {
                Name = name;
                Parameters = parameters.ToArray();
                Body = body.ToArray();
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("function", new Ast[] { Name, new AstList(Parameters), new AstList(Body) });
            }
        }

        public class AstList : Ast
        {
            public Ast[] List;
            public AstList(Ast[] list)
            {
                List = list;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("", List);
            }
        }

        #endregion

        #region AstDisplayer
        /// <summary>
        /// ASTを文字列表現に変換するクラス
        /// </summary>
        public class AstDisplayer
        {
            List<Tuple<int, string>> list_;

            public AstDisplayer() { }

            /// <summary>
            /// ASTから、文字列表現に変換する.
            /// 
            /// prettyPrintにtrueを指定すると、改行やインデントを挟んだ読みやすい表現になる
            /// 
            /// BuildString(1 + 2 * 3 の AST, false) => "Add( 1 Multiply( 2 3 ) )"
            /// 
            /// BuildString(1 + 2 * 3 の AST, true) => 
            ///   "Add( 
            ///     1 
            ///     Multiply(
            ///       2
            ///       3
            ///     )
            ///    )"
            /// </summary>
            /// <param name="ast">対象のAST</param>
            /// <param name="prettyPrint">Pretty pring をするかどうか</param>
            /// <returns></returns>
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
        #endregion
    }

}
