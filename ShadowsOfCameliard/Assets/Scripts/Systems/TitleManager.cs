using System.Collections;
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
    }

    public void StartGameFromChapter(int chapterIndex)
    {
        // Guardamos el capítulo desde el que se inicia el juego
        GameManager.Instance.SetStartChapter(chapterIndex);

        // Hacemos un fade-out y luego cargamos la escena del juego desde el capítulo especificado
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
        }

        // fade-out de la música de fondo y carga de la escena del juego
        StartCoroutine(FadeOutMusicAndLoadNextScene(UIFade.Instance.FadeDuration));
    }

    IEnumerator FadeOutMusicAndLoadNextScene(float fadeDuration)
    {
        // fade-out de la música de fondo
        if (AudioManager.Instance != null)
        {
            float startVolume = AudioManager.Instance.MusicVolume;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                AudioManager.Instance.SetMusicVolume(Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration));
                yield return null;
            }

            AudioManager.Instance.StopMusic();

            // Restablecemos el volumen de la música para futuras reproducciones
            AudioManager.Instance.SetMusicVolume(startVolume);
        }

        // Cargamos la escena del codex
        UnityEngine.SceneManagement.SceneManager.LoadScene("Codex");
    }
}
