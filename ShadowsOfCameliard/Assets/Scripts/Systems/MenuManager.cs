using System.Collections.Generic;
using UnityEngine;
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

    [Header("Audio Settings References")]
    [SerializeField] Toggle musicToggle;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Toggle effectsToggle;
    [SerializeField] Slider effectsVolumeSlider;

    [Header("Chapter Buttons References")]
    [SerializeField] Button[] chapterButtons;

    Stack<GameObject> menuStack = new Stack<GameObject>();

    public void InitializeMenus(int lastUnlockedChapter=0)
    {
        HideAllMenus();

        PresetAudioSettings();

        PresetChapterButtons(lastUnlockedChapter);

        ShowMainMenu();
    }
    void HideAllMenus()
    {
        mainMenu.SetActive(false);
        inGameMenu.SetActive(false);
        chaptersMenu.SetActive(false);
        settingsMenu.SetActive(false);
        audioMenu.SetActive(false);
        controlsMenu.SetActive(false);
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

    void PresetChapterButtons(int lastUnlockedChapter)
    {
        // Habilita los botones de los capítulos desbloqueados y deshabilita los bloqueados

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
        // ToDO: Exit game
    }

    // -------------------------------------------------------------------------
    // InGame Menu Buttons
    // -------------------------------------------------------------------------
    public void InGameMenu_Continue_OnClick()
    {
        inGameMenu.SetActive(false);
        // ToDO: Continue game
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
        // ToDO: Exit game
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
}
