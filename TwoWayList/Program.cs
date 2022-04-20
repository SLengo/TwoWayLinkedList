using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TwoWayList.List;

namespace TwoWayList
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test data
            ListRandom listRandom = new ListRandom();

            listRandom.Add("data 1");
            listRandom.Add(null);
            listRandom.Add("data 3");
            listRandom.Add(string.Empty);
            listRandom.Add("data 5");

            // fill random
            Random randGenerator = new Random();
            foreach (var item in listRandom)
            {
                int randIdx = randGenerator.Next(0, listRandom.Count - 1);
                item.Random = listRandom[randIdx];
            }

            DemoUtils.PrintListStructure(listRandom);

            string filepath = @"serializedList.dat";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            // serialization
            Stopwatch stopwatch = new Stopwatch();
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                Console.WriteLine("Serializing...");
                stopwatch.Start();
                listRandom.Serialize(fs);
                stopwatch.Stop();
                Console.WriteLine("Serialized");
                // Console.WriteLine($"Serialization completed in: {stopwatch.Elapsed}");
            }

            // deserialization
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                Console.WriteLine("Deserializing...");
                stopwatch.Restart();
                ListRandom deserializedList = new ListRandom();
                deserializedList.Deserialize(fs);
                stopwatch.Stop();
                Console.WriteLine("Deserialized");

                DemoUtils.PrintListStructure(deserializedList);
            }

            Console.ReadKey();
        }
    }
}
