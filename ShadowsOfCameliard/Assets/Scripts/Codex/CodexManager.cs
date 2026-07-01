using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// CodexIndex
//
// - Vincula un capítulo del códice con su página de inicio y la escena asociada
// -----------------------------------------------------------------------------
[Serializable]
public class CodexIndex
{
    public int chapterNum;
    public int leafIndex;
    public string sceneName;
}

// -----------------------------------------------------------------------------
// CodexManager
//
// Responsabilidades:
// - Controla el flujo de la escena de introducción y de créditos finales.
// - Detecta cuando el jugador llega al final del códice y lanza la transición.
// - Muestra mensajes de ayuda con fade si el jugador no interactúa.
//
// Atributos principales:
// - isFinalScene: diferencia el comportamiento entre la intro y los créditos.
// - nextSceneName: nombre de la escena a cargar al terminar la intro.
// - movementMessage / restartMessage: textos de ayuda para el jugador.
//
// Notas:
// - Destruye los objetos DontDestroyOnLoad al inicio para evitar duplicados.
// - El fade-out de música y el fade visual se ejecutan en paralelo.
// - En la escena final permite reiniciar el capítulo con la tecla 'R'.
// -----------------------------------------------------------------------------
public class CodexManager : MonoBehaviour
{
    [Header("Final Settings")]
    [SerializeField] bool isFinalScene = false;

    [Header("Codex Settings")]
    [SerializeField] CodexSimpl codex;

    [Header("Next Scene Settings")]
    [SerializeField] float nextSceneDelay  = 5f;

    [Header("UI Message")]
    [SerializeField] [TextArea(3, 10)] string movementMessage = "Use the movement controls<br>to turn the pages of the codex";
    [SerializeField] [TextArea(3, 10)] string enterChapterMessage = "Press [RET] to start the chapter";
    [SerializeField] float messageFadeInDuration = 1f;
    [SerializeField] float messageShowDuration = 1f;
    [SerializeField] float messageFadeOutDuration = 3f;
    [SerializeField] float waitBeforeMessageDisplay = 5f;
    [SerializeField] TextMeshProUGUI UIMessage;

    [Header("Codex Index Settings")]
    [SerializeField] CodexIndex[] codexIndices;

    IEnumerator displayAdviceCoroutine;   
    
    bool exitingCodex = false;
    int lastLeafIndex = 5;

    void Awake()
    {
        // Aseguramos la destrucción de objetos persistentes de las escenas anteriores
        GameReset.DestroyDontDestroyOnLoadObjects();
    }

    void Start()
    {
        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeFromBlack();
        }   

        // Recuperamos el índice de página inicial desde GameManager
        if (GameManager.Instance != null)
        {
            lastLeafIndex = GameManager.Instance.LastLeafIndex;
        }

