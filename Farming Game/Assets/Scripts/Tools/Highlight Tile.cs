using UnityEngine;

public class HighlightTile : MonoBehaviour
{
    public Grid grid;
    public Camera mainCamera;
    public Transform highlightTile;

    public ToolSelector toolSelector;
    public FarmManager farmManager;

    private SpriteRenderer spriteRenderer;

    public Vector3Int currentTile { get; private set; }

    private void Start()
    {
        spriteRenderer = highlightTile.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (grid == null || mainCamera == null || highlightTile == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        currentTile = grid.WorldToCell(mouseWorldPos);
        Vector3 tileCenter = grid.GetCellCenterWorld(currentTile);

        highlightTile.position = tileCenter;

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
       if (spriteRenderer == null || farmManager == null) return;

        bool isValid = farmManager.IsFarmable(currentTile);

        if (isValid)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }


    }
}
