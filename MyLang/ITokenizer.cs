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

        Plus, // "+"
        Minus, // "-"
        Star, // "*"
        Slash, // "/"
        Left_parenthesis, // "("
        Right_parenthesis, // ")"
        Space, // " "
        Semicolon, // ";"
        Equal, // "="
        Comma, // ","
        LBraket, // "{"
        RBraket, // "}"
        Print, // "print"
        Let, // "let"
        Function, // "function"
        Return, // "return"

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

        public bool IsNumber => (Type == TokenType.Number);
        public bool IsSymbol => (Type == TokenType.Symbol);

        
        public bool IsLet => (Type == TokenType.Let);
        public bool IsPrint => (Type == TokenType.Print);
        public bool IsFunction => (Type == TokenType.Function);
        public bool IsReturn => (Type == TokenType.Return);

        public bool IsEqual => (Type == TokenType.Equal);
        public bool IsLBraket => (Type == TokenType.LBraket);
        public bool IsRBraket => (Type == TokenType.RBraket);

        public bool IsSemicolon => (Type == TokenType.Semicolon);
        public bool IsTerminate => (Type == TokenType.Terminate);

        public bool IsBinaryOperator => (Type == TokenType.Plus || Type == TokenType.Minus 
            || Type == TokenType.Star || Type == TokenType.Slash
            || Type == TokenType.Left_parenthesis || Type == TokenType.Right_parenthesis
            || Type == TokenType.Space);
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
