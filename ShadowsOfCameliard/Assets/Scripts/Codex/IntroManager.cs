using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// IntroManager
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
public class IntroManager : MonoBehaviour
{
    [Header("Final Settings")]
    [SerializeField] bool isFinalScene = false;

    [Header("Codex Settings")]
    [SerializeField] CodexSimpl codex;

    [Header("Next Scene Settings")]
    [SerializeField] string nextSceneName = "Countryside";
    [SerializeField] float nextSceneDelay  = 5f;

    [Header("UI Message")]
    [SerializeField] [TextArea(3, 10)] string movementMessage = "Usa los controles de movimiento<br>para pasar las páginas del códice";
    [SerializeField] [TextArea(3, 10)] string restartMessage = "Pulsa [2] para volver a jugar<br>el segundo capítulo<br>[ESC] para salir del juego";
    [SerializeField] float messageFadeInDuration = 1f;
    [SerializeField] float messageShowDuration = 1f;
    [SerializeField] float messageFadeOutDuration = 3f;
    [SerializeField] float waitBeforeMessageDisplay = 5f;
    [SerializeField] TextMeshProUGUI UIMessage;

    AudioSource backgroundMusic;
    CodexFade codexFade;
    IEnumerator displayAdviceCoroutine;   
    
    bool ExitingIntro = false;

    void Awake()
    {
        // Aseguramos la destrucción de objetos persistentes de las escenas anteriores
        GameReset.DestroyDontDestroyOnLoadObjects();
    }

    void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
        codexFade = GetComponent<CodexFade>();

        if (codexFade != null)
        {
            codexFade.FadeFromBlack();
        }

        if (!isFinalScene)
        {
            // Iniciamos la corrutina para esperar a que el jugador interactúe con el códice.
            StartCoroutine(WaitForIntereaction());
        }
    }

    void Update()
    {
        if (ExitingIntro) return;

        if (isFinalScene)
        {
            CheckKeyboardInput();
        }

        // Comprobamos si llegamos al final del codex
        if (codex != null && codex.CurrentRightPageIndex >= codex.TotalPages - 1)
        {
            if (!isFinalScene)
            {
                // Cargamos la siguiente escen
                ProcessChapterEnd();
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

    // -----------------------------------------------------------------------------
    // ProcessChapterEnd
    //
    // - Deshabilita los controles del jugador, inicia el fade visual y lanza la
    //   corrutina que hace fade-out de música y carga la siguiente escena.
    // -----------------------------------------------------------------------------
    void ProcessChapterEnd()
    {
        // Si se alcanza el final de la introducción, deshabilitamos los controles
        // hacemos un fade-out y luego cargamos la siguiente escena.
        if (codex != null && codex.CurrentRightPageIndex >= codex.TotalPages - 1)
        {
            ExitingIntro = true;

            // Deshabilitar los controles del jugador
            PlayerInput playerInput = codex.GetComponentInParent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }

            // Iniciar el fade-out visual
            if (codexFade != null)
            {
                codexFade.FadeToBlack();
            }

            // Iniciar la corrutina para esperar a que el fade-out termine y luego cargar la siguiente escena
            StartCoroutine(FadeOutMusicAndLoadNextScene(nextSceneDelay));
        }
    }

    IEnumerator FadeOutMusicAndLoadNextScene(float fadeDuration)
    {
        // fade-out de la música de fondo
        if (backgroundMusic != null)
        {
            float startVolume = backgroundMusic.volume;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
                yield return null;
            }

            backgroundMusic.Stop();
        }

        // Cargar la siguiente escena
        LoadNextScene();
    }

    void LoadNextScene()
    {
        // Cargar la siguiente escena
        //UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        GameReset.ReloadSceneFromScratch(nextSceneName);
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

            displayAdviceCoroutine = DisplayAdvice(restartMessage, true);
            StartCoroutine(displayAdviceCoroutine);
        }
    }

    void CheckKeyboardInput()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ExitingIntro = true;

            Application.Quit();
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ExitingIntro = true;

            // Iniciar el fade-out visual
            if (codexFade != null)
            {
                codexFade.FadeToBlack();
            }

            // Reiniciar el capítulo
            StartCoroutine(FadeOutMusicAndLoadNextScene(nextSceneDelay));
        }
    }
}
