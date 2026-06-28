# Sistemas principales

La demo se apoya en varios managers pequeños, cada uno con una responsabilidad concreta. No existe un `GameManager` monolítico que coordine toda la lógica.

| Sistema | Responsabilidad |
|---|---|
| [`GameManager`](gamemanager.md) | Establece la música principal de la partida. |
| [`AudioManager`](audiomanager.md) | Música, efectos y fade de música. |
| [`InputManager`](inputmanager.md) | Mapas de input y acceso centralizado a `PlayerControls`. |
| [`CameraManager`](cameramanager.md) | Seguimiento de cámara sobre el jugador. |
| [`InventoryManager`](inventorymanager.md) | Inventario, slots y equipamiento. |
| [`MessageManager`](messagemanager.md) | Mensajes contextuales. |
| [`ResourcesManager`](resourcesmanager.md) | Estado de recursos persistentes. |
| [`SceneManagement`](scenemanagement.md) | Recursos de escena y destino de portales. |
| [`UIManager`](uimanager.md) | HUD de vida y dash. |
| [`DamageReceiver`](damagereceiver.md) | Base común para daño. |
| [`Singletons`](singletons.md) | `PersistentSingleton<T>` y `SceneSingleton<T>`. |
