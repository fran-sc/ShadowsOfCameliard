using System.Collections.Generic;
using UnityEngine;

// -----------------------------------------------------------------------------
// ResourcesManager
//
// Responsabilidades:
// - Carga y almacena en memoria todos los ScriptableObjects SpawnOnceSO.
// - Proporciona acceso a recursos por ID y filtrado por escena.
// - Actualiza el estado de un recurso a Collected al recibir notificación de recogida.
//
// Atributos principales:
// - resources: diccionario clave=itemID, valor=clon de SpawnOnceSO.
//
// Notas:
// - Los recursos se clonan (Instantiate) para no modificar el asset original.
// - Implementa IPlayerCollectObserver para reaccionar a items recogidos.
// -----------------------------------------------------------------------------
public class ResourcesManager : PersistentSingleton<ResourcesManager>, IPlayerCollectObserver
{
    // Diccionario para almacenar los recursos SpawnOnceSO
    // Clave: itemID, Valor: SpawnOnceSO
    Dictionary<string, SpawnOnceSO> resources = new Dictionary<string, SpawnOnceSO>();

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }

        // Cargamos los recursos al iniciar el juego
        LoadResources();
    }

    void Start()
    {
        if (Instance != this)
        {
            return;
        }

        // Nos registramos como observador del PlayerCollectSubject
        PlayerCollectSubject.Instance.AddObserver(this);
    }

    // -----------------------------------------------------------------------------
    // LoadResources
    //
    // - Carga todos los SpawnOnceSO de la carpeta Resources/SpawnOnce y los almacena
    //   como clones en el diccionario para evitar modificar los assets originales.
    // -----------------------------------------------------------------------------
    void LoadResources()
    {        
        // Cargamos todos los recursos de tipo SpawnOnceSO desde la carpeta Resources/SpawnOnce
        SpawnOnceSO[] loadedResources = Resources.LoadAll<SpawnOnceSO>("SpawnOnce");

        foreach (var resource in loadedResources)
        {
            if (!resources.ContainsKey(resource.itemID))
            {
                // Clonamos el recurso para evitar problemas de referencia
                resources.Add(resource.itemID, ScriptableObject.Instantiate(resource));
            }
            else
            {
                Debug.LogWarning($"Resource with name {resource.name} already exists. Skipping duplicate.");
            }
        }
    }

    public SpawnOnceSO GetResource(string resourceID)
    {
        if (resources.TryGetValue(resourceID, out SpawnOnceSO resource))
        {
            return resource;
        }
        else
        {
            Debug.LogError($"Resource {resourceID} not found.");
            return null;
        }
    }

    // -----------------------------------------------------------------------------
    // GetSceneResources
    //
    // - Filtra el diccionario de recursos por sceneID y devuelve la lista resultante.
    // -----------------------------------------------------------------------------
    public List<SpawnOnceSO> GetSceneResources(int sceneID)
    {
        List<SpawnOnceSO> sceneResources = new List<SpawnOnceSO>();
        foreach (var resource in resources.Values)
        {
            if (resource.sceneID == sceneID)
            {
                sceneResources.Add(resource);
            }
        }
        return sceneResources;
    }

    public void OnNotify(string itemID)
    {
        GetResource(itemID).state = SpawnOnceSO.SpawnOnceState.Collected;
    }
}
