using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : PersistentSingleton<MenuManager>
{
    [Header("Menu References")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject chaptersMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject exitConfirmationMenu;

    [Header("Audio Settings References")]
    [SerializeField] Toggle musicToggle;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Toggle effectsToggle;
    [SerializeField] Slider effectsVolumeSlider;

    [Header("Chapter Buttons References")]
    [SerializeField] Button[] chapterButtons;

    Stack<GameObject> menuStack = new Stack<GameObject>();

    public bool IsMenuOpen => 
        mainMenu.activeSelf || 
        inGameMenu.activeSelf || 
        chaptersMenu.activeSelf || 
        settingsMenu.activeSelf || 
        audioMenu.activeSelf || 
        controlsMenu.activeSelf ||
        exitConfirmationMenu.activeSelf;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Gamepad.current?.startButton.wasPressedThisFrame == true)
        {
            // Obtenemos la escena actual
            string currentSceneName = SceneManager.GetActiveScene().name;

            // Si estamos en la escena del título o en la escena del codex, no hacemos nada
            if (currentSceneName == "MainTitle" || currentSceneName == "Codex")
            {
                return;
            }

            if (IsMenuOpen)
            {
                // Si hay un menú activo, lo ocultamos
                HideAllMenus();

                // Activamos el mapa de acciones del jugador para que pueda moverse nuevamente
                InputManager.Instance.SwitchMap(ControlMap.Player);

                // y continuamos el tiempo si estaba pausado
                GameManager.Instance.ResumeTime();
            }
            else
            {
                // Desactivamos el mapa de acciones del jugador para evitar que se mueva mientras el menú está abierto
                InputManager.Instance.SwitchMap(ControlMap.Menu);

                // Si no hay ningún menú activo, mostramos el menú en juego
                ShowInGameMenu();

                // y pausamos el tiempo
                GameManager.Instance.StopTime();
            }
        }
    }

    public void InitializeMenus()
    {
        HideAllMenus();

        PresetAudioSettings();

        PresetChapterButtons();

        ShowMainMenu();
    }

    public void HideAllMenus()
    {
        mainMenu.SetActive(false);
        inGameMenu.SetActive(false);
        chaptersMenu.SetActive(false);
        settingsMenu.SetActive(false);
        audioMenu.SetActive(false);
        controlsMenu.SetActive(false);
        exitConfirmationMenu.SetActive(false);
    }

    void PresetAudioSettings()
    {
        // Preset music settings
        musicToggle.isOn = AudioManager.Instance.MusicOn;
        musicVolumeSlider.value = AudioManager.Instance.MusicVolume;

        // Preset effects settings
        effectsToggle.isOn = AudioManager.Instance.EffectsOn;
        effectsVolumeSlider.value = AudioManager.Instance.EffectsVolume;
    }

    void PresetChapterButtons()
    {
        // Habilita los botones de los capítulos desbloqueados y deshabilita los bloqueados

        int lastUnlockedChapter = GameManager.Instance.LastUnlockedChapter;

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            // +2 porque el primer botón corresponde al capítulo 2 (índice 0)
            chapterButtons[i].interactable = (i + 2 <= lastUnlockedChapter);
        }   
    }

    void StartChapter(int chapterIndex)
    {
        // Obtenemos una referencia al TitleManager
        TitleManager titleManager = FindFirstObjectByType<TitleManager>();

        if (titleManager != null)
        {
            HideAllMenus(); // Ocultamos todos los menús antes de iniciar el juego

            // Iniciamos el juego desde el capítulo seleccionado
            titleManager.StartGameFromChapter(chapterIndex);
        }
        else
        {
            Debug.LogError("No se encontró una instancia de TitleManager en la escena.");
        }
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void ShowInGameMenu()
    {
        inGameMenu.SetActive(true);
    }

    // -------------------------------------------------------------------------
    // Main Menu Buttons
    // -------------------------------------------------------------------------
    public void MainMenu_NewGame_OnClick()
    {
        StartChapter(0);
    }

    public void MainMenu_Chapters_OnClick()
    {
        mainMenu.SetActive(false);
        chaptersMenu.SetActive(true);
    }

    public void MainMenu_Settings_OnClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        menuStack.Push(mainMenu);
    }

    public void MainMenu_Exit_OnClick()
    {
        GameManager.Instance.QuitGame();
    }

    // -------------------------------------------------------------------------
    // InGame Menu Buttons
    // -------------------------------------------------------------------------
    public void InGameMenu_Continue_OnClick()
    {
        inGameMenu.SetActive(false);
        
        // Activamos el mapa de acciones del jugador para que pueda moverse nuevamente
        InputManager.Instance.SwitchMap(ControlMap.Player);

        // Reanudamos el tiempo
        GameManager.Instance.ResumeTime();
    }

    public void InGameMenu_Settings_OnClick()
    {
        inGameMenu.SetActive(false);
        settingsMenu.SetActive(true);
        menuStack.Push(inGameMenu);
    }

    public void InGameMenu_Exit_OnClick()
    {
        inGameMenu.SetActive(false);
        mainMenu.SetActive(false);
        exitConfirmationMenu.SetActive(true);
    }

    // -------------------------------------------------------------------------
    // Chapters Menu Buttons
    // -------------------------------------------------------------------------

    public void ChaptersMenu_Back_OnClick()
    {
        chaptersMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ChaptersMenu_Ch1_OnClick()
    {
        StartChapter(2);
    }

    public void ChaptersMenu_Ch2_OnClick()
    {
        StartChapter(3);        
    }

    public void ChaptersMenu_Ch3_OnClick()
    {
        StartChapter(4);
    }

    // -------------------------------------------------------------------------
    // Settings Menu Buttons
    // -------------------------------------------------------------------------
    public void SettingsMenu_Back_OnClick()
    {
        settingsMenu.SetActive(false);
        menuStack.Pop().SetActive(true);
    }

    public void SettingsMenu_Audio_OnClick()
    {
        settingsMenu.SetActive(false);
        audioMenu.SetActive(true);
        menuStack.Push(settingsMenu);
    }

    public void SettingsMenu_Controls_OnClick()
    {
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        menuStack.Push(settingsMenu);
    }

    // -------------------------------------------------------------------------
    // Audio Menu Buttons
    // -------------------------------------------------------------------------
    public void AudioMenu_MusicVolume_OnValueChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void AudioMenu_MusicCheckbox_OnValueChanged(bool value)
    {
        AudioManager.Instance.ToggleMusic(value);
    }

    public void AudioMenu_EffectsVolume_OnValueChanged(float value)
    {
        AudioManager.Instance.SetEffectsVolume(value);
    }

    public void AudioMenu_EffectsCheckbox_OnValueChanged(bool value)
    {
        AudioManager.Instance.ToggleEffects(value);
    }

    public void AudioMenu_Back_OnClick()
    {
        audioMenu.SetActive(false);
        menuStack.Pop().SetActive(true);
    }

    // -------------------------------------------------------------------------
    // Controls Menu Buttons
    // -------------------------------------------------------------------------
    public void ControlsMenu_Back_OnClick()
    {
        controlsMenu.SetActive(false);
        menuStack.Pop().SetActive(true);    
    }

    // -------------------------------------------------------------------------
    // Exit Confirmation Menu Buttons
    // -------------------------------------------------------------------------
    public void ExitConfirmationMenu_Yes_OnClick()
    {
        exitConfirmationMenu.SetActive(false);

        GameManager.Instance.LoadSceneWithFade(
            "MainTitle", 
            UIFade.Instance.FadeDuration, 
            true);
    }

    public void ExitConfirmationMenu_No_OnClick()
    {
        exitConfirmationMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }
}

