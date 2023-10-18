using UnityEngine;

namespace Gameplay.Input
{
    /// <summary>
    /// Implementation of generation commands based on keys pressed. Used in the Editor by default.
    /// </summary>
    public class InputKeyboard : InputBase
    {
        protected override InputCommand TryGenerateCommand()
        {
            InputCommand result = InputCommand.Undefined;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) 
                || UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)
                || UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                result = InputCommand.Jump;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.A) || UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
            {
                result = InputCommand.Left;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.D) || UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
            {
                result = InputCommand.Right;
            }

            return result;
        }

        protected override string GetLeftDescription => "Left Arrow, A";
        protected override string GetRightDescription => "Right Arrow, D";
        protected override string GetJumpDescription => "Up Arrow, W";
    }
}