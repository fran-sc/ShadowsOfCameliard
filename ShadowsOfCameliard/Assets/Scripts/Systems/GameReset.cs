using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------
// GameReset
//
// Responsabilidades:
// - Destruye todos los objetos DontDestroyOnLoad antes de cargar una escena nueva.
// - Permite reiniciar el juego completamente sin residuos de sesiones anteriores.
//
// Notas:
// - Usa un GameObject temporal para acceder a la escena DontDestroyOnLoad.
// - ReloadSceneFromScratch combina la destrucción de persistentes con la carga.
// -----------------------------------------------------------------------------
public static class GameReset
{
    // -----------------------------------------------------------------------------
    // ReloadSceneFromScratch
    //
    // - Elimina los objetos persistentes y carga la escena indicada desde cero.
    // -----------------------------------------------------------------------------
    public static void ReloadSceneFromScratch(string sceneName)
    {
        DestroyDontDestroyOnLoadObjects();

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }   

    // -----------------------------------------------------------------------------
    // DestroyDontDestroyOnLoadObjects
    //
    // - Localiza la escena DontDestroyOnLoad mediante un GameObject temporal.
    // - Destruye todos sus objetos raíz excepto el temporal auxiliar.
    // -----------------------------------------------------------------------------
    public static void DestroyDontDestroyOnLoadObjects()
    {
        GameObject temp = new GameObject("TempDDOLFinder");
        Object.DontDestroyOnLoad(temp);

        Scene dontDestroyScene = temp.scene;

        foreach (GameObject root in dontDestroyScene.GetRootGameObjects())
        {
            if (root != temp)
            {
                Object.Destroy(root);
            }
        }

        Object.Destroy(temp);
    }
}