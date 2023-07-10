using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    //private static GameManager localInstance;
    //private static object lockObject = new object();

    //public static GameManager instance
    //{
    //    get
    //    {
    //        return localInstance;
    //    }
    //}

    public static GameManager instance;

    [Header("Scene names")]
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string gameSceneName;

    private Coroutine coroutine;

    public string GetMainMenuSceneName() => mainMenuSceneName;
    public string GetGameSceneName() => gameSceneName;

    // CURRENT STATE OF THE GAME
    private GameState gameState;
    public GameState GetGameState() => gameState;
    public void SetGameState(GameState gameState) => this.gameState = gameState;

    // PAUSE CONTROLLER
    private PauseController pauseController;
    public PauseController GetPauseController() => pauseController;

    // PLAYER INPUT
    private ControllerManager controllerManager;
    public ControllerManager GetControllerManager() => controllerManager;

    // CHECKPOINT MANAGER
    private CheckpointManager checkpointManager;
    public CheckpointManager GetCheckpointManager() => checkpointManager;

    // PLAYER RESETTER
    private PlayerResetter playerResetter;
    public PlayerResetter GetPlayerResetter() => playerResetter;

    // PARTICLE SYSTEM MANAGER
    private EnemyResetter enemyResetter;
    public EnemyResetter GetEnemyResetter() => enemyResetter;

    // PARTICLE SYSTEM MANAGER
    private ParticleSystemManager particleSystemManager;
    public ParticleSystemManager GetParticleSystemManager() => particleSystemManager;

    // AUDIO SETTINGS
    private AudioSettings audioSettings;
    public AudioSettings GetAudioSettings() => audioSettings;

    // AUDIO SETTINGS
    private GameFinisher gameFinisher;
    public GameFinisher GetGameFinisher() => gameFinisher;

    // TOTAL SCORE UI
    private TotalScoreUI totalScoreUI;
    public TotalScoreUI GetTotalScoreUI() => totalScoreUI;
    public void SetTotalScoreUI(TotalScoreUI totalScoreUI) => this.totalScoreUI = totalScoreUI;

    // BUBBLE MANAGER
    private BubbleManagerV1 bubbleManager;
    public BubbleManagerV1 GetBubbleManager() => bubbleManager;
    public void SetBubbleManager(BubbleManagerV1 bubbleManager) => this.bubbleManager = bubbleManager;

    // LIFE SYSTEM UI
    private LifeSystemUI lifeSystemUI;
    public LifeSystemUI GetLifeSystemUI() => lifeSystemUI;
    public void SetLifeSystemUI(LifeSystemUI lifeSystemUI) => this.lifeSystemUI = lifeSystemUI;

    // HUD MANAGER
    private HUDManager hudManager;
    public HUDManager GetHUDManager() => hudManager;
    public void SetHUDManager(HUDManager hudManager) => this.hudManager = hudManager;

    // BUBBLE DUMMY
    private GameObject bubbleDummy;
    public GameObject GetBubbleDummy() => bubbleDummy;
    public void SetBubbleDummy(GameObject bubbleDummy) => this.bubbleDummy = bubbleDummy;

    // PLAYER
    private GameObject player;
    public GameObject GetPlayer() => player;
    public void SetPlayer(GameObject player) => this.player = player;

    private void Awake()
    {
        //lock (lockObject)
        //{
        //    if (localInstance == null)
        //    {
        //        localInstance = this;
        //        DontDestroyOnLoad(gameObject);
        //    }
        //    else
        //        Destroy(gameObject);
        //}

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        gameState = GameState.Start;

        pauseController = GetComponent<PauseController>();
        controllerManager = GetComponent<ControllerManager>();
        checkpointManager = GetComponent<CheckpointManager>();
        playerResetter = GetComponent<PlayerResetter>();
        enemyResetter = GetComponent<EnemyResetter>();
        particleSystemManager = GetComponent<ParticleSystemManager>();
        audioSettings = GetComponent<AudioSettings>();
        gameFinisher = GetComponent<GameFinisher>();
    }

    void Start()
    {
        
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit();
        #endif
    }

    public void StartGame()
    {
        audioSettings.StopAllSounds();
        coroutine = StartCoroutine(LoadYourAsyncScene(gameSceneName));
    }

    public void MainMenu()
    {
        audioSettings.StopAllSounds();
        coroutine = StartCoroutine(LoadYourAsyncScene(mainMenuSceneName));
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
