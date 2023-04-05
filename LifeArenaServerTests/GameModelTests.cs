using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;

namespace LifeArenaServerTests;

[TestFixture]
public class GameModelTests
{
    private IGameModel _gameModel;
    
    [SetUp]
    public void SetUp()
    {
        _gameModel = new GameModel(new bool[100, 100], 0, FieldWrapAroundMode.None);
    }
    
    [Test]
    public void GetLivingCellsReturnsEmptyGrid()
    {
        Assert.That(_gameModel.LivingCells, Is.All.False);
    }
    
    [Test]
    public void GetLivingCellsDoesntAllowToModifyGrid()
    {
        _gameModel.LivingCells[0, 0] = true;
        Assert.That(_gameModel.LivingCells[0, 0], Is.False);
    }
    
    [Test]
    public void SetCellStateSetsCellState()
    {
        _gameModel.SetCellState(0, 0, true);
        Assert.That(_gameModel.LivingCells[0, 0], Is.True);
        _gameModel.SetCellState(0, 0, false);
        Assert.That(_gameModel.LivingCells[0, 0], Is.False);
    }
    
    [Test]
    public void NextGenerationIncreasesGeneration()
    {
        Assert.That(_gameModel.Generation, Is.EqualTo(0));
        _gameModel.NextGeneration();
        Assert.That(_gameModel.Generation, Is.EqualTo(1));
    }

    [Test]
    public void CreatingGameModelWithPreexistingStateSetsTheGameToThatState()
    {
        var livingCells = new[,] { { false, true, false }, { false, false, false }, { false, false, false } };
        _gameModel = new GameModel(livingCells, 42, FieldWrapAroundMode.None);
        Assert.That(_gameModel.LivingCells, Is.EqualTo(livingCells));
        Assert.That(_gameModel.Generation, Is.EqualTo(42));
    }

    [Test]
    public void SingleCellDies()
    {
        _gameModel.SetCellState(10, 10, true);
        _gameModel.NextGeneration();
        Assert.That(_gameModel.LivingCells[10, 10], Is.False);
    }

    [Test]
    public void SquareSurvives()
    {
        _gameModel.SetCellState(10, 10, true);
        _gameModel.SetCellState(10, 11, true);
        _gameModel.SetCellState(11, 10, true);
        _gameModel.SetCellState(11, 11, true);
        for (int i = 0; i < 10; i++)
        {
            _gameModel.NextGeneration();
            Assert.That(_gameModel.LivingCells[10, 10], Is.True);
            Assert.That(_gameModel.LivingCells[10, 11], Is.True);
            Assert.That(_gameModel.LivingCells[11, 10], Is.True);
            Assert.That(_gameModel.LivingCells[11, 11], Is.True);
        }
    }

    [Test]
    public void GliderGlides()
    {
        _gameModel.SetCellState(10, 90, true);
        _gameModel.SetCellState(11, 90, true);
        _gameModel.SetCellState(12, 90, true);
        _gameModel.SetCellState(12, 91, true);
        _gameModel.SetCellState(11, 92, true);
        
        for (int i = 0; i < 20; i++)
        {
            _gameModel.NextGeneration();
        }
        
        Assert.That(_gameModel.LivingCells[10, 90], Is.False);
        Assert.That(_gameModel.LivingCells[11, 90], Is.False);
        Assert.That(_gameModel.LivingCells[12, 90], Is.False);
        Assert.That(_gameModel.LivingCells[12, 91], Is.False);
        Assert.That(_gameModel.LivingCells[11, 92], Is.False);
        
        Assert.That(_gameModel.LivingCells[15, 85], Is.True);
        Assert.That(_gameModel.LivingCells[16, 85], Is.True);
        Assert.That(_gameModel.LivingCells[17, 85], Is.True);
        Assert.That(_gameModel.LivingCells[17, 86], Is.True);
        Assert.That(_gameModel.LivingCells[16, 87], Is.True);
    }

    [Test]
    public void SquareAcrossFieldEdgeSurvivesOrDiesDependingOnWrapAroundMode([Values] FieldWrapAroundMode wrapAroundMode)
    {
        _gameModel = new GameModel(new bool[100, 100], 0, wrapAroundMode);
        
        _gameModel.SetCellState(0, 10, true);
        _gameModel.SetCellState(0, 11, true);
        _gameModel.SetCellState(99, 10, true);
        _gameModel.SetCellState(99, 11, true);
        
        _gameModel.SetCellState(10, 0, true);
        _gameModel.SetCellState(11, 0, true);
        _gameModel.SetCellState(10, 99, true);
        _gameModel.SetCellState(11, 99, true);
        
        _gameModel.NextGeneration();

        bool shouldFistSquareSurvive = wrapAroundMode.HasFlag(FieldWrapAroundMode.Horizontal);
        Assert.That(_gameModel.LivingCells[0, 10], Is.EqualTo(shouldFistSquareSurvive));
        Assert.That(_gameModel.LivingCells[0, 11], Is.EqualTo(shouldFistSquareSurvive));
        Assert.That(_gameModel.LivingCells[99, 10], Is.EqualTo(shouldFistSquareSurvive));
        Assert.That(_gameModel.LivingCells[99, 11], Is.EqualTo(shouldFistSquareSurvive));
        
        bool shouldSecondSquareSurvive = wrapAroundMode.HasFlag(FieldWrapAroundMode.Vertical);
        Assert.That(_gameModel.LivingCells[10, 0], Is.EqualTo(shouldSecondSquareSurvive));
        Assert.That(_gameModel.LivingCells[11, 0], Is.EqualTo(shouldSecondSquareSurvive));
        Assert.That(_gameModel.LivingCells[10, 99], Is.EqualTo(shouldSecondSquareSurvive));
        Assert.That(_gameModel.LivingCells[11, 99], Is.EqualTo(shouldSecondSquareSurvive));
    }
}