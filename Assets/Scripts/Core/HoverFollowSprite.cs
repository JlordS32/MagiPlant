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
            spriteRenderer.sortingLayerName = "UIWorld";
            spriteRenderer.sortingOrder = 100;
            followingObject.transform.SetPositionAndRotation(transform.position + Vector3.forward * spriteDepth, Quaternion.Euler(0, 0, angleZ));
        }
    }

    void OnMouseOver()
    {
        if (followingObject != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = spriteDepth;
            followingObject.transform.position = mouseWorldPos;
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
