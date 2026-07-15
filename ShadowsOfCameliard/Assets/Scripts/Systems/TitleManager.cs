using UnityEngine;

public class TitleManager : MonoBehaviour
{  
    void Awake()
    {
        // Nos registramos al evento de carga de escena
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Hacemos un fade-in
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeFromBlack();
        }

        // Reproducimos la música del título
        AudioManager.Instance.PlayMusic(AudioManager.Music.TitleTheme);

        // Mostramos el menú principal si estamos en la escena del título
        if (scene.name == "MainTitle")
        {
            MenuManager.Instance.InitializeMenus();
        }
    }

    public void StartGameFromChapter(int chapterIndex)
    {
        // Guardamos el capítulo desde el que se inicia el juego
        GameManager.Instance.SetStartChapter(chapterIndex);

        // Abrimos la escena del Codex
        GameManager.Instance.LoadSceneWithFade("Codex", UIFade.Instance.FadeDuration);
    }
}
