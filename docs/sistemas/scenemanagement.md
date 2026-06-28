# SceneManagement

`SceneManagement` coordina el destino de portales y el spawneo de recursos de la escena cargada. Hereda de `PersistentSingleton<SceneManagement>`.

## Destino de portal

El sistema conserva el nombre del portal de destino:

```csharp
public string destPortalName { get; private set; }
public void SetDestPortalName(string name) => destPortalName = name;
```

Ese valor es usado por `PortalSpawn` para recolocar al jugador.

## Spawneo al cargar escena

`SceneManagement` se suscribe a `SceneManager.sceneLoaded`:

```csharp
SceneManager.sceneLoaded += OnSceneLoaded;
```

Cuando se carga una escena, obtiene los recursos asociados y los instancia si su estado lo permite:

```csharp
if (resource.state == SpawnOnceSO.SpawnOnceState.ToSpawn ||
    resource.state == SpawnOnceSO.SpawnOnceState.Spawned)
{
    GameObject gameObject = Instantiate(resource.itemPrefab, resource.spawnPosition, Quaternion.identity);
    gameObject.name = resource.itemID;
    resource.state = SpawnOnceSO.SpawnOnceState.Spawned;
}
```

## Responsabilidades

- Mantener `destPortalName`.
- Instanciar recursos persistentes al cargar una escena.
- Permitir spawneo manual de recursos por `resourceID`.
- Evitar suscripciones duplicadas al evento de carga.

## Relación con portales

`PortalExit` establece el destino antes de cargar escena. `PortalSpawn` lee ese destino al empezar y coloca a Lancelot en el punto adecuado.

[< volver](README.md)