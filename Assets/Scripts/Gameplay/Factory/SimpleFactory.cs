using System;
using System.Collections.Generic;
using Gameplay.Objects;
using Gameplay.Objects.Boosts;
using Sound;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Gameplay.Factory
{
    /// <summary>
    /// A straightforward implementation of IWorldObjectFactory interface
    /// with simple pooling. Available objects can be set up in the Editor.
    /// </summary>
    public class SimpleFactory : MonoBehaviour, IWorldObjectFactory
    {
        [Serializable]
        class ObjectDesc
        {
            public string Key;
            public GameObject ViewPrototype;
        }

        [SerializeField] 
        private List<ObjectDesc> _knownObjects;

        private Dictionary<string, Stack<WorldObject>> _objPool = new Dictionary<string, Stack<WorldObject>>();

        private Dictionary<string, GameObject> _objectsDict;
        private ISoundManager _soundManager;

        private void Awake()
        {
            _objectsDict = new Dictionary<string, GameObject>(_knownObjects.Count);
            for (int i = 0; i < _knownObjects.Count; i++)
            {
                _objectsDict.Add(_knownObjects[i].Key, _knownObjects[i].ViewPrototype);
            }
        }

        public void Init(ISoundManager soundManager)
        {
            _soundManager = soundManager;
        }
        
        WorldObject IWorldObjectFactory.GetObjectForType(string objType)
        {
            return GetObjFromPoolOrCreate(objType);
        }

        void IWorldObjectFactory.ReleaseObject(WorldObject obj)
        {
            obj.Cleanup();
            PoolForType(obj.ObjType).Push(obj);
            obj.BasePosition = Vector3.One * -1000;
            obj.ForceSyncViewPos();
        }

        private WorldObject GetObjFromPoolOrCreate(string objType)
        {
            WorldObject result = null;

            Stack<WorldObject> typePool = PoolForType(objType);
            if (typePool.Count > 0)
            {
                result = typePool.Pop();
            }
            else
            {
                result = CreateNewWorldObjectForType(objType);
                bool isExist = _objectsDict.TryGetValue(objType, out GameObject viewPrototype);
                Debug.Assert(isExist == true, $"cannot find object with type - {objType}");
                IWorldObjectView view = Instantiate(viewPrototype).GetComponent<IWorldObjectView>();
                Debug.Assert(view != null);
                result.View = view;
            }

            return result;
        }

        private Stack<WorldObject> PoolForType(string objType)
        {
            bool isExist = _objPool.TryGetValue(objType, out Stack<WorldObject> typePool);
            if (isExist == false)
            {
                typePool = new Stack<WorldObject>(10);
                _objPool.Add(objType, typePool);
            }

            return typePool;
        }
        
        private WorldObject CreateNewWorldObjectForType(string objType)
        {
            WorldObject result = null;
            switch (objType)
            {
                case WorldObjectConstants.OBJ_TYPE_CHARACTER:
                    result = new CharacterObject(_soundManager);
                    break;
                case WorldObjectConstants.OBJ_TYPE_COIN:
                case WorldObjectConstants.OBJ_TYPE_OBSTACLE:
                case WorldObjectConstants.OBJ_TYPE_BOOST_FLY:    
                case WorldObjectConstants.OBJ_TYPE_BOOST_SPEED_UP:    
                case WorldObjectConstants.OBJ_TYPE_BOOST_SLOW_DOWN:
                    string collisionSound = CollisionSoundKeyForObjType(objType);
                    Func<IBoost> collisionEffect = OnCollisionEffectForObjType(objType);
                    bool isObstacle = objType == WorldObjectConstants.OBJ_TYPE_OBSTACLE;
                    int collisionScore = CollisionScoreForObjectType(objType);
                    result = new WorldObject(objType, collisionSound, collisionScore, collisionEffect, isObstacle);
                    break;
                default:
                    Debug.Assert(false, $"unhandled object type - {objType}");
                    break;
            }

            return result;
        }

        private string CollisionSoundKeyForObjType(string objType)
        {
            string result = null;
            switch (objType)
            {
                case WorldObjectConstants.OBJ_TYPE_COIN:
                    result = SoundConstants.SFX_COIN_COLLECT;
                    break;
                case WorldObjectConstants.OBJ_TYPE_BOOST_FLY:
                    result = SoundConstants.SFX_FLY;
                    break;
                case WorldObjectConstants.OBJ_TYPE_BOOST_SPEED_UP:
                    result = SoundConstants.SFX_SPEED_UP;
                    break;
                case WorldObjectConstants.OBJ_TYPE_BOOST_SLOW_DOWN:
                    result = SoundConstants.SFX_SLOW_DOWN;
                    break;
                case WorldObjectConstants.OBJ_TYPE_OBSTACLE:
                case WorldObjectConstants.OBJ_TYPE_CHARACTER:
                    // do nothing
                    break;
                default:
                    Debug.Assert(false, $"unhandled object type - {objType}");
                    break;
            }

            return result;
        }

        private Func<IBoost> OnCollisionEffectForObjType(string objType)
        {
            Func<IBoost> result = null;
            switch (objType)
            {
                case WorldObjectConstants.OBJ_TYPE_BOOST_FLY:
                    result = () => new BoostAddPosOffset(duration: WorldObjectConstants.BOOST_FLY_DURATION,
                                                            offset: new Vector3(0f, WorldObjectConstants.OBJ_VERTICAL_POS_AIR, 0f),
                                                            isHorizontalPosChangeAvailable: true); 
                    break;
                case WorldObjectConstants.OBJ_TYPE_BOOST_SPEED_UP:
                    result = () => new BoostChangeCharacterSpeed(duration: WorldObjectConstants.BOOST_SPEED_UP_DURATION,
                                                                    speedChange: WorldObjectConstants.BOOST_SPEED_UP_CHANGE);
                    break;
                case WorldObjectConstants.OBJ_TYPE_BOOST_SLOW_DOWN:
                    result = () => new BoostChangeCharacterSpeed(duration: WorldObjectConstants.BOOST_SLOW_DOWN_DURATION,
                                                                    speedChange: WorldObjectConstants.BOOST_SLOW_DOWN_CHANGE);
                    break;
                case WorldObjectConstants.OBJ_TYPE_COIN:
                case WorldObjectConstants.OBJ_TYPE_OBSTACLE:
                case WorldObjectConstants.OBJ_TYPE_CHARACTER:
                    // do nothing
                    break;
                default:
                    Debug.Assert(false, $"unhandled object type - {objType}");
                    break;
            }

            return result;
        }

        private int CollisionScoreForObjectType(string objType)
        {
            int result = 0;
            switch (objType)
            {
                case WorldObjectConstants.OBJ_TYPE_COIN:
                    result = 1;
                    break;
                case WorldObjectConstants.OBJ_TYPE_BOOST_FLY:
                case WorldObjectConstants.OBJ_TYPE_BOOST_SPEED_UP:
                case WorldObjectConstants.OBJ_TYPE_BOOST_SLOW_DOWN:
                    result = 10;
                    break;
                case WorldObjectConstants.OBJ_TYPE_OBSTACLE:
                case WorldObjectConstants.OBJ_TYPE_CHARACTER:
                    result = 0;
                    break;
                default:
                    Debug.Assert(false, $"unhandled object type - {objType}");
                    break;
            }

            return result;
        }
    }
}