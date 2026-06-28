# ResourcesManager

`ResourcesManager` mantiene el estado de los recursos persistentes del juego. Es esencial para evitar que armas, bloqueos u objetos reaparezcan duplicados al volver a una escena.

## Carga de recursos

Al iniciar, carga todos los `SpawnOnceSO` desde `Resources/SpawnOnce` y los clona en ejecución:

```csharp
SpawnOnceSO[] loadedResources = Resources.LoadAll<SpawnOnceSO>("SpawnOnce");

foreach (var resource in loadedResources)
{
    if (!resources.ContainsKey(resource.itemID))
    {
        resources.Add(resource.itemID, ScriptableObject.Instantiate(resource));
    }
}
```

La clonación evita modificar directamente los assets originales de Unity.

## Estado de recursos

Cada recurso se identifica por `itemID` y puede estar en estados como:

```text
NotSpawned
ToSpawn
Spawned
Collected
Destroyed
```

## Notificación de recogida

`ResourcesManager` observa `PlayerCollectSubject`. Cuando un item se recoge, marca el recurso como `Collected`:

```csharp
public void OnNotify(string itemID)
{
    GetResource(itemID).state = SpawnOnceSO.SpawnOnceState.Collected;
}
```

## Relación con SceneManagement

`SceneManagement` pregunta a `ResourcesManager` qué recursos pertenecen a la escena cargada y los instancia si están pendientes de aparición.

[< volver](README.md)