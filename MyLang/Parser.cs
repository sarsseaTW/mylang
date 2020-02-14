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

            return ast;
        }
        
    }
}
