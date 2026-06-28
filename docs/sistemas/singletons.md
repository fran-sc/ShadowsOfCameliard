# PersistentSingleton y SceneSingleton

El proyecto usa dos bases de singleton para diferenciar objetos persistentes y objetos propios de escena.

## PersistentSingleton

`PersistentSingleton<T>` conserva una unica instancia entre escenas mediante `DontDestroyOnLoad`. Es adecuado para managers globales como input, audio, inventario, recursos y escena.

```csharp
// -----------------------------------------------------------------------------
// Singleton persistente entre escenas para componentes MonoBehaviour
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
```

## SceneSingleton

`SceneSingleton<T>` mantiene una unica instancia dentro de la escena actual, pero no esta pensado para persistir indefinidamente.

```csharp
using UnityEngine;

// -----------------------------------------------------------------------------
// Singleton genérico para componentes MonoBehaviour ligado a la vida de la escena.
// El genérico T cumple el patrón CRTP (Curiously Recurring Template Pattern)
// para forzar la instancia (T) al tipo del nuevo componente.
// Ejemplo:
//      EnemySpawner : SceneSingleton<EnemySpawner>
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
```

Implementa el patrón CRTP (_Curiously Recurring Template Pattern_) para que el tipo de la instancia sea el del nuevo componente. De esta forma, declaramos clases como en el siguiente ejemplo:

```csharp
PlayerCOntroller : Singleton<PlayerController>
```

## Uso recomendado

- Usar `PersistentSingleton<T>` para sistemas que deben sobrevivir a cambios de escena.
- Usar `SceneSingleton<T>` para sistemas ligados al contenido cargado.
- En `Awake`, comprobar `if (Instance != this) return;` antes de inicializar suscripciones o recursos propios.

[< volver](README.md)