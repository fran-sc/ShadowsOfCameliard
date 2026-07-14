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
        Initialize();
    }

    public void Initialize()
    {
        // Recupera el último capítulo desbloqueado
        lastUnlockedChapter = SaveManager.Instance.LastUnlockedChapter;

        // Inicializa el sistema de menús
        MenuManager.Instance.InitializeMenus(lastUnlockedChapter);   
    }

    public void SaveLastUnlockedChapter(int chapterIndex)
    {
        SaveManager.Instance.UnlockChapter(chapterIndex);
        lastUnlockedChapter = chapterIndex;
    }
}
