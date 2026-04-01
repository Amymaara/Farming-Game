using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).
public class Chest : MonoBehaviour, IInteractable
{

    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;
    public int quantity = 1;
    public Sprite openedSprite;
    
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    void Update()
    {
        
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        Debug.Log("Chest interacted");

        if (!CanInteract()) return;
        OpenChest();

    }

    private void OpenChest()
    {
        SetOpened(true);
        SoundManager.PlaySound(SoundType.CHEST);

        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);

            Item item = droppedItem.GetComponent<Item>();
            if (item != null)
            {
                item.quantity = quantity;
            }

            BounceEffect bounce = droppedItem.GetComponent<BounceEffect>();
            if (bounce != null)
            {
                bounce.StartBounce();
            }
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
