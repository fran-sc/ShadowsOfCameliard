using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// CodexSimpl
//
// Responsabilidades:
// - Controla la navegación entre páginas del códice simplificado.
// - Anima el desplazamiento y centrado del libro al abrirlo o cerrarlo.
// - Ajusta el tamaño ortográfico de la cámara al abrir/cerrar el libro.
//
// Atributos principales:
// - codexPages: array de páginas del códice en orden de aparición.
// - fastTurnDuration: duración reducida para saltar páginas al inicio.
// - codexCamera: cámara exclusiva del códice para el zoom.
//
// Notas:
// - Solo puede haber una transición activa a la vez (isTransitionInProgress).
// - El evento TurnCompleted de cada página notifica el fin del giro.
// - GotoPageRoutine permite posicionar el libro en una página inicial concreta.
// -----------------------------------------------------------------------------
public class CodexSimpl : MonoBehaviour
{
    [Header("Book Settings")]
    [SerializeField] float bookWidth = 8.25f;
    [SerializeField] float bookHeight = 5.5f;
    [SerializeField] float traslationTime = 1f;
    [SerializeField] float fastTurnDuration = 0.4f;

    [Header("Codex Pages")]
    [SerializeField] CodexPageSimpl[] codexPages;
    [SerializeField] int startingPageIndex = 0;

    [Header("Camera Settings")]
    [SerializeField] Camera codexCamera;
    [SerializeField] float maxOrthographicSize = 5f;
    [SerializeField] float minOrthographicSize = 3f;

    int currentRightPageIndex = 0;

    // Flag que indica si se está avanzando a una posición del libro
    bool isTransitionInProgress = false;
    
    // Flag que indica si el libro se está desplazando
    bool isBookMoving = false;

    // Flag que indica si se está girando una página
    bool isPageTurning = false; 

    public bool IsPerformingAction => isBookMoving || isPageTurning || isTransitionInProgress;

    int pendingPageToHideIndex = -1;

    public float Width => bookWidth;
    public float Height => bookHeight;
    public int TotalPages => codexPages.Length;
    public int CurrentRightPageIndex => currentRightPageIndex;

    void Awake()
    {
        SubscribeToPages();
    }

    void OnDestroy()
    {
        UnsubscribeFromPages();
    }

    void Start()
    {
        Vector3 initialPosition = transform.localPosition - new Vector3(bookWidth / 2f, 0f, 0f);
        transform.localPosition = initialPosition;

        // Recuperamos el índice de página inicial desde GameManager
        if (GameManager.Instance != null)
        {
            startingPageIndex = GameManager.Instance.StartingLeafIndex;
        }

        if (startingPageIndex > 0)
        {
            StartCoroutine(GotoPageRoutine(startingPageIndex));
        }
    }

    void SubscribeToPages()
    {
        if (codexPages == null) return;

        foreach (CodexPageSimpl page in codexPages)
        {
            if (page == null) continue;
            page.TurnCompleted += HandlePageTurnCompleted;
        }
    }

    void UnsubscribeFromPages()
    {
        if (codexPages == null) return;

        foreach (CodexPageSimpl page in codexPages)
        {
            if (page == null) continue;
            page.TurnCompleted -= HandlePageTurnCompleted;
        }
    }

    IEnumerator GotoPageRoutine(int pageIndex)
    {
        yield return new WaitForSeconds(1f); // Pequeña espera para asegurar que la escena y el códice estén listos

        pageIndex = Mathf.Clamp(pageIndex, 0, codexPages.Length - 1);

        while (currentRightPageIndex < pageIndex)
        {
            float originalTurnDuration = codexPages[currentRightPageIndex].TurnDuration;

            codexPages[currentRightPageIndex].SetTurnDuration(fastTurnDuration);

            GoForward();

            yield return new WaitUntil(() => !isTransitionInProgress);

            codexPages[currentRightPageIndex - 1].SetTurnDuration(originalTurnDuration);
        }
    }

