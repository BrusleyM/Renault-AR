using System.Collections.Generic;
using Common.Interfaces;
using Common.Objects;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Services
{
    public class RuntimeTouchProvider : ITouchProvider
    {
        public IReadOnlyList<CommonTouch> GetActiveTouches()
        {
            List<CommonTouch> touches = new();
            foreach (var finger in Touch.activeTouches)
            {
                touches.Add(new CommonTouch(finger.screenPosition));
            }
            return touches;
        }
    }
}
