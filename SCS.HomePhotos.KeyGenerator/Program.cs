using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var Random = new Random();

#if DEBUG
args = new string[] { "c:\\temp\\appsettings.json" };
#endif

if (args.Length == 0)
{
    ShowUsage();
}

try
{
    var json = File.ReadAllText(args[0]);

    json = SetValue(json, "ImageEncryptKey", GetAesKey());
    json = SetValue(json, "ImageEncryptPasscode", GetAesPasscode());
    json = SetValue(json, "SecurityKey", GetJwtKey());

    File.WriteAllText(args[0], json, Encoding.UTF8);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine();
    ShowUsage();
}

string GetAesKey()
{
    var aesKey = GetGenericKey(32);
    Console.WriteLine($"Set new \"ImageEncryptKey\": {aesKey}");
    
    return aesKey;
}

string GetAesPasscode()
{
    var aesPasscode = GetGenericKey(16);
    Console.WriteLine($"Set new \"ImageEncryptPasscode\": {aesPasscode}");

    return aesPasscode;
}

string GetGenericKey(int length)
{
    const string _availChars = "abcdefghijklmnopqrstuvwxyz0123456789";
    return new string(Enumerable.Repeat(_availChars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
}

string GetJwtKey()
{
    var jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    Console.WriteLine($"Set new JWT \"SecurityKey\": {jwtKey}");

    return jwtKey;
}
string SetValue(string json, string key, string newValue)
{
    var find = "\"" + key + "\": \".+\"";
    var replace = $"\"{key}\": \"{newValue}\"";

    return Regex.Replace(json, find, replace);
}

void ShowUsage()
{
    Console.WriteLine("KeyGenerator.exe \"<path to appsettings.json>\"");
}
