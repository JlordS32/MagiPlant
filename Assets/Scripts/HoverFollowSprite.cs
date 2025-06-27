using UnityEngine;

public class HoverFollowSprite : MonoBehaviour
{
    public Sprite followSprite;
    public float angleZ = 35f;
    public float spriteDepth = 10f;

    private GameObject followingObject;

    void OnMouseEnter()
    {
        if (followSprite != null)
        {
            followingObject = new GameObject("FollowSprite");
            SpriteRenderer spriteRenderer = followingObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = followSprite;
            spriteRenderer.sortingLayerName = "foreground";
            spriteRenderer.sortingOrder = 100;
            followingObject.transform.position = transform.position + Vector3.forward * spriteDepth;
            followingObject.transform.rotation = Quaternion.Euler(0, 0, angleZ); // angled the sprite
        }
    }

    void OnMouseOver()
    {
        if (followingObject != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = spriteDepth; 
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            followingObject.transform.position = worldPosition;
        }
    }

    void OnMouseExit()
    {
        if (followingObject != null)
        {
            Destroy(followingObject);
        }
    }
}
