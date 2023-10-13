namespace Avangardum.LifeArena.UnityClient.Interfaces
{
    public interface IWindowManager
    {
        public bool IsServerUnavailableWindowVisible { set; }
        public bool IsNoInternetConnectionWindowVisible { set; }
        bool IsHelpWindowVisible { set; }
    }
}