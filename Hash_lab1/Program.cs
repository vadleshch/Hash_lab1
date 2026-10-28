using Org.BouncyCastle.Crypto.Digests;
using System.Text;
Console.OutputEncoding = Encoding.UTF8;

static string ComputeRipemd160(string input)
{
    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
    var digest = new RipeMD160Digest();
    digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
    byte[] hashBytes = new byte[digest.GetDigestSize()];
    digest.DoFinal(hashBytes, 0);
    return Convert.ToHexString(hashBytes).ToLower();
}

Random r = new Random();

string str = "Лещенко Владислав Олесьєвич";
Console.WriteLine($"{str}");
string hash = ComputeRipemd160(str);
Console.WriteLine($"Геш-значення: {hash}");
Console.WriteLine($"");


string concat(string input, int i)
{
    return input + i.ToString();
}

string RandomChange(ref string input)
{
    char[] chars = input.ToCharArray();
    char randomChar = (char)r.Next(32, 256);
    int ri = r.Next(0, input.Length);
    while (randomChar == input[ri])
    { 
        randomChar = (char)r.Next(32, 256);
    }
    chars[ri] = randomChar;
    input = new string(chars);
    return input;
}

(string, int) PrRandomChange(string input, string hash, bool printf30 = false)
{
    string vstr = input;
    string newHash;
    int counter = 0;
    do
    {
        counter++;
        RandomChange(ref vstr);
        newHash = ComputeRipemd160(vstr);
        if (printf30 && counter < 31)
        {
            Console.WriteLine($"{counter}: {vstr} {newHash}");
        }
    }
    while (!CompareHashes(newHash, hash, 4));
    return (vstr, counter);
}


int PrCountChange(string input, string hash, bool printf30 = false)
{
    string vstr;
    string newHash;
    int counter = 0;
    do
    {
        counter++;
        vstr = concat(input, counter);
        newHash = ComputeRipemd160(vstr);
        if (printf30 && counter < 31)
        {
            Console.WriteLine($"{vstr} {newHash}");
        }
    }
    while (!CompareHashes(newHash, hash, 4));
    return counter;
}

Console.WriteLine($"Змінений вхід: {str + PrCountChange(str, hash, true)}");
Console.WriteLine($"Геш-значення: {ComputeRipemd160(str + PrCountChange(str, hash))}");
Console.WriteLine($"");

bool CompareHashes(string hash1, string hash2, int length)
{
    length = Math.Min(length, Math.Min(hash1.Length, hash2.Length));
    bool areEqual = true;
    for (int i = hash1.Length - 1; i >= hash1.Length - length; i--)
    {
        if (hash1[i] != hash2[i])
        {
            areEqual = false;
            break;
        }
    }
    return areEqual;
}

string vstr;
int counter;
(vstr, counter) = PrRandomChange(str, hash, true);

Console.WriteLine($"Змінений вхід: {vstr}");
Console.WriteLine($"Геш-значення: {ComputeRipemd160(vstr)}");
Console.WriteLine($"Кількість ітерацій: {counter}");
Console.WriteLine($"");

(int, int) PrCountCollision(string input, bool printf30 = false)
{
    Dictionary<string, int> seen = new Dictionary<string, int>(); 
    int counter = 0;
    string newHash, suffix;
    while (true)
    {
        counter++;
        newHash = ComputeRipemd160(concat(input, counter));
        suffix = newHash[^8..];
        if (seen.TryGetValue(suffix, out int collisionNumber))
        {
            return (collisionNumber, counter);
        } 
        seen[suffix] = counter;
        if (printf30 && counter < 31)
        {
            Console.WriteLine($"{concat(input, counter)} {newHash}");
        }
    }
}

int collisionNumber;

(collisionNumber, counter) = PrCountCollision(str, true);

Console.WriteLine($"колізія:");
Console.WriteLine($"{str + counter} {ComputeRipemd160(str + counter)}");
Console.WriteLine($"{str + collisionNumber} {ComputeRipemd160(str + collisionNumber)}");
Console.WriteLine($"");

(int, string, string) PrRandomCollision(string input, bool printf30 = false)
{
    int counter = 0;
    string vstr = input;
    Dictionary<string, string> seen = new Dictionary<string, string>(); 
    string newHash = ComputeRipemd160(vstr);
    seen[newHash[^8..]] = vstr; 
    string vstr2, suffix;
    int id;
    while (true)
    {
        counter++;
        RandomChange(ref vstr);
        newHash = ComputeRipemd160(vstr);
        suffix = newHash[^8..];
        if (seen.TryGetValue(suffix, out vstr2))
        {
            if (vstr2 != vstr)
            {
                break;
            }
            continue;
        }
        seen[suffix] = vstr;
        if (printf30 && counter < 31)
        {
            Console.WriteLine($"{counter}: {vstr} {newHash}");
        }
    }
    return (counter, vstr, vstr2!);
}

string vstr2;
(counter, vstr, vstr2) = PrRandomCollision(str, true);
Console.WriteLine($"колізія після {counter} кроків із елементом згенерованим на кроці:");
Console.WriteLine($"{vstr2} - {ComputeRipemd160(vstr2)}");
Console.WriteLine($"{vstr} - {ComputeRipemd160(vstr)}");

string temp = str;
int iteations = 100;

string RandomSuf(string input)
{
    char[] chars = input.ToCharArray();
    char[] suffix = new char[5];
    for (int i = 0; i < 5; i++)
    { 
        suffix[i] = (char)r.Next(32, 256);
    }
    input = new string(chars) + new string(suffix);
    return input;
}

for (int i = 0; i < iteations; i++)
{
    temp = RandomSuf(str);
    hash = ComputeRipemd160(temp);
    Console.WriteLine($"{PrCountChange(temp, hash)}");
}

Console.WriteLine($"");

for (int i = 0; i < iteations; i++)
{
    temp = RandomSuf(str);
    hash = ComputeRipemd160(temp);
    (vstr, counter) = PrRandomChange(temp, hash);
    Console.WriteLine($"{counter}");
}

Console.WriteLine($"");

for (int i = 0; i < iteations; i++)
{
    temp = RandomSuf(str);
    hash = ComputeRipemd160(temp);
    (collisionNumber, counter) = PrCountCollision(temp);
    Console.WriteLine($"{counter}");
}

Console.WriteLine($"");

for (int i = 0; i < iteations; i++)
{
    temp = RandomSuf(str);
    hash = ComputeRipemd160(temp);
    (counter, vstr, vstr2) = PrRandomCollision(temp);
    Console.WriteLine($"{counter}");
}