    // -----------------------------------------------------------------------------
    // GoForward / GoBackward
    //
    // - Avanzan o retroceden una página si no hay transición en curso.
    // - Al abrir o cerrar la cubierta llaman a RecentreBook para desplazar el libro.
    // - Gestionan la visibilidad de la página pendiente de ocultar.
    // -----------------------------------------------------------------------------
    public void GoForward()
    {
        if (isTransitionInProgress) return;
        if (currentRightPageIndex >= codexPages.Length - 1) return;

        isTransitionInProgress = true;
        isPageTurning = true;
        pendingPageToHideIndex = currentRightPageIndex - 1;

        if (currentRightPageIndex == 0)
        {
            RecentreBook(true);
        }

        CodexPageSimpl page = codexPages[currentRightPageIndex];
        page.TurnPage();

        currentRightPageIndex++;

        CodexPageSimpl nextPage = codexPages[currentRightPageIndex];
        nextPage.gameObject.SetActive(true);
    }

    public void GoBackward()
    {
        if (isTransitionInProgress) return;
        if (currentRightPageIndex <= 0) return;

        isTransitionInProgress = true;
        isPageTurning = true;

        if (currentRightPageIndex == 1)
        {
            RecentreBook(false);
        }

        currentRightPageIndex--;

        CodexPageSimpl page = codexPages[currentRightPageIndex];
        page.gameObject.SetActive(true);
        page.TurnPage();

        pendingPageToHideIndex = currentRightPageIndex + 1;

        if (currentRightPageIndex - 1 >= 0)
        {
            CodexPageSimpl previousPage = codexPages[currentRightPageIndex - 1];
            previousPage.gameObject.SetActive(true);
        }
    }

    void HandlePageTurnCompleted(CodexPageSimpl page)
    {
        isPageTurning = false;

        HidePendingPage();

        TryFinishTransition();
    }

    void HidePendingPage()
    {
        if (pendingPageToHideIndex < 0 || pendingPageToHideIndex >= codexPages.Length)
        {
            pendingPageToHideIndex = -1;
            return;
        }

        codexPages[pendingPageToHideIndex].gameObject.SetActive(false);
        pendingPageToHideIndex = -1;
    }

    // -----------------------------------------------------------------------------
    // RecentreBook
    //
    // - Desplaza el libro hacia la derecha al abrir (cubierta) o izquierda al cerrar.
    // - Lanza en paralelo AnimateBookPosition y AnimateCameraSize.
    // -----------------------------------------------------------------------------
    void RecentreBook(bool isOpening)
    {
        Vector3 direction = isOpening
            ? new Vector3(bookWidth / 2f, 0f, 0f)
            : new Vector3(-bookWidth / 2f, 0f, 0f);

        Vector3 targetPosition = transform.localPosition + direction;

        StartCoroutine(AnimateBookPosition(targetPosition, traslationTime));

        StartCoroutine(AnimateCameraSize(isOpening));
    }

    IEnumerator AnimateBookPosition(Vector3 targetPosition, float duration)
    {
        isBookMoving = true;

        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        transform.localPosition = targetPosition;

        isBookMoving = false;

        TryFinishTransition();
    }

    // -----------------------------------------------------------------------------
    // AnimateCameraSize
    //
    // - Interpola el orthographicSize de la cámara entre min y max según si se abre
    //   o se cierra el libro, sincronizado con traslationTime.
    // -----------------------------------------------------------------------------
    IEnumerator AnimateCameraSize(bool isOpening)
    {
        float targetSize = isOpening ? maxOrthographicSize : minOrthographicSize;
        float startSize = codexCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < traslationTime)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / traslationTime);
            codexCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        codexCamera.orthographicSize = targetSize;
    }

    // Marca la transición como completa solo cuando ni el libro ni la página están animando
    void TryFinishTransition()
    {
        if (isPageTurning) return;
        if (isBookMoving) return;

        isTransitionInProgress = false;
    }
}