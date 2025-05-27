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

    // Новые поля для кнопок
    public Button openInventoryButton; // Кнопка открытия инвентаря
    public Button closeInventoryButton; // Кнопка-крестик для закрытия

    private bool isInventoryOpen = false;

    public void Start()
    {
        if (items == null || items.Count == 0)
        {
            AddGraphics();
        }

        for (int i = 0; i < maxCount; i++) // Тестовое наполнение
        {
            AddItem(i, data.items[Random.Range(0, data.items.Count)], Random.Range(1, 99));
        }
        UpdateInventory();

        // Инициализация кнопок
        openInventoryButton.onClick.AddListener(ToggleInventory);
        closeInventoryButton.onClick.AddListener(ToggleInventory);

        // Убедимся, что инвентарь изначально скрыт
        backGround.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        if (currentID != -1)
        {
            MoveObject();
        }
    }

    // Переключение состояния инвентаря
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        backGround.SetActive(isInventoryOpen);

        // Пауза/возобновление игры
        Time.timeScale = isInventoryOpen ? 0f : 1f;

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
                if (items[i].count < 128)
                {
                    items[i].count += count;
                    if (items[i].count > 128)
                    {
                        count = items[i].count - 128;
                        items[i].count = 128; // Исправлено с 64 на 128
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
        if (currentID == -1)
        {
            currentID = int.Parse(es.currentSelectedGameObject.name);
            currentItem = CopyInventoryItem(items[currentID]);
            movingObject.gameObject.SetActive(true);
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].img;

            AddItem(currentID, data.items[0], 0);
        }
        else
        {
            ItemInventory ii = items[int.Parse(es.currentSelectedGameObject.name)];
            if (currentItem.id != ii.id)
            {
                AddInventoryItem(currentID, ii);
                AddInventoryItem(int.Parse(es.currentSelectedGameObject.name), currentItem);
            }
            else
            {
                if (ii.count + currentItem.count <= 128)
                {
                    ii.count += currentItem.count;
                }
                else
                {
                    AddItem(currentID, data.items[ii.id], ii.count + currentItem.count - 128);
                    ii.count = 128;
                }

                ii.itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = ii.count.ToString();
            }

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