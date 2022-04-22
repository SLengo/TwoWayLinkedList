using System;

namespace TwoWayList.List
{
    public static class DemoUtils
    {
        public static void PrintCompareListStructure(ListRandom originList, ListRandom restoredList)
        {
            Console.WriteLine("-------------- List Comparison ---------------");
            Console.WriteLine();
            Console.WriteLine("--- Origin | Restored ---");
            Console.WriteLine();

            for (int i = 0; i < originList.Count; i++)
            {
                Console.WriteLine($"Object #{i}");

                Console.WriteLine($"Data: {GetDataForPrint(originList[i])} | {GetDataForPrint(restoredList[i])}");
                Console.WriteLine($"Next: {(originList[i].Next == null ? "null" : GetDataForPrint(originList[i].Next))} | {(restoredList[i].Next == null ? "null" : GetDataForPrint(restoredList[i].Next))}");
                Console.WriteLine($"Previous: {(originList[i].Previous == null ? "null" : GetDataForPrint(originList[i].Previous))} | {(restoredList[i].Previous == null ? "null" : GetDataForPrint(restoredList[i].Previous))}");
                Console.WriteLine($"Random: {(originList[i].Random == null ? "null" : GetDataForPrint(originList[i].Random))} | {(restoredList[i].Random == null ? "null" : GetDataForPrint(restoredList[i].Random))}");
                Console.WriteLine();
            }

            Console.WriteLine("----------------------------------------------");
        }

        public static string GetDataForPrint(ListNode nodeToPrint)
        {
            if (nodeToPrint == null)
            {
                return "NODE NULL";
            }
            else
            {
                return $"{(nodeToPrint.Data == null ? "data is null" : (nodeToPrint.Data.Length > 30 ? $"\"{nodeToPrint.Data.Substring(0, 30)}\"" : $"\"{nodeToPrint.Data}\""))}";
            }
        }
    }
}
