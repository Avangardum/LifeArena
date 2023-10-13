using System.Threading.Tasks;
using Avangardum.LifeArena.UnityClient.Data;

namespace Avangardum.LifeArena.UnityClient.Interfaces
{
    public interface IServerFacade
    {
        Task<GameState> GetGameState();
        Task<GameState> AddCell(int x, int y);
    }
}