using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MyLang;

class Program
{
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
    /// エントリポイント
    /// 
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    static void Main(string[] args)
    {
        bool tokenizeOnly = false;
        bool parseOnly = false;
        bool trace = false;

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
                default:
                    rest.Add(arg);
                    break;
            }
        }


        if( rest.Count <= 0)
        {
            showHelpAndExit();
        }

        // 各実行器を用意する
        ITokenizer tokenizer = new SpaceSeparatedTokenizer();
        var parser = new Parser();
        var interpreter = new Interpreter();



        // Tokenize を行う
        var tokens = tokenizer.Tokenize(string.Join(" ", rest.ToArray()));

        if( tokenizeOnly)
        {
            Console.WriteLine(string.Join(" ", tokens.Select(t => t.Text).ToArray()));
            waitKey();
            return;
        }

        var ast = parser.Parse(tokens);

        if( parseOnly)
        {
            Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, false));
            waitKey();
            return;
        }

        var result = interpreter.Run(ast);

        Console.WriteLine(result);

        waitKey();
    }

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
        waitKey();
        Environment.Exit(0);
    }

}

