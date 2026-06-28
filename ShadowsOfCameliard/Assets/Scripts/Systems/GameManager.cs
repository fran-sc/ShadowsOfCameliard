using UnityEngine;

// -----------------------------------------------------------------------------
// GameManager
//
// Responsabilidades:
// - Punto de entrada central del juego.
// - Inicia la reproducción de la música principal al arrancar.
// -----------------------------------------------------------------------------
public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] AudioClip mainTheme;

    void Start()
    {
        AudioManager.Instance.PlayMusic(mainTheme);
    }


}
