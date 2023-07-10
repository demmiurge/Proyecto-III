using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("HUD screens")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject blackFadeScreen;
    [SerializeField] private GameObject inGamePlayScreen;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject pauseMenuScreen;
    [SerializeField] private GameObject menuOptionsScreen;
    [SerializeField] private GameObject soundOptionsScreen;
    [SerializeField] private GameObject controlsOptionsScreen;
    [SerializeField] private GameObject graphicOptionsScreen;
    [SerializeField] private GameObject creditsScreen;

    [Header("HUD basics")] 
    [SerializeField] private string mainScene;

    [Header("InGamePlayScreenOptions")]
    [SerializeField] private GameObject crosshair;

    private bool inGame;

    private PauseController pauseController;
    private ControllerManager controllerManager;

    private bool changeDenied = false;

    private void DifferentiatorStart()
    {
        Scene scene = SceneManager.GetActiveScene();
        inGame = scene.name != mainScene;
    }

    void Awake()
    {
        // Tenemos que diferenciar en qué escena para mostrar un menú u otro
        DifferentiatorStart();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.SetHUDManager(this);

        CheckInstances();

        TurnOffAllInterfaces();

        // Mostraremos la UI del menú principal o la interfaz de juego dependiendo de la escena
        if (inGame)
            GoToInGamePlayScreen();
        else
            GoToMainMenuScreen();
    }

    public void CheckInstances()
    {
        if (pauseController == null && GameManager.instance.GetPauseController()) pauseController = GameManager.instance.GetPauseController();
        if (controllerManager == null && GameManager.instance.GetControllerManager()) controllerManager = GameManager.instance.GetControllerManager();
    }

    void Update()
    {
        CheckInstances();

        if (Input.GetKeyDown(KeyCode.L))
        {
            MainMenu();
        }
    }

    public void ActiveCrosshair()
    {
        crosshair.SetActive(true);
    }

    public void DisableCrosshair()
    {
        crosshair.SetActive(false);
    }

    public void TurnOffAllInterfaces()
    {
        deathScreen.SetActive(false);
        blackFadeScreen.SetActive(false);
        inGamePlayScreen.SetActive(false);
        mainMenuScreen.SetActive(false);
        pauseMenuScreen.SetActive(false);
        menuOptionsScreen.SetActive(false);
        soundOptionsScreen.SetActive(false);
        controlsOptionsScreen.SetActive(false);
        graphicOptionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void GoToDeathScreen()
    {
        // Escalamos el tiempo de todos los objetos a 0
        GameManager.instance.GetPauseController().PauseTime();

        TurnOffAllInterfaces();
        deathScreen.SetActive(true);
    }

    public void GoToBlackFadeScreen()
    {
        // TurnOffAllInterfaces();
        blackFadeScreen.SetActive(true);
    }

    public void GoToBlackFadeScreen(float timeFadeOut = 2.5f, float timeFadeIn = 2.5f)
    {
        TurnOffAllInterfaces();
        blackFadeScreen.SetActive(true);
    }

    public void GoToInGamePlayScreen()
    {
        // Escalamos el tiempo de todos los objetos a 0
        pauseController?.ResumeTime();

        // Activamos el puntero del ratón para la selección de los botones
        controllerManager?.DeactivateCursor();

        TurnOffAllInterfaces();
        inGamePlayScreen.SetActive(true);
    }

    public void GoToMainMenuScreen()
    {
        // Siempre que se muestra el menú principal se reanuda el tiempo por si no se contempla su procedencia
        // Escalamos el tiempo de todos los objetos a 0
        pauseController?.ResumeTime();
        // Reiniciamos la flag para que pueda volver al menú de pausa
        changeDenied = false;

        // Activamos el puntero del ratón para la selección de los botones
        controllerManager.ActiveCursor();

        TurnOffAllInterfaces();
        mainMenuScreen.SetActive(true);
    }

    public void GoToPauseMenuScreen()
    {
        // Por si está en los créditos.
        if (changeDenied) return;

        // Si se encontraba en el menú de pausa y lo que quiere es salir pulsando la misma tecla
        if (pauseMenuScreen.activeSelf)
        {
            GoToInGamePlayScreen();
        }
        else
        {
            // Escalamos el tiempo de todos los objetos a 0
            pauseController.PauseTime();

            // Activamos el puntero del ratón para la selección de los botones
            controllerManager.ActiveCursor();

            TurnOffAllInterfaces();
            pauseMenuScreen.SetActive(true);
        }
    }

    public void GoToMenuOptionsScreen()
    {
        TurnOffAllInterfaces();
        menuOptionsScreen.SetActive(true);
    }

    public void GoToSoundOptionsScreen()
    {
        TurnOffAllInterfaces();
        soundOptionsScreen.SetActive(true);
    }

    public void GoToControlsOptionsScreen()
    {
        TurnOffAllInterfaces();
        controlsOptionsScreen.SetActive(true);
    }

    public void GoToGraphicOptionsScreen()
    {
        TurnOffAllInterfaces();
        graphicOptionsScreen.SetActive(true);
    }

    public void GoToCreditsScreen()
    {
        changeDenied = true;

        // Escalamos el tiempo de todos los objetos a 0
        GameManager.instance.GetPauseController().PauseTime();

        TurnOffAllInterfaces();
        creditsScreen.SetActive(true);
    }

    public void ExitOptionsMenu()
    {
        if (inGame)
            GoToPauseMenuScreen();
        else
            GoToMainMenuScreen();
    }

    public void ExitGame()
    {
        GameManager.instance.ExitGame();
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void MainMenu()
    {
        GameManager.instance.MainMenu();
    }
}
