using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLang
{
    /// <summary>
    /// 単純なトークナイザ
    /// 
    /// トークンは、必ず一つ以上のスペースで区切られている必要がある
    /// </summary>
    class SpaceSeparatedTokenizer : ITokenizer
    {
        static Regex sepratorPattern = new Regex(@"\s+");
        static Regex numberPattern = new Regex(@"^\d+$");
        static Regex symbolPattern = new Regex(@"^[\w_][\w_0-9]*$");

        public SpaceSeparatedTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            var tokenStrs = sepratorPattern.Split(src);
            return tokenStrs.Select(str => strToToken(str)).ToArray();
        }

        Token strToToken(string str)
        {
            switch( str)
            {
                case "+":
                    return new Token(TokenType.Plus, str);
                case "-":
                    return new Token(TokenType.Minus, str);
                case "*":
                    return new Token(TokenType.Star, str);
                case "/":
                    return new Token(TokenType.Slash, str);
                default:
                    if(numberPattern.IsMatch(str))
                    {
                        return new Token(TokenType.Number, str);
                    }
                    else if (symbolPattern.IsMatch(str))
                    {
                        return new Token(TokenType.Symbol, str);
                    }
                    else
                    {
                        throw new Exception($"Invalid token {str}");
                    }
            }
        }
    }
}
