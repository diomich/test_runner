using System.Numerics;

namespace Gameplay.Objects.Boosts
{
    /// <summary>
    /// Boost that adds some offset to world object position. Used for jumps and flying.
    ///
    /// Unity3d-agnostic.
    /// </summary>
    public class BoostAddPosOffset : BoostDurationBase
    {
        private Vector3 _positionOffset;
        
        public BoostAddPosOffset(float duration, Vector3 offset , bool isHorizontalPosChangeAvailable) : 
            base(duration, 
                    isVerticalPosChangeAvailable: false,
                    isHorizontalPosChangeAvailable: isHorizontalPosChangeAvailable)
        {
            _positionOffset = offset;
        }

        protected override void DoApply()
        {
            this.Target.ApplyPositionOffset(_positionOffset);
        }

        protected override void DoDiscard()
        {
            this.Target.ApplyPositionOffset(_positionOffset * -1);
        }
    }
}