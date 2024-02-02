using System.Text;
using System.Text.RegularExpressions;

namespace FileshareBackend;

public class Utils
{
    public static string CleanString(string dirtyString)
    {
        if (dirtyString == null) dirtyString = "";
        HashSet<char> removeChars = new HashSet<char>("?&^$#@!()+-,:;<>’\'-_*");
        StringBuilder result = new StringBuilder(dirtyString.Length);
        foreach (char c in dirtyString)
            if (!removeChars.Contains(c)) // prevent dirty chars
                result.Append(c);
        return result.ToString().Replace("..","");
    }
}