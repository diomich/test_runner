namespace Gameplay.Objects.Boosts
{
    /// <summary>
    /// Changing speed boost implementation.
    ///
    /// Unity3d-agnostic.
    /// </summary>
    public class BoostChangeCharacterSpeed : BoostDurationBase
    {
        private float _speedChange;

        public BoostChangeCharacterSpeed(float duration, float speedChange) 
            : base(duration, 
                    isVerticalPosChangeAvailable: true,
                    isHorizontalPosChangeAvailable: true)
            
        {
            _speedChange = speedChange;
        }

        protected override void DoApply()
        {
            this.Target.ApplySpeedModifier(_speedChange);
        }

        protected override void DoDiscard()
        {
            this.Target.ApplySpeedModifier(_speedChange * -1);
        }
    }
}