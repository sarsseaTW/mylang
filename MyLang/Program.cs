using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MyLang;

class Program
{
    /// <summary>
    /// コマンド の entry point
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    static void Main()
    {
        bool tokenizeOnly = false; // tokenize だけで終わるかどうか
        bool parseOnly = false; // parse だけで終わるかどうか

        // 引数をparseする
        var rest = new List<string>();
        int len = 0;
        Console.WriteLine("--------------------Input----------------------------------");
        while (true)
        {
            string _input = Console.ReadLine();
            if (_input.Equals("end") == false)
            {
                if(_input.Equals("-h") || _input.Equals("--help"))
                {
                    showHelpAndExit();
                }
                else if (_input.Equals("-t") || _input.Equals("--tokenize"))
                {
                    tokenizeOnly = true;
                }
                else if (_input.Equals("-p") || _input.Equals("--parse"))
                {
                    parseOnly = true;
                }
                else if (_input.Equals("-d") || _input.Equals("--debug"))
                {
                    Logger.LogEnabled = true;
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
        }
        Console.WriteLine("------------------------End--------------------------------");
        // 引数がないなら、ヘルプを表示して終わる
        if ( rest.Count <= 0)
        {
            showHelpAndExit();
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

