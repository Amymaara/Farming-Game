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
            SoundManager.PlaySoundForDuration(SoundType.HOE, 0.5f);

            if (TutorialProgress.Instance != null && !TutorialProgress.Instance.seedSelectionShown)
            {
                TutorialProgress.Instance.seedSelectionShown = true;

                if (TutorialPopupManager.Instance != null)
                {
                    TutorialPopupManager.Instance.ShowPopup("Equip the seed bag. Right-click to choose a seed, left-click to plant.");
                }
            }
        }
        
    }

    public void WaterTile(Vector3Int pos)
    {
        if (!groundTilemap.HasTile(pos)) return;

        if (farmTilemap.GetTile(pos) == tilledTile)
        {
            farmTilemap.SetTile(pos, wateredTile);
            SoundManager.PlaySoundForDuration(SoundType.WATERING, 0.5f);

        }

    }

    public bool IsFarmable(Vector3Int cellPos)
    {
        bool isInFarmableArea = farmableTilemap != null && farmableTilemap.HasTile(cellPos);
        bool isAlreadyFarmTile = farmTilemap != null && farmTilemap.HasTile(cellPos);

        return isInFarmableArea || isAlreadyFarmTile;
    }


}
