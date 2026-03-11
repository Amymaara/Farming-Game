using UnityEngine;

public class RewardsController : MonoBehaviour
{
    public static RewardsController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveQuestReward(Quests quest)
    {
        if(quest?.questRewards == null) return;

        foreach (var reward in quest.questRewards)
        {
            switch (reward.type)
            {
                case RewardType.item:
                    //give reward
                    GiveItemReward(reward.RewardID, reward.amount);
                    break;

                case RewardType.experience:
                    //give exp
                    break;

                case RewardType.gold:
                    //give gold
                    break;

                case RewardType.custom:
                    // give custom reward
                    break;
            }
        }
    }

    public void GiveItemReward(int itemID, int amount)
    {
        var itemPrefab = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab(itemID);

        if (itemPrefab == null) return;

        for (int i = 0; i < amount; i++)
        {
           if (!InventoryController.Instance.AddItem(itemPrefab))
            {
                GameObject dropItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
                dropItem.GetComponent<BounceEffect>().StartBounce();
            }

           else
            {
                // show notification popup
                Item item = itemPrefab.GetComponent<Item>();
                if (item != null && ItemPickupUIController.Instance != null)
                {
                    Sprite icon = null;

                    SpriteRenderer sr = itemPrefab.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        icon = sr.sprite;
                    }
                    else
                    {
                        UnityEngine.UI.Image img = itemPrefab.GetComponent<UnityEngine.UI.Image>();
                        if (img != null)
                        {
                            icon = img.sprite;
                        }
                    }

                    ItemPickupUIController.Instance.ShowItemPickup(item.Name, icon);
                }
            }
        }
    }
}
