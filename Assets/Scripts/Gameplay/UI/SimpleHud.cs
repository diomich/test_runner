using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    /// <summary>
    /// A simple implementation of the IGameHud interface. Stores
    /// previously visualized values to prevent unnecessary string
    /// allocations and recreating text meshes.
    /// </summary>
    public class SimpleHud : MonoBehaviour, IGameHud
    {
        [SerializeField] 
        private TMP_Text _scoreCountText;
        [SerializeField] 
        private TMP_Text _curSpeedText;

        [SerializeField] 
        private float _minSpeedChangeToUpdateLabel = 0.1f;
        
        private int _lastAppliedScore = -1;
        private float _lastAppliedSpeed = -1f;
        
        void IGameHud.Show()
        {
            this.gameObject.SetActive(true);
        }

        void IGameHud.Hide()
        {
            this.gameObject.SetActive(false);
        }

        void IGameHud.SetCurScore(int score)
        {
            if (score != _lastAppliedScore)
            {
                _lastAppliedScore = score;
                _scoreCountText.text = score.ToString();
            }
        }

        void IGameHud.SetCurSpeed(float speed)
        {
            if (Mathf.Abs(speed - _lastAppliedSpeed) > _minSpeedChangeToUpdateLabel)
            {
                _lastAppliedSpeed = speed;
                _curSpeedText.text = speed.ToString("F1");
            }
        }
    }
}