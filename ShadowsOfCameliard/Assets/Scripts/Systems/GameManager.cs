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
    [Header("Codex Settings")]
    [SerializeField] int startingLeafIndex = 0;
    [SerializeField] int lastLeafIndex = 5;
    [SerializeField] string nextSceneName = "Countryside";

    public int StartingLeafIndex => startingLeafIndex;
    public void SetStartingLeafIndex(int index) 
    {
        startingLeafIndex = index;
    }

    public int LastLeafIndex => lastLeafIndex;
    public void SetLastLeafIndex(int index) 
    {
        lastLeafIndex = index;
    }

    public string NextSceneName => nextSceneName;
    public void SetNextSceneName(string sceneName) 
    {
        nextSceneName = sceneName;
    }
}
