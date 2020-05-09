using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
namespace User
{
    public class PointerClick : UIBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick.Invoke(eventData);
				EventDispatcher.DispatchEvent(UIEventConst.CLICK_UGUI_JUST, gameObject);
            }
        }
        [Serializable]
        public class ClickedEvent : UnityEvent<PointerEventData>
        {
        }
        public ClickedEvent OnClick = new ClickedEvent();
    }

}
