using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap farmTilemap;
    public Tilemap farmableTilemap;

    public TileBase tilledTile;
    public TileBase wateredTile;

    public void HoeTile(Vector3Int pos)
    {
        Debug.Log("Ground tilemap: " + groundTilemap.name);
        Debug.Log("Farm tilemap: " + farmTilemap.name);

        if (!groundTilemap.HasTile(pos)) return;

        if (farmTilemap.GetTile(pos) == null)
        {
            farmTilemap.SetTile(pos, tilledTile);
        }
        
    }

    public void WaterTile(Vector3Int pos)
    {
        if (!groundTilemap.HasTile(pos)) return;

        if (farmTilemap.GetTile(pos) == tilledTile)
        {
            farmTilemap.SetTile(pos, wateredTile);
        }

    }

    public bool IsFarmable(Vector3Int cellPos)
    {
        bool isInFarmableArea = farmableTilemap != null && farmableTilemap.HasTile(cellPos);
        bool isAlreadyFarmTile = farmTilemap != null && farmTilemap.HasTile(cellPos);

        return isInFarmableArea || isAlreadyFarmTile;
    }

}
