# ScriptableObjects de recursos

Los recursos persistentes se definen mediante [**`SpawnOnceSO`**](../../ShadowsOfCameliard/Assets/Scripts/Spawn/SpawnOnceSO.cs) y [**`SpawnOnceWeaponSO`**](../../ShadowsOfCameliard/Assets/Scripts/Spawn/SpawnOnceWeaponSO.cs)

## SpawnOnceSO

Campos principales:

| Campo | Descripción |
|---|---|
| `itemID` | Identificador único. |
| `sceneID` | Build index de la escena donde aparece. |
| `itemType` | Tipo: arma, vida, ammo, llave, bloqueo. |
| `state` | Estado actual del recurso. |
| `spawnPosition` | Posición de aparición. |
| `itemPrefab` | Prefab a instanciar. |
| `message` | Mensaje opcional. |

Estados:

```text
NotSpawned
ToSpawn
Spawned
Collected
Destroyed
```

## SpawnOnceWeaponSO

Extiende `SpawnOnceSO` con datos de arma:

| Campo | Descripción |
|---|---|
| `damage` | Daño del arma. |
| `pushForce` | Empuje aplicado. |
| `weaponSprite` | Sprite para inventario. |
| `weaponName` | Nombre visible. |
| `weaponPrefab` | Prefab funcional. |
| `animatorItemID` | Valor enviado al Animator. |
| `attackSound` | Sonido de ataque. |
| `isFiringWeapon` | Indica si dispara proyectiles. |
| `requiresReload` | Indica si requiere recarga. |
| `projectilePrefab` | Prefab del proyectil. |
| `canUseShield` | Permite bloquear. |
| `blockMoveSpeedMultiplier` | Penalización de velocidad al bloquear. |

`OnValidate()` fuerza `itemType = Weapon`.
