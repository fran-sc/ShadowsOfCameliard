using UnityEngine;

// -----------------------------------------------------------------------------
// PortalSpawn
//
// Responsabilidades:
// - Posiciona al jugador en este portal si coincide con el portal de destino.
// - Restaura controles, sprite, cámara y animaciones al llegar a la nueva escena.
// - Aplica fade in desde negro al completarse la transición.
//
// Notas:
// - Se activa en Start(), que se ejecuta justo después de cargar la escena.
// - Solo actúa si su nombre coincide con SceneManagement.destPortalName.
// -----------------------------------------------------------------------------
public class PortalSpawn : MonoBehaviour
{
    [SerializeField] string portalName;
    public string PortalName => portalName;

    void Start()
    {
        if (SceneManagement.Instance == null)
        {
            Debug.LogError("No se ha encontrado una instancia de SceneManagement en la escena. Asegúrate de que haya un objeto SceneManagement en la escena.");
            return;
        }
        
        if (SceneManagement.Instance.destPortalName == portalName)
        {
            // Establecemos la posición del jugador
            PlayerController.Instance.transform.position = transform.position;

            // Habilitamos los controles del jugador
            InputManager.Instance.Controls.Player.Enable();

            // Habilitamos el sprite del jugador
            PlayerController.Instance.GetComponentInChildren<SpriteRenderer>().enabled = true;

            // Establecemos la cámara para que siga al jugador
            CameraManager.Instance.SetPlayerCameraFollow();

            // Reseteamos las animaciones del jugador
            PlayerController.Instance.ResetTriggerAnimations();

            // Fade in desde negro
            UIFade.Instance.FadeFromBlack();
        }
    }   
}
