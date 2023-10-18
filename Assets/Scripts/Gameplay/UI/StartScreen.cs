using System;
using Gameplay.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    /// <summary>
    /// A simple start screen with info labels about controls. Shows
    /// description for the current input module used.
    /// </summary>
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _lblLeft;
        [SerializeField] 
        private TMP_Text _lblRight;
        [SerializeField] 
        private TMP_Text _lblJump;
        [SerializeField] 
        private Button _btnStart;

        private Action _onBtnClickStart;

        public void Init(IInputDescription description)
        {
            _lblLeft.text = description.LeftDesc;
            _lblRight.text = description.RightDesc;
            _lblJump.text = description.JumpDesc;
        }
        
        public void Show(Action onBtnClickStart)
        {
            this.gameObject.SetActive(true);
            _onBtnClickStart = onBtnClickStart;
        }

        private void Awake()
        {
            _btnStart.onClick.AddListener(OnBtnClickStart);
        }

        private void OnDestroy()
        {
            _btnStart.onClick.RemoveListener(OnBtnClickStart);
        }

        private void OnBtnClickStart()
        {
            if (_onBtnClickStart != null)
            {
                _onBtnClickStart.Invoke();
                _onBtnClickStart = null;
                this.gameObject.SetActive(false);
            }
        }
    }
}
