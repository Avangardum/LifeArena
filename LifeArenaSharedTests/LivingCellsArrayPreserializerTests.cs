namespace Avangardum.LifeArena.Shared.Tests;

[TestFixture]
public class LivingCellsArrayPreserializerTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private LivingCellsArrayPreserializer _preserializer;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        _preserializer = new LivingCellsArrayPreserializer();
    }

    [Test]
    public void Serializes()
    {
        var array = new bool[2, 2];
        array[0, 0] = true;
        array[1, 0] = true;
        var actual = _preserializer.Preserialize(array);
        var expected = new List<string> { "..", "00" };
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test]
    public void Deserializes()
    {
        var expected = new bool[2, 2];
        expected[0, 0] = true;
        expected[1, 0] = true;
        var preserializedObject = new List<string> { "..", "00" };
        var actual = _preserializer.Depreserialize(preserializedObject);
        Assert.That(actual, Is.EqualTo(expected));
    }
}