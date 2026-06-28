using UnityEngine;

// -----------------------------------------------------------------------------
// SceneSingleton<T>
//
// Responsabilidades:
// - Singleton genérico ligado a la vida de la escena actual.
// - Solo mantiene una instancia activa; los duplicados se destruyen en Awake().
// - Limpia la referencia estática al destruirse para evitar referencias colgantes.
//
// Patrón:
// - Usa CRTP (Curiously Recurring Template Pattern) para tipar la instancia. Ejemplo:
//       public class EnemySpawner : SceneSingleton<EnemySpawner>
// -----------------------------------------------------------------------------
public class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{   
    private static T instance;
    public static T Instance => instance;
    
    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = (T)this;
        }
    }

    // Limpiamos la instancia al destruir el objeto para evitar referencias colgantes
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}