using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Linq.Expressions;

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
