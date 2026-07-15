using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------
// GameManager
//
// Responsabilidades:
// - Punto de entrada central del juego.
// - Inicia la reproducción de la música principal al arrancar.
// -----------------------------------------------------------------------------
public class GameManager : PersistentSingleton<GameManager>
{
    Coroutine fadeAndLoadCoroutine;

    // Capítulo desde el que se inicia el juego
    int startChapter = 0;
    public int StartChapter => startChapter;
    public void SetStartChapter(int value)
    {
        startChapter = value;
    }

    // primer capítulo desbloqueado
    int lastUnlockedChapter = 2;
    public int LastUnlockedChapter => lastUnlockedChapter;

    void Start()
    {
        // Recupera el último capítulo desbloqueado
        lastUnlockedChapter = SaveManager.Instance.LastUnlockedChapter;
    }

    public void SaveLastUnlockedChapter(int chapterIndex)
    {
        SaveManager.Instance.UnlockChapter(chapterIndex);
        lastUnlockedChapter = chapterIndex;
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void LoadSceneWithFade(string sceneName, float fadeDuration, bool reset=false)
    {
        ResumeTime(); // Aseguramos que el tiempo esté en marcha antes de cambiar de escena

        if (fadeAndLoadCoroutine != null)
        {
            StopCoroutine(fadeAndLoadCoroutine);
        }
        
        fadeAndLoadCoroutine = StartCoroutine(FadeOutAndLoadScene(sceneName, fadeDuration, reset));
    }

    // -----------------------------------------------------------------------------
    // FadeOutMusicAndLoadScene
    //
    // - Realiza un fade-out de la música de fondo durante fadeDuration segundos.
    // - Carga la escena después de completar el fade-out.
    // -----------------------------------------------------------------------------
    IEnumerator FadeOutAndLoadScene(string sceneName, float fadeDuration, bool reset=false)
    {
        // Hacemos un fade-out de la pantalla
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
        }

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

        // Cargamos la escena
        if (reset)
        {
            // Si reset es true, recargamos los managers del juego
            GameReset.ReloadSceneFromScratch(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }    

    public void QuitGame()
    {
        // Si estamos en el editor de Unity, detenemos la reproducción
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Si estamos en una compilación, cerramos la aplicación
        Application.Quit();
        #endif
    }
}
