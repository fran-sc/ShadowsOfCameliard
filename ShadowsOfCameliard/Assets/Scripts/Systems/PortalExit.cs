using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// -----------------------------------------------------------------------------
// PortalExit
//
// Responsabilidades:
// - Gestiona la transición de escena al entrar el jugador en un portal de salida.
// - Deshabilita controles y sprite del jugador durante la transición.
// - Registra el nombre del portal de destino en SceneManagement.
//
// Notas:
// - El fade out visual se lanza en paralelo con la espera antes de cargar la escena.
// - La corrutina LoadScene está definida como función local dentro del trigger.
// -----------------------------------------------------------------------------
public class PortalExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string destPortalName;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            // Deshabilita el control del jugador para evitar que se mueva durante la transición
            InputManager.Instance.Controls.Player.Disable();

            // Oculta el sprite del jugador
            PlayerController.Instance.GetComponentInChildren<SpriteRenderer>().enabled = false;

            // Carga la nueva escena
            StartCoroutine(LoadScene());

            // Establece el nombre del portal de destino en SceneManagement para que el jugador 
            // aparezca en el lugar correcto al cargar la nueva escena
            SceneManagement.Instance.SetDestPortalName(destPortalName); 

            // Fade Out de la pantalla
            UIFade.Instance.FadeToBlack();
        }

        IEnumerator LoadScene()
        {
            // Espera a que la animación de fade termine antes de cargar la nueva escena
            yield return new WaitForSeconds(UIFade.Instance.FadeDuration);

            // Carga la nueva escena
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
