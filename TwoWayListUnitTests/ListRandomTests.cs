using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TwoWayList.List;

namespace TwoWayListUnitTests
{
    [TestClass]
    public class ListRandomTests
    {
        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void Add_AddElementToList_ElementAddedCorrectly()
        {
            // Arrange
            string expectedData = "added data";
            ListRandom listRandomContext = new ListRandom();

            // Act
            listRandomContext.Add("added data");

            // Assert
            Assert.AreEqual(expectedData, listRandomContext.Tail.Data);
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void Clear_ClearNonEmptyList_ListCleared()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();
            listRandomContext.Add("data 1");

            // Act
            listRandomContext.Clear();

            // Assert
            Assert.AreEqual(0, listRandomContext.Count);
            Assert.IsNull(listRandomContext.Head);
            Assert.IsNull(listRandomContext.Tail);
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void Count_GetCountOfAddedElements_CountWasEqualToExpeceted()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();
            listRandomContext.Add("data 1");
            listRandomContext.Add("data 2");
            listRandomContext.Add("data 1");

            // Act
            int count = listRandomContext.Count;

            // Assert
            Assert.AreEqual(3, count);
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void GetItemByIndex_GetDesiredItem_ItemWasRetrived()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();
            listRandomContext.Add("data 1");
            listRandomContext.Add("data 2");
            listRandomContext.Add("data 1");

            // Act
            ListNode retrivedData = listRandomContext[1];

            // Assert
            Assert.AreEqual("data 2", retrivedData.Data);
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        [DataRow(-1)]
        [DataRow(999)]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetItemByIndex_GetItemByInvaldIndex_ExpectedExceptionWasThrowed(int itemIndex)
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();
            listRandomContext.Add("data 1");

            // Act
            _ = listRandomContext[itemIndex];

            // Assert
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void Serialize_SerializeData_SerializationWasFinishedWithoutErrors()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();
            listRandomContext.Add("dat>a 1");
            listRandomContext.Add("<data 2>");
            listRandomContext.Add("dat\0a 3\0");
            listRandomContext[0].Random = listRandomContext[2];

            long actualDataLength = -1;
            

            // Act
            using (MemoryStream actualData = new MemoryStream())
            {
                listRandomContext.Serialize(actualData);
                actualDataLength = actualData.Length;
            }

            // Assert
            Assert.IsTrue(actualDataLength > 0);
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Serialize_StreamWithoutWritePermPassed_ExpectedExceptionWasThrowed()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();

            // Act
            using (MemoryStream actualData = new MemoryStream(new byte[4], false))
            {
                listRandomContext.Serialize(actualData);
            }

            // Assert
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void Deserialize_DeserializeByteData_DataWasDeserializedAsExpected()
        {
            // Arrange
            byte[] dataToRestore = new byte[] { 166, 76, 114, 55, 223, 234, 7, 212, 60, 100, 97,
                116, 95, 120, 48, 48, 51, 69, 95, 97, 95, 120, 48, 48, 50, 48, 95, 49, 62, 0,
                78, 222, 210, 99, 255, 255, 255, 255, 60, 95, 120, 48, 48, 51, 67, 95, 100, 97,
                116, 97, 95, 120, 48, 48, 50, 48, 95, 50, 95, 120, 48, 48, 51, 69, 95, 62, 0,
                223, 234, 7, 212, 255, 255, 255, 255, 60, 100, 97, 116, 95, 120, 48, 48, 48, 48,
                95, 97, 95, 120, 48, 48, 50, 48, 95, 51, 95, 120, 48, 48, 48, 48, 95, 62, 0, };

            ListRandom restoredList = new ListRandom();

            // Act
            using (MemoryStream ms = new MemoryStream(dataToRestore))
            {
                restoredList.Deserialize(ms);
            }

            // Assert
            Assert.AreEqual("dat>a 1", restoredList.Head.Data);
            Assert.AreEqual("dat\0a 3\0", restoredList.Tail.Data);

            Assert.AreEqual("dat>a 1", restoredList[0].Data);
            Assert.AreEqual("<data 2>", restoredList[1].Data);
            Assert.AreEqual("dat\0a 3\0", restoredList[2].Data);

            Assert.AreEqual("dat\0a 3\0", restoredList[0].Random.Data);
            Assert.IsNull(restoredList[1].Random);
            Assert.IsNull(restoredList[2].Random);

            Assert.IsNull(restoredList[0].Previous);
            Assert.AreEqual("dat>a 1", restoredList[1].Previous.Data);
            Assert.AreEqual("<data 2>", restoredList[2].Previous.Data);

            Assert.AreEqual("<data 2>", restoredList[0].Next.Data);
            Assert.AreEqual("dat\0a 3\0", restoredList[1].Next.Data);
            Assert.IsNull(restoredList[2].Next);
        }
        
        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        public void Deserialize_PassZeroLengthStream_EmptyListWasRecieved()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();

            // Act
            using (MemoryStream actualData = new MemoryStream())
            {
                listRandomContext.Deserialize(actualData);
            }

            // Assert
            Assert.AreEqual(0, listRandomContext.Count);
            Assert.IsNull(listRandomContext.Head);
            Assert.IsNull(listRandomContext.Tail);
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        [ExpectedException(typeof(NullReferenceException))]
        public void Deserialize_PassNullStream_ExpectedExceptionWasThrowed()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();

            // Act
            listRandomContext.Deserialize(null);

            // Assert
        }

        [TestMethod()]
        [TestProperty("TwoWayList", "ListRandom")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_PassStreamWithoutReadPerm_ExpectedExceptionWasThrowed()
        {
            // Arrange
            ListRandom listRandomContext = new ListRandom();
            MemoryStream actualData = new MemoryStream();
            actualData.Close();

            // Act
            listRandomContext.Deserialize(actualData);

            // Assert
        }
    }
}
