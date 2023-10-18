namespace Sound
{
    /// <summary>
    /// Simple interface used to play audio in the game.
    /// </summary>
    public interface ISoundManager
    {
        void TryPlayOneshot(string id);
        void PlayOneshot(string id);
        void PlayBackground(string id);
        void PauseBackground();
        void ResumeBackground();
        void StopBackground();
    }
}
