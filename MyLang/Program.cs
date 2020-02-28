using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using MyLang;

class Program
{
    /// <summary>
    /// コマンド の entry point
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    public static Dictionary<string, string> function_key = new Dictionary<string, string>();
    public static Dictionary<int, string> function_number = new Dictionary<int, string>();
    public static int function_key_number = 0;
    public static List<string> function_rest = new List<string>();
    public static bool chk_is_else_break, chk_is_elif_break;
    static void Main()
    {
        bool tokenizeOnly = false; // tokenize だけで終わるかどうか
        bool parseOnly = false; // parse だけで終わるかどうか

        // 引数をparseする
        var rest = new List<string>();
        int len = 0;
        bool chk_LBraket;
        bool chk_RBraket;
        bool chk_one_Braket;
        bool own_Semicolon;
        bool chk_if_L, chk_if_R, chk_elif_L, chk_elif_R, chk_else_L, chk_else_R;
        string _input = "";
        Console.WriteLine("--------------------Input----------------------------------");
        while (true)
        {
            _input = Console.ReadLine();
            chk_if_L = Regex.IsMatch(_input, @"^(if)(\()[^A-Za-z0-9]*[A-Za-z0-9]+[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*(\))({)$");
            chk_LBraket = Regex.IsMatch(_input, "^(function)[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*({)[^A-Za-z0-9]*");
            chk_one_Braket = Regex.IsMatch(_input, "^(function)[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*[{][^A-Za-z0-9]*([A-Za-z0-9]*|[^A-Za-z0-9]*)*[^A-Za-z0-9]*[}]$");
            own_Semicolon = Regex.IsMatch(_input, "[^A-Za-z0-9]*([A-Za-z0-9]*|[^A-Za-z0-9]*)*[^A-Za-z0-9]*[;][^A-Za-z0-9]*");
            bool test = Regex.IsMatch(_input, @"(\})(elif)(\()[^A-Za-z0-9]*[A-Za-z0-9]+[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*(\))(\{)$");
            if (_input.Equals("")) continue;
            if (_input.Equals("end") == false)
            {
                //if (test) Console.WriteLine("-----------------GGGGGGGGGGGGGGGGGGGGGGEGEGEGE----------------------");
                if (!own_Semicolon && !chk_LBraket && !chk_if_L)
                {
                    _input += ";";
                }
                if(_input.Equals("-h;") || _input.Equals("--help;"))
                {
                    showHelpAndExit();
                }
                else if (_input.Equals("-t;") || _input.Equals("--tokenize;"))
                {
                    tokenizeOnly = true;
                }
                else if (_input.Equals("-p;") || _input.Equals("--parse;"))
                {
                    parseOnly = true;
                }
                else if (_input.Equals("-d;") || _input.Equals("--debug;"))
                {
                    Logger.LogEnabled = true;
                }
                //--------------------------   IF   --------------------------------------------
                if (chk_if_L)
                {
                    while (true)
                    {
                        //Console.WriteLine("-----------------it's IF while----------------------");
                        string if_input = Console.ReadLine();

                        own_Semicolon = Regex.IsMatch(if_input, "(;)$");// chk ";"
                        chk_elif_L = Regex.IsMatch(if_input, @"(\})(elif)(\()[^A-Za-z0-9]*[A-Za-z0-9]+[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*(\))(\{)$"); // chk elif
                        chk_else_L = Regex.IsMatch(if_input, @"(\})(else)(\{)$"); // chk else
                        chk_if_R = Regex.IsMatch(if_input, @"(\})$"); // chk end if

                        if (!own_Semicolon && !chk_if_R && !chk_elif_L && !chk_else_L)
                        {
                            if_input += ";";
                        }
                        _input += if_input;
                        if (chk_else_L)
                        {
                            while (true)
                            {
                                //Console.WriteLine("-----------------it's ELSE while----------------------");
                                string else_input = Console.ReadLine();

                                own_Semicolon = Regex.IsMatch(else_input, "(;)$");// chk ;
                                chk_else_R = Regex.IsMatch(else_input, @"(\})$");

                                if (!own_Semicolon && !chk_else_R)
                                {
                                    else_input += ";";
                                }
                                _input += else_input;
                                if (chk_else_R)
                                {
                                    chk_is_else_break = true;
                                    break;
                                }
                            }
                        }
                        if (chk_elif_L)
                        {
                            while (true)
                            {
                                //Console.WriteLine("-----------------it's ELIF while----------------------");
                                string elif_input = Console.ReadLine();

                                own_Semicolon = Regex.IsMatch(elif_input, "(;)$");// chk ;
                                chk_elif_L = Regex.IsMatch(elif_input, @"(\})(elif)(\()[^A-Za-z0-9]*[A-Za-z0-9]+[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*(\))(\{)$"); // chk elif
                                chk_else_L = Regex.IsMatch(elif_input, @"(\})(else)(\{)$");
                                chk_elif_R = Regex.IsMatch(elif_input, @"(\})$");

                                if (!own_Semicolon && !chk_elif_R && !chk_else_L && !chk_elif_L)
                                {
                                    elif_input += ";";
                                }
                                _input += elif_input;
                                if (chk_else_L)
                                {
                                    while (true)
                                    {
                                        //Console.WriteLine("-----------------it's ELSE while----------------------");
                                        string else_input = Console.ReadLine();

                                        own_Semicolon = Regex.IsMatch(else_input, "(;)$");// chk ;
                                        chk_else_R = Regex.IsMatch(else_input, @"(\})$");

                                        if (!own_Semicolon && !chk_else_R)
                                        {
                                            else_input += ";";
                                        }
                                        _input += else_input;
                                        if (chk_else_R)
                                        {
                                            chk_is_else_break = true;
                                            break;
                                        }
                                    }
                                }
                                if (chk_elif_R|| chk_is_else_break)
                                {
                                    chk_is_elif_break = true;
                                    break;
                                }
                            }
                        }
                        
                        if (chk_if_R || chk_is_else_break|| chk_is_elif_break)
                        {
                            chk_is_elif_break = false;
                            chk_is_else_break = false;
                            break;
                        }
                    }
                }
                //Console.WriteLine("-----------------it's Copy Input----------------------");
                //Console.WriteLine(_input);
                //--------------------------   Function   --------------------------------------------
                if (chk_one_Braket)
                {
                    rest.Insert(len++, _input);
                }
                else if (chk_LBraket)
                {
                    while (true)
                    {
                        string Break_input = Console.ReadLine();
                        own_Semicolon = Regex.IsMatch(Break_input,"(;)$");// chk ;
                        chk_RBraket = Regex.IsMatch(Break_input, @"(\})$");
                        if (!own_Semicolon && !chk_RBraket)
                        {
                            Break_input += ";";
                        }
                        _input += Break_input;

                        if (chk_RBraket)
                        {
                            break;
                        }
                    }
                    rest.Insert(len++, _input);
                }
                else
                {
                    rest.Insert(len++, _input);
                }
            }
            else
            {
                break;
            }
            _input = "";
        }
        Console.WriteLine("------------------------End--------------------------------\n");
        // 引数がないなら、ヘルプを表示して終わる
        //if ( rest.Count <= 0)
        //{
        //    showHelpAndExit();
        //}
        //for(int i = 0; i < function_rest.Count; i++)
        //{
        //    Console.WriteLine("------------------------function_rest " + i.ToString() + "--------------------------------");
        //    Console.WriteLine("function_rest" + i.ToString() + " Val => " + function_rest[i]);
            
        //}
        // 各実行器を用意する
        ITokenizer tokenizer = new SpaceSeparatedTokenizer();
        var parser = new Parser();
        var interpreter = new Interpreter();

        for (int i = 0; i< rest.Count; i++)
        {
            Console.WriteLine("------------------------Token" + i.ToString() + "--------------------------------");
            Console.WriteLine("rest [" + i.ToString() + "]");
            Console.WriteLine(rest[i] + "\n");

            // Tokenize を行う
            var tokens = tokenizer.Tokenize(rest[i].ToString());
            for(int j = 0; j < tokens.Count; j++)
            {
                Console.WriteLine("No."+i.ToString()+"   tokens [" + j.ToString() + "]    " + tokens[j].Type + "\n");
            }
            //if(tokens[0].Type == TokenType.Function) Console.WriteLine("it's function");
            if (tokenizeOnly)
            {
                Console.WriteLine(string.Join(" ", tokens.Select(t => t.Text).ToArray()));
                if(i < rest.Count)
                {
                    continue;
                }
                //exit(0);
            }

            // Parse を行う
            Console.WriteLine("--------------Parser" + i.ToString() + "------------- ");
            MyLang.Ast.Ast ast;
            if(tokens[0].Type == TokenType.Function)
            {
                ast = parser.Function_Parse(tokens);
            }
            else
            {
                ast = parser.Parse(tokens);
            }
            

            if (parseOnly)
            {
                if (i < rest.Count)
                {
                    continue;
                }
                //exit(0);
            }

            // Interpreter で実行する
            Console.WriteLine("--------------Interpreter" + i.ToString() + "------------- ");
            interpreter.Run(ast);
            
        }
        exit(0);
    }
    /// <summary>
    /// ヘルプを表示して終わる
    /// </summary>
    static void showHelpAndExit()
    {
        Console.WriteLine(@"
My Small Language.

Usage:
    > MyLang.exe [options...] ""program""

Options:
    -t, --tokenize : Show token list.
    -p, --parse    : Show parsed abstract syntax tree.
    -d, --debug    : Print debug log (for debug).
    -h, --help     : Show help.

Example:
    > MyLang.exe ""1 + 2""
    > MyLang.exe --debug ""1 + 2 * 3""
    > MyLang.exe --tokenize ""1 + 2 * 3""
    > MyLang.exe --parse ""1 + 2 * 3""
");
        exit(0);
    }

    /// <summary>
    /// デバッガがアタッチされている場合は、キーの入力を待つ
    ///
    /// Visual Studioの開津環境で、コンソールがすぐに閉じてしまうのの対策として使用している
    /// </summary>
    static void waitKey()
    {
        if (Debugger.IsAttached)
        {
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 終了する
    /// </summary>
    /// <param name="resultCode"></param>
    static void exit(int resultCode) 
    {
        waitKey();
        Environment.Exit(resultCode);
    }

}

