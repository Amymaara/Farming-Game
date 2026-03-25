using UnityEngine;

public class HighlightTile : MonoBehaviour
{
    public Grid grid;
    public Camera mainCamera;
    public GameObject highlightTile;

    public Vector3Int currentTile {  get; private set; }
    public Vector3 worldPosition {  get; private set; }

    private void Update()
    {
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (grid == null || mainCamera == null || highlightTile == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3Int cellPosition = grid.WorldToCell(mouseWorldPos);
        Vector3 cellWorldPos = grid.GetCellCenterWorld(cellPosition);

        currentTile = cellPosition;
        worldPosition = cellWorldPos;

        highlightTile.transform.position = cellWorldPos;
    }
}
