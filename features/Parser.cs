using System.Globalization;
using System.Text;

namespace PoProj.features;

public static class Parser<T> where T: IParsable<T>
{
    public static T[] StringToArray(string arrayString)
    {
        // Trimming first and last char of the arrayString, which are '[' and ']'
        string trimmedInput = arrayString.Substring(1, arrayString.Length - 2);
        string[] subStrings = trimmedInput.Split(';');

        // Initializing output array
        var output = new T[subStrings.Length];

        // Filling each element of the array
        for (int i = 0; i < subStrings.Length; i++)
        {
            output[i] = T.Parse(subStrings[i], CultureInfo.InvariantCulture);
        }
        return output;
    }

    public static string GetStringFromBytes(byte[] bytes, int offset, int length)
    {
        // Used to trim strings
        char[] nullBytes = { '\0' };
        return Encoding.ASCII.GetString(bytes, offset, length).Trim(nullBytes);
    }
}