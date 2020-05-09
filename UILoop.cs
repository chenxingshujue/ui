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
    public class UILoop : UIList
    {
        // Use this for initialization

        public GameObject rendererObject;
        public GridAndLoop loop;
        public ScrollRect scroll;
        RectTransform scrollRt;
        [System.NonSerialized]
        bool hasDrawLoop = false;

        int dataNum_;
        public override int count
        {
            set
            {
                dataNum_ = value;
                if (loop != null)
                {
                    loop.dataNum = value;
                }
            }
        }
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
				if (t != null) {
					rendererObject = t.gameObject;
				}
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
                if (loop == null)
                {
                    loop = listContainer.GetComponent<GridAndLoop>();
                }

            }
        }

        void DrawLoop()
        {
            if (hasDrawLoop == false && loop != null && scrollRt != null)
            {


                //初始化行列数
                int rows = 1, cols = 1;
                if (loop.arrangeType == GridAndLoop.ArrangeType.Vertical) //垂直排列 则适应行数
                {
                    rows = loop.ConstraintCount;
                    cols = (int)Mathf.Ceil((float)scrollRt.rect.width / (float)loop.cell_x) + 1;
                }
                else if (loop.arrangeType == GridAndLoop.ArrangeType.Horizontal) //水平排列则适应列数
                {
                    cols = loop.ConstraintCount;
                    rows = (int)Mathf.Ceil((float)scrollRt.rect.height / (float)loop.cell_y) + 1;
                }
                base.DrawList(rows * cols);

                hasDrawLoop = true;
            }

        }
        [ContextMenu("Redraw")]
        override public void Redraw()
        {
            this.scrollToSelected_ = false;
            this.DrawList(loop.dataNum);
        }


        public override void DrawList(int count_)
        {
            if (loop != null)
            {
                loop.dataNum = count_;
                loop.onInitializeItem = this.OnInitializeLoopItem;
                if (DrawCompleted)
                {
                    loop.easeValue = 0;
                    DrawSelect(true,this.scrollToSelected_);
                }
                else
                {
                    DrawLoop();
                }
            }
        }
        public override void DrawListAndSelect(int count_, int realindex_)
        {
            if (loop != null)
            {
                loop.dataNum = count_;
                loop.onInitializeItem = this.OnInitializeLoopItem;
                this.SetSelectIndex(realindex_);
                if (DrawCompleted)
                {
                    DrawSelect(true, this.scrollToSelected_);
                }
                else
                {
                    DrawLoop();
                }
            }
        }
        public override void DrawListDelay(int count_, float delay_ = 0)
        {
            this.DrawList(count_);
        }

        /// <summary>
        /// ScrollRect复位
        /// </summary>
        [ContextMenu("ResetPosition")]
        public void ResetPosition()
        {
            if (loop != null)
            {
                loop.dataNum = dataNum_;
                loop.onInitializeItem = this.OnInitializeLoopItem;
                if (DrawCompleted)
                {
                    loop.ResetPosition();
                }
                else
                {
                    DrawLoop();
                }
            }
        }
        
        protected override void DrawSelect(bool update ,bool scrollToSelected)
        {
            if (loop != null)
            {
                loop.SetSelectIndex(selectedIndex,update, scrollToSelected);
            }
            base.DrawSelect(update,scrollToSelected);
        }


        protected override void DrawRenderer(GameObject renderer, int index)
        {
        }
        private void OnInitializeLoopItem(GameObject renderer, int index, int realIndex)
        {
            if (onInitializeItem != null) onInitializeItem(renderer, index, realIndex);
            if (OnLuaInitializeItem != null) OnLuaInitializeItem(this.GetRendererLua(index), index, realIndex);
        }


        protected override int Index2RealIndex(int index)
        {
            if (loop != null)
            {
                return loop.Index2RealIndex(index);
            }
            return base.Index2RealIndex(index);
        }

		protected override int GetRealCount ()
		{
			if (loop != null)
			{
				return loop.dataNum;
			}
			return this.dataNum_;
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
