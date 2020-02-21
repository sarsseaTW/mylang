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
        bool is_function = false;
        static Dictionary<TokenType, Ast.BinOpType> BinOpMap = new Dictionary<TokenType, Ast.BinOpType>
        {
            {TokenType.Plus, Ast.BinOpType.Add }, // '+'
            {TokenType.Minus, Ast.BinOpType.Sub }, // '-'
            {TokenType.Star, Ast.BinOpType.Multiply }, // '*'
            {TokenType.Slash, Ast.BinOpType.Divide },  // '/'
            {TokenType.Equal, Ast.BinOpType.Equal },  // '='
            {TokenType.Let, Ast.BinOpType.Let },  // 'Let'
            {TokenType.Semicolon, Ast.BinOpType.Semicolon },  // ';'
            {TokenType.LBraket, Ast.BinOpType.LBraket },  // '{'
            {TokenType.RBraket, Ast.BinOpType.RBraket },  // '{'
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
        public Ast.Exp Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            Console.WriteLine("Ast.Ast Parse ");
            Console.WriteLine(string.Join(" ", tokens_.Select(t => t.Text).ToArray()) + "\n");
            return statement();
        }
        //Ast.Exp block()
        //{
        //    var main_block = statement();
        //    return block_realArea(main_block);
        //}
        //Ast.Exp block_realArea(Ast.Exp left_hs)
        //{
        //    var R_block = statement();
        //    return statement();
        //}
        Ast.Exp statement()
        {
            var left_hs = keyWord();
            return statement_realArea(left_hs);
        }
        Ast.Exp statement_realArea(Ast.Exp left_hs)
        {
            var tokenPos = currentToken();

            if (tokenPos.IsSymbol)
            {
                return statement();
            }
            else if (tokenPos.IsEqual)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                Console.WriteLine("\n" + tokenPos.Text + "            IsEqual  statement_realArea tokenPos.ToString\n");
                progress();
                var right_hs = exp1();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 A = 1 + 2 ;
                return statement_realArea(exp);
            }
            else if (tokenPos.IsLBraket)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                Console.WriteLine("\n" + tokenPos.Text + "            IsLBraket  statement_realArea tokenPos.ToString\n");
                progress();
                //var right_hs = block(); // jump to block
                var right_hs = statement(); // jump to block
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 A { };
                return statement_realArea(exp);
            }
            else if (tokenPos.IsRBraket)
            {
                is_function = false;
                return left_hs;
            }
            else
                return left_hs;
        }
        Ast.Exp keyWord()
        {
            var tokenPos = currentToken();
            if (tokenPos.IsSymbol)
            {
                progress();
                Console.WriteLine("\n" + tokenPos.Text + "            IsSymbol keyWord tokenPos.ToString\n");
                return new Ast.Symbol(tokenPos.Text);
            }
            if (tokenPos.IsLet)
            {
                progress();
                Console.WriteLine("\n" + tokenPos.Text + "            IsLet keyWord tokenPos.ToString\n");

                return new Ast.Let(tokenPos.Text);
            }
            else if (tokenPos.IsPrint)
            {
                progress();

                Console.WriteLine("\n" + tokenPos.Text + "            IsPrint keyWord tokenPos.ToString\n");

                return exp1();
            }
            else if (tokenPos.IsFunction)
            {
                progress();
                is_function = true;
                Console.WriteLine("\n" + tokenPos.Text + "            IsFunction keyWord tokenPos.ToString\n");

                return new Ast.Function(tokenPos.Text);
            }
            else if (tokenPos.IsReturn)
            {
                progress();

                Console.WriteLine("\n" + tokenPos.Text + "            IsReturn keyWord tokenPos.ToString\n");

                return new Ast.Return(tokenPos.Text);
            }
            return new Ast.Symbol(tokenPos.Text);
        }
        Ast.Exp assign()
        {
            var left_hs = keyWord();
            return exp1_realArea(left_hs);
        }
        Ast.Exp exp1()
        {
            var left_hs = exp2();
            return exp1_realArea(left_hs);
        }
        Ast.Exp exp1_realArea(Ast.Exp left_hs)
        {
            var tokenPos = currentToken();
            if (tokenPos.Type == TokenType.Plus || tokenPos.Type == TokenType.Minus)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                Console.WriteLine(now_tokenBioMap.ToString() + "        exp1_realArea ");
                progress();
                var right_hs = exp2();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 ex: 1+4+8-82+5
                return exp1_realArea(exp);
            }
            else
            {
                return left_hs;
            }
        }
        Ast.Exp exp2()
        {
            var left_hs = expVal();
            return exp2_realArea(left_hs);
        }
        Ast.Exp exp2_realArea(Ast.Exp left_hs)
        {
            var tokenPos = currentToken();
            if (tokenPos.IsNumber || tokenPos.IsSymbol) 
            {
                exp2();
            }
            if (tokenPos.Type == TokenType.Star || tokenPos.Type == TokenType.Slash)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                Console.WriteLine(now_tokenBioMap.ToString() + "        exp2_realArea ");
                progress();
                var right_hs = expVal();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 ex: 1*4 *5/d
                return exp2_realArea(exp);
            }
            else if (tokenPos.Type == TokenType.Semicolon || tokenPos.IsTerminate)
            {
                Console.WriteLine("\n" + tokenPos.Text + "            IsSemicolon statement_realArea tokenPos.ToString\n");

                if (is_function)
                {
                    progress();
                    return statement();
                }
                else
                {
                    return left_hs;
                }
            }
            else
            {
                return left_hs;
            }
        }
        Ast.Exp expVal()
        {
            var tokenPos = currentToken();
            if (tokenPos.IsNumber)
            {
                progress();
                
                Console.WriteLine("\n" + tokenPos.Text + "            IsNumber tokenPos.ToString\n");

                return new Ast.Number(float.Parse(tokenPos.Text));
            }
            else if (tokenPos.IsSymbol)
            {
                progress();
                return new Ast.Symbol(tokenPos.Text);
            }
            else
                return new Ast.Symbol(tokenPos.Text);
        }
    }
}
