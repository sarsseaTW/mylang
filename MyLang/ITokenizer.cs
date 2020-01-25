using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    /// <summary>
    /// トークンの種類
    /// </summary>
    public enum TokenType
    {
        Terminate, // ソースの終わりを表す

        Number, // 数値
        Symbol, // 識別子
        String, // 文字列

        Plus, // "+"
        Minus, // "-"
        Star, // "*"
        Slash, // "/"
        Semicolon, // ";"
        Equal, // "="
        LParen, // "("
        RParen, // ")"
        Comma, // ","
        LBraket, // "{"
        RBraket, // "}"

        EqualEqual, // "=="
        NotEqual, // "!="
        Less, // "<"
        LessEqual, // "<="
        Greater, // ">"
        GreaterEqual, // ">="

        Let, 
        Print,
        Function,
        End,
        Return,
        If,
        Else,
        Loop,
        Break,
    }

    /// <summary>
    /// トークン
    /// </summary>
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Text;

        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
        }

        public bool IsTerminate => (Type == TokenType.Terminate);
        public bool IsNumber => (Type == TokenType.Number);
        public bool IsSymbol => (Type == TokenType.Symbol);
        public bool IsBinaryOperator => (Type == TokenType.Plus || Type == TokenType.Minus || Type == TokenType.Star || Type == TokenType.Slash);

        static Dictionary<string, TokenType> strToTokenDict = new Dictionary<string, TokenType>()
        {
            { "+", TokenType.Plus },
            { "-", TokenType.Minus },
            { "*", TokenType.Star },
            { "/", TokenType.Slash },
            { ";", TokenType.Semicolon },
            { "=", TokenType.Equal },
            { "(", TokenType.LParen },
            { ")", TokenType.RParen },
            { "{", TokenType.LBraket },
            { "}", TokenType.RBraket },
            { ",", TokenType.Comma },

            { "==", TokenType.EqualEqual },
            { "!=", TokenType.NotEqual },
            { "<", TokenType.Less },
            { "<=", TokenType.LessEqual },
            { ">", TokenType.Greater },
            { ">=", TokenType.GreaterEqual },

            { "print", TokenType.Print },
            { "let", TokenType.Let },
            { "function", TokenType.Function },
            { "end", TokenType.End },
            { "return", TokenType.Return },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "loop", TokenType.Loop },
            { "break", TokenType.Break },
        };

        /// <summary>
        /// stringをTokenに変換する
        /// </summary>
        /// <param name="str">対象の文字列</param>
        /// <param name="errorIfNotFound">trueなら、変換できないときに例外を投げる</param>
        /// <returns></returns>
        public static Token FromString(string str, bool errorIfNotFound = true)
        {
            TokenType tt;
            if (strToTokenDict.TryGetValue(str, out tt))
            {
                return new Token(tt, str);
            }
            else
            {
                if (errorIfNotFound)
                {
                    throw new Exception($"Invalid token {str}");
                }
                else
                {
                    return null;
                }
            }
        }

        public static Token CreateTerminator()
        {
            return new Token(TokenType.Terminate, "[EOF]");
        }


    }

    public interface ITokenizer
    {
        /// <summary>
        /// ソースコードをトークンに分割する
        /// </summary>
        /// <param name="src">ソースコード</param>
        /// <returns>トークンのリスト</returns>
        IList<Token> Tokenize(string src);
    }
}
