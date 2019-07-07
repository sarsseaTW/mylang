using System;
using System.Diagnostics;

class Program
{
    /// <summary>
    /// デバッガがアタッチされている場合は、キーの入力を待つ
    ///
    /// Windowsの開津環境で、コンソールがすぐに閉じてしまうのの対策として使用している
    /// </summary>
    static void waitKey()
    {
        if (Debugger.IsAttached)
        {
            Console.ReadKey();
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        waitKey();
    }
}

