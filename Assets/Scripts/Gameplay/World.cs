using System;
using System.Collections.Generic;
using Gameplay.Factory;
using Gameplay.Input;
using Gameplay.Objects;
using Gameplay.UI;
using Misc;
using ProjectDebug;
using Sound;
using Vector3 = System.Numerics.Vector3;

namespace Gameplay
{
    /// <summary>
    /// Root gameplay class. Creates and destroys world objects, updates them,
    /// checks collisions, and handles user game score. Moves world objects
    /// instead of the main character to make sure that all positions will
    /// always be within permissible limits.
    ///
    /// Unity3d-agnostic.
    /// </summary>
    public class World : IInputTarget
    {
        private const float MIN_WORLD_OBJ_POS_Z = -2f;
        private const float CREATION_WORLD_OBJ_POS_Z = 20f;
        private const float CHARACTER_POSITION_Z = 0f;
        private const float CREATION_INTERVAL = 5f;
        
        private List<WorldObject> _worldObjects = new List<WorldObject>(16);
        private IWorldListener _listener;
        private IInput _input;
        private ISoundManager _soundManager;
        private float _elapsed = 0;
        private IWorldObjectFactory _worldObjectFactory;
        private CharacterObject _character;
        private bool _isNeedUpdate = false;

        private List<WorldObject> _objectsToDelete = new List<WorldObject>(16);
        private List<WorldObject> _collidedObjects = new List<WorldObject>(4);
        
        private Vector3[] _linePositions;

        private int _curCharacterPosIndex;

        private float _elapsedFromLastCreation = 0f;

        private SimpleWeightRandomizer<string> _objRandomizer;

        private bool _isGameEnded = false;

        private IGameHud _gameHud;

        private int _score;

        private Random _random;
        
        public World(IWorldListener listener, 
                        IInput input, 
                        ISoundManager soundManager, 
                        IWorldObjectFactory worldObjectFactory,
                        IGameHud gameHud, 
                        SimpleWeightRandomizer<string> objRandomizer)
        {
            _listener = listener;
            _input = input;
            _input.SetTarget(this);
            _soundManager = soundManager;
            _worldObjectFactory = worldObjectFactory;
            _gameHud = gameHud;
            
            _linePositions = new[] {
                new Vector3(-0.4f, 0f, 0f), 
                Vector3.Zero, 
                new Vector3(0.4f, 0f, 0f)
            };

            _objRandomizer = objRandomizer;
            
            _random = new Random();
        }

        public void Start()
        {
            CreateCharacter();

            _isGameEnded = false;
            _isNeedUpdate = false;
            _character.Greet(() =>
            {
                _isNeedUpdate = true;
                _input.Start();
                _elapsed = 0;
                _score = 0;
                UpdateGameHud();
                _gameHud.Show();
            });
        }
        
        
        public void Update(float dt)
        {
            if (_isNeedUpdate == false)
            {
                return;
            }
            
            _elapsed += dt;
            _elapsedFromLastCreation += dt;
            
            MoveWorldObjects(dt);
            UpdateObjects(dt);
            ProcessCollisions();
            DeleteObjectsIfNeeded();
            CreateObjectsIfNeeded();
            UpdateGameHud();
            
            if(_isGameEnded == true)
            {
                EndGame();                
            }
        }

        public void Cleanup()
        {
            DestroyCharacter();
            for (int i = 0; i < _worldObjects.Count; i++)
            {
                _worldObjectFactory.ReleaseObject(_worldObjects[i]);
            }
            _worldObjects.Clear();
        }
        
        void IInputTarget.ApplyCommand(InputCommand command)
        {
            switch (command)
            {
                case InputCommand.Jump:
                    _character.TryJumpUp();
                    break;
                case InputCommand.Left:
                case InputCommand.Right:
                        TryChangeCharacterPos(command);
                        break;
                default:
                    Debug.Assert(false, $"unhandled command - {command}");
                    break;
            }
        }

        private void EndGame()
        {
            _isNeedUpdate = false;
            _input.Stop();
            _listener.OnGameEnded();
            _gameHud.Hide();
        }

        private void CreateCharacter()
        {
            _curCharacterPosIndex = -1;
            _character = _worldObjectFactory.GetObjectForType(WorldObjectConstants.OBJ_TYPE_CHARACTER) as CharacterObject;
            Debug.Assert(_character != null);
            _character.BaseSpeed = 5f;
            
            ChangeCharacterPosIndex(1);
        }

