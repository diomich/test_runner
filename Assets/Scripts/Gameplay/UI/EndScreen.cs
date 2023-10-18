using System;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    /// <summary>
    /// A simple end game screen inspired by the Dark Souls game.
    /// </summary>
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] 
        private Button _btnForceComplete;

        private Action _onComplete;

        private ISoundManager _soundManager;

        public void Init(ISoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        
        public void Show(Action onComplete)
        {
            _onComplete = onComplete;
            this.gameObject.SetActive(true);
            _soundManager.PlayOneshot(SoundConstants.SFX_DEATH);
        }

        private void Awake()
        {
            _btnForceComplete.onClick.AddListener(OnBtnClickForceComplete);
        }

        private void OnDestroy()
        {
            _btnForceComplete.onClick.RemoveListener(OnBtnClickForceComplete);
        }

        private void OnBtnClickForceComplete()
        {
            if (_onComplete != null)
            {
                _onComplete.Invoke();
                _onComplete = null;
                this.gameObject.SetActive(false);
            }
        }
    }
}
