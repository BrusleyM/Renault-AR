using System.Collections.Generic;
using Common.Interfaces;
using Common.Objects;

namespace Tests
{
    public class MockTouchProvider : ITouchProvider
    {
        private List<CommonTouch> _touches = new();

        public void SetTouches(params CommonTouch[] touches)
        {
            _touches = new List<CommonTouch>(touches);
        }

        public IReadOnlyList<CommonTouch> GetActiveTouches()
        {
            return _touches;
        }
    }
}