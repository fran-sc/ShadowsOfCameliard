using UnityEngine;

public enum ControlMap
{
    Player,
    Inventory,
    Dialogue,
    Menu
}

// -----------------------------------------------------------------------------
// InputManager
//
// Responsabilidades:
// - Centraliza el acceso al sistema de input del juego.
// - Gestiona el cambio entre mapas de controles (Player, Inventory, UI).
//
// Atributos principales:
// - controls: instancia de PlayerControls generada por el Input System.
// - currentMap: mapa de controles activo en este momento.
//
// Notas:
// - Al activar/desactivar el componente restaura o deshabilita el mapa actual.
// - Dispone los controles al destruirse para evitar fugas de memoria.
// -----------------------------------------------------------------------------
public class InputManager : PersistentSingleton<InputManager>
{
    PlayerControls controls;

    public PlayerControls Controls => controls;

    ControlMap currentMap = ControlMap.Player;

    protected override void Awake()
    {
        base.Awake();

        // Si este InputManager es un duplicado, base.Awake() ya habrá llamado
        // a Destroy(gameObject).
        // Salimos para no crear otro PlayerControls.
        if (Instance != this)
        {
            return;
        }

        controls = new PlayerControls();

        SwitchMap(currentMap);
    }

    void OnEnable()
    {
        if (Instance != this || controls == null)
        {
            return;
        }

        SwitchMap(currentMap);
    }

    void OnDisable()
    {
        if (Instance != this || controls == null)
        {
            return;
        }

        controls.Disable();
    }

    protected override void OnDestroy()
    {
        if (Instance == this && controls != null)
        {
            controls.Disable();
            controls.Dispose();
            controls = null;
        }

        base.OnDestroy();
    }

    // -----------------------------------------------------------------------------
    // SwitchMap
    //
    // - Deshabilita todos los mapas activos y habilita únicamente el solicitado.
    //
    // Notas:
    // - Player → controles de movimiento y combate.
    // - Inventory → navegación por el inventario.
    // - Dialogue / Menu → mapa UI para confirmar y cerrar paneles.
    // -----------------------------------------------------------------------------
    public void SwitchMap(ControlMap newMap)
    {
        if (controls == null)
        {
            return;
        }

        currentMap = newMap;

        controls.Player.Disable();
        controls.Inventory.Disable();
        controls.UI.Disable();

        switch (newMap)
        {
            case ControlMap.Player:
                controls.Player.Enable();
                break;

            case ControlMap.Inventory:
                controls.Inventory.Enable();
                break;

            case ControlMap.Dialogue:
                controls.UI.Enable();
                break;

            case ControlMap.Menu:
                controls.UI.Enable();
                break;
        }
    }
}