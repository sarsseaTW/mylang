require 'pp'
require 'fileutils'
require 'open3'

MY_LANG_EXE = "MyLang/bin/Debug/MyLang.exe"

def run_test(testcases, cmd)
  total = 0
  success = 0

  testcases.each do |input, expected_output|
    total += 1
    output, status = Open3.capture2e(*cmd, input)
	#puts "output => #{output}\n"
	#puts "status => #{status}\n"
    output.strip!

    if status.exitstatus != 0
      puts "ERROR: #{input}\n"
      next
    end
    
    if output != expected_output.to_s
      puts "NG: #{input} => #{output}, but expect #{expected_output}\n"
    else
      success += 1
      #puts "OK: #{input} => #{output}\n"
	  print "."
    end
  end
  if total == success
    puts "\nOK: #{total} testcases passed\n"
  else
    puts "\nERR: #{total-success} testcases failed\n"
    raise
  end
end

def test_tokenizer
  testcases = [
    ["1", "1 ;"],
    ["1 + 2", "1 + 2 ;"],
    ["1   +   2", "1 + 2 ;"],
    ["1   +  2 * 3", "1 + 2 * 3 ;"],
    #["1+2", "1 + 2 [EOF]"], # スペースがなくても、Tokenizeできるようにする
    #["a + b", "a + b [EOF]"], # Symbolも対応する
    #["(1 + 2) * 3", "( 1 + 2 ) [EOF]"], # "(", ")" に対応する
  ]
  puts "** Testing Tokenizer ..."
  run_test(testcases, [MY_LANG_EXE, '-t'])
end


def test_parser
  testcases = [
    ["1", "1"],
    ["1 + 2", "Add( 1 2 )"],
    ["2 * 3", "Multiply( 2 3 )"],
    ["1 + 2 * 3", "Add( 1 Multiply( 2 3 ) )"],
    ["1 + 2 + 3", "Add( Add( 1 2 ) 3 )"],
    ["1 * 2 * 3", "Multiply( Multiply( 1 2 ) 3 )"],
  ]
  puts "** Testing Parser ..."
  run_test(testcases, [MY_LANG_EXE, '-p'])
end

def test_interpreter
  testcases = [
    ["1", "Ans => 1"],
    ["1 + 2", "Ans => 3"],
    ["2 * 3", "Ans => 6"],
    ["1 + 2 * 3", "Ans => 7"],
    ["1 + 2 + 3", "Ans => 6"],
    ["1 * 2 * 3", "Ans => 6"],
  ]
  puts "** Testing Interpreter ..."
  run_test(testcases, [MY_LANG_EXE])
end
def test_Let
  testcases = [
    ["let a = 2", "Run_exp(L.Exp) => 2"],
    ["let a = 21 + 2", "Run_exp(L.Exp) => 23"],
    ["let a = 22 * 3", "Run_exp(L.Exp) => 66"],
    ["let a = 21 + 2 * 3", "Run_exp(L.Exp) => 27"],
    ["let a = 21 + 2 + 3", "Run_exp(L.Exp) => 26"],
    ["let a = 21 * 2 * 3", "Run_exp(L.Exp) => 126"],
  ]
  puts "** Testing Let ..."
  run_test(testcases, [MY_LANG_EXE])
end
def test_Print
  testcases = [
    ["let a = 21 * 2 * 3; print a;", "Run_exp(L.Exp) => 126\nRun_exp(P.Exp) => 126"]
  ]
  puts "** Testing Print ..."
  run_test(testcases, [MY_LANG_EXE])
end
def test_IF
  testcases = [
    ["let a = 21 * 2 * 3; if(a>0){print a;}", "Run_exp(L.Exp) => 126\nRun_exp(P.Exp) => 126"],#if
	["let a = 21 * 2 * 3; if( a == 0 ){ print a ;}elif( a > 0){ print a + 2 ;}else{ print a + 1 ;}", "Run_exp(L.Exp) => 126\nRun_exp(P.Exp) => 128"],#elif
	["let a = 21 * 2 * 3; if( a == 0 ){ print a ;}elif( a < 0){ print a + 2 ;}else{ print a + 1 ;}", "Run_exp(L.Exp) => 126\nRun_exp(P.Exp) => 127"],#else
	["let a = 21 * 2 * 3; if( a == 0 ){ print a ;}elif( a < 0){ print a + 2 ;}elif( a > 0){ print a + 100 ;}else{ print a + 1 ;}", "Run_exp(L.Exp) => 126\nRun_exp(P.Exp) => 226"],
	["if( a > 999 ){ let v = 445 ; let s = 990 ; print v ;}elif( a == 999){ let x = 999 ; print xd ;}else{ let a = 5 ; print 998 ;}","Run_exp(L.Exp) => 5\nRun_exp(P.Exp) => 998"]
  ]
  puts "** Testing IF ..."
  run_test(testcases, [MY_LANG_EXE])
end
def test_Function
  testcases = [
    ["function ccc{ let a = @0 - @1 ; print @0 + @1 ; print a ;  return @0 * @1 ;} print ccc(3,4,5) ; print ccc(1+9-3,9*9) ;  print ccc() ;","Run_exp(L.Exp) => 0\nRun_exp(P.Exp) => 0\nRun_exp(P.Exp) => 0\nRun_exp(R.Exp) => 0\nRun_exp(L.Exp) => -1\nRun_exp(P.Exp) => 7\nRun_exp(P.Exp) => -1\nRun_exp(R.Exp) => 12\nRun_exp(L.Exp) => -74\nRun_exp(P.Exp) => 88\nRun_exp(P.Exp) => -74\nRun_exp(R.Exp) => 567\nRun_exp(L.Exp) => 0\nRun_exp(P.Exp) => 0\nRun_exp(P.Exp) => 0\nRun_exp(R.Exp) => 0"]
  ]
  puts "** Testing Function ..."
  run_test(testcases, [MY_LANG_EXE])
end

test_tokenizer
puts "\n"
test_parser
puts "\n"
test_interpreter
puts "\n"
test_Let
puts "\n"
test_Print
puts "\n"
test_IF
puts "\n"
test_Function
puts "\n"
