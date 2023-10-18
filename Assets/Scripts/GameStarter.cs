using Gameplay;
using UnityEngine;

/// <summary>
/// Gameplay starter
/// </summary>
public class GameStarter : MonoBehaviour
{
    private GameRoot _root;

    public void StartGame(GameRoot root)
    {
        _root = root;
        _root.Restart();
    }
    
    private void OnDestroy()
    {
        _root?.Cleanup();
    }
}
