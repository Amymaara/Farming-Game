using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

public class MapTransition : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] PolygonCollider2D newBoundary;

    CinemachineConfiner2D confiner;

    void Start()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
      
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.position = spawnPoint.position;

        confiner.BoundingShape2D = newBoundary;
        confiner.InvalidateBoundingShapeCache();
    }

}
