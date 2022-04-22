using System;
using System.IO;
using TwoWayList.List;

namespace TwoWayList
{
    class Program
    {
        static void Main(string[] args)
        {
            // Fill some data
            ListRandom listRandom = new ListRandom();

            listRandom.Add("data < 0");
            listRandom.Add(null);
            listRandom.Add("<data 2>");
            listRandom.Add(string.Empty);
            listRandom.Add("   ");
            listRandom.Add("da\0ta 5");

            // Fill random
            Random randGenerator = new Random();
            foreach (var item in listRandom)
            {
                int randIdx = randGenerator.Next(0, listRandom.Count - 1);
                item.Random = listRandom[randIdx];
            }

            #region Write/read from MemoryStream
            using (MemoryStream serializedData = new MemoryStream())
            {
                // serialization
                Console.WriteLine("Serializing...");
                listRandom.Serialize(serializedData);
                Console.WriteLine("Serialized");

                // deserialization
                Console.WriteLine("Deserializing...");
                ListRandom deserializedList = new ListRandom();

                try
                {
                    deserializedList.Deserialize(serializedData);
                    Console.WriteLine("Deserialized");
                    DemoUtils.PrintCompareListStructure(listRandom, deserializedList);
                }
                catch (Exception error) when
                        (error is InvalidDataException
                        || error is NullReferenceException)
                {
                    Console.WriteLine($"Errors occured:{Environment.NewLine}{error.Message}");
                }
            }
            #endregion

            #region Write/read from file
            //// serialization
            //string filepath = @"serializedList.dat";
            //if (File.Exists(filepath))
            //{
            //    File.Delete(filepath);
            //}
            //using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
            //{
            //    Console.WriteLine("Serializing...");
            //    listRandom.Serialize(fs);
            //    Console.WriteLine("Serialized");
            //}

            //// deserialization
            //using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
            //{
            //    Console.WriteLine("Deserializing...");
            //    ListRandom deserializedList = new ListRandom();

            //    try
            //    {
            //        deserializedList.Deserialize(fs);
            //        Console.WriteLine("Deserialized");
            //        DemoUtils.PrintCompareListStructure(listRandom, deserializedList);
            //    }
            //    catch (Exception error) when
            //            (error is InvalidDataException
            //            || error is NullReferenceException)
            //    {
            //        Console.WriteLine($"Errors occured:{Environment.NewLine}{error.Message}");
            //    }
            //}
            #endregion

            Console.ReadKey();
        }
    }
}
