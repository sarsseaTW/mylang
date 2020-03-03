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
        #region Var
        public Dictionary<string, float> local_symbol_dict = new Dictionary<string, float>();
        public string local_symbol_str = "";
        public Dictionary<string, float> global_symbol_dict = new Dictionary<string, float>();
        public string global_symbol_str = "";
        public Dictionary<string, float> function_symbol_dict = new Dictionary<string, float>();
        public string function_symbol_str = "";
        public Dictionary<string, Ast.Ast> function_body_dict = new Dictionary<string, Ast.Ast>();
        public string function_body_str = "";

        public string index_str;

        bool isFunction = false;
        bool isIF_body = false;
        bool isElse = false;
        bool isElif = false;
        bool isElif_body = false;
        bool binop = false;
        bool isWhile = false;
        bool isFor = false;
        #endregion
        #region Run
        public void Run(Ast.Ast ast)
        {
            if(ast.GetType().ToString() == "MyLang.Ast.BinOp" || ast.GetType().ToString() == "MyLang.Ast.Number" 
                || ast.GetType().ToString() == "MyLang.Ast.Symbol")
            {
                binop = true;
                Console.WriteLine("Ans => " + Run_exp(ast as Exp).ToString()); 
                binop = false;
            }
            if (ast is Ast.OtherStatement other)
            {
                for (int i = 0; i < other.Body.Length; i++)
                {
                    Run(other.Body[i]);
                    if (i + 1 != other.Body.Length)
                    {
                        if (other.Body[i + 1].GetType().ToString() == "MyLang.Ast.ELIFStatement")
                        {
                            isElif = true;
                            isElse = false;
                        }
                        else
                        {
                            isElif = false;
                        }
                    }
                }
            }
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
                            //Console.WriteLine("@"+i.ToString()+"----->" + local_symbol_dict["@" + i.ToString()]);
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
                    Console.WriteLine("Run_exp(P.Exp) => " + Run_exp(P.Exp));
                }
            }
            if (ast is ReturnStatement R)
            {
                if (isFunction)
                {
                    function_symbol_dict[function_symbol_str] = Run_exp(R.Exp);
                    Console.WriteLine("Run_exp(R.Exp) => " + function_symbol_dict[function_symbol_str]);
                }
            }
            if (ast is LetStatement L)
            {
                if (isFunction || isIF_body || isElse || isElif_body ||isWhile || isFor)
                {
                    local_symbol_str = L.Variable.Value;
                    local_symbol_dict[local_symbol_str] = Run_exp(L.Exp);
                    Console.WriteLine("Run_exp(L.Exp) => " + local_symbol_dict[local_symbol_str]);
                    local_symbol_str = "";
                }
                else
                {
                    global_symbol_str = L.Variable.Value;
                    global_symbol_dict[global_symbol_str] = Run_exp(L.Exp);
                    Console.WriteLine("Run_exp(L.Exp) => " + global_symbol_dict[global_symbol_str]);
                    global_symbol_str = "";
                }
            }
            if(ast is ForStatement Fora)
            {
                isFor = true;
                Run(Fora.let_val);
                while(Run_exp(Fora.select) != 0)
                {
                    for(int i = 0; i < Fora.Body.Length; i++)
                    {
                        Run(Fora.Body[i]);
                    }
                    Run(Fora.let_val_op);
                }
                isFor = false;
            }
            if(ast is WhileStatement W)
            {
                var TF = Run_exp(W.SelectBody);
                while(TF != 0)
                {
                    isWhile = true;
                    for(int i = 0; i < W.Body.Length; i++)
                    {
                        Run(W.Body[i]);
                    }
                    isWhile = false;
                    local_symbol_dict.Clear();
                    TF = Run_exp(W.SelectBody);
                }
            }
            if(ast is FunctionStatement afs)
            {
                isFunction = true;
                function_symbol_str = afs.Name.Value;
                function_body_dict[afs.Name.Value] = afs;
                for (int i = 0; i < afs.Body.Length; i++)
                {
                    Run(afs.Body[i]);
                }
                isFunction = false;
            }
            if(ast is IFStatement ifs)
            {
                IF_Body(ifs);
            }
            else if(ast is ELIFStatement elifs)
            {
                if(isElif) ELIF_Body(elifs);
            }
            else if(ast is ELSEStatement elses)
            {
                if(isElse) ELSE_Body(elses);
            }
        }
        void ELIF_Body(Statement ELIFB)
        {
            var elif_ifst = ELIFB as ELIFStatement;
            var TF = Run_exp(elif_ifst.SelectBody);
            if (TF != 0)
            {
                isElse = false;
                isElif_body = true;
                for (int i = 0; i < elif_ifst.Body.Length; i++)
                {
                    Run(elif_ifst.Body[i]);
                }
                isElif_body = false;
                local_symbol_dict.Clear();
            }
            else isElse = true;

            isElif = false;
        }
        void ELSE_Body(Statement ELSEB)
        {
            var else_ifst = ELSEB as ELSEStatement;
            for (int i = 0; i < else_ifst.Body.Length; i++)
            {
                Run(else_ifst.Body[i]);
            }
            local_symbol_dict.Clear();
            isElse = false;
        }
        void IF_Body(Statement IFB)
        {
            var if_ifst = IFB as IFStatement;
            var TF = Run_exp(if_ifst.SelectBody);
            if (TF != 0)
            {
                isIF_body = true;
                for (int i = 0; i < if_ifst.Body.Length; i++)
                {
                    Run(if_ifst.Body[i]);
                }
                isIF_body = false;
                local_symbol_dict.Clear();
            }
            else
            {
                isElse = true;
                isElif = true;
            }
        }
        #endregion
        #region Exp
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
                    case BinOpType.Less:
                        if(exp_LHS < exp_RHS)return 1;
                        else return 0;
                    case BinOpType.More:
                        if (exp_LHS > exp_RHS) return 1;
                        else return 0;
                    case BinOpType.LessEqual:
                        if (exp_LHS <= exp_RHS) return 1;
                        else return 0;
                    case BinOpType.MoreEqual:
                        if (exp_LHS >= exp_RHS) return 1;
                        else return 0;
                    case BinOpType.DoubleEqual:
                        if (exp_LHS == exp_RHS) return 1;
                        else return 0;
                }
                if (binop)
                {
                    return sum;
                }
                else
                {
                    if (isFunction || isIF_body || isElif_body || isElse || isWhile || isFor)
                    {
                        float found;
                        if (global_symbol_dict.TryGetValue(index_str, out found))
                        {
                            global_symbol_dict[index_str] = sum;
                            return global_symbol_dict[index_str];
                        }
                        else
                        {
                            local_symbol_dict[local_symbol_str] = sum;
                            return local_symbol_dict[local_symbol_str];
                        }
                    }
                    else
                    {
                        global_symbol_dict[global_symbol_str] = sum;
                        return global_symbol_dict[global_symbol_str];
                    }
                }
            }
            else if(exp is Number)
            {
                var num = exp as Number;
                return num.Value;
            }
            else if (exp is Symbol)
            {
                var num2 = exp as Symbol;
                index_str = num2.Value;
                float found;
                if (isFunction||isIF_body || isElif_body || isElse || isWhile || isFor)
                {
                    if (global_symbol_dict.TryGetValue(num2.Value, out found))
                    {
                        return found;
                    }
                    else
                    {
                        if (local_symbol_dict.TryGetValue(num2.Value, out found))
                        {
                            return found;
                        }
                        else
                        {
                            local_symbol_dict[local_symbol_str] = 0;
                            return local_symbol_dict[local_symbol_str];
                        }
                    }
                }
                else
                {
                    if (function_symbol_dict.TryGetValue(num2.Value, out found))
                    {
                        return found;
                    }
                    else
                    {
                        if (global_symbol_dict.TryGetValue(num2.Value, out found))
                        {
                            return found;
                        }
                        else
                        {
                            global_symbol_str = num2.Value;
                            global_symbol_dict[global_symbol_str] = 0;
                            return global_symbol_dict[global_symbol_str];
                        }
                    }
                }
            }
            return 0;
        }
        #endregion
    }
}
