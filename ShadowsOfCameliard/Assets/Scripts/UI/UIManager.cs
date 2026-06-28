using Unity.VisualScripting;
using UnityEngine;

// -----------------------------------------------------------------------------
// UIManager
//
// Responsabilidades:
// - Gestiona los elementos de la interfaz de usuario del juego.
// - Actualiza los corazones de salud en pantalla según el estado del jugador.
//
// Atributos principales:
// - heartPrefab: prefab instanciado una vez por cada punto de vida máximo.
// - heartContainer: transform padre en la UI donde se ubican los corazones.
// - heartSpacing: separación horizontal entre corazones consecutivos.
// - topLeftAnchorOffset: desplazamiento desde la esquina superior izquierda del contenedor.
//
// Notas:
// - Los corazones se destruyen y recrean en cada actualización; no se mantienen
//   referencias individuales entre llamadas.
// - Los corazones vacíos (salud perdida) se instancian igualmente pero se desactivan,
//   mostrando el hueco correspondiente en la UI.
// -----------------------------------------------------------------------------
public class UIManager : PersistentSingleton<UIManager>
{
    [Header("Health UI Settings")]
    [SerializeField] GameObject heartPrefab;
    [SerializeField] Transform heartContainer;
    [SerializeField] Vector2 heartSpacing;
    [SerializeField] Vector2 topLeftAnchorOffset;
    
    // -----------------------------------------------------------------------------
    // UpdateHealth
    //
    // - Destruye todos los corazones actuales y recrea uno por cada punto de vida
    //   máximo, activando únicamente los correspondientes a la salud actual.
    //
    // Notas:
    // - Los corazones se reconstruyen completamente en cada llamada; no se reutilizan.
    // - Los corazones vacíos se instancian pero se desactivan para mostrar el hueco.
    // -----------------------------------------------------------------------------
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        // Destruye los corazones existentes antes de regenerarlos
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        // Posición inicial anclada a la esquina superior izquierda del contenedor
        Vector2 startPosition = new Vector2(topLeftAnchorOffset.x, -topLeftAnchorOffset.y);

        // Instancia un corazón por cada punto de vida máximo y lo posiciona
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            RectTransform heartRect = heart.GetComponent<RectTransform>();
            heartRect.anchoredPosition = startPosition + new Vector2(i * heartSpacing.x, 0); // Desplazamiento horizontal acumulado

            // Los corazones por encima de la salud actual se muestran vacíos (inactivos)
            heart.SetActive(i < currentHealth);
        }
    }
}
