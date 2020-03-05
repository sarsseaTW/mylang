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
            #region Var
            // TODO: 仮のダミー実装
            var dummy = new List<Token>();

            bool src_skip = false;
            bool src_skip2 = false;
            bool int_isStr = false;
            bool eng_isStr = false;
            string sum_int = "";
            string sum_eng = "";
            bool sem_tf = false;
            #endregion
            #region Token
            for (int i = 0; i < src.Length; i++)
            {
                // 初めステータス　を　設定
                var any = src[i];
                var any_2 = ' ';
                var any_3 = ' ';
                if (i != 0)
                {
                    any_3 = src[i - 1];
                }
                // 今まのステータス　と　またのステータス　を　取得する
                if (i != src.Length - 1)
                {
                    any_2 = src[i + 1];
                }
                //　"//" を　取得している
                if (any.ToString() == "=" && any_2.ToString() == "=")
                {
                    dummy.Add(new Token(TokenType.DoubleEqual, "=="));
                    i++;
                    continue;
                }
                if (any.ToString() == ">" && any_2.ToString() == "=")
                {
                    dummy.Add(new Token(TokenType.MoreEqual, ">="));
                    i++;
                    continue;
                }
                if (any.ToString() == "<" && any_2.ToString() == "=")
                {
                    dummy.Add(new Token(TokenType.LessEqual, "<="));
                    i++;
                    continue;
                }
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
                if (src_skip2)
                {
                    src_skip2 = false;
                    continue;
                }
                //　Eng Token　を　取得している

                bool reg_symbol;
                bool reg_number;
                reg_symbol = Regex.IsMatch(any.ToString(), "[A-Za-z]");
                reg_number = Regex.IsMatch(any.ToString(), "[0-9]");
                if (reg_symbol)
                {
                    sum_eng += any.ToString();
                    eng_isStr = true;
                }
                else
                {
                    if (eng_isStr)
                    {
                        switch (sum_eng.ToString())
                        {
                            case "for":
                                dummy.Add(new Token(TokenType.FOR, "for"));
                                break;
                            case "while":
                                dummy.Add(new Token(TokenType.WHILE,"while"));
                                break;
                            case "if":
                                dummy.Add(new Token(TokenType.IF, "if"));
                                break;
                            case "elif":
                                dummy.Add(new Token(TokenType.ELIF, "elif"));
                                break;
                            case "else":
                                dummy.Add(new Token(TokenType.ELSE, "else"));
                                break;
                            case "print":
                                dummy.Add(new Token(TokenType.Print, "print"));
                                break;
                            case "let":
                                dummy.Add(new Token(TokenType.Let, "let"));
                                break;
                            case "function":
                                dummy.Add(new Token(TokenType.Function, "function"));
                                break;
                            case "return":
                                dummy.Add(new Token(TokenType.Return, "return"));
                                break;
                            default:
                                dummy.Add(new Token(TokenType.Symbol, sum_eng.ToString()));
                                break;
                        }
                        sum_eng = "";
                    }
                    eng_isStr = false;
                }
                //　Int Token　を　取得している
                if (reg_number)
                {
                    sum_int += any.ToString();
                    int_isStr = true;
                }
                else
                {
                    if (int_isStr)
                    {
                        dummy.Add(new Token(TokenType.Number, sum_int.ToString()));
                        sum_int = "";
                    }
                    int_isStr = false;
                }
                //　トークンの種類　を　取得している
                switch (any.ToString())
                {
                    case "<":
                        dummy.Add(new Token(TokenType.Less, "<"));
                        break;
                    case ">":
                        dummy.Add(new Token(TokenType.More, ">"));
                        break;
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
                        dummy.Add(new Token(TokenType.LParenthesis, "("));
                        break;
                    case ")":
                        dummy.Add(new Token(TokenType.RParenthesis, ")")); 
                        break;
                    case ";":
                        dummy.Add(new Token(TokenType.Semicolon, ";"));
                        sem_tf = true;
                        break;
                    case "=":
                        dummy.Add(new Token(TokenType.Equal, "="));
                        break;
                    case ",":
                        dummy.Add(new Token(TokenType.Comma, ","));
                        break;
                    case "@":
                        dummy.Add(new Token(TokenType.Inser, "@"));
                        break;
                    case "{":
                        dummy.Add(new Token(TokenType.LBraket, "{"));
                        break;
                    case "}":
                        //if (any_3 != ';' && any_3 != '{')
                        //{
                        //    dummy.Add(new Token(TokenType.Terminate, "{EOF}"));
                        //}
                        dummy.Add(new Token(TokenType.RBraket, "}"));
                        break;
                }
            }
            //if (!sem_tf)
            //{
            //    dummy.Add(new Token(TokenType.Terminate, "{EOF}"));
            //}
            #endregion 
            //Console.WriteLine("Token");
            //Console.WriteLine(string.Join(" ", dummy.Select(t => t.Text).ToArray()) + "\n");
            return dummy;
        }

    }
}
