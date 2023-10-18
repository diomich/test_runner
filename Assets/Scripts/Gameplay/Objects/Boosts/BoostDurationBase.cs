using ProjectDebug;

namespace Gameplay.Objects.Boosts
{
    /// <summary>
    /// Common functionality for every boost with duration.
    ///
    /// Unity3d-agnostic.
    /// </summary>
    public abstract class BoostDurationBase : IBoost
    {
        private float _elapsed;
        private float _duration;
        private bool _isVerticalPosChangeAvailable;
        private bool _isHorizontalPosChangeAvailable;
        private IBoostTarget _target;

        protected BoostDurationBase(float duration, bool isVerticalPosChangeAvailable, bool isHorizontalPosChangeAvailable)
        {
            _duration = duration;
            _isVerticalPosChangeAvailable = isVerticalPosChangeAvailable;
            _isHorizontalPosChangeAvailable = isHorizontalPosChangeAvailable;
        }

        protected IBoostTarget Target => _target;
        
        void IBoost.Apply(IBoostTarget target)
        {
            Debug.Assert(_target == null);
            _target = target;
            _elapsed = 0f;
            DoApply();
        }

        void IBoost.Update(float dt)
        {
            _elapsed += dt;
            if ((this as IBoost).IsEnded == true)
            {
                (this as IBoost).Discard();
            }
            DoUpdate(dt);
        }

        void IBoost.Discard()
        {
            DoDiscard();
            _target = null;
        }

        bool IBoost.IsEnded => (_elapsed >= _duration);

        bool IBoost.IsVerticalPosChangeAvailable => _isVerticalPosChangeAvailable;
        bool IBoost.IsHorizontalPosChangeAvailable => _isHorizontalPosChangeAvailable;
        
        protected virtual void DoApply() { }
        protected virtual void DoUpdate(float dt) { }
        protected virtual void DoDiscard() { }
    }
}