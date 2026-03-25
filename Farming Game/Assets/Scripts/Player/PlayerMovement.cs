using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.InputSystem;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public InputActionReference actionInput;

    public enum ToolType 
    {
        hoe,
        wateringCan,
        seeds,
        basket,
    }

    public ToolType currentTool;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }
        rb.linearVelocity = moveInput * movementSpeed;
        animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0);

 
        
       
       
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        

        if (ctx.canceled)
        {
            animator.SetBool("isWalking", false);
        }
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }

   

}
