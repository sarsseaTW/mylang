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
        public SpaceSeparatedTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {

            Console.WriteLine("rest to array");
            Console.WriteLine(src + " \n");
            Console.WriteLine("src length");
            Console.WriteLine(src.Length + " \n");
            /* Console.WriteLine("src[0]");
             Console.WriteLine(src[0] + " \n");
             Console.WriteLine("src[11]");
             Console.WriteLine(src[11] + " \n");*/

            // TODO: 仮のダミー実装
            var dummy = new List<Token>();

            bool src_skip = false;
            bool src_skip2 = false;
            for (int i = 0; i < src.Length; i++)
            {
                // 初めステータス　を　設定
                var any = src[i];
                var any_2 = ' ';
              　// 今まのステータス　と　またのステータス　を　取得する
                if (i != src.Length - 1) {
                    any_2 = src[i + 1];
                }
                //　"//" を　取得している
                if (any.ToString() == "/" && any_2.ToString() == "/")
                {
                    break;
                }
                //　"/*" と "*/" を　取得している
                if (src_skip)
                {
                    if (any.ToString() == "*" && any_2.ToString() == "/")
                    {
                        src_skip = false;
                        src_skip2 = true;
                    }
                    continue;
                }
                else
                {
                    if (any.ToString() == "/" && any_2.ToString() == "*")
                    {
                        src_skip = true;
                        continue;
                    }
                }
                //　"/*" と "*/" の　二次skip
                if (src_skip2) {
                    src_skip2 = false;
                    continue;
                }
                //　Token　を　取得している
                switch (any.ToString())
                {
                    case "":
                        dummy.Add(new Token(TokenType.Space, ""));
                        break;
                    case "+":
                        dummy.Add(new Token(TokenType.Plus, "+"));
                        break;
                    case "-":
                        dummy.Add(new Token(TokenType.Minus, "-"));
                        break;
                    case "*":
                        dummy.Add(new Token(TokenType.Star, "*"));
                        break;
                    case "/":
                        dummy.Add(new Token(TokenType.Slash, "/"));
                        break;
                    case "(":
                        dummy.Add(new Token(TokenType.Left_parenthesis, "("));
                        break;
                    case ")":
                        dummy.Add(new Token(TokenType.Right_parenthesis, ")"));
                        break;
                }

                bool reg_symbol;
                if (reg_symbol = Regex.IsMatch(any.ToString(), "^[A-Za-z]"))
                {
                    dummy.Add(new Token(TokenType.Symbol, any.ToString()));
                }

                bool reg_number;
                if (reg_number = Regex.IsMatch(any.ToString(), "^[0-9]"))
                {
                    dummy.Add(new Token(TokenType.Number, any.ToString()));
                }
            }
            dummy.Add(new Token(TokenType.Terminate, "{EOF}"));


            Console.WriteLine("Token");
            Console.WriteLine(string.Join(" ", dummy.Select(t => t.Text).ToArray()) + "\n");
            return dummy;
        }

    }
}
