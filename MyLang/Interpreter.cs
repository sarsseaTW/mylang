using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    public class Interpreter
    {
        public Interpreter()
        {
        }
        public Dictionary<string, float> local_symbol_dict = new Dictionary<string, float>();
        public string local_symbol_str = "";
        public Dictionary<string, float> global_symbol_dict = new Dictionary<string, float>();
        public string global_symbol_str = "";
        public Dictionary<string, float> function_symbol_dict = new Dictionary<string, float>();
        public string function_symbol_str = "";
        public Dictionary<string, Ast.Ast> function_body_dict = new Dictionary<string, Ast.Ast>();
        public string function_body_str = "";

        bool isFunction = false;

        public void Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装
            //Console.WriteLine(new AstDisplayer().BuildString(ast, false));
            if (ast is PrintStatement P)
            {
                if(P.Exp is VarFunctionStatement VF)
                {
                    // var_function_symbol_str = VF.Name.Value;
                    float found;
                    if (function_symbol_dict.TryGetValue(VF.Name.Value, out found))
                    {
                        //function_symbol_str = var_function_symbol_str;
                        function_symbol_str = VF.Name.Value;
                        isFunction = true;
                        //var_function_symbol_dict[var_function_symbol_str] = VF.Args;
                        for (int i = 0; i < VF.Args.Length; i++)
                        {
                            local_symbol_dict["@" + i.ToString()] = Run_exp(VF.Args[i]);
                            Console.WriteLine("@"+i.ToString()+"----->" + local_symbol_dict["@" + i.ToString()]);
                        }
                        Run(function_body_dict[function_symbol_str]);
                        local_symbol_dict.Clear();
                    }
                    else
                    {
                        Run(function_body_dict[VF.Name.Value]);
                        local_symbol_dict.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("Run_exp(P.Exp) =>" + Run_exp(P.Exp));
                }
            }
            if (ast is ReturnStatement R)
            {
                if (isFunction)
                {
                    function_symbol_dict[function_symbol_str] = Run_exp(R.Exp);
                    Console.WriteLine("Run_exp(R.Exp) =>" + Run_exp(R.Exp));
                }
            }
            if (ast is LetStatement L)
            {
                if (isFunction)
                {
                    local_symbol_str = L.Variable.Value;
                    Console.WriteLine("Run_exp(L.Exp) =>" + Run_exp(L.Exp));
                    local_symbol_str = "";
                }
                else
                {
                    global_symbol_str = L.Variable.Value;
                    Console.WriteLine("Run_exp(L.Exp) =>" + Run_exp(L.Exp));
                    global_symbol_str = "";
                }
            }
            if(ast is Ast.Program pg)
            {
                isFunction = true;

                Statement[] f_st = pg.Statements;
                FunctionStatement f_bd = f_st[0] as FunctionStatement;
                function_body_dict[f_bd.Name.Value] = pg;

                function_statement(pg.Statements);
            }
        }
        void function_statement(Statement[] f_st)
        {
            function_Body(f_st[0] as FunctionStatement);
        }
        void function_Body(FunctionStatement f_bd)
        {
            function_symbol_str = f_bd.Name.Value;
            for(int i = 0;i< f_bd.Body.Length;i++)
            {
                Run(f_bd.Body[i]);
            }
            isFunction = false;
        }
        float Run_exp(Exp exp)
        {
            if (exp is BinOp)//  [ '+' , '-' , '*' , '/' , '=']
            {
                var _binOp = exp as BinOp;

                float exp_LHS = Run_exp(_binOp.Lhs);
                float exp_RHS = Run_exp(_binOp.Rhs);
                float sum = 0;
                switch (_binOp.Operator)
                {
                    case BinOpType.Add:
                        sum = exp_LHS + exp_RHS;
                        break;
                    case BinOpType.Sub:
                        sum = exp_LHS - exp_RHS;
                        break;
                    case BinOpType.Multiply:
                        sum = exp_LHS * exp_RHS;
                        break;
                    case BinOpType.Divide:
                        sum = exp_LHS / exp_RHS;
                        break;
                    case BinOpType.Equal:
                        sum = exp_RHS;
                        break;
                }
                if (isFunction)
                {
                    local_symbol_dict[local_symbol_str] = sum;
                    return local_symbol_dict[local_symbol_str];
                }
                else
                {
                    global_symbol_dict[global_symbol_str] = sum;
                    return global_symbol_dict[global_symbol_str];
                }
            }
            else if(exp is Number)
            {
                var num = exp as Number;
                return num.Value;
            }
            else if (exp is Symbol)
            {
                var num = exp as Symbol;
                float found;
                if (isFunction)
                {
                    if (local_symbol_dict.TryGetValue(num.Value, out found))
                    {
                        return found;
                    }
                    else
                    {
                        local_symbol_dict[local_symbol_str] = 0;
                        return local_symbol_dict[local_symbol_str];
                    }
                }
                else
                {
                    if (function_symbol_dict.TryGetValue(num.Value, out found))
                    {
                        return found;
                    }
                    else
                    {
                        if (global_symbol_dict.TryGetValue(num.Value, out found))
                        {
                            return found;
                        }
                        else
                        {
                            global_symbol_str = num.Value;
                            global_symbol_dict[global_symbol_str] = 0;
                            return global_symbol_dict[global_symbol_str];
                        }
                    }
                }
            }
            return 0;
        }
    }
}
