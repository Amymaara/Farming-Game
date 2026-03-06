using System.IO;
using Unity.Cinemachine;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    private void Start()
    {
        LoadGame();
    }

    private void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (player == null)
        {
            Debug.LogError("Save failed: Player not found.");
            return;
        }

        if (confiner == null)
        {
            Debug.LogError("Save failed: Confiner not found.");
            return;
        }

        if (confiner.BoundingShape2D == null)
        {
            Debug.LogError("Save failed: BoundingShape2D is null.");
            return;
        }

        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = confiner.BoundingShape2D.gameObject.name
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveLocation, json);

        Debug.Log("Game saved.");
        Debug.Log(json);
    }

    public void LoadGame()
    {
        Debug.Log("LoadGame called.");

        if (!File.Exists(saveLocation))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(saveLocation);
        Debug.Log("Raw JSON: " + json);

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (saveData == null)
        {
            Debug.LogError("Load failed: saveData is null.");
            return;
        }

        Debug.Log("Loaded playerPosition: " + saveData.playerPosition);
        Debug.Log("Loaded mapBoundary: " + saveData.mapBoundary);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Load failed: Player not found.");
            return;
        }

        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner == null)
        {
            Debug.LogError("Load failed: Confiner not found.");
            return;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = saveData.playerPosition;
            Debug.Log("Player moved with Rigidbody2D.");
        }
        else
        {
            player.transform.position = saveData.playerPosition;
            Debug.Log("Player moved with transform.");
        }

        GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
        if (boundaryObject == null)
        {
            Debug.LogError("Load failed: Boundary object not found: " + saveData.mapBoundary);
            return;
        }

        PolygonCollider2D boundary = boundaryObject.GetComponent<PolygonCollider2D>();
        if (boundary == null)
        {
            Debug.LogError("Load failed: No PolygonCollider2D on " + boundaryObject.name);
            return;
        }

        confiner.BoundingShape2D = boundary;
        confiner.InvalidateBoundingShapeCache();

        Debug.Log("Camera boundary updated to: " + boundary.name);
        Debug.Log("Load complete.");
    }

}

