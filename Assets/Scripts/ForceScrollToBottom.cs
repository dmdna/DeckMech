using UnityEngine;
using UnityEngine.UI;

public class ForceScrollToBottom : MonoBehaviour
{
    public ScrollRect scrollRect;

    // Call this method whenever you add new content or want to force scroll to bottom
    public void ScrollToBottom()
    {
        scrollRect.verticalNormalizedPosition = 0f;
    }
}