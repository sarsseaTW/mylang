﻿using System;
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
            bool int_isStr = false;
            bool eng_isStr = false;
            string sum_int = "";
            string sum_eng = "";
            for (int i = 0; i < src.Length; i++)
            {
                // 初めステータス　を　設定
                var any = src[i];
                var any_2 = ' ';
                // 今まのステータス　と　またのステータス　を　取得する
                if (i != src.Length - 1)
                {
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
                        Console.WriteLine("sum_eng");
                        Console.WriteLine(sum_eng + " \n");
                        dummy.Add(new Token(TokenType.Symbol, sum_eng.ToString()));
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
                        Console.WriteLine("sum_int");
                        Console.WriteLine(sum_int + " \n");
                        dummy.Add(new Token(TokenType.Number, sum_int.ToString()));
                        sum_int = "";
                    }
                    int_isStr = false;
                }
                //　トークンの種類　を　取得している
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

            }
            if (int_isStr)
            {
                Console.WriteLine("sum_int");
                Console.WriteLine(sum_int + " \n");
                dummy.Add(new Token(TokenType.Number, sum_int.ToString()));
                sum_int = "";
            }

            if (eng_isStr)
            {
                Console.WriteLine("sum_eng");
                Console.WriteLine(sum_eng + " \n");
                dummy.Add(new Token(TokenType.Symbol, sum_eng.ToString()));
                sum_eng = "";
            }
            dummy.Add(new Token(TokenType.Terminate, "{EOF}"));


            Console.WriteLine("Token");
            Console.WriteLine(string.Join(" ", dummy.Select(t => t.Text).ToArray()) + "\n");
            return dummy;
        }

    }
}
