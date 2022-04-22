using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace TwoWayList.List
{
    public class ListRandom : IEnumerable<ListNode>
    {
        private const byte ObjectSeparator = 0x00;

        private const string SerErrMsg = "Serialization error occured: ";
        private const string DeserErrMsg = "Deserialization error occured: ";
        private static readonly string SerNullStreamErrMsg = $"{SerErrMsg}incoming stream was null";
        private static readonly string SerCannotWriteErrMsg = $"{SerErrMsg}cannot write to passed stream";
        private static readonly string DeserNullStreamErrMsg = $"{DeserErrMsg}incoming stream was null";
        private static readonly string SerCannotReadErrMsg = $"{DeserErrMsg}cannot read from passed stream";
        private static readonly string DeserInvalidDataErrMsg = $"{DeserErrMsg}incoming stream contains invalid data";
        private static readonly string DeserUnexepectedEndOfDataErrMsg = $"{DeserErrMsg}incoming stream contains unexpected end of data";

        public ListNode Head { get; private set; }
        public ListNode Tail { get; private set; }

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
                if (index < 0 || index >= count)
                {
                    throw new IndexOutOfRangeException();
                }

                int counter = 0;
                ListNode found = null;

                if (index < count / 2)
                {
                    foreach (var item in this)
                    {
                        if (counter == index)
                        {
                            found = item;
                            break;
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
                            found = item;
                            break;
                        }
                        counter--;
                    }
                }

                return found;
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
            if (s == null)
            {
                throw new NullReferenceException(SerNullStreamErrMsg);
            }

            if (s.CanWrite)
            {
                s.Position = 0;
                foreach (var item in this)
                {
                    WriteNodeRowToStream(item, s);
                }
            }
            else
            {
                throw new InvalidOperationException(SerCannotWriteErrMsg);
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
                string encodedData = EscapeData(listNode.Data);
                byte[] data = Encoding.UTF8.GetBytes(encodedData);
                s.Write(data, 0, data.Length);
            }
            s.WriteByte(ObjectSeparator);
        }
        private string EscapeData(string dataToEncode)
        {
            StringBuilder encodedData = new StringBuilder();

            encodedData.Append("<");
            if (dataToEncode != string.Empty)
            {
                encodedData.Append(XmlConvert.EncodeName(dataToEncode));
            }
            encodedData.Append(">");

            return encodedData.ToString();
        }

        public void Deserialize(Stream s)
        {
            if (s == null)
            {
                throw new NullReferenceException(DeserNullStreamErrMsg);
            }

            Clear();

            if (s.CanRead)
            {
                if (s.Length > 0)
                {
                    List<Tuple<int, int>> randRestoredIdx = new List<Tuple<int, int>>();
                    Dictionary<int, ListNode> restoredObjects = new Dictionary<int, ListNode>();

                    s.Position = 0;
                    while (s.Position < s.Length)
                    {
                        int currentObjCode = ReadNextNodeHashFromStream(s);
                        int randomRefCode = ReadNextNodeHashFromStream(s);

                        string restoredData = ReadNodeDataFromStream(s);

                        if (restoredObjects.ContainsKey(currentObjCode))
                        {
                            throw new InvalidDataException(DeserInvalidDataErrMsg);
                        }

                        Add(restoredData);
                        restoredObjects.Add(currentObjCode, Tail);

                        if (restoredObjects.ContainsKey(randomRefCode))
                        {
                            restoredObjects[currentObjCode].Random = restoredObjects[randomRefCode];
                        }
                        else
                        {
                            randRestoredIdx.Add(new Tuple<int, int>(currentObjCode, randomRefCode));
                        }
                    }

                    foreach (var randRestoreItem in randRestoredIdx)
                    {
                        if (randRestoreItem.Item2 != -1)
                        {
                            if (!restoredObjects.ContainsKey(randRestoreItem.Item1)
                                || !restoredObjects.ContainsKey(randRestoreItem.Item2))
                            {
                                throw new InvalidDataException(DeserInvalidDataErrMsg);
                            }

                            restoredObjects[randRestoreItem.Item1].Random = restoredObjects[randRestoreItem.Item2];
                        }
                    }

                    randRestoredIdx.Clear();
                    restoredObjects.Clear();
                }
            }
            else
            {
                throw new InvalidOperationException(SerCannotReadErrMsg);
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
            while ((byteRead = s.ReadByte()) != 0x00 && byteRead != -1)
            {
                dataBytes.Add((byte)byteRead);
            }

            if (byteRead == -1)
            {
                throw new InvalidDataException(DeserUnexepectedEndOfDataErrMsg);
            }

            if (dataBytes.Any())
            {
                return UnescapeData(Encoding.UTF8.GetString(dataBytes.ToArray()));
            }
            else
            {
                return null;
            }
        }
        private string UnescapeData(string toUnescape)
        {
            if (toUnescape != string.Empty)
            {
                StringBuilder stringBuilder = new StringBuilder(XmlConvert.DecodeName(toUnescape));

                stringBuilder.Remove(0, 1);
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

                return stringBuilder.ToString();
            }

            return toUnescape;
        }
    }
}
