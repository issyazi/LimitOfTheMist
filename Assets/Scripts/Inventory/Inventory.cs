using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory : MonoBehaviour
{
    public DataBase data;
    public List<ItemInventory> items = new List<ItemInventory>();
    public GameObject gameObjShow;
    public GameObject InventoryMainObject;
    public int maxCount;
    public Camera cam;
    public EventSystem es;
    public int currentID;
    public ItemInventory currentItem;
    public RectTransform movingObject;
    public Vector3 offset;
    public GameObject backGround;

    public Button openInventoryButton;
    public Button closeInventoryButton;
    public Button trashCanButton;

    private bool isInventoryOpen = false;
    public bool IsInventoryOpen => isInventoryOpen;

    public void Start()
    {
        if (items == null || items.Count == 0)
        {
            AddGraphics();
        }

        UpdateInventory();

        AddClickHandler(openInventoryButton, ToggleInventory);
        AddClickHandler(closeInventoryButton, ToggleInventory);

        backGround.SetActive(false);

        if (GameInput.Instance != null)
        {
            GameInput.Instance.SetCombatEnabled(true);
        }
    }

    private void AddClickHandler(Button button, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDownEntry.callback.AddListener((eventData) => OnButtonPointerDown());
        trigger.triggers.Add(pointerDownEntry);

        button.onClick.AddListener(action);
    }

    private void OnButtonPointerDown()
    {
        if (GameInput.Instance != null)
        {
            Debug.Log("Клик по кнопке UI: временно отключаем Combat");
            GameInput.Instance.SetCombatEnabled(false);
        }
    }

    public void Update()
    {
        if (currentID != -1)
        {
            MoveObject();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        backGround.SetActive(isInventoryOpen);
        Time.timeScale = isInventoryOpen ? 0f : 1f;

        if (GameInput.Instance != null)
        {
            GameInput.Instance.SetCombatEnabled(!isInventoryOpen);
        }

        if (isInventoryOpen)
        {
            UpdateInventory();
        }
    }

    public void SeachForSameItem(Item item, int count)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (items[i].id == item.id)
            {
                if (items[i].count < 16)
                {
                    items[i].count += count;
                    if (items[i].count > 16)
                    {
                        count = items[i].count - 16;
                        items[i].count = 16;
                    }
                    else
                    {
                        count = 0;
                        break;
                    }
                }
            }
        }
        if (count > 0)
        {
            for (int i = 0; i < maxCount; i++)
            {
                if (items[i].id == 0)
                {
                    AddItem(i, item, count);
                    break;
                }
            }
        }
    }

    public void AddItem(int id, Item item, int count)
    {
        items[id].id = item.id;
        items[id].count = count;
        items[id].itemGameObj.GetComponent<Image>().sprite = item.img;

        if (count > 1 && item.id != 0)
        {
            items[id].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        }
        else
        {
            items[id].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public void AddInventoryItem(int id, ItemInventory invItem)
    {
        items[id].id = invItem.id;
        items[id].count = invItem.count;
        items[id].itemGameObj.GetComponent<Image>().sprite = data.items[invItem.id].img;

        if (invItem.count > 1 && invItem.id != 0)
        {
            items[id].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = invItem.count.ToString();
        }
        else
        {
            items[id].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public void AddGraphics()
    {
        for (int i = 0; i < maxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform);
            newItem.name = i.ToString();

            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            newItem.GetComponentInChildren<RectTransform>().localScale = Vector3.one;

            Button tempButton = newItem.GetComponent<Button>();
            tempButton.onClick.AddListener(SelectObject);

            items.Add(ii);
        }
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (items[i].id != 0 && items[i].count > 1)
            {
                items[i].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = items[i].count.ToString();
            }
            else
            {
                items[i].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            items[i].itemGameObj.GetComponent<Image>().sprite = data.items[items[i].id].img;
        }
    }

    public void SelectObject()
    {
        int targetID = int.Parse(es.currentSelectedGameObject.name);

        if (currentID == -1)
        {
            if (items[targetID].id != 0) // Проверка, чтобы не выбирать пустую ячейку
            {
                currentID = targetID;
                currentItem = CopyInventoryItem(items[currentID]);
                movingObject.gameObject.SetActive(true);
                movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].img;
                AddItem(currentID, data.items[0], 0);
            }
        }
        else
        {
            ItemInventory targetItem = items[targetID];
            if (currentID == targetID) // Игнорируем, если перетаскиваем обратно в ту же ячейку
            {
                AddItem(currentID, data.items[currentItem.id], currentItem.count); // Возвращаем предмет
                currentID = -1;
                movingObject.gameObject.SetActive(false);
            }
            else if (targetItem.id == 0 && currentItem.id != 0) // Пустая ячейка
            {
                AddInventoryItem(targetID, currentItem);
                AddItem(currentID, data.items[0], 0);
            }
            else if (currentItem.id != targetItem.id)
            {
                AddInventoryItem(currentID, targetItem);
                AddInventoryItem(targetID, currentItem);
            }
            else
            {
                if (targetItem.count + currentItem.count <= 16)
                {
                    targetItem.count += currentItem.count;
                }
                else
                {
                    AddItem(currentID, data.items[targetItem.id], targetItem.count + currentItem.count - 16);
                    targetItem.count = 16;
                }

                targetItem.itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = targetItem.count.ToString();
            }

            currentID = -1;
            movingObject.gameObject.SetActive(false);
        }
    }

    public void DeleteItem(int id)
    {
        if (id >= 0 && id < maxCount)
        {
            items[id].id = 0;
            items[id].count = 0;
            items[id].itemGameObj.GetComponent<Image>().sprite = null;
            items[id].itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = "";
            UpdateInventory();
            currentID = -1;
            movingObject.gameObject.SetActive(false);
        }
    }

    public void MoveObject()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            movingObject.parent as RectTransform,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );

        movingObject.localPosition = new Vector3(localPoint.x, localPoint.y, 0) + offset;
    }

    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory newItem = new ItemInventory();
        newItem.id = old.id;
        newItem.itemGameObj = old.itemGameObj;
        newItem.count = old.count;
        return newItem;
    }
}

[System.Serializable]
public class ItemInventory
{
    public int id;
    public GameObject itemGameObj;
    public int count;
}