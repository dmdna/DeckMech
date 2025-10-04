using UnityEngine;
using UnityEngine.UI;

public class RobotPart : MonoBehaviour
{
    public Image baseRenderer;
    public Image detailRenderer;

    public void SetSprites(Sprite baseSprite, Sprite detailSprite, bool flipX = false)
    {
        if (baseRenderer != null) baseRenderer.sprite = baseSprite;
        if (detailRenderer != null) detailRenderer.sprite = detailSprite;

        // flip horizontally if needed
        Vector3 scale = transform.localScale;
        scale.x = flipX ? -1 : 1;
        transform.localScale = scale;
    }
}
