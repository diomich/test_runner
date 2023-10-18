using System;
using Gameplay.Objects.Boosts;
using ProjectDebug;
using Sound;
using Vector3 = System.Numerics.Vector3;

namespace Gameplay.Objects
{
    /// <summary>
    /// Slightly extended WorldObject to cover character-specific functionality.
    ///
    /// Unity3d-agnostic.
    /// </summary>
    public class CharacterObject : WorldObject
    {
        private const float DURATION_JUMP = 0.7f;
        
        private ICharacterView _view;
        private ISoundManager _soundManager;
        private float _elapsedFromJump = 0f;
        private bool _isJumping = false;

        private IBoost _simpleJumpBoost;

        public CharacterObject(ISoundManager soundManager, float collidionRadius = 0.1f)
            : base(WorldObjectConstants.OBJ_TYPE_CHARACTER, 
                    collisionSound: "",
                    collisionScore: 0,
                    onCollisionEffect: null,
                    isObstacle: false,
                    collidionRadius)
        {
            _soundManager = soundManager;
            Vector3 jumpOffset = new Vector3(0f, WorldObjectConstants.OBJ_VERTICAL_POS_AIR, 0f);

            _simpleJumpBoost = new BoostAddPosOffset(DURATION_JUMP, 
                                                        jumpOffset,
                                                        isHorizontalPosChangeAvailable: false);
        }
        
        public void Greet(Action onComplete)
        {
            Debug.Assert(_view != null);
            _view.Greet(_soundManager, onComplete);
        }

        public void TryJumpUp()
        {
            if (this.IsVerticalPosChangeAvailable == true)
            {
                ApplyBoost(_simpleJumpBoost);
                _soundManager.PlayOneshot(SoundConstants.SFX_CHARACTER_JUMP);
            }
        }

        protected override void DoUpdate(float dt)
        {
            _view.SetIsGrounded(this.Position.Y < float.Epsilon);
        }

        protected override void OnViewSet(IWorldObjectView view)
        {
            _view = view as ICharacterView;
            Debug.Assert(_view != null);
            _view.SetIsMoving(true);
        }
    }
}