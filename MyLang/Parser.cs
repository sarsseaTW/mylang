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

        /// <summary>
        /// パースする
        /// 
        /// BNF:
        /// 
        /// exp ::= exp1
        /// 
        /// exp1 ::= exp_2 (A) | exp_2 (B) exp1_op exp1
        /// 
        /// exp2 ::= exp_value (A) | exp_value (B) exp1_op exp2
        /// 
        /// exp_value = number | symbol
        /// 
        /// exp1_op::= + | -
        /// exp1_op::= * | /
        /// 
        /// </summary>
        public Ast.Ast Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            return parseExp1();
        }

        Ast.Exp parseExp1()
        {
            var lhs = parseExp2();
            if (lhs == null)
            {
                return null;
            }

            var t = currentToken();
            if (t.Type == TokenType.Plus || t.Type == TokenType.Minus)
            {
                // Pattern (B)
                var binopType = BinOpMap[t.Type];
                progress();
                var rhs = parseExp1();
                if (rhs == null)
                {
                    throw new Exception("No rhs parsed");
                }

                return new Ast.BinOp(binopType, lhs, rhs);
            }
            else
            {
                // Pattern (A)
                return lhs;
            }
        }

        Ast.Exp parseExp2()
        {
            var lhs = parseExpValue();
            if (lhs == null)
            {
                return null;
            }

            var t = currentToken();
            if (t.Type == TokenType.Star || t.Type == TokenType.Slash)
            {
                // Pattern (B)
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
                // Pattern (A)
                return lhs;
            }
        }

#if false
        /// <summary>
        /// 左結合のために、Exp1を分割したもの
        /// </summary>
        /// <param name="lhs">パース済みの左辺項</param>
        Ast.Exp parseExp1Rest(Ast.Exp lhs)
        {
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

                var exp = new Ast.BinOp(binopType, lhs, rhs);

                return parseExp1Rest(exp);
            }
            else
            {
                return lhs;
            }
        }

        /// <summary>
        /// "*", "/" の演算子をパースする
        /// </summary>
        Ast.Exp parseExp2()
        {
            var lhs = parseValue();
            if( lhs == null)
            {
                return null;
            }

            return parseExp2Rest(lhs);
        }

        /// <summary>
        /// 左結合のために、Exp2を分割したもの
        /// </summary>
        /// <param name="lhs">パース済みの左辺項</param>
        Ast.Exp parseExp2Rest(Ast.Exp lhs)
        {
            var t = currentToken();
            if (t.Type == TokenType.Star || t.Type == TokenType.Slash)
            {
                var binopType = BinOpMap[t.Type];
                progress();

                var rhs = parseValue();
                if (rhs == null)
                {
                    throw new Exception("No rhs parsed");
                }

                var exp = new Ast.BinOp(binopType, lhs, rhs);

                return parseExp2Rest(exp);
            }
            else
            {
                return lhs;
            }
        }
#endif

        /// <summary>
        /// 値をパースする
        /// </summary>
        /// <returns></returns>
        Ast.Exp parseExpValue()
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