        // Reproducimos la música de fondo del códice si está disponible
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Music.CodexTheme);
        }

        if (!isFinalScene)
        {
            // Iniciamos la corrutina para esperar a que el jugador interactúe con el códice.
            StartCoroutine(WaitForIntereaction());
        }
    }

    /*
    void Update()
    {
        if (exitingCodex) return;

        // Comprobamos si llegamos a la última página desbloqueada del codex
        if (codex != null && codex.CurrentRightPageIndex > lastLeafIndex)
        {
            if (!isFinalScene)
            {
                // Cargamos la siguiente escena
                ProcessChapterSceneEnter();
            }
            else
            {
                // Mostramos un mensaje en pantalla indicando que el jugador puede reiniciar el capítulo o salir del juego.
                if (displayAdviceCoroutine != null)
                {
                    StopCoroutine(displayAdviceCoroutine);
                }
                displayAdviceCoroutine = DisplayAdvice(restartMessage, true);
                StartCoroutine(displayAdviceCoroutine);
            }
        }
    }
    */

    // -----------------------------------------------------------------------------
    // ProcessChapterSceneEnter
    //
    // - Deshabilita los controles del jugador, inicia el fade visual y lanza la
    //   corrutina que hace fade-out de música y carga la siguiente escena.
    // -----------------------------------------------------------------------------
    void ProcessChapterSceneEnter(int chapterIndex)
    {
        exitingCodex = true;

        // Si está en curso la corrutina de avisos, la detenemos y hacemos fade-out del mensaje en pantalla
        if (displayAdviceCoroutine != null)
        {
            StopCoroutine(displayAdviceCoroutine);
            StartCoroutine(FadeOutAdvice(1));
        }

        // Deshabilitar los controles del jugador
        PlayerInput playerInput = codex.GetComponentInParent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
        }

        // Lanzamos la corrutina de avance de página
        codex.GoForward();

        // Obtenemos el nombre de la escena correspondiente al capítulo actual
        string nextScene = codexIndices[chapterIndex].sceneName;

        // Guardamos el nombre de la siguiente escena en GameManager para que pueda ser cargada después del fade-out de música
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetNextSceneName(nextScene);
        }

        // Iniciar la corrutina para esperar a que el fade-out termine y luego cargar la siguiente escena
        StartCoroutine(FadeOutMusicAndLoadNextScene(nextSceneDelay));
    }

    IEnumerator FadeOutMusicAndLoadNextScene(float fadeDuration)
    {
        // fade-out de la música de fondo
        if (AudioManager.Instance != null)
        {
            float startVolume = AudioManager.Instance.MusicVolume;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                AudioManager.Instance.SetMusicVolume(Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration));
                yield return null;
            }

            AudioManager.Instance.StopMusic();

            // Restablecemos el volumen de la música para futuras reproducciones
            AudioManager.Instance.SetMusicVolume(startVolume);
        }

        // Cargamos la siguiente escena
        LoadNextScene();
    }

    void LoadNextScene()
    {
        // Cargamos la siguiente escena
        string nextScene = "";

        if (GameManager.Instance != null)
        {
            nextScene = GameManager.Instance.NextSceneName;
        }

        if (!string.IsNullOrEmpty(nextScene))
        {
            GameReset.ReloadSceneFromScratch(nextScene);
        }

    }

    // Si durante los primeros segundos de la introducción el jugador no ha pasado ninguna página, 
    // se muestra un mensaje en pantalla indicando que use los controles de movimiento para pasar las páginas del códice.
    IEnumerator WaitForIntereaction()
    {
        float elapsedTime = 0f;

        while (elapsedTime < waitBeforeMessageDisplay)
        {
            if (codex != null && codex.CurrentRightPageIndex > 0)
            {
                yield break; // El jugador ha pasado una página, no mostrar el mensaje.
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (displayAdviceCoroutine != null)
        {
            StopCoroutine(displayAdviceCoroutine);
        }
        displayAdviceCoroutine = DisplayAdvice(movementMessage, true);
        StartCoroutine(displayAdviceCoroutine);
    }

    // -----------------------------------------------------------------------------
    // DisplayAdvice
    //
    // - Muestra el mensaje con fade-in, lo mantiene visible y aplica fade-out.
    // - Si loop es true, se rellanza al terminar para mostrar el mensaje en bucle.
    // -----------------------------------------------------------------------------
    IEnumerator DisplayAdvice(string msg, bool loop = false)
    {
        float elapsedTime = 0f;

        // Mostrar el mensaje en pantalla con un fade-in y fade-out
        if (UIMessage != null)
        {
            UIMessage.text = msg;
            UIMessage.alpha = 0f;

            // Fade-in
            elapsedTime = 0f;
            while (elapsedTime < messageFadeInDuration)
            {
                UIMessage.alpha = Mathf.Lerp(0f, 1f, elapsedTime / messageFadeInDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            UIMessage.alpha = 1f;

            // Mantener el mensaje visible durante un tiempo
            yield return new WaitForSeconds(messageShowDuration);

            // Fade-out
            elapsedTime = 0f;
            while (elapsedTime < messageFadeOutDuration)
            {
                UIMessage.alpha = Mathf.Lerp(1f, 0f, elapsedTime / messageFadeOutDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            UIMessage.alpha = 0f;
        }

        // Relanzamos la corrutina para volver a comprobar si el jugador ha pasado alguna página.
        if (loop)
        {
            if (displayAdviceCoroutine != null)
            {
                StopCoroutine(displayAdviceCoroutine);
            }

            displayAdviceCoroutine = DisplayAdvice(enterChapterMessage, true);
            StartCoroutine(displayAdviceCoroutine);
        }
    }

    IEnumerator FadeOutAdvice(float fadeDuration)
    {
        if (UIMessage != null)
        {
            float elapsedTime = 0f;
            float currentAlpha = UIMessage.alpha;

            // Fade-out
            while (elapsedTime < fadeDuration)
            {
                UIMessage.alpha = Mathf.Lerp(currentAlpha, 0f, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            UIMessage.alpha = 0f;
        }
    }

    // -----------------------------------------------------------------------------
    // ChapterToPlay
    // - Determina si la página actual del códice se corresponde con una escena y 
    //   retorna su índice en codexIndices. Devuelve -1 si no corresponde a ningún 
    //   inicio de capítulo.
    // -----------------------------------------------------------------------------
    int ChapterToPlay()
    {
        // Determinamos si la página actual se corresponde con una página de inicio de capítulo
        if (codex != null && codexIndices != null)
        {
            int currentPageIndex = codex.CurrentRightPageIndex;

            for (int i = 0; i < codexIndices.Length; i++)
            {
                var index = codexIndices[i];
                if (index.leafIndex == currentPageIndex)
                {
                    return i;
                }
            }
        }
        
        return -1;
    }

    // -----------------------------------------------------------------------------
    // Codex Page Navigation Actions
    //
    // - Métodos que se vinculan a los InputActions para pasar páginas del códice
    //   e iniciar un capítulo.
    // -----------------------------------------------------------------------------
    public void ActionForward(InputAction.CallbackContext context)
    {
        if (context.performed && !codex.IsPerformingAction && !exitingCodex)
        {
            if (codex.CurrentRightPageIndex == lastLeafIndex)
            {
                // No avanzamos más allá de la última página desbloqueada

                // Si nos encontramos en un inicio de capítulo
                if (ChapterToPlay() != -1)
                {
                    // mostramos mensaje indicando que el jugador puede iniciarlo
                    if (displayAdviceCoroutine != null)
                    {
                        StopCoroutine(displayAdviceCoroutine);
                    }

                    displayAdviceCoroutine = DisplayAdvice(enterChapterMessage, false);
                    StartCoroutine(displayAdviceCoroutine);
                }

                return;
            }

            codex.GoForward();
        }
    }

    public void ActionBackward(InputAction.CallbackContext context)
    {
        if (context.performed && !codex.IsPerformingAction && !exitingCodex)
        {
            codex.GoBackward();
        }
    }

    public void ActionPlay(InputAction.CallbackContext context)
    {
        if (context.performed && !codex.IsPerformingAction && !exitingCodex)
        {
            int chapterIndex = ChapterToPlay();
            if (chapterIndex != -1)
            {
                // Iniciar el capítulo
                ProcessChapterSceneEnter(chapterIndex);
            }
        }
    }    
}
