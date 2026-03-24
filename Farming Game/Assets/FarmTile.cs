using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class FarmTile : MonoBehaviour


{
    public enum GrowthStage 
    {
      empty,
      ploughed,
      planted, 
      growth1,
      growth2,
      fullgrown,
    }

    public GrowthStage currentStage;

    public SpriteRenderer spriteRenderer;
    public Sprite spriteTilled;


    void Start()
    {

    }

    private void Update()
    {
       /* if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            AdvanceStage();

            SetSprite();
        }
       */
    }
    public void AdvanceStage()
    {
        currentStage = currentStage + 1;
        Debug.Log($"Current stage = {currentStage}");

        if ((int)currentStage >= 6)
        {
            currentStage = GrowthStage.empty;
        }

    }

    public void SetSprite()
    {
        if (currentStage == GrowthStage.empty)
        {
            spriteRenderer.sprite = null;
        }
        else
        {
            spriteRenderer.sprite = spriteTilled;
        }
    }

    public void PloughSoil()
    {
        if (currentStage == GrowthStage.empty)
        {
            currentStage = GrowthStage.ploughed;

            SetSprite();    
        }
    }
}
