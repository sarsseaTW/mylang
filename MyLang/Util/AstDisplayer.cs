using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLang
{
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
        /// <param name="prettyPrint">Pretty print をするかどうか</param>
        /// <returns></returns>
        public string BuildString(Ast.Ast ast, bool prettyPrint = true)
        {
            list_ = new List<Tuple<int, string>>();
            build(0, ast);
            if (prettyPrint)
            {
                return string.Join("\n", list_.Select(s => new string(' ', s.Item1 * 2) + s.Item2).ToArray());
            }
            else
            {
                return string.Join(" ", list_.Select(s => s.Item2).ToArray());
            }
        }

        void build(int level, Ast.Ast ast)
        {
            var displayInfo = ast.GetDisplayInfo();
            if (displayInfo.Item2 == null)
            {
                add(level, displayInfo.Item1);
            }
            else
            {
                add(level, displayInfo.Item1 + "(");
                foreach (var child in displayInfo.Item2)
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
