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

        foreach (byte b in array)
        {
            s += (char)b;
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
        bool founded = false;

        for (int i = 0; i < str.Length; i++)
        {
            if (String.Compare(mot, 0, str, i, mot.Length) == 0)
            {
                founded = true;
            }
            if (founded == true) break;
        }
        return founded;
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