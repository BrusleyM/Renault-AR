using System.Collections.Generic;
using Common.Objects;

namespace Common.Interfaces
{
    public interface ITouchProvider
    {
        IReadOnlyList<CommonTouch> GetActiveTouches();
    }
}
