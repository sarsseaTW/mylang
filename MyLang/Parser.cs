using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    public class Parser
    {
        #region Var
        IList<Token> tokens_;
        public static int pos_ = 0;
        bool is_function = false;
        static Dictionary<TokenType, Ast.BinOpType> BinOpMap = new Dictionary<TokenType, Ast.BinOpType>
        {
            {TokenType.Plus, Ast.BinOpType.Add }, // '+'
            {TokenType.Minus, Ast.BinOpType.Sub }, // '-'
            {TokenType.Star, Ast.BinOpType.Multiply }, // '*'
            {TokenType.Slash, Ast.BinOpType.Divide },  // '/'
            {TokenType.Equal, Ast.BinOpType.Equal },  // '='
            {TokenType.Let, Ast.BinOpType.Let },  // 'Let'
            {TokenType.Semicolon, Ast.BinOpType.Semicolon },  // ';'
            {TokenType.LBraket, Ast.BinOpType.LBraket },  // '{'
            {TokenType.RBraket, Ast.BinOpType.RBraket },  // '}'
            {TokenType.Less, Ast.BinOpType.Less },  // '<'
            {TokenType.More, Ast.BinOpType.More },  // '>'
            {TokenType.LessEqual, Ast.BinOpType.LessEqual },  // '<='
            {TokenType.MoreEqual, Ast.BinOpType.MoreEqual },  // '>='
            {TokenType.DoubleEqual, Ast.BinOpType.DoubleEqual },  // '=='
        };
        private int _pos;
        #endregion
        //------------------------------------------------------------------------------------------
        public Parser()
        {
        }
        //------------------------------------------------------------------------------------------
        #region Utilities
        /// <summary>
        /// 現在のトークンを取得する
        /// </summary>
        /// <returns></returns>
        Token currentToken()
        {
            return tokens_[pos_];
        }
        Token next_currentToken()
        {
            if(tokens_.Count-1 != pos_)
            {
                return tokens_[pos_ + 1];
            }
            else
            {
                return tokens_[pos_];
            }
            
        }
        /// <summary>
        /// 次のトークンに進む
        /// </summary>
        void progress()
        {
            Logger.Trace($"progress {currentToken().Text}");
            pos_++;
        }
        /// <summary>
        /// 現在のトークンを取得する と 次のトークンに進む
        /// </summary>
        Token runToken(TokenType _token)
        {
            var tokenPos = currentToken();
            progress();
            return tokenPos;
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Number and Symbol Parse
        public Ast.Ast NS_Parse(IList<Token> tokens) //Number and Symbol Parse
        {
            tokens_ = tokens;
            pos_ = 0;
           // Console.WriteLine("Ast.Ast NS_Parse \n");
            //Console.WriteLine(string.Join(" ", tokens_.Select(t => t.Text).ToArray()) + "\n");
            return exp();
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region OtherParse
        public Ast.Ast Parse(IList<Token> tokens) //other
        {
            tokens_ = tokens;
            pos_ = 0;
            //Console.WriteLine("Ast.Ast Parse \n");
            //Console.WriteLine(string.Join(" ", tokens_.Select(t => t.Text).ToArray()) + "\n");
            return aa();
        }
        Ast.Program Other_program()
        {
            var statement_group = new List<Ast.Statement>();
            while (!currentToken().IsSemicolon)
            {
                statement_group.Add(aa());
            }
            return new Ast.Program(statement_group);
        }
        Ast.Statement aa()
        {
            var block_val = Other_Block();// block
            return new Ast.OtherStatement(block_val);
        }
        Ast.Statement[] Other_Block()
        {
            var statement_group = new List<Ast.Statement>();
            while (true)
            {
                var stat = statement();
                if (stat == null)
                {
                    break;
                }
                statement_group.Add(stat);
                if(pos_ != tokens_.Count - 1)
                {
                    progress();
                }
            }
            return statement_group.ToArray();
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region IF Parse 
        public Ast.Ast IF_Parse(IList<Token> tokens) // IF
        {
            tokens_ = tokens;
            pos_ = 0;
            //Console.WriteLine("Ast.Ast IF_Parse \n");
            //Console.WriteLine(string.Join(" ", tokens_.Select(t => t.Text).ToArray()) + "\n");
            return IF_statement_program();
        }
        Ast.IFProgram IF_statement_program() //這裡判斷結束條件 if elif else都要考慮
        {
            var IF_statement_group = new List<Ast.Statement>();
            var ELSE_statement_group = new List<Ast.Statement>();
            while (!currentToken().IsRBraket && !currentToken().IsTerminate)
            {
                IF_statement_group.Add(statement());
            }
            while(next_currentToken().Type == TokenType.ELIF)
            {
                if (currentToken().IsRBraket && currentToken().IsTerminate || currentToken().Type == TokenType.ELSE)
                {
                    break;
                }
                runToken(TokenType.RBraket);
                while (!currentToken().IsRBraket && !currentToken().IsTerminate)
                {
                    IF_statement_group.Add(statement());
                    //progress();
                }
            }
            if (next_currentToken().Type == TokenType.ELSE)
            {
                runToken(TokenType.RBraket);
                while (!currentToken().IsRBraket && !currentToken().IsTerminate)
                {
                    IF_statement_group.Add(statement());
                    //progress();
                }
            }
            return new Ast.IFProgram(IF_statement_group);
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Function Parse
        public Ast.Ast Function_Parse(IList<Token> tokens) // function
        {
            tokens_ = tokens;
            pos_ = 0;
            //Console.WriteLine("Ast.Ast Function_Parse ");
            //Console.WriteLine(string.Join(" ", tokens_.Select(t => t.Text).ToArray()) + "\n");
            return statement_program();
        }
        Ast.Program statement_program()
        {
            var statement_group = new List<Ast.Statement>();
            while (!currentToken().IsRBraket)
            {
                statement_group.Add(statement());
            }
            return new Ast.Program(statement_group);
        }
        Ast.Statement[] Block()
        {
            var statement_group = new List<Ast.Statement>();
            while (true)
            {
                var stat = statement();
                if (stat == null)
                {
                    break;
                }
                statement_group.Add(stat);
                progress();
            }

            return statement_group.ToArray();
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region statement
        Ast.Statement statement()
        {
            var tokenPos = currentToken();
            if (tokenPos.IsLet)
            {
                return LetStatement();
            }
            else if (tokenPos.IsPrint)
            {
                return PrintStatement();
            }
            else if (tokenPos.IsFunction)
            {
                return FunctionStatement();
            }
            else if (tokenPos.IsReturn)
            {
                return ReturnStatement();
            }
            else if (tokenPos.Type == TokenType.IF)
            {
                return IFStatement();
            }
            else if (tokenPos.Type == TokenType.ELIF)
            {
                return ELIFStatement();
            }
            else if (tokenPos.Type == TokenType.ELSE)
            {
                return ELSEStatement();
            }
            else
            {
                return null;
            }
        }
        Ast.Statement LetStatement()// let a = 3;
        {
            runToken(TokenType.Let);//return let ,pos++                    
            var tokenPos = currentToken();//return a
            var symbol = new Ast.Symbol(tokenPos.Text);//symbol = a
            progress();
            runToken(TokenType.Equal);//return =, pos++   
            var exp_val = exp();// 3
            return new Ast.LetStatement(symbol, exp_val);
        }
        Ast.Statement PrintStatement()// print a ; 
        {
            runToken(TokenType.Print);//return print, pos++
            var exp_val = exp();// a
            return new Ast.PrintStatement(exp_val);
        }
        Ast.Statement ReturnStatement()// Return a ; 
        {
            runToken(TokenType.Return);//return Return, pos++
            var exp_val = exp();// a
            return new Ast.ReturnStatement(exp_val);
        }
        Ast.Statement ELIFStatement()// if( a < 1 ){let a = 0;}elif(a > 2){let a = 0;}elif(a <= 999){let a = 0;}elif(a >= 889){let a = 0;}elif(a == 889){let a = 0;}else{let a = 0;}
        {
            runToken(TokenType.IF);//return if ,pos++      
            runToken(TokenType.LParenthesis);//return ( ,pos++  
            var select = exp(); // return true or false
            runToken(TokenType.RParenthesis);//return ), pos++   
            runToken(TokenType.LBraket);//return ), pos++  
            var block_val = Block();// block
            return new Ast.ELIFStatement(select, block_val);
        }
        Ast.Statement ELSEStatement()
        {
            runToken(TokenType.ELSE);
            runToken(TokenType.LBraket);
            var block_val = Block();// block
            return new Ast.ELSEStatement(block_val);
        }
        Ast.Statement IFStatement()// if( a < 1 ){let a = 0;}elif(a > 2){let a = 0;}elif(a <= 999){let a = 0;}elif(a >= 889){let a = 0;}elif(a == 889){let a = 0;}else{let a = 0;}
        {
            runToken(TokenType.IF);//return if ,pos++      
            runToken(TokenType.LParenthesis);//return ( ,pos++  
            var select = exp(); // return true or false
            runToken(TokenType.RParenthesis);//return ), pos++   
            runToken(TokenType.LBraket);//return ), pos++  
            var block_val = Block();// block
            return new Ast.IFStatement(select,block_val);
        }
        Ast.Statement FunctionStatement()// function v{
        {
            runToken(TokenType.Function);//return function ,pos++                    
            var tokenPos = currentToken();//return v
            var symbol = new Ast.Symbol(tokenPos.Text);//symbol = v
            progress();
            runToken(TokenType.LBraket);//return {, pos++   
            var block_val = Block();// block
            return new Ast.FunctionStatement(symbol,block_val);
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region exp

        Ast.Exp exp()
        {
            return exp1();
        }
        Ast.Exp exp1()
        {
            var left_hs = exp2();
            if (left_hs == null)
            {
                return null;
            }
            return exp1_realArea(left_hs);
        }
        Ast.Exp exp1_realArea(Ast.Exp left_hs)
        {
            var tokenPos = currentToken();
            if (tokenPos.Type == TokenType.Less || tokenPos.Type == TokenType.More || 
                tokenPos.Type == TokenType.LessEqual || tokenPos.Type == TokenType.MoreEqual 
                || tokenPos.Type == TokenType.DoubleEqual)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                //Console.WriteLine(now_tokenBioMap.ToString() + "        exp1_realArea ");
                progress();
                var right_hs = exp2();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 ex: a>1
                return exp1_realArea(exp);
            }
            else
            {
                return left_hs;
            }
        }
        Ast.Exp exp2()
        {
            var left_hs = exp3();
            if (left_hs == null)
            {
                return null;
            }
            return exp2_realArea(left_hs);
        }
        Ast.Exp exp2_realArea(Ast.Exp left_hs)
        {
            var tokenPos = currentToken();
            if (tokenPos.Type == TokenType.Plus || tokenPos.Type == TokenType.Minus)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                //Console.WriteLine(now_tokenBioMap.ToString() + "        exp2_realArea ");
                progress();
                var right_hs = exp3();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 ex: 1+4+8-82+5
                return exp2_realArea(exp);
            }
            else
            {
                return left_hs;
            }
        }
        Ast.Exp exp3()
        {
            var left_hs = expVal();
            if (left_hs == null)
            {
                return null;
            }
            return exp3_realArea(left_hs);
        }
        Ast.Exp exp3_realArea(Ast.Exp left_hs)
        {
            var tokenPos = currentToken();
            if (tokenPos.Type == TokenType.Star || tokenPos.Type == TokenType.Slash)
            {
                var now_tokenBioMap = BinOpMap[tokenPos.Type]; //儲存算術邏輯
                //Console.WriteLine(now_tokenBioMap.ToString() + "        exp3_realArea ");
                progress();
                var right_hs = expVal();
                var exp = new Ast.BinOp(now_tokenBioMap, left_hs, right_hs);// 儲存式子 ex: 1*4 *5/d
                return exp3_realArea(exp);
            }
            else
            {
                return left_hs;
            }
        }
        Ast.Exp expVal()
        {
            var tokenPos = currentToken();
            if (tokenPos.IsNumber)
            {
                progress();

                //Console.WriteLine("\n" + tokenPos.Text + "            IsNumber tokenPos.ToString\n");

                return new Ast.Number(float.Parse(tokenPos.Text));
            }
            else if (tokenPos.IsSymbol)
            {
                var name = new Ast.Symbol(tokenPos.Text);
                if (next_currentToken().Type != TokenType.LParenthesis)
                {
                    progress();
                    return name;
                }
                else
                {
                    progress();
                    runToken(TokenType.LParenthesis);
                    var add = new List<Ast.Exp>();
                    while (true)
                    {
                        add.Add(exp());
                        if (currentToken().Type != TokenType.Comma)
                        {
                            break;
                        }
                        runToken(TokenType.Comma);
                    }
                    runToken(TokenType.RParenthesis);
                    var a = add.ToArray();
                    return new Ast.VarFunctionStatement(name, a); 
                }
            }
            else if (tokenPos.Type == TokenType.Inser)
            {
                string str = tokenPos.Text;
                runToken(TokenType.Inser);
                tokenPos = currentToken();
                str += tokenPos.Text;
                progress();
                return new Ast.Symbol(str);
            }
            else
                return null;
        }
        #endregion
    }
}
