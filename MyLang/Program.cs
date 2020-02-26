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
        string _input = "";

        //string str_test = "function b{let b = 3  let hk = 5; let gk = 3  ;}";

        //int functionName_L = str_test.IndexOf(" ");
        //int functionName_R = str_test.LastIndexOf("{");
        //int block_L = str_test.IndexOf("{");
        //int block_R = str_test.LastIndexOf("}");
        //Console.WriteLine(str_test.Substring(functionName_L + 1, functionName_R - functionName_L - 1));
        //Console.WriteLine(str_test.Substring(block_L + 1, block_R - block_L - 1));
        
        Console.WriteLine("--------------------Input----------------------------------");
        while (true)
        {
            _input = Console.ReadLine();
            chk_LBraket = Regex.IsMatch(_input, "^(function)[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*({)[^A-Za-z0-9]*");
            chk_one_Braket = Regex.IsMatch(_input, "^(function)[^A-Za-z0-9]+[A-Za-z0-9]+[^A-Za-z0-9]*[{][^A-Za-z0-9]*([A-Za-z0-9]*|[^A-Za-z0-9]*)*[^A-Za-z0-9]*[}]$");
            own_Semicolon = Regex.IsMatch(_input, "[^A-Za-z0-9]*([A-Za-z0-9]*|[^A-Za-z0-9]*)*[^A-Za-z0-9]*[;][^A-Za-z0-9]*");
            bool test = Regex.IsMatch(_input, @"(\})$");
            if (_input.Equals("")) continue;
            if (_input.Equals("end") == false)
            {
                //if(test) Console.WriteLine("-----------------GGGGGGGGGGGGGGGGGGGGGGEGEGEGE----------------------");
                if (!own_Semicolon && !chk_LBraket)
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
                if(chk_one_Braket)
                {
                    getFunction(_input);
                    //rest.Insert(len++, _input);
                }
                else if (chk_LBraket)
                {
                    while (true)
                    {
                        string Break_input = Console.ReadLine();
                        own_Semicolon = Regex.IsMatch(Break_input,"(;)$");
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
                    getFunction(_input);
                    //rest.Insert(len++, _input);
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
        for(int i = 0; i < function_rest.Count; i++)
        {
            Console.WriteLine("------------------------function_rest " + i.ToString() + "--------------------------------");
            Console.WriteLine("function_rest" + i.ToString() + " Val => " + function_rest[i]);
            
        }
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
            var ast = parser.Parse(tokens);

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
            var result = interpreter.Run(ast);

            // 答えを出力する
            Console.WriteLine("-----答えを出力する" + i.ToString() + "--------- ");
            Console.WriteLine(result);
        }
        exit(0);
    }
    static void getFunction(string _input)
    {
        int functionName_L = _input.IndexOf(" ");
        int functionName_R = _input.LastIndexOf("{");
        int block_L = _input.IndexOf("{");
        int block_R = _input.LastIndexOf("}");
        string function_name = _input.Substring(functionName_L + 1, functionName_R - functionName_L - 1);
        string function_val = _input.Substring(block_L + 1, block_R - block_L - 1);

        Console.WriteLine("-----getFunction Str----------------------------- ");
        Console.WriteLine("-----Function Number => " + function_key_number);
        Console.WriteLine("-----Function Name => " + function_name);
        Console.WriteLine("-----Function Val => " + function_val);
        Console.WriteLine("-----getFunction End----------------------------- \n");

        function_key[function_name] = function_val;
        function_number[function_key_number] = function_name;
        function_rest.Insert(function_key_number, function_val);
        function_key_number++;
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

