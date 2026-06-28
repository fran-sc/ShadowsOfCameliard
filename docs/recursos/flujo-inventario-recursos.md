# Flujo inventario-recursos

El flujo conecta `ScriptableObject`, spawneo, recogida, estado persistente e inventario.

```text
SpawnOnceSO
    ↓
ResourcesManager carga y clona
    ↓
SceneManagement instancia si corresponde
    ↓
Lancelot recoge o destruye
    ↓
PlayerCollectSubject notifica
    ↓
ResourcesManager actualiza estado
    ↓
InventoryManager añade/equipa si es arma
```

## Caso Steel of Benwick

1. `ResourcesManager` carga `Steel of Benwick` desde `Resources/SpawnOnce`.
2. `SceneManagement` lo instancia en `(2.5, 0.3, 0)`.
3. El jugador lo recoge.
4. Se muestra el mensaje narrativo.
5. El item se marca como `Collected`.
6. `InventoryManager` añade el sprite de espada al primer slot.
7. Al cerrar inventario, `ActiveWeapon` equipa `SwordWeapon`.
8. El Animator cambia a `item = 1`.

[< volve](README.md)