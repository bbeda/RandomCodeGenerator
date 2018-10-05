using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RandomStringGenerator
{
    class Program
    {
        private static Dictionary<string, int> generated = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            string str = string.Empty;
            var generator = new VerySimpleRandomCodeGenerator();
            var consecutive = 1;
            Dictionary<int, int> consecutiveStats = new Dictionary<int, int>();
            while (true)
            {
                str = generator.Generate(5);
                if (generated.ContainsKey(str))
                {
                    consecutive++;
                    generated[str]++;
                    //Console.WriteLine($"{str}--{generated.Count}--{generated[str]}--{consecutive}");
                }
                else
                {
                    if (consecutiveStats.ContainsKey(consecutive))
                    {
                        consecutiveStats[consecutive]++;
                    }
                    else
                    {
                        consecutiveStats[consecutive] = 1;
                    }

                    consecutive = 1;

                    generated.Add(str, 1);
                }

                if (generated.Count % 500000 == 0)
                    Console.WriteLine(generated.Count);

                if (generated.Count == 25000000)
                    break;
            }

            var foundStats = generated.Values.GroupBy(g => g).Select(g => (value: g.Key, count: g.Count())).OrderBy(o => o.value).ToArray(); ;

            var sb = new StringBuilder();

            sb.AppendLine("Colisions:");
            foreach (var found in foundStats)
                sb.AppendLine($"{found.value},{found.count}");

            sb.AppendLine("Consecutives");
            foreach (var csv in consecutiveStats)
            {
                sb.AppendLine($"{csv.Key},{csv.Value}");
            }

            var s = sb.ToString();

            Console.WriteLine(s);
            File.WriteAllText("output.txt", s);

            Console.ReadKey();
        }

    }

    public class SimpleRandomCodeGenerator : IRandomCodeGenerator
    {

        static readonly string Alphabet1 = "abcdefghijklmnopqestuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        static readonly Random random = new Random();
        public string Generate(int characterCount)
        {
            //var generated = Enumerable.Range(0, 10).Select(ix => GenerateInternal(characterCount)).ToArray();
            //return generated[random.Next(10)];

            return GenerateInternal(characterCount);
        }

        private static string GenerateInternal(int characterCount)
        {
            //var Alphabet = string.Join(string.Empty, Alphabet1.OrderBy(c => random.Next()));
            var Alphabet = Alphabet1;
            //Console.WriteLine(Alphabet);
            var sb = new StringBuilder();
            var randomNumberGenerator = new RNGCryptoServiceProvider();
            for (int ix = 0; ix < characterCount; ix++)
            {
                var data = new byte[4];
                randomNumberGenerator.GetBytes(data);
                var randomInteger = BitConverter.ToUInt32(data);
                sb.Append(Alphabet[(int)(randomInteger % (uint)Alphabet.Length)]);
            }

            return sb.ToString();
        }
    }

    public class VerySimpleRandomCodeGenerator : IRandomCodeGenerator
    {
        static readonly Random RandomNumberGenerator = new Random();
        static readonly string Alphabet = "abcdefghijklmnopqestuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        public string Generate(int characterCount)
        {
            return new string(Enumerable.Repeat(Alphabet, characterCount)
                .Select(alphabet => alphabet.ElementAt(RandomNumberGenerator.Next() % alphabet.Length))
                .ToArray());
        }
    }

    public interface IRandomCodeGenerator
    {
        string Generate(int characterCount);
    }
}
