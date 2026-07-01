using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// Codex
//
// Responsabilidades:
// - Controla la navegación del libro físico (versión con mesh real).
// - Desplaza y centra el libro en pantalla al abrir o cerrar la cubierta.
// - Oculta páginas ya pasadas tras un retraso para optimizar la escena.
//
// Atributos principales:
// - codexPages: array de páginas en orden de aparición.
// - translationTime: duración del desplazamiento al abrir el libro.
// - startingPageIndex: página inicial al cargar la escena.
//
// Notas:
// - El giro se bloquea con isTurning para evitar solapamientos.
// - HandlePageTurn espera 1.25× TurnDuration para dar margen a la animación.
// -----------------------------------------------------------------------------
public class Codex : MonoBehaviour
{
    [Header("Book Settings")]
    [SerializeField] float bookWidth = 8.25f; // Ancho del libro
    [SerializeField] float bookHeight = 5.5f; // Alto del libro
    [SerializeField] float translationTime = 1f; // Tiempo de transición para el desplazamiento del libro

    [Header("Codex Pages")]
    [SerializeField] CodexPage[] codexPages;
    [SerializeField] int startingPageIndex = 0; 

    public float Width => bookWidth;
    public float Height => bookHeight;

    int currentRightPageIndex = 0;

    // Flag que indica si se está girando una página
    bool isTurning = false; 
    public bool IsTurning => isTurning;

    void Start()
    {
        Vector3 initialPosition = transform.localPosition - new Vector3(bookWidth / 2f, 0f, 0f);
        transform.localPosition = initialPosition;
        
        GotoPage(startingPageIndex);
    }

    void GotoPage(int pageIndex)
    {
        if (pageIndex <= 0 || pageIndex >= codexPages.Length - 1) return;

        // Avanzamos desde la portada hasta la página deseada
        for (int i = 0; i < pageIndex; i++)
        {
            GoForward();
        }
    }

    void Update()
    {
        // Al pulsar la tecla 'A', avanzamos la página
        if (InputSystem.GetDevice<Keyboard>().aKey.wasPressedThisFrame)
        {
            if (!isTurning)
            {
                GoForward();
            }
        }

        // Al pulsar la tecla 'D', retrocedemos la página
        if (InputSystem.GetDevice<Keyboard>().dKey.wasPressedThisFrame)
        {
            if (!isTurning)
            {
                GoBackward();
            }
        }
    }

    // -----------------------------------------------------------------------------
    // GoForward
    //
    // - Voltea la página actual hacia adelante, desplaza el libro si es la cubierta
    //   y programa ocultar la página anterior tras el TurnDuration.
    // -----------------------------------------------------------------------------
    public void GoForward()
    {
        if (currentRightPageIndex >= codexPages.Length - 1) return;

        // Marcamos que se está girando una página
        StartCoroutine(HandlePageTurn());

        // Si es la primera de las páginas (cover), desplaza el libro hacia la 
        // derecha antes de voltear la cubierta
        if (currentRightPageIndex == 0)
        {
            RecentreBook(true);
        }

        // Volteamos la página actual
        CodexPage page = codexPages[currentRightPageIndex];
        page.TurnPage();
        
        // Ocultamos la página anterior tras un pequeño retraso
        StartCoroutine(HidePage(currentRightPageIndex - 1, page.TurnDuration));

        // Avanzamos al siguiente índice de página y la mostramos
        if (currentRightPageIndex + 1 < codexPages.Length)
        {
            currentRightPageIndex++;
            CodexPage nextPage = codexPages[currentRightPageIndex];
            nextPage.gameObject.SetActive(true);         
        }
    }

    // Espera TurnDuration × 1.25 para garantizar que la animación de giro termine
    IEnumerator HandlePageTurn()
    {
        isTurning = true;

        // Esperamos a que la animación de giro de página termine
        yield return new WaitForSeconds(codexPages[currentRightPageIndex].TurnDuration * 1.25f);

        isTurning = false;
    }

    IEnumerator HidePage(int pageIndex, float delay)
    {
        if (pageIndex < 0 || pageIndex >= codexPages.Length) 
        {
            yield break;
        }   

        yield return new WaitForSeconds(delay);

        codexPages[pageIndex].gameObject.SetActive(false);
    }


    public void GoBackward()
    {
        if (currentRightPageIndex <= 0) return;

        // Si es la primera de las páginas (cover), desplaza el libro hacia la
        // izquierda antes de voltear la cubierta
        if (currentRightPageIndex == 1)
        {
            RecentreBook(false);
        }

        // Volteamos la página anterior a la actual
        currentRightPageIndex--;
        CodexPage page = codexPages[currentRightPageIndex];
        page.TurnPage();

        // Ocultamos la página sw la derecha tras un pequeño retraso
        StartCoroutine(HidePage(currentRightPageIndex + 1, page.TurnDuration));

        // Mostramos la página anterior a la actual
        if (currentRightPageIndex - 1 >= 0)
        {
            CodexPage previousPage = codexPages[currentRightPageIndex - 1];
            previousPage.gameObject.SetActive(true);
        }
    }

    void RecentreBook(bool isOpening)
    {
        if (isOpening)
        {
            // Reposicionar el libro para su lectura, desplazándolo hacia la derecha
            Vector3 targetPosition = transform.localPosition + new Vector3(bookWidth / 2f, 0f, 0f); 

            StartCoroutine(AnimateBookPosition(targetPosition, translationTime));
        }
        else
        {
            // Recentrar el libro en la posición original con una animación suave
            Vector3 targetPosition = transform.localPosition - new Vector3(bookWidth / 2f, 0f, 0f); 

            StartCoroutine(AnimateBookPosition(targetPosition, translationTime));
        }        
    }

    IEnumerator AnimateBookPosition(Vector3 targetPosition, float duration)
    {
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
    }

    public void ActionForward(InputAction.CallbackContext context)
    {
        if (context.performed && !isTurning)
        {
            GoForward();
        }
    }

    public void ActionBackward(InputAction.CallbackContext context)
    {
        if (context.performed && !isTurning)
        {
            GoBackward();
        }
    }

}


