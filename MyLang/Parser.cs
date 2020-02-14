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
            {TokenType.Plus, Ast.BinOpType.Add }, // '+'
            {TokenType.Minus, Ast.BinOpType.Sub }, // '-'
            {TokenType.Star, Ast.BinOpType.Multiply }, // '*'
            {TokenType.Slash, Ast.BinOpType.Divide },  // '/'

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

            Console.WriteLine("Ast.Ast Parse ");
            Console.WriteLine(string.Join(" ", tokens_.Select(t => t.Text).ToArray()) + "\n");
            // TODO: 仮のダミー実装
            
            var lhs = new Ast.Number(555);
            var rhs = new Ast.Number(2159);
            var ast = new Ast.BinOp(Ast.BinOpType.Sub, lhs, rhs);
            exp1();

            Console.WriteLine("tokenPos");
            for (int i = 0; i < tokens_.LongCount();i++)
            {
                Console.WriteLine(tokens_[i].Text);
            }
            Console.WriteLine("\ntokenCount");
            Console.WriteLine(tokens_.LongCount());
            return ast;
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
            if (tokenPos.Type == TokenType.Star || tokenPos.Type == TokenType.Slash)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                Console.WriteLine(now_tokenBioMap.ToString() + "        exp2_realArea ");
                progress();
                var right_hs = exp2();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 ex: 1*4 *5/d
                return exp2_realArea(exp);
            }
            else
            {
                return left_hs;
            }
        }
        Ast.Exp expVal()
        {
            var tokenPos = currentToken();
            //var tokenPos = tokens_[14];
            if (tokenPos.IsNumber)
            {
                progress();

                Console.WriteLine("\nIsNumber tokenPos.ToString");
                Console.WriteLine(tokenPos.Text + "\n");

                return new Ast.Number(float.Parse(tokenPos.Text));
            }
            else if (tokenPos.IsSymbol)
            {
                progress();

                Console.WriteLine("\nIsSymbol tokenPos.ToString");
                Console.WriteLine(tokenPos.Text + "\n");

                return new Ast.Symbol(tokenPos.Text);
            }
            return null;
        }
    }
}
