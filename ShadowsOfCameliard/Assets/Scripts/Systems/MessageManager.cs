using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// MessageManager
//
// Responsabilidades:
// - Muestra mensajes de texto en pantalla con efecto de máquina de escribir.
// - Pausa el juego mientras el mensaje está visible.
// - Cambia el mapa de controles a UI para cerrar el mensaje con acción dedicada.
//
// Atributos principales:
// - charDelay: retardo entre caracteres del efecto typewriter.
// - txtMessage: componente TextMeshPro donde se escribe el texto.
// - messagePanel: panel UI que contiene el mensaje.
//
// Notas:
// - Implementa IPlayerCollectObserver para mostrar el mensaje de items recogidos.
// - Time.timeScale se pone a 0 mientras el mensaje está activo.
// -----------------------------------------------------------------------------
public class MessageManager : PersistentSingleton<MessageManager>, IPlayerCollectObserver
{
    [SerializeField] float charDelay;
    [SerializeField] TextMeshProUGUI txtMessage;
    [SerializeField] GameObject messagePanel;

    PlayerControls controls;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }
    }

    void Start()
    {
        if (Instance != this)
        {
            return;
        }

        controls = InputManager.Instance.Controls;
        controls.UI.Close.performed += OnClosePerformed;

        PlayerCollectSubject.Instance.AddObserver(this);
    }

    protected override void OnDestroy()
    {
        if (Instance == this && controls != null)
        {
            controls.UI.Close.performed -= OnClosePerformed;
        }

        base.OnDestroy();
    }

    void OnClosePerformed(InputAction.CallbackContext context)
    {
        HideMessage();
    }

    // -----------------------------------------------------------------------------
    // ShowMessage
    //
    // - Pausa el juego, muestra el panel y lanza el efecto de escritura carácter a carácter.
    // - Cambia el mapa de controles a Dialogue para que el jugador pueda cerrar el mensaje.
    // -----------------------------------------------------------------------------
    public void ShowMessage(string message)
    {
        txtMessage.text = string.Empty;

        Time.timeScale = 0;

        messagePanel.SetActive(true);

        InputManager.Instance.SwitchMap(ControlMap.Dialogue);

        StartCoroutine(TypeText(message));
    }

    // -----------------------------------------------------------------------------
    // HideMessage
    //
    // - Oculta el panel, restaura el tiempo y vuelve al mapa de controles del jugador.
    // -----------------------------------------------------------------------------
    void HideMessage()
    {
        StopAllCoroutines();

        messagePanel.SetActive(false);

        Time.timeScale = 1;

        InputManager.Instance.SwitchMap(ControlMap.Player);
    }

    // -----------------------------------------------------------------------------
    // TypeText
    //
    // - Añade cada carácter del mensaje al TextMeshPro con un retardo de charDelay
    //   segundos entre caracteres para simular el efecto de máquina de escribir.
    // -----------------------------------------------------------------------------
    IEnumerator TypeText(string message)
    {
        foreach (char letter in message)
        {
            txtMessage.text += letter;

            yield return new WaitForSecondsRealtime(charDelay);
        }
    }

    public void OnNotify(string itemID)
    {
        SpawnOnceSO resource = ResourcesManager.Instance.GetResource(itemID);

        if (resource != null && !string.IsNullOrEmpty(resource.message))
        {
            /*
            string message = resource.message;

            if (resource.itemType == SpawnOnceSO.SpawnOnceType.Weapon)
            {
                message += " Se añadió a tu inventario.";
            }

            ShowMessage(message);
            */
            ShowMessage(resource.message);
        }
    }
}