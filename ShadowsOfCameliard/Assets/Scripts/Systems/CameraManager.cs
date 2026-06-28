using Unity.Cinemachine;
using UnityEngine;

// -----------------------------------------------------------------------------
// CameraManager
//
// Responsabilidades:
// - Gestiona la cámara del juego.
// - Reasigna el target de seguimiento de Cinemachine al jugador al cargar escena.
// -----------------------------------------------------------------------------
public class CameraManager : PersistentSingleton<CameraManager>
{
    // -----------------------------------------------------------------------------
    // SetPlayerCameraFollow
    //
    // - Localiza la CinemachineCamera activa en la escena y la apunta al jugador.
    // -----------------------------------------------------------------------------
    public void SetPlayerCameraFollow()
    {
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        cam.Follow = PlayerController.Instance.transform;
    }
}
