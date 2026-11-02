using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework;

public class SimpleInventory : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField]private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI[] slotText;



    private List<InventoryItem> items = new List<InventoryItem>();

    private Playercontroller playercontroller;
    private PlayerPickup PlayerPickup;
    private ThirdPersonCamera thirdPersonCamera;

    private bool isOpen;

    private void Awake()
    {
        playercontroller = GetComponent<Playercontroller>();
        PlayerPickup = GetComponent<PlayerPickup>();

        if (Camera.main != null )
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        inventoryPanel.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

    }

    private void ToggleInventory()
    {
        if (isOpen == false && playercontroller.IsNormalState() == false)
        {
            return;
        }

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            playercontroller.ChangeState(PlayerState.inventory);
            PlayerPickup.enabled = false;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playercontroller.ChangeState(PlayerState.Nomal);
            PlayerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
    }


    // Update is called once per frame
    void UpdateUI()
    {
        for(int i = 0; i < slotText.Length; i++)
        {
            if (i < items.Count)
            {
                slotText[i].text = items[i].itemName + "\n" + items[i].amount + "개";
            }
            else
            {
                slotText[i].text = "비어 있음";
            }
        } 
    }

    public bool AddIteam(string itemName, int amount)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUI();
                return true;
            }
        }

        if (items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찾습니다.");
            return false;
        }
        
        InventoryItem newItem = new InventoryItem();
        newItem.itemName = itemName;
        newItem.amount = amount;

        items.Add(newItem);
        UpdateUI();

        return true;



    }


}
