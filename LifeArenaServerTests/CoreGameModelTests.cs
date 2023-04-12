using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;

namespace LifeArena.Server.Tests;

[TestFixture]
public class CoreGameModelTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private ICoreGameModel _coreGameModel;
    #pragma warning restore CS8618
    
    [SetUp]
    public void SetUp()
    {
        _coreGameModel = new CoreGameModel(new bool[100, 100], 0, FieldWrapAroundMode.None);
    }
    
    [Test]
    public void GetLivingCellsReturnsEmptyGrid()
    {
        Assert.That(_coreGameModel.LivingCells, Is.All.False);
    }
    
    [Test]
    public void GetLivingCellsDoesntAllowToModifyGrid()
    {
        _coreGameModel.LivingCells[0, 0] = true;
        Assert.That(_coreGameModel.LivingCells[0, 0], Is.False);
    }
    
    [Test]
    public void SetCellStateSetsCellState()
    {
        _coreGameModel.SetCellState(0, 0, true);
        Assert.That(_coreGameModel.LivingCells[0, 0], Is.True);
        _coreGameModel.SetCellState(0, 0, false);
        Assert.That(_coreGameModel.LivingCells[0, 0], Is.False);
    }
    
    [Test]
    public void NextGenerationIncreasesGeneration()
    {
        Assert.That(_coreGameModel.Generation, Is.EqualTo(0));
        _coreGameModel.NextGeneration();
        Assert.That(_coreGameModel.Generation, Is.EqualTo(1));
    }

    [Test]
    public void CreatingGameModelWithPreexistingStateSetsTheGameToThatState()
    {
        var livingCells = new[,] { { false, true, false }, { false, false, false }, { false, false, false } };
        _coreGameModel = new CoreGameModel(livingCells, 42, FieldWrapAroundMode.None);
        Assert.That(_coreGameModel.LivingCells, Is.EqualTo(livingCells));
        Assert.That(_coreGameModel.Generation, Is.EqualTo(42));
    }

    [Test]
    public void SingleCellDies()
    {
        _coreGameModel.SetCellState(10, 10, true);
        _coreGameModel.NextGeneration();
        Assert.That(_coreGameModel.LivingCells[10, 10], Is.False);
    }

    [Test]
    public void SquareSurvives()
    {
        _coreGameModel.SetCellState(10, 10, true);
        _coreGameModel.SetCellState(10, 11, true);
        _coreGameModel.SetCellState(11, 10, true);
        _coreGameModel.SetCellState(11, 11, true);
        for (int i = 0; i < 10; i++)
        {
            _coreGameModel.NextGeneration();
            Assert.That(_coreGameModel.LivingCells[10, 10], Is.True);
            Assert.That(_coreGameModel.LivingCells[10, 11], Is.True);
            Assert.That(_coreGameModel.LivingCells[11, 10], Is.True);
            Assert.That(_coreGameModel.LivingCells[11, 11], Is.True);
        }
    }

    [Test]
    public void GliderGlides()
    {
        _coreGameModel.SetCellState(10, 90, true);
        _coreGameModel.SetCellState(11, 90, true);
        _coreGameModel.SetCellState(12, 90, true);
        _coreGameModel.SetCellState(12, 91, true);
        _coreGameModel.SetCellState(11, 92, true);
        
        for (int i = 0; i < 20; i++)
        {
            _coreGameModel.NextGeneration();
        }
        
        Assert.That(_coreGameModel.LivingCells[10, 90], Is.False);
        Assert.That(_coreGameModel.LivingCells[11, 90], Is.False);
        Assert.That(_coreGameModel.LivingCells[12, 90], Is.False);
        Assert.That(_coreGameModel.LivingCells[12, 91], Is.False);
        Assert.That(_coreGameModel.LivingCells[11, 92], Is.False);
        
        Assert.That(_coreGameModel.LivingCells[15, 85], Is.True);
        Assert.That(_coreGameModel.LivingCells[16, 85], Is.True);
        Assert.That(_coreGameModel.LivingCells[17, 85], Is.True);
        Assert.That(_coreGameModel.LivingCells[17, 86], Is.True);
        Assert.That(_coreGameModel.LivingCells[16, 87], Is.True);
    }

    [Test]
    public void SquareAcrossFieldEdgeSurvivesOrDiesDependingOnWrapAroundMode([Values] FieldWrapAroundMode wrapAroundMode)
    {
        _coreGameModel = new CoreGameModel(new bool[100, 100], 0, wrapAroundMode);
        
        _coreGameModel.SetCellState(0, 10, true);
        _coreGameModel.SetCellState(0, 11, true);
        _coreGameModel.SetCellState(99, 10, true);
        _coreGameModel.SetCellState(99, 11, true);
        
        _coreGameModel.SetCellState(10, 0, true);
        _coreGameModel.SetCellState(11, 0, true);
        _coreGameModel.SetCellState(10, 99, true);
        _coreGameModel.SetCellState(11, 99, true);
        
        _coreGameModel.NextGeneration();

        bool shouldFistSquareSurvive = wrapAroundMode.HasFlag(FieldWrapAroundMode.Horizontal);
        Assert.That(_coreGameModel.LivingCells[0, 10], Is.EqualTo(shouldFistSquareSurvive));
        Assert.That(_coreGameModel.LivingCells[0, 11], Is.EqualTo(shouldFistSquareSurvive));
        Assert.That(_coreGameModel.LivingCells[99, 10], Is.EqualTo(shouldFistSquareSurvive));
        Assert.That(_coreGameModel.LivingCells[99, 11], Is.EqualTo(shouldFistSquareSurvive));
        
        bool shouldSecondSquareSurvive = wrapAroundMode.HasFlag(FieldWrapAroundMode.Vertical);
        Assert.That(_coreGameModel.LivingCells[10, 0], Is.EqualTo(shouldSecondSquareSurvive));
        Assert.That(_coreGameModel.LivingCells[11, 0], Is.EqualTo(shouldSecondSquareSurvive));
        Assert.That(_coreGameModel.LivingCells[10, 99], Is.EqualTo(shouldSecondSquareSurvive));
        Assert.That(_coreGameModel.LivingCells[11, 99], Is.EqualTo(shouldSecondSquareSurvive));
    }
}