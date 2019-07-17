using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using MyLang;

class Program
{
    /// <summary>
    /// コマンド の entry point
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    static void Main(string[] args)
    {
        bool tokenizeOnly = false; // tokenize だけで終わるかどうか
        bool parseOnly = false; // parse だけで終わるかどうか
        string src = null; // 直接実行するソース

        // 引数をparseする
        var rest = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "-h":
                case "--help":
                    showHelpAndExit();
                    break;
                case "-t":
                case "--tokenize":
                    tokenizeOnly = true;
                    break;
                case "-p":
                case "--parse":
                    parseOnly = true;
                    break;
                case "-d":
                case "--debug":
                    Logger.LogEnabled = true;
                    break;
                case "-e":
                    src = args[i + 1];
                    i++;
                    break;
                default:
                    rest.Add(arg);
                    break;
            }
        }

        // 各実行器を用意する
        ITokenizer tokenizer = new SimpleTokenizer();
        var parser = new Parser();
        var interpreter = new Interpreter();

        // ソースファイルを読み込む
        if( src == null)
        {
            // 引数がないなら、ヘルプを表示して終わる
            if (rest.Count != 1)
            {
                showHelpAndExit();
            }

            src = File.ReadAllText(rest[0]);
        }

        // Tokenize を行う
        var tokens = tokenizer.Tokenize(src);

        if( tokenizeOnly)
        {
            Console.WriteLine(string.Join(" ", tokens.Select(t => t.Text).ToArray()));
            exit(0);
        }

        // Parse を行う
        var ast = parser.Parse(tokens);

        if( parseOnly)
        {
            Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, true));
            exit(0);
        }

        // Interpreter で実行する
        interpreter.Run(ast);

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
    > MyLang.exe [options...] <source-file>

Options:
    -t, --tokenize : Show token list.
    -p, --parse    : Show parsed abstract syntax tree.
    -d, --debug    : Print debug log (for debug).
    -h, --help     : Show help.
    -e ""source""  : Run specified source text.

Example:
    > MyLang.exe program.mylang
    > MyLang.exe -e ""1 + 2""
    > MyLang.exe --debug -e ""1 + 2 * 3""
    > MyLang.exe --tokenize -e ""1 + 2 * 3""
    > MyLang.exe --parse -e ""1 + 2 * 3""
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

