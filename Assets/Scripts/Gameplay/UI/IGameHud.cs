namespace Gameplay.UI
{
    /// <summary>
    /// Interface for game hud to be used in the game world.
    /// </summary>
    public interface IGameHud
    {
        void Show();
        void Hide();
        void SetCurScore(int score);
        void SetCurSpeed(float speed);
    }
}