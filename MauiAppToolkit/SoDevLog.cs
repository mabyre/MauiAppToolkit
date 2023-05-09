using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDevLog;

public class Strings
{ 
    static public string ByteArrayToString(byte[] array)
    {
        string s = "";

        for (int i = 0; i < array.Length; i++)
        {
            s += (char)array[i];
        }

        // Dos vs Unix
        //
        if (s.Contains("\r\n"))
        {
            return s;
        }

        if (s.Contains("\n"))
        {
            s = s.Replace("\n", "\r\n");
        }
        return s;
    }
    static public bool StringSearchWord(string str, string mot)
    {
        bool trouve = false;

        for (int i = 0; i < str.Length; i++)
        {
            if (String.Compare(mot, 0, str, i, mot.Length) == 0)
            {
                trouve = true;
            }
            if (trouve == true) break;
        }
        return trouve;
    }

    static public byte[] StringToByteArray(string str)
    {
        char[] s = str.ToCharArray();
        byte[] b = new byte[s.Length];

        for (int i = 0; i < s.Length; i++)
        {
            b[i] = (byte)s[i];
        }
        return b;
    }
}