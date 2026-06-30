using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------
// SceneManagement
//
// Responsabilidades:
// - Coordina el spawneo de recursos persistentes al cargar cada escena.
// - Registra el portal de destino para posicionar al jugador correctamente.
// - Permite instanciar recursos individuales en posiciones arbitrarias.
//
// Atributos principales:
// - initialPortal: portal por defecto cuando el jugador aún no ha viajado.
// - destPortalName: nombre del portal donde debe aparecer el jugador.
//
// Notas:
// - Se suscribe a SceneManager.sceneLoaded para responder a cargas de escena.
// - Solo se spawnean recursos en estado ToSpawn o Spawned (no Collected/Destroyed).
// -----------------------------------------------------------------------------
public class SceneManagement : PersistentSingleton<SceneManagement>
{
    [Header("Game Start")]
    [SerializeField] PortalSpawn initialPortal;

    // Nombre del portal de destino al que se debe teletransportar el jugador
    public string destPortalName { get; private set; }
    public void SetDestPortalName(string name) => destPortalName = name;

    protected override void Awake()
    {
        base.Awake();

        // Si es un duplicado, salimos para evitar la suscripción múltiple al 
        // evento de carga de escena
        if (Instance != this)
        {
            return;
        }

        // Establecemos el valor inicial del portal de destino
        // Se utiliza cuando aún no se ha atravesado ningún portal y 
        // el jugador muere
        destPortalName = initialPortal != null ? initialPortal.PortalName : string.Empty;

        // Nos suscribimos al evento de carga de escena para instanciar los 
        // recursos de la escena actual una vez que la escena haya sido 
        // completamente cargada.
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    protected override void OnDestroy()
    {
        if (Instance == this)
        {
            // Nos desuscribimos del evento para evitar referencias colgantes
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        base.OnDestroy();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Instanciamos los recursos de la escena actual
        SpawnSceneResources(scene.buildIndex);
    }

    // -----------------------------------------------------------------------------
    // SpawnSceneResources
    //
    // - Obtiene los recursos de la escena con el sceneID dado y los instancia.
    // - Solo instancia recursos en estado ToSpawn o Spawned; los ya recogidos se omiten.
    // - El nombre del GameObject se establece al itemID para identificarlo al recogerlo.
    // -----------------------------------------------------------------------------
    void SpawnSceneResources(int sceneID)
    {
        // Obtenemos los recursos de la escena actual
        var resources = ResourcesManager.Instance.GetSceneResources(sceneID);
        
        // Instanciamos los recursos
        foreach (var resource in resources)
        {
            if (resource.itemPrefab == null)
            {
                Debug.LogWarning($"No se ha asignado el prefab del recurso {resource.name}.");
                continue;    
            }

            // Si está pendiente de spawneo o ya ha sido spawneado (pero no
            // recogido ni destruido), lo instanciamos en la posición predeterminada
            if (resource.state == SpawnOnceSO.SpawnOnceState.ToSpawn ||
                resource.state == SpawnOnceSO.SpawnOnceState.Spawned)
            {               
                GameObject gameObject = Instantiate(
                    resource.itemPrefab, resource.spawnPosition, Quaternion.identity);
                
                // Almacenamos el ID del item en el nombre del GameObject
                gameObject.name = resource.itemID; 

                // Actualizamos el estado del recurso a Spawned
                resource.state = SpawnOnceSO.SpawnOnceState.Spawned;
            }
        }
    }

    // -----------------------------------------------------------------------------
    // SpawnResource (posición por defecto)
    //
    // - Instancia el recurso en la posición predeterminada definida en el SpawnOnceSO.
    // -----------------------------------------------------------------------------
    public void SpawnResource(string resourceID)
    {
        SpawnOnceSO resource = ResourcesManager.Instance.GetResource(resourceID);

        if (resource == null)
        {
            Debug.Log($"SceneManagement.SpawnResource(): resource {resourceID} not found.");
            return;
        }

        Vector3 position = ResourcesManager.Instance.GetResource(resourceID).spawnPosition;

        SpawnResource(resourceID, position);
    }

    // -----------------------------------------------------------------------------
    // SpawnResource (posición específica)
    //
    // - Instancia el recurso en la posición indicada y actualiza su estado a Spawned.
    // -----------------------------------------------------------------------------
    public void SpawnResource(string resourceID, Vector3 position)
    {
        var resource = ResourcesManager.Instance.GetResource(resourceID);
        
        if (resource != null)
        {
            if (resource.itemPrefab != null)
            {
                // Instanciamos el recurso en la posición especificada
                GameObject gameObject = Instantiate(resource.itemPrefab, position, Quaternion.identity);
                
                // Almacenamos el ID del item en el nombre del GameObject
                gameObject.name = resource.itemID; 

                // Actualizamos el estado del recurso a Spawned
                resource.state = SpawnOnceSO.SpawnOnceState.Spawned;
            }
            else
            {
                Debug.LogWarning($"Prefab for resource {resource.name} is not assigned.");
            }
        }
    }

}
