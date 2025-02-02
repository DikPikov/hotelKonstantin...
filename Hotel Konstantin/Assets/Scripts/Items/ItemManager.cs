using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private GameObject[] ItemObjectPrefab;
    [SerializeField] private GameObject[] ItemModelPrefab;

    [SerializeField] private AudioSource DropSound;

    [SerializeField] private GameObject DropButton;

    [SerializeField] private Player Player;
    [SerializeField] private Image[] ItemsIcons;
    [SerializeField] private Text[] ItemsBinds;

    private void Start()
    {
        Player.OnItemChanges += UpdateItemInfo;
        UpdateItemInfo();
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Item1))
        {
            SelectItem(0);
        }
        else if (InputManager.GetButtonDown(InputManager.ButtonEnum.Item2))
        {
            SelectItem(1);
        }
        else if (InputManager.GetButtonDown(InputManager.ButtonEnum.Item3))
        {
            SelectItem(2);
        }
        else if (InputManager.GetButtonDown(InputManager.ButtonEnum.DropItem))
        {
            DropItem();
        }
    }

    public void SelectItem(int index)
    {
        SetPlayerCurrentItem(Player._Items.Length > index ? Player._Items[index] : null);
    }

    public void DropItem()
    {
        if (Player._CurrentItem != null)
        {
            Item item = Player._CurrentItem._Item;

            Transform pick = SpawnItem(item).transform;
            pick.transform.position = Player.transform.position;
            pick.transform.rotation = Player.transform.rotation;

            DropSound.pitch = Random.Range(0.9f, 1.1f);
            DropSound.Play();

            Player.ApplyItem(item, true);
        }
    }

    public ItemPick SpawnItem(Item item)
    {
        ItemPick pick = new GameObject().AddComponent<ItemPick>();
        pick.SetInfo(item);

        foreach (GameObject gameObject in ItemModelPrefab)
        {
            if (gameObject.name == item._Prefab + "Model")
            {
                Instantiate(gameObject, pick.transform);
                break;
            }
        }

        return pick;
    }

    private void SetPlayerCurrentItem(Item item)
    {
        if(Player._CurrentItem != null && Player._CurrentItem._Item == item)
        {
            Player._CurrentItem = null;
            return;
        }
         
        if(item == null)
        {
            Player._CurrentItem = null;
            return;
        }

        ItemObject itemObject = null;
        foreach(GameObject prefab in ItemObjectPrefab)
        {
            if(prefab.name == item._Prefab)
            {
                itemObject = Instantiate(prefab, Player.transform).GetComponent<ItemObject>();
                break;
            }
        }

        if(itemObject != null)
        {
            itemObject.SetInfo(Player, item);
            Player._CurrentItem = itemObject;
        }
    }

    public void UpdateItemInfo()
    {
        DropButton.SetActive(Player._CurrentItem != null);

        ItemsBinds[0].text = InputManager._Instance._KeyMap.Item1.Replace("Alpha", "");
        ItemsBinds[1].text = InputManager._Instance._KeyMap.Item2.Replace("Alpha", "");
        ItemsBinds[2].text = InputManager._Instance._KeyMap.Item3.Replace("Alpha", "");

        for (int i = 0; i < 3; i++)
        {
            if (i < Player._Items.Length && Player._Items[i] != null)
            {
                ItemsIcons[i].gameObject.SetActive(true);
                ItemsIcons[i].sprite = Player._Items[i]._Icon;
            }
            else
            {
                ItemsIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
