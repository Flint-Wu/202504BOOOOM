using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CodeTrans : MonoBehaviour
{
    public Text Text;
    public string inputtext;

    void Start()
    {
        
    }

    void Update()
    {
        inputtext = Text.text;
    }

    public string StrTransCode(string codetext)
    {
        char[] chars = codetext.ToCharArray();
        string[] parts = new string[chars.Length];
        string[] rightCode;

        for (int i = 0; i < chars.Length; i++)
        {
            parts[i] = chars[i].ToString();
        }

        rightCode = new string[parts.Length];//创建转译代码的词组
        for (int i = 0; i < parts.Length; i++) //将代码转换回源代码
        {
            if(parts[i]== "0") rightCode[i] = "q";
            else if(parts[i] == "1") rightCode[i] = "w";
            else if(parts[i] == "2") rightCode[i] = "e";
            else if(parts[i] == "3") rightCode[i] = "r";
            else if(parts[i] == "4") rightCode[i] = "t";
            else if(parts[i] == "5") rightCode[i] = "y";
            else if(parts[i] == "6") rightCode[i] = "u";
            else if(parts[i] == "7") rightCode[i] = "i";
            else if(parts[i] == "8") rightCode[i] = "o";
            else if(parts[i] == "9") rightCode[i] = "p";
            else if(parts[i] == "a" ) rightCode[i] = "z";
            else if(parts[i] == "b" ) rightCode[i] = "x";
            else if(parts[i] == "c" ) rightCode[i] = "c";
            else if(parts[i] == "d" ) rightCode[i] = "v";
            else if(parts[i] == "e" ) rightCode[i] = "b";
            else if(parts[i] == "f" ) rightCode[i] = "n";
            else if(parts[i] == "g" ) rightCode[i] = "m";
            else if(parts[i] == "h" ) rightCode[i] = "a";
            else if(parts[i] == "i" ) rightCode[i] = "s";
            else if(parts[i] == "j" ) rightCode[i] = "d";
            else if(parts[i] == "k" ) rightCode[i] = "f";
            else if(parts[i] == "l" ) rightCode[i] = "g";
            else if(parts[i] == "m" ) rightCode[i] = "h";
            else if(parts[i] == "n" ) rightCode[i] = "j";
            else if(parts[i] == "o" ) rightCode[i] = "k";
            else if(parts[i] == "p" ) rightCode[i] = "l";
            else if(parts[i] == "q" ) rightCode[i] = "0";
            else if(parts[i] == "r" ) rightCode[i] = "3";
            else if(parts[i] == "s" ) rightCode[i] = "6";
            else if(parts[i] == "t" ) rightCode[i] = "9";
            else if(parts[i] == "u" ) rightCode[i] = "8";
            else if(parts[i] == "v" ) rightCode[i] = "5";
            else if(parts[i] == "w" ) rightCode[i] = "2";
            else if(parts[i] == "x" ) rightCode[i] = "1";
            else if(parts[i] == "y" ) rightCode[i] = "4";
            else if(parts[i] == "z" ) rightCode[i] = "7";
            else if(parts[i] == "A" ) rightCode[i] = "Z";
            else if(parts[i] == "B" ) rightCode[i] = "X";
            else if(parts[i] == "C" ) rightCode[i] = "C";
            else if(parts[i] == "D" ) rightCode[i] = "V";
            else if(parts[i] == "E" ) rightCode[i] = "B";
            else if(parts[i] == "F" ) rightCode[i] = "N";
            else if(parts[i] == "G" ) rightCode[i] = "M";
            else if(parts[i] == "H" ) rightCode[i] = "A";
            else if(parts[i] == "I" ) rightCode[i] = "S";
            else if(parts[i] == "J" ) rightCode[i] = "D";
            else if(parts[i] == "K" ) rightCode[i] = "F";
            else if(parts[i] == "L" ) rightCode[i] = "G";
            else if(parts[i] == "M" ) rightCode[i] = "H";
            else if(parts[i] == "N" ) rightCode[i] = "J";
            else if(parts[i] == "O" ) rightCode[i] = "K";
            else if(parts[i] == "P" ) rightCode[i] = "L";
            else if(parts[i] == "Q" ) rightCode[i] = "P";
            else if(parts[i] == "R" ) rightCode[i] = "O";
            else if(parts[i] == "S" ) rightCode[i] = "I";
            else if(parts[i] == "T" ) rightCode[i] = "U";
            else if(parts[i] == "U" ) rightCode[i] = "Y";
            else if(parts[i] == "V" ) rightCode[i] = "T";
            else if(parts[i] == "W" ) rightCode[i] = "R";
            else if(parts[i] == "X" ) rightCode[i] = "E";
            else if(parts[i] == "Y" ) rightCode[i] = "W";
            else if(parts[i] == "Z" ) rightCode[i] = "Q";
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < rightCode.Length; i++)//将转译后的代码输入文本框
        {
            sb.Append(rightCode[i]);
        }

        string codetxt = sb.ToString(); 

        sb = null;chars = null;parts = null;rightCode = null;

        return codetxt;

    }
    
    
    public string BakTransCode(string codetext)
    {
        char[] chars = codetext.ToCharArray();
        string[] parts = new string[chars.Length];
        string[] rightCode;

        for (int i = 0; i < chars.Length; i++)
        {
            parts[i] = chars[i].ToString();
        }

        rightCode = new string[parts.Length];//创建转译代码的词组
        for (int i = 0; i < parts.Length; i++) //将代码转换回源代码
        {
            if(parts[i]== "q") rightCode[i] = "0";
            else if(parts[i] == "w") rightCode[i] = "1";
            else if(parts[i] == "e") rightCode[i] = "2";
            else if(parts[i] == "r") rightCode[i] = "3";
            else if(parts[i] == "t") rightCode[i] = "4";
            else if(parts[i] == "y") rightCode[i] = "5";
            else if(parts[i] == "u") rightCode[i] = "6";
            else if(parts[i] == "i") rightCode[i] = "7";
            else if(parts[i] == "o") rightCode[i] = "8";
            else if(parts[i] == "p") rightCode[i] = "9";
            else if(parts[i] == "z" ) rightCode[i] = "a";
            else if(parts[i] == "x" ) rightCode[i] = "b";
            else if(parts[i] == "c" ) rightCode[i] = "c";
            else if(parts[i] == "v" ) rightCode[i] = "d";
            else if(parts[i] == "b" ) rightCode[i] = "e";
            else if(parts[i] == "n" ) rightCode[i] = "f";
            else if(parts[i] == "m" ) rightCode[i] = "g";
            else if(parts[i] == "a" ) rightCode[i] = "h";
            else if(parts[i] == "s" ) rightCode[i] = "i";
            else if(parts[i] == "d" ) rightCode[i] = "j";
            else if(parts[i] == "f" ) rightCode[i] = "k";
            else if(parts[i] == "g" ) rightCode[i] = "l";
            else if(parts[i] == "h" ) rightCode[i] = "m";
            else if(parts[i] == "j" ) rightCode[i] = "n";
            else if(parts[i] == "k" ) rightCode[i] = "o";
            else if(parts[i] == "l" ) rightCode[i] = "p";
            else if(parts[i] == "0" ) rightCode[i] = "q";
            else if(parts[i] == "3" ) rightCode[i] = "r";
            else if(parts[i] == "6" ) rightCode[i] = "s";
            else if(parts[i] == "9" ) rightCode[i] = "t";
            else if(parts[i] == "8" ) rightCode[i] = "u";
            else if(parts[i] == "5" ) rightCode[i] = "v";
            else if(parts[i] == "2" ) rightCode[i] = "w";
            else if(parts[i] == "1" ) rightCode[i] = "x";
            else if(parts[i] == "4" ) rightCode[i] = "y";
            else if(parts[i] == "7" ) rightCode[i] = "z";
            else if(parts[i] == "Z" ) rightCode[i] = "A";
            else if(parts[i] == "X" ) rightCode[i] = "B";
            else if(parts[i] == "C" ) rightCode[i] = "C";
            else if(parts[i] == "V" ) rightCode[i] = "D";
            else if(parts[i] == "B" ) rightCode[i] = "E";
            else if(parts[i] == "N" ) rightCode[i] = "F";
            else if(parts[i] == "M" ) rightCode[i] = "G";
            else if(parts[i] == "A" ) rightCode[i] = "H";
            else if(parts[i] == "S" ) rightCode[i] = "I";
            else if(parts[i] == "D" ) rightCode[i] = "J";
            else if(parts[i] == "F" ) rightCode[i] = "K";
            else if(parts[i] == "G" ) rightCode[i] = "L";
            else if(parts[i] == "H" ) rightCode[i] = "M";
            else if(parts[i] == "J" ) rightCode[i] = "N";
            else if(parts[i] == "K" ) rightCode[i] = "O";
            else if(parts[i] == "L" ) rightCode[i] = "P";
            else if(parts[i] == "P" ) rightCode[i] = "Q";
            else if(parts[i] == "O" ) rightCode[i] = "R";
            else if(parts[i] == "I" ) rightCode[i] = "S";
            else if(parts[i] == "U" ) rightCode[i] = "T";
            else if(parts[i] == "Y" ) rightCode[i] = "U";
            else if(parts[i] == "T" ) rightCode[i] = "V";
            else if(parts[i] == "R" ) rightCode[i] = "W";
            else if(parts[i] == "E" ) rightCode[i] = "X";
            else if(parts[i] == "W" ) rightCode[i] = "Y";
            else if(parts[i] == "Q" ) rightCode[i] = "Z";
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < rightCode.Length; i++)//将转译后的代码输入文本框
        {
            sb.Append(rightCode[i]);
        }

        string codetxt = sb.ToString();

        return codetxt;
    }
}
