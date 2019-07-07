require 'pp'
require 'fileutils'
require 'open3'

MY_LANG_EXE = "MyLang/bin/Debug/MyLang.exe"

# テストケースを分割する
#
# - テストケースは、１行ごとに一つのテストケース
# - 入力と出力は "#" で分割される
# - 前後のスペースは、削除される
# - 空行は無視される
# - 先頭が"#"の場合も無視される
#
# parse_testcases("a # b") => [['a', 'b']]
# parse_testcases("a # b\n c # d") => [['a', 'b']]
def parse_testcases(str)
  lines = str.split(/\n/).reject(&:empty?).reject{|s| s =~ /^\s+#/ }
  testcases = lines.map do |l| 
    elements = l.split(/#/).map{|element| element.strip }
  end
end

def test_tokenizer
  puts "Testing tokenizer ..."
  test_str = <<EOT
  1             # 1
  1 + 2         # 1 + 2
  1   +   2     # 1 + 2
  1   +  2 * 3  # 1 + 2 * 3
  # 1+2         # 1 + 2         # スペースがなくても、Tokenizeできるようにする
  # a + b       # a + b         # Symbolも対応する
  # (1 + 2) * 3 # ( 1 + 2 )     # "(", ")" に対応する
EOT
  testcases = parse_testcases(test_str)
  testcases.each do |input, expected_output|
    output, status = Open3.capture2e(MY_LANG_EXE, '-t', input)
    output.strip!

    if status.exitstatus != 0
      puts "ERROR: #{input}"
      next
    end
    
    if output != expected_output
      puts "NG: #{input} => #{output} ,but expect #{expected_output}"
    else
      puts "OK: #{input} => #{output}"
    end
  end
end


def test_parser
  puts "Testing parser ..."
  test_str = <<EOT
EOT
  testcases = parse_testcases(test_str)
  testcases.each do |input, expected_output|
    output, status = Open3.capture2e(MY_LANG_EXE, '-p', input)
    output.strip!

    if status.exitstatus != 0
      puts "ERROR: #{input}"
      next
    end
    
    if output != expected_output
      puts "NG: #{input} => #{output} ,but expect #{expected_output}"
    else
      puts "OK: #{input} => #{output}"
    end
  end
end

test_tokenizer
test_parser
