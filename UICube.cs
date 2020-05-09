using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace User
{

    //[ExecuteInEditMode]
    public class UICube :UIList{
	    // Use this for initialization
	    public const string item_ = "Item";
        override protected void Awake()
	    {
            base.Awake();
            if (listContainer == null)
            {
                listContainer = gameObject.GetComponent<RectTransform>();
            }
	    }
	    // Update is called once per frame
	    public void DrawList()
	    {
            int count_ = 0;
            while(listContainer.Find(item_ + count_) != null)
            {
                count_++;
            }
            DrawList(count_);
	    }

        protected override GameObject CreateItem(RectTransform parent)
	    {
            Transform item = listContainer.Find(item_ + drawIndex);
            if (item != null) return item.gameObject;
            return null;
	    }

	    protected override void ClearList(int count)
	    {
	    }

    }
}
