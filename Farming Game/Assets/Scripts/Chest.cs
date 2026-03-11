using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{

    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;
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
        // set opened 
        SetOpened(true);

        // drop item
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab,transform.position + Vector3.down, Quaternion.identity);
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
