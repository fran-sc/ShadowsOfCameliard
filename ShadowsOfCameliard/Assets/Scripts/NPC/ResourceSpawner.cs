using UnityEngine;

// -----------------------------------------------------------------------------
// ResourceSpawner
//
// Responsabilidades:
// - Permite a un NPC spawnear un item específico cuando el jugador se acerca.
// - Comprueba que el item no haya sido ya spawneado antes de instanciarlo.
// - Soporta usar la posición del propio NPC o la posición por defecto del item.
//
// Atributos principales:
// - spawnOnceID: identificador del SpawnOnceSO que gestiona este spawner.
// - useCurrentPosition / offsetPosition: control de la posición de spawn.
// -----------------------------------------------------------------------------
public class ResourceSpawner : MonoBehaviour
{
    // Identificador del objeto spawndeado por el NPC
    [SerializeField] string spawnOnceID;
    [SerializeField] bool useCurrentPosition;
    [SerializeField] Vector3 offsetPosition;

    public void SpawnResource()
    {
        if (!CheckResourceToSpawn())
        {
            return;
        }

        if (useCurrentPosition)
        {
            Vector3 spawnPosition = transform.position + offsetPosition;
            SceneManagement.Instance.SpawnResource(spawnOnceID, spawnPosition);
        }
        else
        {
            SceneManagement.Instance.SpawnResource(spawnOnceID);
        }
    }

    // -----------------------------------------------------------------------------
    // CheckResourceToSpawn
    //
    // - Devuelve true solo si el recurso existe y está en estado NotSpawned.
    // -----------------------------------------------------------------------------
    public bool CheckResourceToSpawn()
    {
        SpawnOnceSO resource = ResourcesManager.Instance.GetResource(spawnOnceID);
        
        return resource != null && 
            resource.state == SpawnOnceSO.SpawnOnceState.NotSpawned;
    }
}
