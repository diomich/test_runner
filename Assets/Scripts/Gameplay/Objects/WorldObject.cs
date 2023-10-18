using System;
using System.Collections.Generic;
using Gameplay.Objects.Boosts;
using Vector3 = System.Numerics.Vector3;

namespace Gameplay.Objects
{
    /// <summary>
    /// Base class for any world object in the game. Can be configured for
    /// any current needs.
    ///
    /// Unity3d-agnostic. 
    /// </summary>
    public class WorldObject : IBoostTarget
    {
        private IWorldObjectView _view;
        private float _baseSpeed;
        private Vector3 _basePosition;
        private float _collisionRadius;
        private string _objType;

        private float _speedBonus = 0f;
        private Vector3 _posOffset = Vector3.Zero;

        private List<IBoost> _activeBoosts = new List<IBoost>();
        private List<IBoost> _boostToDel = new List<IBoost>();

        private string _collisionSound;

        private Func<IBoost> _onCollisionEffect;
        private bool _isObjstacle = false;
        private int _collisionScore;

        public int CollisionScore => _collisionScore;
        public string ObjType => _objType;
        public string CollisionSound => _collisionSound;

        public float BaseSpeed
        {
            get { return _baseSpeed; }
            set { _baseSpeed = value; }
        }

        public float Speed => Math.Max(_baseSpeed + _speedBonus, 0f);

        public bool IsObstacle => _isObjstacle;
        
        public Vector3 BasePosition
        {
            get { return _basePosition; }
            set
            {
                _basePosition = value;
                SyncViewPos();
            }
        }

        public Vector3 Position 
        {
            get
            {
                Vector3 result = _basePosition + _posOffset;
                result.Y = Math.Clamp(result.Y, 0f, WorldObjectConstants.OBJ_VERTICAL_POS_AIR);
                return result;
            }
        }

    public float CollisionRadius => _collisionRadius;
        
        public IWorldObjectView View
        {
            get { return _view; }
            set
            {
                _view = value; 
                OnViewSet(_view);
            }
        }

        public bool IsVerticalPosChangeAvailable
        {
            get
            {
                bool result = true;
                for (int i = 0; i < _activeBoosts.Count; i++)
                {
                    if (_activeBoosts[i].IsVerticalPosChangeAvailable == false)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }
        }

        public bool IsHorizontalPosChangeAvailable
        {
            get
            {
                bool result = true;
                for (int i = 0; i < _activeBoosts.Count; i++)
                {
                    if (_activeBoosts[i].IsHorizontalPosChangeAvailable == false)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }
        }

        public bool HasOnCollisionEffect => _onCollisionEffect != null;
        public Func<IBoost> OnCollisionEffect => _onCollisionEffect;

        
        public WorldObject(string objType, 
                            string collisionSound = null,
                            int collisionScore = 0,
                            Func<IBoost> onCollisionEffect = null,
                            bool isObstacle = false,
                            float collisionRadius = 0.1f)
        {
            _objType = objType;
            _collisionRadius = collisionRadius;
            _collisionSound = collisionSound;
            _onCollisionEffect = onCollisionEffect;
            _isObjstacle = isObstacle;
            _collisionScore = collisionScore;
        }
        
        public void Update(float dt)
        {
            SyncViewPos();
            UpdateActiveBoosts(dt);
            DoUpdate(dt);
        }

        public void ApplyBoost(IBoost boost)
        {
            boost.Apply(this);
            _activeBoosts.Add(boost);
        }
        
        public void ForceSyncViewPos()
        {
            SyncViewPos();            
        }

        public void Cleanup()
        {
            for (int i = 0; i < _activeBoosts.Count; i++)
            {
                _activeBoosts[i].Discard();
            }
            _activeBoosts.Clear();
        }
        
        protected virtual void DoUpdate(float dt) { }
        protected virtual void OnViewSet(IWorldObjectView view) { }

        private void SyncViewPos()
        {
            _view?.SetPosition(this.Position);
        }

        private void UpdateActiveBoosts(float dt)
        {
            for (int i = 0; i < _activeBoosts.Count; i++)
            {
                IBoost boostIt = _activeBoosts[i];
                boostIt.Update(dt);
                if (boostIt.IsEnded == true)
                {
                    _boostToDel.Add(boostIt);
                }
            }

            if (_boostToDel.Count > 0)
            {
                for (int i = 0; i < _boostToDel.Count; i++)
                {
                    _activeBoosts.Remove(_boostToDel[i]);
                }
                _boostToDel.Clear();
            }
        }
        
        void IBoostTarget.ApplySpeedModifier(float modifier)
        {
            _speedBonus += modifier;
        }

        void IBoostTarget.ApplyPositionOffset(Vector3 offset)
        {
            _posOffset += offset;
            SyncViewPos();
        }
    }
}