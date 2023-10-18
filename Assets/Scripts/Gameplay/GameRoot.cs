using Gameplay.Factory;
using Gameplay.Input;
using Gameplay.UI;
using Misc;
using Sound;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Gameplay
{
    /// <summary>
    /// Root gameplay object. Starts the game, and also is responsible for
    /// showing the start game screen and endgame screen. Implements custom
    /// update method to be able to get updates without using Monobehaviours
    /// for this.
    /// </summary>
    public class GameRoot : IWorldListener
    {
        private ISoundManager _soundManager;

        private World _world;

        private StartScreen _startScreen;
        private EndScreen _endScreen;
        
        private bool _isGameActive = false;
        
        public GameRoot(IInput input,
                        ISoundManager soundManager,
                        StartScreen startScreen,
                        EndScreen endScreen, 
                        IGameHud gameHud,
                        IWorldObjectFactory worldObjectFactory)
        {
            _soundManager = soundManager;

            _startScreen = startScreen;
            _endScreen = endScreen;

            SimpleWeightRandomizer<string> objRandomizer = new SimpleWeightRandomizer<string>(10);
            objRandomizer.Add(90, WorldObjectConstants.OBJ_TYPE_COIN);
            objRandomizer.Add(30, WorldObjectConstants.OBJ_TYPE_OBSTACLE);
            objRandomizer.Add(10, WorldObjectConstants.OBJ_TYPE_BOOST_SPEED_UP);
            objRandomizer.Add(10, WorldObjectConstants.OBJ_TYPE_BOOST_FLY);
            objRandomizer.Add(10, WorldObjectConstants.OBJ_TYPE_BOOST_SLOW_DOWN);

            _world = new World(listener: this, 
                                input, 
                                soundManager,
                                worldObjectFactory,
                                gameHud,
                                objRandomizer);
            
            RegisterCustomUpdate();
        }

        public void Restart()
        {
            _startScreen.Show(OnNeedStartGame);
        }

        public void Cleanup()
        {
            DeregisterCustomUpdate();
        }

        private void MyCustomUpdate()
        {
            if (_isGameActive == false)
            {
                return;
            }
            _world.Update(Time.deltaTime);
        }

        private void OnNeedStartGame()
        {
            _isGameActive = true;
            _soundManager.PlayBackground(SoundConstants.BACKGROUND_DEFAULT);
            _world.Start();
        }

        private void RegisterCustomUpdate()
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < loop.subSystemList.Length; ++i)
            {
                if (loop.subSystemList[i].type == typeof(Update))
                {
                    loop.subSystemList[i].updateDelegate += MyCustomUpdate;
                }
            }

            PlayerLoop.SetPlayerLoop(loop);
        }

        private void DeregisterCustomUpdate()
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < loop.subSystemList.Length; ++i)
            {
                if (loop.subSystemList[i].type == typeof(Update))
                {
                    loop.subSystemList[i].updateDelegate -= MyCustomUpdate;
                }
            }

            PlayerLoop.SetPlayerLoop(loop);
        }

        void IWorldListener.OnGameEnded()
        {
            _soundManager.StopBackground();
            _isGameActive = false;
            _endScreen.Show(Restart);
            _world.Cleanup();
        }
    }
}