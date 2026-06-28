# InputManager

`InputManager` centraliza el acceso a `PlayerControls`. Su prefab no expone campos en Inspector: es un sistema lógico.

## Función

- Crear o conservar la instancia de `PlayerControls`.
- Activar y desactivar mapas de control.
- Permitir que sistemas como jugador, inventario y mensajes no creen controles duplicados.

## Mapas de control

La demo alterna al menos entre:

| Mapa | Uso |
|---|---|
| `Player` | Movimiento, ataque, dash, bloqueo e interacción. |
| `Inventory` | Navegación y cierre del inventario. |
| `Message` | Cierre de mensajes contextuales. |

## Ejemplo de uso

`InventoryManager` cambia de mapa al abrir/cerrar el inventario:

```csharp
if (IsInventoryOpen)
{
    Time.timeScale = 0;
    InputManager.Instance.SwitchMap(ControlMap.Inventory);
}
else
{
    Time.timeScale = 1;
    InputManager.Instance.SwitchMap(ControlMap.Player);
}
```

[< volver](README.md)
