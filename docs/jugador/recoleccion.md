# Recolección

La recolección se apoya en `PlayerCollectSubject` y en el patrón **Observer**.

## Flujo general

1. Lancelot recoge un objeto spawneado.
2. `PlayerCollectSubject` notifica el `itemID`.
3. `ResourcesManager` marca el recurso como `Collected`.
4. `InventoryManager` añade el arma si el recurso es de tipo `Weapon`.
5. `MessageManager` puede mostrar el mensaje asociado.

## Ventaja

El jugador no necesita conocer directamente a todos los sistemas. Solo emite una notificación, y cada observador reacciona según su responsabilidad.

[< volver](../README.md)