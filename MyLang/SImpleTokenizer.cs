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
    class SimpleTokenizer : ITokenizer
    {
        string src_;
        int pos_;
        int start_;
        List<Token> tokens_ = new List<Token>();

        public SimpleTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            src_ = src;
            pos_ = 0;
            while (pos_ < src_.Length)
            {
                start_ = pos_;
                var c = src_[pos_];
                if (isSpace(c))
                {
                    lexSeparator();
                }
                else if (isDigit(c))
                {
                    lexNumber();
                }
                else if (isAlphabet(c))
                {
                    lexSymbolOrKeyword();
                }
                else
                {
                    lexOperator();
                }
            }

            tokens_.Add(new Token(TokenType.Terminate, "[EOF]"));
            return tokens_;
        }

        bool isSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        bool isAlphabet(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == '@';
        }

        bool isAlphabetOrDigit(char c)
        {
            return isAlphabet(c) || isDigit(c);
        }

        void lexSeparator()
        {
            while (pos_ < src_.Length)
            {
                var c = src_[pos_];
                if (!isSpace(c))
                {
                    break;
                }
                pos_++;
            }
        }

        void lexNumber()
        {
            while (pos_ < src_.Length)
            {
                var c = src_[pos_];
                if (!isDigit(c))
                {
                    break;
                }
                pos_++;
            }
            tokens_.Add(new Token(TokenType.Number, src_.Substring(start_, pos_ - start_)));
        }

        void lexSymbolOrKeyword()
        {
            while (pos_ < src_.Length)
            {
                var c = src_[pos_];
                if (!isAlphabetOrDigit(c))
                {
                    break;
                }
                pos_++;
            }
            var str = src_.Substring(start_, pos_ - start_);
            var token = strToToken(str, false);
            if (token != null)
            {
                tokens_.Add(token);
            }
            else {
                tokens_.Add(new Token(TokenType.Symbol, str));
            }
        }

        void lexOperator()
        {
            var c = src_[pos_];
            pos_++;
            tokens_.Add(strToToken(src_.Substring(start_, pos_ - start_)));
        }

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
            { "print", TokenType.Print },
            { "let", TokenType.Let },
            { "def", TokenType.Def },
            { "end", TokenType.End },
            { "return", TokenType.Return },
        };

        Token strToToken(string str, bool errorIfNotFound = true)
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
    }
}
