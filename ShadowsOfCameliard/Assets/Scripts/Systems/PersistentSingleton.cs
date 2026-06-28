// -----------------------------------------------------------------------------
// PersistentSingleton<T>
//
// Responsabilidades:
// - Extiende SceneSingleton añadiendo persistencia entre escenas.
// - Desacopla el objeto de su jerarquía padre antes de llamar a DontDestroyOnLoad.
//
// Patrón:
// - Hereda de SceneSingleton<T>, que ya gestiona la unicidad de instancia. Ejemplo:
//       public class AudioManager : PersistentSingleton<AudioManager>
// -----------------------------------------------------------------------------
public class PersistentSingleton<T> : SceneSingleton<T> where T : PersistentSingleton<T>
{
    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }

        // Desacoplamos en la jerarquía para poder invocar DontDestroyOnLoad
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
    }
}