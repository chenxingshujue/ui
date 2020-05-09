using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace User
{
    public class UIDropdown : UIBehaviour, IPointerClickHandler
    {
        public GridAndLoop.OnInitializeItem onInitializeItem
        {
            set
            {
                if (list != null)
                {
                    list.onInitializeItem = value;
                }
            }
        }
        public System.Action<GameObject, int, int> OnRendererClick;
        public System.Action<GameObject, int,int,bool> OnRendererSelect
        {
            set
            {
                if (list != null)
                {
                    list.OnRendererSelect = value;
                }
            }
        }
		public int selectedIndex {
			get{ 
				if (list != null)
				{
					return list.selectedIndex;
				}
				return -1;
			}
		}

        public System.Action OnListWillOpen;

        public PointerClick mode;
        public UIList list;
        public int count = 0;
        private int _selectedIndex = -1;
        void Awake()
        {
            list.OnRendererClick = OnListRendererClick;
            mode.OnClick.AddListener(OnMaskClick);
            mode.gameObject.SetActive(false);
        }

        public void DrawList(int count)
        {
            this.count = count;
        }
        void OnMaskClick(PointerEventData eventData)
        {
            mode.gameObject.SetActive(false);
        }

        void OnListRendererClick(GameObject go, int index, int realindex)
        {
            mode.gameObject.SetActive(false);
            if (OnRendererClick != null)
            {
                OnRendererClick(go, index, realindex);
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnListWillOpen != null)
            {
                OnListWillOpen();
            }
            this.Open();
        }
        public void Open()
        {
            if (count <= 0) return;
            mode.gameObject.SetActive(true);
            if (list != null)
            {
                if(_selectedIndex < 0)
                {
                    list.DrawList(count);
                }else
                {
                    list.DrawListAndSelect(count, _selectedIndex);
                    _selectedIndex = -1;
                }
            }
        }

        public virtual void Select(int realindex)
        {
            _selectedIndex = realindex;
            if (list != null && mode.gameObject.activeSelf)
            {
                list.Select(realindex);
            }
        }

    }


}
