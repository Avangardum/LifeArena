using Avangardum.LifeArena.Server.Helpers;

namespace Avangardum.LifeArena.Server.UnitTests;

public class TwoDimensionalArrayHelperTests
{
    [Test]
    public void ToListOfListsConvertsArrayToListOfLists()
    {
        var array = new int[2, 2];
        array[0, 1] = 1;
        array[1, 1] = 2;
        array[0, 0] = 3;
        array[1, 0] = 4;
        var expectedList = new List<List<int>> { new() { 1, 2 }, new() { 3, 4 } };
        var actualList = array.ToListOfLists();
        Assert.That(actualList, Is.EqualTo(expectedList));
    }
}