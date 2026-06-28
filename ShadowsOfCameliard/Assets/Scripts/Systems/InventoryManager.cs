using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// -----------------------------------------------------------------------------
// InventoryManager
//
// Responsabilidades:
// - Gestiona el inventario de armas del jugador.
// - Controla la apertura/cierre del panel de inventario y pausa el tiempo.
// - Notifica al ActiveWeapon el arma seleccionada al cerrar el inventario.
//
// Atributos principales:
// - slots: referencias UI a los huecos del inventario.
// - items: lista de IDs de armas recogidas.
// - selectedItemIndex / currentMarkedSlot: índices de selección actual y navegación.
//
// Notas:
// - Implementa IPlayerCollectObserver para recibir notificaciones de recogida.
// - Al abrir el inventario se congela el tiempo (Time.timeScale = 0).
// - El mapa de controles cambia a Inventory al abrir y a Player al cerrar.
// -----------------------------------------------------------------------------
public class InventoryManager : PersistentSingleton<InventoryManager>, IPlayerCollectObserver
{
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject[] slots;

    List<string> items = new List<string>();

    int currentMarkedSlot = -1;
    int selectedItemIndex = -1;

    public bool IsInventoryOpen { get; private set; } = false;

    PlayerControls controls;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }
    }

    void Start()
    {
        if (Instance != this)
        {
            return;
        }

        controls = InputManager.Instance.Controls;

        controls.Inventory.Close.performed += OnClosePerformed;
        controls.Inventory.Next.performed += OnNextPerformed;
        controls.Inventory.Prev.performed += OnPrevPerformed;

        PlayerCollectSubject.Instance.AddObserver(this);
    }

    protected override void OnDestroy()
    {
        if (Instance == this && controls != null)
        {
            controls.Inventory.Close.performed -= OnClosePerformed;
            controls.Inventory.Next.performed -= OnNextPerformed;
            controls.Inventory.Prev.performed -= OnPrevPerformed;
        }

        base.OnDestroy();
    }

    void OnClosePerformed(InputAction.CallbackContext context)
    {
        CloseInventory();
    }

    void OnNextPerformed(InputAction.CallbackContext context)
    {
        SelectNextSlot();
    }

    void OnPrevPerformed(InputAction.CallbackContext context)
    {
        SelectPreviousSlot();
    }

    void SelectNextSlot()
    {
        if (items.Count == 0) return;

        currentMarkedSlot = (currentMarkedSlot + 1) % items.Count;
        SetSelectedSlot(currentMarkedSlot);
    }

    void SelectPreviousSlot()
    {
        if (items.Count == 0) return;

        currentMarkedSlot = (currentMarkedSlot - 1 + items.Count) % items.Count;
        SetSelectedSlot(currentMarkedSlot);
    }

    void SetSelectedSlot(int index)
    {
        for (int i = 0; i < items.Count; i++)
        {
            GameObject activeImage = slots[i].transform.Find("Active").gameObject;
            activeImage.SetActive(i == index);
        }
    }

    // -----------------------------------------------------------------------------
    // CloseInventory
    //
    // - Si el slot marcado cambió, notifica al ActiveWeapon para equipar el arma.
    // - Cierra el panel de inventario y restaura el mapa de controles del jugador.
    // -----------------------------------------------------------------------------
    void CloseInventory()
    {
        if (currentMarkedSlot != selectedItemIndex)
        {
            selectedItemIndex = currentMarkedSlot;

            ActiveWeapon activeWeapon = PlayerController.Instance.GetComponent<ActiveWeapon>();

            if (activeWeapon != null)
            {
                activeWeapon.SetActiveWeapon(items[selectedItemIndex]);
            }
        }

        ToggleInventory();
    }

    public void ShowInventory(bool visible)
    {
        inventoryUI.SetActive(visible);

        if (selectedItemIndex > -1)
        {
            currentMarkedSlot = selectedItemIndex;
            SetSelectedSlot(selectedItemIndex);
        }
    }

    // -----------------------------------------------------------------------------
    // AddItem
    //
    // - Añade un itemID a la lista si no existe ya y actualiza el slot de la UI.
    //
    // Notas:
    // - Si no hay slots suficientes en la UI emite un aviso y no añade el item.
    // -----------------------------------------------------------------------------
    public void AddItem(string itemID, Sprite itemImage)
    {
        if (!items.Contains(itemID))
        {
            items.Add(itemID);

            int index = items.Count - 1;

            if (index < slots.Length)
            {
                GameObject slotImage = slots[index].transform.Find("ItemImage").gameObject;
                slotImage.GetComponent<Image>().sprite = itemImage;
                slotImage.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Not enough slots in the inventory UI.");
            }
        }
    }

    public void OnNotify(string itemID)
    {
        SpawnOnceSO data = ResourcesManager.Instance.GetResource(itemID);

        if (data == null)
        {
            Debug.LogError($"No se encontró SpawnOnceSO para el item {itemID}");
            return;
        }

        if (data.itemType == SpawnOnceSO.SpawnOnceType.Weapon)
        {
            SpawnOnceWeaponSO weaponData = data as SpawnOnceWeaponSO;
            AddItem(weaponData.itemID, weaponData.weaponSprite);
        }
    }

    // -----------------------------------------------------------------------------
    // ToggleInventory
    //
    // - Alterna la visibilidad del inventario, pausa/reanuda el tiempo y cambia
    //   el mapa de controles según el estado resultante.
    // -----------------------------------------------------------------------------
    public void ToggleInventory()
    {
        IsInventoryOpen = !IsInventoryOpen;

        if (IsInventoryOpen)
        {
            Time.timeScale = 0;
            ShowInventory(true);
            InputManager.Instance.SwitchMap(ControlMap.Inventory);
        }
        else
        {
            ShowInventory(false);
            Time.timeScale = 1;
            InputManager.Instance.SwitchMap(ControlMap.Player);
        }
    }

    public void UnselectCurrentItem()
    {
        if (selectedItemIndex > -1)
        {
            GameObject activeImage = slots[selectedItemIndex].transform.Find("Active").gameObject;
            activeImage.SetActive(false);

            selectedItemIndex = -1;
            currentMarkedSlot = -1;
        }
    }
}