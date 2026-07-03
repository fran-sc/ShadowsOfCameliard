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
    }
}
