using System.Collections;
using TMPro;
using UnityEngine;

// -----------------------------------------------------------------------------
// SignPostAnim
//
// Responsabilidades:
// - Muestra el texto del cartel con animación de subida y desvanecimiento.
// - La animación se dispara al entrar el jugador en el trigger del cartel.
//
// Atributos principales:
// - signText: texto mostrado en el cartel.
// - animationTime: duración de la animación.
// - moveDistance: distancia vertical que recorre el texto.
//
// Notas:
// - El canvas se activa y desactiva con la animación para ahorrar recursos.
// - La opacidad se reduce linealmente con el movimiento del texto.
// -----------------------------------------------------------------------------
public class SignPostAnim : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] TextMeshProUGUI textMesh;
    [TextArea(3, 10)][SerializeField] string signText;
    [SerializeField] float textSize = 36f;

    [Header("Animation Settings")]
    [SerializeField] float animationTime = 2f;
    [SerializeField] float moveDistance = 350f;

    [SerializeField] Canvas canvas;

    CanvasGroup canvasGroup;
    RectTransform textRect;
    
    bool isAnimating = false;

    void Awake()
    {
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        textRect = textMesh.GetComponent<RectTransform>();

        InitilizeSingPost();
    }

    void InitilizeSingPost()
    {
        // COnfiguracion del texto
        textMesh.text = signText;
        textMesh.fontSize = textSize;

        // Configuracion del canvas
        canvas.enabled = false;
        canvasGroup.alpha = 0f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimating) return;

        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AnimateText());
        }
    }

    // -----------------------------------------------------------------------------
    // AnimateText
    //
    // - Mueve el RectTransform del texto hacia arriba y reduce su alpha en paralelo
    //   a lo largo de animationTime segundos.
    // - Al terminar llama a RestartSignPost para dejar el cartel listo de nuevo.
    // -----------------------------------------------------------------------------
    IEnumerator AnimateText()
    {
        isAnimating = true;
        canvas.enabled = true;
        canvasGroup.alpha = 1f;

        float elapsedTime = 0f;
        float alpha;

        RectTransform textRect = textMesh.GetComponent<RectTransform>();

        while (elapsedTime < animationTime)
        {
            float progress = elapsedTime / animationTime;

            // Mueve el texto hacia arriba
            textRect.anchoredPosition = new Vector2(0, progress * moveDistance);  

            // Reduce la opacidad del texto
            alpha = Mathf.Lerp(1f, 0f, progress);
            canvasGroup.alpha = alpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        RestartSignPost();
    }

    void RestartSignPost()
    {
        // Reinicia el estado del cartel
        canvasGroup.alpha = 0f;
        textRect.anchoredPosition = new Vector2(0, 0);
        canvas.enabled = false;
        
        isAnimating = false;
    }
}
