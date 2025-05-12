using System;
using UnityEngine.UI;
namespace Extensions.UI
{
    public class ScrollAwareDropdown : Dropdown
    {
        public Action OnDropdownOpened;

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnDropdownOpened?.Invoke();
        }
    }
}
