using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TwoWayList.List
{
    public class ListRandom : IEnumerable<ListNode>
    {
        private const byte ObjectSeparator = 0x00;

        public ListNode Head;
        public ListNode Tail;
        
        private int count { get; set; }
        public int Count => count;

        public void Add(string data)
        {
            ListNode toAdd = new ListNode { Data = data, };

            if (Head == null)
            {
                Head = toAdd;
            }
            else
            {
                Tail.Next = toAdd;
                toAdd.Previous = Tail;
            }

            Tail = toAdd;
            count++;
        }
        public void Clear()
        {
            count = 0;
            Head = null;
            Tail = null;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }
        IEnumerator<ListNode> IEnumerable<ListNode>.GetEnumerator()
        {
            ListNode nextNode = Head;
            while (nextNode != null)
            {
                yield return nextNode;
                nextNode = nextNode.Next;
            }
        }
        public IEnumerable<ListNode> GetReverseEnumerator()
        {
            ListNode prevNode = Tail;
            while (prevNode != null)
            {
                yield return prevNode;
                prevNode = prevNode.Previous;
            }
        }
        public ListNode this[int index]
        {
            get
            {
                if (count == 0)
                {
                    return null;
                }
                else
                {
                    int counter = 0;
                    if (index >= count)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else if (index < count / 2)
                    {
                        foreach (var item in this)
                        {
                            if (counter == index)
                            {
                                return item;
                            }
                            counter++;
                        }
                    }
                    else
                    {
                        counter = count - 1;
                        foreach (var item in GetReverseEnumerator())
                        {
                            if (counter == index)
                            {
                                return item;
                            }
                            counter--;
                        }
                    }
                }

                return null;
            }
        }
        private int GetNodeHash(ListNode listNode)
        {
            if (listNode == null)
            {
                return -1;
            }

            return (listNode, listNode.Data).GetHashCode();
        }

        public void Serialize(Stream s)
        {
            if (s.CanWrite)
            {
                s.Position = 0;
                foreach (var item in this)
                {
                    WriteNodeRowToStream(item, s);
                }
            }
        }
        private void WriteNodeRowToStream(ListNode listNode, Stream s)
        {
            byte[] code = BitConverter.GetBytes(GetNodeHash(listNode));
            s.Write(code, 0, code.Length);

            byte[] random = BitConverter.GetBytes(GetNodeHash(listNode.Random));
            s.Write(random, 0, random.Length);

            if (listNode.Data != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(listNode.Data);
                s.Write(data, 0, data.Length);
            }
            s.WriteByte(ObjectSeparator);
        }

        public void Deserialize(Stream s)
        {
            if (s.CanRead)
            {
                Dictionary<int, int> randRestoredIdx = new Dictionary<int, int>();
                Dictionary<int, ListNode> restoredObjects = new Dictionary<int, ListNode>();

                s.Position = 0;
                while (s.Position < s.Length)
                {
                    int currentObjCode = ReadNextNodeHashFromStream(s);
                    int randomRefCode = ReadNextNodeHashFromStream(s);

                    string restoredData = ReadNodeDataFromStream(s);

                    Add(restoredData);
                    restoredObjects.Add(currentObjCode, Tail);

                    if (restoredObjects.ContainsKey(randomRefCode))
                    {
                        restoredObjects[currentObjCode].Random = restoredObjects[randomRefCode];
                    }
                    else
                    {
                        randRestoredIdx.Add(currentObjCode, randomRefCode);
                    }
                }

                foreach (var randRestoreItem in randRestoredIdx)
                {
                    if (randRestoreItem.Value > -1)
                    {
                        restoredObjects[randRestoreItem.Key].Random = restoredObjects[randRestoreItem.Value];
                    }
                }
            }
        }
        private int ReadNextNodeHashFromStream(Stream s)
        {
            byte[] code = new byte[4];
            s.Read(code, 0, 4);
            return BitConverter.ToInt32(code, 0);
        }
        private string ReadNodeDataFromStream(Stream s)
        {
            List<byte> dataBytes = new List<byte>();
            int byteRead;
            while ((byteRead = s.ReadByte()) != 0x00)
            {
                dataBytes.Add((byte)byteRead);
            }

            if (dataBytes.Any())
            {
                return Encoding.UTF8.GetString(dataBytes.ToArray());
            }
            else
            {
                return null;
            }
        }
    }
}
