using UnityEngine;

namespace Gameplay.Input
{
    /// <summary>
    /// Common logic for both implemented inputs (keyboard and touch).
    /// </summary>
    public abstract class InputBase : MonoBehaviour, IInput, IInputDescription
    {
        private IInputTarget _target;
        private bool _isTargetSet = false;
        private bool _isActive = false;

        public bool IsActive => _isActive;

        protected abstract InputCommand TryGenerateCommand();
        protected abstract string GetLeftDescription { get; }
        protected abstract string GetRightDescription { get; }
        protected abstract string GetJumpDescription { get; }
        
        private void Update()
        {
            if (_isActive == false || _isTargetSet == false)
            {
                return;
            }
            
            InputCommand command = TryGenerateCommand(); 
            if (command != InputCommand.Undefined)
            {
                _target.ApplyCommand(command);
            }
        }

        void IInput.Start()
        {
            Debug.Assert(_isActive == false);
            _isActive = true;
        }

        void IInput.Stop()
        {
            Debug.Assert(_isActive == true);
            _isActive = false;
        }

        void IInput.SetTarget(IInputTarget target)
        {
            _target = target;
            _isTargetSet = (_target != null);
        }

        string IInputDescription.LeftDesc => GetLeftDescription;

        string IInputDescription.RightDesc => GetRightDescription;

        string IInputDescription.JumpDesc => GetJumpDescription;
    }
}