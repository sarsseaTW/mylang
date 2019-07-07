using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    public class Parser
    {
        IList<Token> tokens_;
        int pos_ = 0;

        static Dictionary<TokenType, Ast.BinOpType> BinOpMap = new Dictionary<TokenType, Ast.BinOpType>
        {
            {TokenType.Plus, Ast.BinOpType.Add },
            {TokenType.Minus, Ast.BinOpType.Sub },
            {TokenType.Star, Ast.BinOpType.Multiply },
            {TokenType.Slash, Ast.BinOpType.Divide },
        };

        public Parser()
        {

        }

        /// <summary>
        /// 現在のトークンを取得する
        /// </summary>
        /// <returns></returns>
        Token currentToken()
        {
            return tokens_[pos_];
        }

        /// <summary>
        /// 次のトークンに進む
        /// </summary>
        void progress()
        {
            Logger.Trace($"progress {currentToken().Text}");
            pos_++;
        }

        public Ast.Ast Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            return parseExp1();
        }

        Ast.Ast parseExp1()
        {
            var lhs = parseExp2();
            if (lhs == null)
            {
                return null;
            }

            var t = currentToken();
            if (t.Type == TokenType.Plus || t.Type == TokenType.Minus)
            {
                var binopType = BinOpMap[t.Type];
                progress();

                var rhs = parseExp2();
                if (rhs == null)
                {
                    throw new Exception("No rhs parsed");
                }
                return new Ast.BinOp(binopType, lhs, rhs);
            }
            else
            {
                return lhs;
            }
        }

        Ast.Exp parseExp2()
        {
            var lhs = parseValue();
            if( lhs == null)
            {
                return null;
            }

            var t = currentToken();
            if (t.Type == TokenType.Star || t.Type == TokenType.Slash)
            {
                var binopType = BinOpMap[t.Type];
                progress();

                var rhs = parseValue();
                if( rhs == null)
                {
                    throw new Exception("No rhs parsed");
                }
                return new Ast.BinOp(binopType, lhs, rhs);
            }
            else
            {
                return lhs;
            }
        }

        Ast.Exp parseValue()
        {
            var t = currentToken();
            if (t.IsNumber)
            {
                progress();
                return new Ast.Number(float.Parse(t.Text));
            }
            else
            {
                return null;
            }
        }
    }
}
