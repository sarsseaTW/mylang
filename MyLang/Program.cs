using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

using MyLang;

class Program
{
    #region GVar
    /// <summary>
    /// コマンド の entry point
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    public static Dictionary<string, string> function_key = new Dictionary<string, string>();
    public static Dictionary<int, string> function_number = new Dictionary<int, string>();
    public static int function_key_number = 0;
    public static List<string> function_rest = new List<string>();
    //public static bool chk_is_else_break, chk_is_elif_break;
    

    #endregion
    static void Main(string[] args)
    {
        //------------------------------------------------------------------------------------------
        #region LVar
        bool tokenizeOnly = false; // tokenize だけで終わるかどうか
        bool parseOnly = false; // parse だけで終わるかどうか
        // 引数をparseする
        var rest = new List<string>();
        int len = 0;
        string _input = "";
        bool while_key = false;

        // 各実行器を用意する
        ITokenizer tokenizer = new SpaceSeparatedTokenizer();
        var parser = new Parser();
        var interpreter = new Interpreter();

        #endregion
        if (args.Length != 0) while_key = false;
        else while_key = true;
        //------------------------------------------------------------------------------------------
        #region Input
        reIn:if (!while_key)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-t" || args[i] == "--tokenize")
                {
                    tokenizeOnly = true;
                    continue;
                }
                else if (args[i] == "-p" || args[i] == "--parse")
                {
                    parseOnly = true;
                    continue;
                }
                if (i != 0)
                {
                    _input += " ";
                }
                _input += args[i];

            }

            if (_input.Equals("-h;") || _input.Equals("--help;"))
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
            rest.Insert(len++, _input);
        }
        #endregion        
        //------------------------------------------------------------------------------------------
        #region Run
        else
        {
            StreamReader IF_file = new StreamReader(@"C:\Users\hungjih\Documents\mylang\MyLang\IF.txt");
            rest.Clear();
            _input = Console.ReadLine();
            bool endIsSem = Regex.IsMatch(_input, "(;)$");
            if (_input == "end") exit(0);
            if (_input == "cls")
            {
                Console.Clear();
                goto reIn;
            }
            if (!endIsSem) _input += _input;
            if(_input == "mylang if.txt") _input = IF_file.ReadToEnd();
            rest.Insert(len, _input);
        }
        for (int i = 0; i< rest.Count; i++)
        {
            // Tokenize を行う
            var tokens = tokenizer.Tokenize(rest[i].ToString());
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
            MyLang.Ast.Ast ast;
            if (tokens[0].Type == TokenType.Number || tokens[0].Type == TokenType.Symbol)
            {
                ast = parser.NS_Parse(tokens);
            }
            else
            {
                ast = parser.Parse(tokens);
            }
            if (parseOnly)
            {
                Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, false));
                if (i < rest.Count)
                {
                    continue;
                }
                //exit(0);
            }
            // Interpreter で実行する
            interpreter.Run(ast);
            
        }
        if (while_key) goto reIn;
        exit(0);
        #endregion
    }
    //------------------------------------------------------------------------------------------
    #region Utilities
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
    #endregion
}

