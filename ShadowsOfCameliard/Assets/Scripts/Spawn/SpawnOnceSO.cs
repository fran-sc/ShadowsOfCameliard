using System;
using UnityEngine;

// -----------------------------------------------------------------------------
// SpawnOnceSO
//
// Responsabilidades:
// - ScriptableObject que describe un ítem que solo se spawnea una vez por partida.
// - Almacena el estado del ciclo de vida del item (NotSpawned → Spawned → Collected).
// - Define el tipo de item para que los sistemas distingan su comportamiento.
//
// Atributos principales:
// - itemID: clave única usada en ResourcesManager.
// - sceneID: escena en la que debe aparecer el item.
// - state: estado actual del ciclo de vida.
// - spawnPosition: posición de spawn por defecto en el mundo.
//
// Notas:
// - Los ScriptableObjects se clonan en ResourcesManager para no modificar el asset.
// - SpawnOnceWeaponSO extiende esta clase con propiedades específicas de armas.
// -----------------------------------------------------------------------------
[CreateAssetMenu(fileName = "SpawnOnceSO", menuName = "Scriptable Objects/SpawnOnceSO")]
public class SpawnOnceSO : ScriptableObject
{
    [Header("Spawn Once Item Properties")]
    public string itemID; // Identificador único del item
    public int sceneID; // ID de la escena donde aparece el item
    public SpawnOnceType itemType; // Tipo de item
    public SpawnOnceState state; // Estado actual del item (no spawn, to spawn, collected)
    public Vector3 spawnPosition; // Posición donde se debe spawnear el item
    public GameObject itemPrefab; // Prefab del item para instanciar en la escena
    [TextArea(3, 10)]
    public string message; // Mensaje que se muestra al recoger el item

    [Serializable]
    public enum SpawnOnceState
    {
        NotSpawned,
        ToSpawn,
        Spawned,
        Collected,
        Destroyed
    }

    [Serializable]
    public enum SpawnOnceType
    {
        Weapon,
        Health,
        Ammo,
        Key,
        Blockade
    }

}

