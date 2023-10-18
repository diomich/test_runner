using Gameplay;
using Gameplay.Factory;
using Gameplay.Input;
using Gameplay.UI;
using ProjectDebug;
using Reflex.Core;
using Sound;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Root project installer and entry point. Reflex(https://github.com/gustavopsantos/Reflex)
/// implementation of the DI container is used.
/// </summary>
public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        ISoundManager soundManager = InstallSound(descriptor);
        InputBase input = InstallInput(descriptor);
        InstallUI(descriptor, input, soundManager);
        InstallWorldObjectFactory(descriptor, soundManager);
        
        InitSettings();
        
        descriptor.OnContainerBuilt += StartGame;
    }

    private ISoundManager InstallSound(ContainerDescriptor descriptor)
    {
        GameObject goSound = Instantiate(Resources.Load("SoundManager") as GameObject);
        ISoundManager soundManager = goSound.GetComponent<ISoundManager>();
        Debug.Assert(soundManager != null, "cannot find sound manager instance on given object");
        DontDestroyOnLoad(goSound);

        descriptor.AddInstance(soundManager, typeof(ISoundManager));
        return soundManager;
    }
    
    private InputBase InstallInput(ContainerDescriptor descriptor)
    {
        InputBase input = null;
        GameObject goInput = new GameObject("InputHandler");
        DontDestroyOnLoad(goInput);
        
#if UNITY_EDITOR
        input = goInput.AddComponent<InputKeyboard>();
#elif UNITY_ANDROID || UNITY_IOS
        input = goInput.AddComponent<InputTouch>();
#else
        Debug.Assert(false, "there is no input defined for current platform");
#endif
        descriptor.AddInstance(input, typeof(IInput));
        return input;
    }

    private void InstallUI(ContainerDescriptor descriptor, InputBase input, ISoundManager soundManager)
    {
        GameObject goUi = Instantiate(Resources.Load("simple_ui") as GameObject);
        DontDestroyOnLoad(goUi);
        
        StartScreen startScreen = goUi.GetComponentInChildren<StartScreen>(includeInactive: true);
        Debug.Assert(startScreen != null);
        startScreen.Init(input);
        descriptor.AddInstance(startScreen);
        
        EndScreen endScreen = goUi.GetComponentInChildren<EndScreen>(includeInactive: true);
        Debug.Assert(endScreen != null);
        endScreen.Init(soundManager);
        descriptor.AddInstance(endScreen);
        
        IGameHud gameHud = goUi.GetComponentInChildren<IGameHud>(includeInactive: true);
        Debug.Assert(gameHud != null);
        descriptor.AddInstance(gameHud, typeof(IGameHud));
    }

    private void InstallWorldObjectFactory(ContainerDescriptor descriptor, ISoundManager soundManager)
    {
        GameObject go = Instantiate(Resources.Load("simple_world_objects_factory") as GameObject);
        DontDestroyOnLoad(go);
        SimpleFactory factory = go.GetComponent<SimpleFactory>();
        factory.Init(soundManager);
        Debug.Assert(factory != null);
        descriptor.AddInstance(factory, typeof(IWorldObjectFactory));
    }

    private void InitSettings()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ProjectDebug.Debug.SetLogger(new UnityLogger());
    }
    
    private void StartGame(Container container)
    {
        GameObject gameStarterGo = new GameObject("game_starter");
        DontDestroyOnLoad(gameStarterGo);
        GameStarter starter = gameStarterGo.AddComponent<GameStarter>();
        starter.StartGame(container.Construct<GameRoot>());
    }
}