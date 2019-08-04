using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    ALLY,
    WEAPON
}

[System.Serializable]
public struct ShopItem
{
    public GameObject Item;
    public GameObject Icon;
    public ItemType Type;
    [Range(0, 16)] public int Cost;
    [System.NonSerialized] public GameObject Copy;
    [System.NonSerialized] public Vector2Int Position;
}

public class Shop : Singleton<Shop>
{
    [SerializeField] ShopItem[] m_ShopItems = null;
    [SerializeField] Transform m_TreeRoot = null;
    [SerializeField] Transform m_RegularLane = null;
    [SerializeField, Range(1, 4)] int m_MinItems = 2;
    [SerializeField, Range(1, 4)] int m_MaxItems = 4;
    [SerializeField] SpriteRenderer m_WonIcon = null;
    [SerializeField] Color m_AffordColor = Color.yellow;
    [SerializeField] Color m_NoMoneyColor = Color.red;
    [SerializeField, Range(0.0f, 2.0f)] float m_IconScale = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)] float m_MarginW = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] float m_MarginH = 0.3f;
    [SerializeField, Range(0.0f, 1.0f)] float m_PaddingW = 0.33f;
    [SerializeField, Range(0.0f, 1.0f)] float m_PaddingH = 0.66f;
    [SerializeField] GameObject m_ShopIcon = null;

    public bool Visiting { get; set; }

    List<ShopItem> m_CurrentItems;
    GameObject m_TempIcons = null;
    int m_LastX = -1;

    private void Start()
    {
        m_CurrentItems = new List<ShopItem>();
        m_TreeRoot.position = m_RegularLane.position;
    }

    public void Visit()
    {
        Visiting = true;
        m_TreeRoot.gameObject.SetActive(true);
        m_RegularLane.gameObject.SetActive(false);
        WarriorBeat.Instance.PlayShopMusic();
    }

    public void Refresh()
    {
        ShopItem[] items = m_CurrentItems.ToArray();
        foreach (ShopItem i in items)
        {
            Destroy(i.Copy);
        }
        m_CurrentItems.Clear();

        int numOfItems = Random.Range(m_MinItems, m_MaxItems); // No +1 on max because there is always 1 ally

        AddItem(0);
        for (int i = 0; i < numOfItems; i++)
        {
            AddItem();
        }
        m_TreeRoot.gameObject.SetActive(false);
        m_RegularLane.gameObject.SetActive(true);
        //Visit();
    }

    private void Update()
    {
        if (!Visiting)
        {
            m_ShopIcon.SetActive(false);
            return;
        }

        if (MakazeClan.Instance.Enemy.Position.x == 2)
        {
            // Display exit
            m_ShopIcon.SetActive(true);
        }
        else
        {
            m_ShopIcon.SetActive(false);
        }

        if (MakazeClan.Instance.Enemy.Position.x != m_LastX)
        {
            Destroy(m_TempIcons);
            foreach (ShopItem shopItem in m_CurrentItems)
            {
                if (shopItem.Position.x - MakazeClan.Instance.Enemy.Position.x == 0)
                {
                    // Display each item cost
                    bool canAfford = MakazeClan.Instance.Enemy.Won > shopItem.Cost;
                    ShowCost(shopItem, canAfford);
                    break;
                }
            }
        }

        m_LastX = MakazeClan.Instance.Enemy.Position.x;
    }

    void ShowCost(ShopItem item, bool canAfford)
    {
        Vector3 start = BattleGrid.Instance.GetWorldPosition(item.Position) + BattleGrid.Instance.TileOffset;
        m_TempIcons = new GameObject("Temp Icons");
        m_TempIcons.transform.parent = m_TreeRoot;
        for (int i = 0; i < item.Cost; i++)
        {
            int x = i + 1;
            Vector3 v = start + Vector3.up * x;
            CreateSprite(m_TempIcons.transform, v, canAfford);
        }
    }

    void CreateSprite(Transform parent, Vector3 position, bool canAfford)
    {
        SpriteRenderer s = Instantiate(m_WonIcon, parent);
        s.transform.position = position;
        s.transform.localScale = Vector3.one;
        if (canAfford) s.color = m_AffordColor;
        else s.color = m_NoMoneyColor;
    }

    public void BuyItem()
    {
        ShopItem item = default;
        bool found = false;
        foreach (ShopItem shopItem in m_CurrentItems)
        {
            if (shopItem.Position.x - MakazeClan.Instance.Enemy.Position.x == 0)
            {
                found = true;
                item = shopItem;

            }
        }

        if (!found || item.Cost > MakazeClan.Instance.Enemy.Won)
        {
            // TODO Play error sound
            
            return;
        }

        GameObject itemCopy = Instantiate(item.Item, null);

        switch (item.Type)
        {
            case ItemType.ALLY:
                Ally ally = itemCopy.GetComponent<Ally>();
                ally.transform.parent = null;
                won won = MakazeClan.Instance.Enemy;
                Vector2Int p = won.Position;
                Ally cur = won.GetAllyOnTile(p);
                while (cur != null)
                {
                    p += Vector2Int.right;
                    cur = won.GetAllyOnTile(p);
                }
                ally.Move(p - ally.Position);
                ally.m_StartingColumn = p.x;
                ally.StartingPosition = p;
                ally.ShopItem = false;
                won.Allies.Add(ally);
                break;
            case ItemType.WEAPON:
                MakazeClan.Instance.Enemy.EquipWeapon(itemCopy.GetComponent<Weapon>());
                break;
            default:
                break;
        }

        print("Bought Item");
        m_CurrentItems.Remove(item);
        Destroy(item.Copy);
        m_LastX = -1;
        MakazeClan.Instance.Enemy.SpendWon(item.Cost);
    }

    void AddItem(int index = -1)
    {
        int i = index < 0 ? Random.Range(1, m_ShopItems.Length) : index;

        ShopItem shopItem = m_ShopItems[i];
        int x = Random.Range(0, BattleGrid.Instance.Bounds.x);
        while (x == 2 || m_CurrentItems.Where(si => si.Position.x == x).Count() > 0)
        {
            x++;
            x %= BattleGrid.Instance.Bounds.x + 1;
        }

        shopItem.Position = new Vector2Int(x, BattleGrid.Instance.Bounds.y);
        shopItem.Copy = Instantiate(shopItem.Icon, m_TreeRoot);
        shopItem.Copy.transform.position = BattleGrid.Instance.GetWorldPosition(shopItem.Position) + BattleGrid.Instance.TileOffset;
        //if (shopItem.Type == ItemType.ALLY)
        //{
        //    shopItem.Copy.GetComponent<Ally>().StartingPosition = shopItem.Position;
        //    shopItem.Copy.GetComponent<Ally>().ShopItem = true;
        //}
        //else shopItem.Copy.transform.position += new Vector3(0.3f, 0.2f);

        m_CurrentItems.Add(shopItem);
    }

    public void Exit(bool died)
    {
        m_TreeRoot.gameObject.SetActive(false);
        m_RegularLane.gameObject.SetActive(true);
        if (!died) Game.Instance.PlayNextLevel();
        else Game.Instance.ReplayCurrentLevel();
        WarriorBeat.Instance.PlayNormalMusic();
    }
}
