using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            ListRandom ListRandomContext = new ListRandom();

            // Act
            ListRandomContext.Add("added data");

            // Assert
            Assert.AreEqual(expectedData, ListRandomContext.Tail.Data);
        }
    }
}
