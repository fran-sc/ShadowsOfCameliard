using System.Collections.Generic;
using UnityEngine;

public class MenuManager : PersistentSingleton<MenuManager>
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject chaptersMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject controlsMenu;

    Stack<GameObject> menuStack = new Stack<GameObject>();

    void Start()
    {
        HideAllMenus();

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
        // ToDO: Start new game
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
        chaptersMenu.SetActive(false);
        // ToDO: Load Chapter 1
    }

    public void ChaptersMenu_Ch2_OnClick()
    {
        chaptersMenu.SetActive(false);
        // ToDO: Load Chapter 2
    }

    public void ChaptersMenu_Ch3_OnClick()
    {
        chaptersMenu.SetActive(false);
        // ToDO: Load Chapter 3
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
    public void AudioMenu_Back_OnClick()
    {
        audioMenu.SetActive(false);
        menuStack.Pop().SetActive(true);
    }

    public void AudioMenu_MusicVolume_OnValueChanged(float value)
    {
        // ToDo: Adjust music volume
    }

    public void AudioMenu_MusicCheckbox_OnValueChanged(bool value)
    {
        // ToDo: music on/off
    }

    public void AudioMenu_EffectsVolume_OnValueChanged(float value)
    {
        // ToDo: Adjust effects volume
    }

    public void AudioMenu_EffectsCheckbox_OnValueChanged(bool value)
    {
        // ToDo: effects on/off
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
