using UnityEngine;
using UnityEngine.UI;

namespace Helpers.UI
{
    public class DropdownScrollHelper : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public RectTransform dropdownRectTransform;

        public void ScrollToDropdown()
        {
            Canvas.ForceUpdateCanvases();

            var viewport = scrollRect.viewport;
            var content = scrollRect.content;

            Vector3[] dropdownCorners = new Vector3[4];
            dropdownRectTransform.GetWorldCorners(dropdownCorners);

            Vector3[] viewportCorners = new Vector3[4];
            viewport.GetWorldCorners(viewportCorners);

            float dropdownBottomY = dropdownCorners[0].y;
            float viewportBottomY = viewportCorners[0].y;

            float delta = dropdownBottomY - viewportBottomY;

            if (delta < 0) return; 

            Vector2 anchoredPos = content.anchoredPosition;
            anchoredPos.y += delta;
            content.anchoredPosition = anchoredPos;
        }
    }
}