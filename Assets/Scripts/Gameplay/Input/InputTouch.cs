using UnityEngine;

namespace Gameplay.Input
{
    /// <summary>
    /// Simple touch-based implementation of generation commands. Used in the Android and iOS build by default.
    /// </summary>
    public class InputTouch : InputBase
    {
        private const float MIN_SWIPE_LENGTH = 50f;
        private InputCommand _lastDetectedCommand;
        private Vector2 _startPos;
        private bool _isInProgress = false;
        private Vector2 _lastSavedPos;
        
        protected override InputCommand TryGenerateCommand()
        {
            InputCommand result = InputCommand.Undefined;

            bool isNeedBeginInput = _isInProgress == false && UnityEngine.Input.touchCount == 1;
            bool isInputInProgress = _isInProgress == true && UnityEngine.Input.touchCount == 1;
            bool isNeedEndInput = _isInProgress == true && UnityEngine.Input.touchCount == 0;
            
            if (isNeedBeginInput == true)
            {
                _startPos = UnityEngine.Input.GetTouch(0).position;
                _isInProgress = true;
            }
            else if (isInputInProgress == true)
            {
                _lastSavedPos = UnityEngine.Input.GetTouch(0).position;
            }
            else if (isNeedEndInput == true)
            {
                float xOffset = _lastSavedPos.x - _startPos.x;
                if (Mathf.Abs(xOffset) <= MIN_SWIPE_LENGTH)
                {
                    result = InputCommand.Jump;
                }
                else
                {
                    result = (xOffset < 0) ? InputCommand.Left : InputCommand.Right;
                }
                
                _isInProgress = false;
            }
            else
            {
                // do nothing
            }
            return result;
        }

        protected override string GetLeftDescription => "Swipe Left";
        protected override string GetRightDescription => "Swipe Right";
        protected override string GetJumpDescription => "Tap";
    }
}