using System;

namespace TwoWayList.List
{
    public static class DemoUtils
    {
        public static void PrintListStructure(ListRandom listToPrint)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"List structure");
            Console.WriteLine($"Head: {listToPrint.Head.Data}");
            Console.WriteLine($"Tail: {listToPrint.Tail.Data}");
            Console.WriteLine("-----------------------------");

            foreach (var item in listToPrint)
            {
                Console.WriteLine($"Object: \"{(item.Data == null ? "null" : item.Data)}\"");
                Console.WriteLine($"Next: \"{(item.Next == null ? "null" : item.Next.Data)}\"");
                Console.WriteLine($"Previous: \"{(item.Previous == null ? "null" : item.Previous.Data)}\"");
                Console.WriteLine($"Random: \"{(item.Random == null ? "null" : item.Random.Data)}\"");
                Console.WriteLine("");
            }
            Console.WriteLine("-----------------------------");
        }
    }
}
