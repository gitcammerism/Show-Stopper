using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleDefinition definition;
    public int hitPoints;                            // only applicable if tile can be destroyed
    
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] damageSprites; // only applicable if tile can be destroyed
    
    public bool BlocksSwap => definition.blocksSwap;
    public bool FallsWithGravity => definition.fallsWithGravity;

    public void Init(ObstacleDefinition def)
    {
        definition = def;
        hitPoints = def.hitPoints;
        UpdateVisual();
    }

    public bool TakeHit(int damage = 1)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        UpdateVisual();
        return false;
    }

    private void UpdateVisual()
    {
        if (damageSprites != null && damageSprites.Length > 0 && spriteRenderer != null)
        {
            int index = Mathf.Clamp(hitPoints - 1, 0, damageSprites.Length - 1);
            spriteRenderer.sprite = damageSprites[index];
        }
    }
}