        private void DestroyCharacter()
        {
            _worldObjectFactory.ReleaseObject(_character);
            _character = null;
        }
        
        private void TryChangeCharacterPos(InputCommand command)
        {
            if (_character.IsHorizontalPosChangeAvailable == false)
            {
                return;
            }
            
            int newIndex = _curCharacterPosIndex;
            switch (command)
            {
                case InputCommand.Left:
                    newIndex--;
                    break;
                case InputCommand.Right:
                    newIndex++;
                    break;
                default:
                    Debug.Assert(false, $"unsupported command - {command}");
                    break;
            }

            newIndex = Math.Clamp(newIndex, 0, _linePositions.Length -1);
            if (newIndex != _curCharacterPosIndex)
            {
                ChangeCharacterPosIndex(newIndex);
            }
        }

        private void ChangeCharacterPosIndex(int newIndex)
        {
            Debug.Assert(newIndex != _curCharacterPosIndex);
            _curCharacterPosIndex = newIndex;
            Vector3 pos = _linePositions[_curCharacterPosIndex];
            pos.Z = CHARACTER_POSITION_Z;
            _character.BasePosition = pos;
        }

        private void UpdateObjects(float dt)
        {
            _character.Update(dt);
            for (int i = 0; i < _worldObjects.Count; i++)
            {
                _worldObjects[i].Update(dt);
            }
        }

        private void ProcessCollisions()
        {
            for (int i = 0; i < _collidedObjects.Count; i++)
            {
                ProcessCollisionWithCharacter(_collidedObjects[i]);
            }
            _collidedObjects.Clear();
        }

        private void MoveWorldObjects(float dt)
        {
            Vector3 worldVelocity = new Vector3(0, 0, _character.Speed * -1);
            for (int i = 0; i < _worldObjects.Count; i++)
            {
                WorldObject objIt = _worldObjects[i];
                objIt.BasePosition += worldVelocity * dt;

                if (IsCollided(_character, objIt))
                {
                    _collidedObjects.Add(objIt);
                }
                else if (objIt.BasePosition.Z < MIN_WORLD_OBJ_POS_Z)
                {
                    _objectsToDelete.Add(objIt);
                }
            }
        }

        private void DeleteObjectsIfNeeded()
        {
            if (_objectsToDelete.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _objectsToDelete.Count; i++)
            {
                WorldObject objIt = _worldObjects[i];
                _worldObjects.Remove(objIt);
                _worldObjectFactory.ReleaseObject(objIt);
            }
            _objectsToDelete.Clear();
        }

        private void CreateObjectsIfNeeded()
        {
            if (_elapsedFromLastCreation > CREATION_INTERVAL / _character.Speed)
            {
                string objType = _objRandomizer.Next();
                WorldObject obj = _worldObjectFactory.GetObjectForType(objType);
                int posIndex = _random.Next(0, _linePositions.Length);
                Vector3 pos = _linePositions[posIndex];
                pos.Y = _random.Next(0, 2) == 0 ? 0f : WorldObjectConstants.OBJ_VERTICAL_POS_AIR;
                pos.Z = CREATION_WORLD_OBJ_POS_Z;
                obj.BasePosition = pos;
                
                _worldObjects.Add(obj);
                _elapsedFromLastCreation = 0f;
            }
        }

        private void ProcessCollisionWithCharacter(WorldObject obj)
        {
            if (obj.IsObstacle == true)
            {
                _isGameEnded = true;
            }
            else
            {
                _objectsToDelete.Add(obj);
                _score += obj.CollisionScore;
                _soundManager.TryPlayOneshot(obj.CollisionSound);
                if (obj.HasOnCollisionEffect)
                {
                    _character.ApplyBoost(obj.OnCollisionEffect());
                }
            }
        }
        
        private bool IsCollided(WorldObject first, WorldObject second)
        {
            float distSquared = (first.Position - second.Position).LengthSquared();
            float minCollisionDist = first.CollisionRadius + second.CollisionRadius;
            float collisionDistSquared = minCollisionDist * minCollisionDist;
            return distSquared <= collisionDistSquared;
        }

        private void UpdateGameHud()
        {
            _gameHud.SetCurScore(_score);
            _gameHud.SetCurSpeed(_character.Speed);
        }
    }
}