using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LuaInterface;
using LuaFramework;
using SimpleFramework;
using sw.util;
//[ExecuteInEditMode]
namespace User
{
    public class UIGrid : UIList
    {
        // Use this for initialization

        public GameObject rendererObject;
        public GridLayoutGroup grid;
        public ScrollRect scroll;
        RectTransform scrollRt;


        protected override void Awake()
        {
            base.Awake();
            if (scroll == null)
            {
                scroll = gameObject.GetComponentInParent<ScrollRect>();
            }


            if (scroll != null)
            {
                scrollRt = scroll.GetComponent<RectTransform>();
            }
            if (rendererObject == null)
            {
                Transform t = gameObject.transform.Find("Renderer");
                if(t!= null)
                rendererObject = t.gameObject;
            }
			if (rendererObject != null) {
				rendererObject.SetActive (false);
			}
            if (listContainer == null)
            {
                listContainer = gameObject.transform.Find("Grid") as RectTransform;
            }
            if (listContainer != null)
            {
                if (grid == null)
                {
                    grid = listContainer.GetComponent<GridLayoutGroup>();
                }
            }
        }




        protected override IEnumerator OnDrawCompleted()
        {
            yield return 0; // 延迟一帧自动计算

            if (scroll)
            {

                Rect rect = listContainer.rect;
                if (scroll.horizontalScrollbar && grid.minWidth > rect.width)
                    scroll.horizontalScrollbar.gameObject.SetActive(true);
                if (scroll.verticalScrollbar && grid.minHeight > rect.height)
                    scroll.verticalScrollbar.gameObject.SetActive(true);
            }
            DrawCompleted = true;
            
            yield return base.OnDrawCompleted();
        }


        protected override GameObject CreateItem(RectTransform parent)
        {
            if (rendererObject)
            {
				GameObject obj = GameObjectUtil.Instantiate(rendererObject, parent, false);                
				obj.SetActive (true);
				return obj;
            }
            return null;
        }
        protected override void ClearList(int count_)
        {
            base.DestroyList();
            base.ClearList(count_);
        }
        public override Vector3 GetRendererPositionSameRoot(int index)
        {
            Vector3 p = base.GetRendererPositionSameRoot(index);
            return p + listContainer.localPosition;
        }

    }

}
