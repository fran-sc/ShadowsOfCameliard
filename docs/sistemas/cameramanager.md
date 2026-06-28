# CameraManager

`CameraManager` reasigna el seguimiento de la camara Cinemachine al jugador cuando se carga una escena o se atraviesa un portal.

## Codigo

```csharp
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : PersistentSingleton<CameraManager>
{
    public void SetPlayerCameraFollow()
    {
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        cam.Follow = PlayerController.Instance.transform;
    }
}
```

## Funcion dentro del flujo de portales

Despues de que `PortalSpawn` coloca al jugador en el punto correcto, se llama a:

```csharp
CameraManager.Instance.SetPlayerCameraFollow();
```

Esto asegura que la camara vuelve a seguir a `PlayerController.Instance.transform` tras el cambio de escena.

[< volver](README.md)