using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using sw.util;
namespace User
{
    public class UICircleLoop : UIList, IPointerClickHandler,IEndDragHandler,IBeginDragHandler
    {
        public GameObject rendererObject;
        public GridLayoutGroup grid;
        public int selectOffset = 1;
        public int extendCount = 1;
        public bool ScrollOnClick = false;
        public bool ScrollOnDrag = true;
        public bool ForceOneStepOnDrag = true;
        public UnityEvent OnBeginDragHandler = new UnityEvent();
        RectTransform rt;
        Vector3[] corners;
        float centerx;
        float centery;
        RectTransform content;
        private bool hasDrawLoop;
        ScrollRect mScroll;
        CenterOnChildWithoutDrag centerChild;

        int dataNum_;
        int dataIndex_;
        int rows = 1, cols = 1;

        override protected void Awake()
        {
            rt = GetComponent<RectTransform>();
            mScroll = GetComponent<ScrollRect>();
            if (grid == null)
            {
                grid = GetComponentInChildren<GridLayoutGroup>();
            }

            centerChild = GetComponent<CenterOnChildWithoutDrag>();
            if (centerChild != null)
            {
                centerChild.OnCompleted.AddListener(() =>
                {
                    this.DrawListAndSelect(dataNum_, selectedIndex);
                });
            }

            content = grid.GetComponent<RectTransform>();
        }

        protected override void OnPointClick(BaseEventData eventData)
        {
            if(!DrawCompleted){
                return;
            }
            if(centerChild != null && centerChild.IsMoving)
            {
                return;
            }
            int preIndex = selectedIndex;
            base.OnPointClick(eventData);
            if (selectedIndex != preIndex && centerChild)
            {
                PointerEventData evt = eventData as PointerEventData;
                int index = rendererList.IndexOf(evt.pointerPress);
                if (index >= 0)
                {
                    if (centerChild != null)
                    {
                        centerChild.Center(evt.pointerPress.transform as RectTransform);
                    }
                }
            }
        }



        private int GetInvisibleChildCountByOffset(float offset,float cellsize,float spacing)
        {
            if (Mathf.Abs(offset) < cellsize )
            {
                return 0;
            }
            return Mathf.FloorToInt(offset / (cellsize + spacing));
        }
        override protected void OnEnable()
        {
            corners = new Vector3[4];
            rt.GetLocalCorners(corners);
            centerx = (corners[0].x + corners[3].x) * 0.5f;
            centery = (corners[0].y + corners[1].y) * 0.5f;
        }
        protected override IEnumerator OnDrawCompleted()
        {
            yield return base.OnDrawCompleted();
            if(centerChild != null)
            {
                if(selectedIndex >= 0)
                {
                    int index = RealIndex2Index(selectedIndex);
                    centerChild.CenterImmediately(rendererList[index].transform as RectTransform);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!DrawCompleted){
                return;
            }
            if (!ScrollOnClick)
            {
                return;
            }
            if(centerChild != null && centerChild.IsMoving)
            {
                return;
            }
           if((eventData.position -  eventData.pressPosition).magnitude > grid.cellSize.magnitude * 0.5f)
            {
                return;
            }

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out localCursor))
                return;
            if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                if (localCursor.x > centerx + grid.cellSize.x *0.5f)
                {
                    if(grid.startCorner == GridLayoutGroup.Corner.LowerLeft || grid.startCorner == GridLayoutGroup.Corner.UpperLeft)
                    {
                        Step(1,true);
                    }else
                    {
                        Step(-1, true);
                    }
                }
                else if(localCursor.x < centerx - grid.cellSize.x * 0.5f)
                {
                    if (grid.startCorner == GridLayoutGroup.Corner.LowerLeft || grid.startCorner == GridLayoutGroup.Corner.UpperLeft)
                    {
                        Step(-1, true);
                    }
                    else
                    {
                        Step(1, true);
                    }
                }

            }
            else
            {
                if (localCursor.y > centery + grid.cellSize.y * 0.5f)
                {
                    if (grid.startCorner == GridLayoutGroup.Corner.LowerLeft || grid.startCorner == GridLayoutGroup.Corner.LowerRight)
                    {
                        Step(1, true);
                    }
                    else
                    {
                        Step(-1, true);
                    }
                }
                else if(localCursor.y < centery - grid.cellSize.y * 0.5f)
                {
                    if (grid.startCorner == GridLayoutGroup.Corner.LowerLeft || grid.startCorner == GridLayoutGroup.Corner.LowerRight)
                    {
                        Step(-1, true);
                    }
                    else
                    {
                        Step(1, true);
                    }
                }
            }
        }



        public void Step(int step,bool ease)
        {
            int realindex = this.selectedIndex + step;
            if(realindex >= this.dataNum_)
            {
                realindex = 0;
            }
            if (realindex < 0)
            {
                realindex = this.dataNum_ - 1;
            }
            if(centerChild != null && ease)
            {
                if (!centerChild.IsMoving)
                {
                    int index = RealIndex2Index(this.selectedIndex) + step;
                    if (index > 0 && index < rendererList.Count) 
                    {
                        this.SetSelectIndex(realindex);
                        centerChild.Center(rendererList[index].transform as RectTransform);
                    }
                }
            }else{
                this.Select(realindex);
            }
        }


        public override void DrawList(int count_)
        {
            this.dataNum_ = count_;
            if (DrawCompleted)
            {
                base.DrawList(rows * cols);
            }
            else
            {
                DrawLoop();
            }
        }

        public override void Redraw()
        {
            this.DrawListAndSelect(dataNum_, selectedIndex);
        }

        private void DrawLoop()
        {
            if (hasDrawLoop == false)
            {
                //初始化行列数
                if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount) //垂直排列 则适应行数
                {
                    cols = grid.constraintCount;
                    float h = rt.rect.height - grid.padding.vertical;
                    Vector2 cellSize = grid.cellSize;
                    Vector2 spacing = grid.spacing;
                    if (h < 2 * cellSize.y + spacing.y)
                    {
                        rows = 1;
                    }
                    else
                    {
                        rows = Mathf.CeilToInt((h + spacing.y) / (cellSize.y + spacing.y));
                    }
                    rows += extendCount * 2;
                }
                else if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount) //水平排列则适应列数
                {
                    rows = grid.constraintCount;
                    float w = rt.rect.width - grid.padding.horizontal;
                    Vector2 cellSize = grid.cellSize;
                    Vector2 spacing = grid.spacing;
                    if(w < 2 * cellSize.x + spacing.x)
                    {
                        cols = 1;
                    }else
                    {
                        cols = Mathf.CeilToInt((w + spacing.x) / (cellSize.x + spacing.x));
                    }
                    cols += extendCount * 2;
                }
                base.DrawList(rows * cols);

                hasDrawLoop = true;
            }
        }
        public override void Select(int realindex)
        {
            if(mScroll != null)
            {
                mScroll.StopMovement();
            }
            this.DrawListAndSelect(this.dataNum_, realindex);
        }

        protected override bool SetSelectIndex(int realindex)
        {
            bool can_select = base.SetSelectIndex(realindex);
            if (can_select)
            {
                this.dataIndex_ = this.selectedIndex - this.selectOffset; 
            }
            return can_select;
        }

        protected override void DrawSelect(bool update, bool scrollToSelected)
        {
            int index = -1;
            if (OnRendererSelect != null || OnLuaRendererSelect != null || this.OnSelectChange != null)
            {
                for (int i = 0; i < rendererList.Count; i++)
                {
                    int realindex = Index2RealIndex(i);
                    bool selected = selectedIndex == realindex ;
                    if (realindex >= 0)
                    {
                        if (OnRendererSelect != null)
                        {
                            OnRendererSelect(rendererList[i], i, realindex, selected && index < 0);
                        }
                        if (OnLuaRendererSelect != null)
                        {
                            OnLuaRendererSelect(rendererLuaList[i], i, realindex, selected && index < 0);
                        }
                        if (selected)
                        {
                            index = i;
                        }
                    }
                }
            }
            if (OnSelectChange != null && preSelectedIndex != selectedIndex)
            {
                OnSelectChange(index, preSelectedIndex, selectedIndex);
            }
        }

        protected override int Index2RealIndex(int index)
        {
            int i = index + dataIndex_;
            if (i >= dataNum_)
            {
                i = i - dataNum_;
            }
            if( i < 0)
            {
                i = dataNum_ + i;
            }
            return i;
        }

        protected int RealIndex2Index(int realindex)
        {
           return realindex - dataIndex_;
        }

        protected override int GetRealCount()
        {
            return this.dataNum_;
        }

        public override int count
        {
            set
            {
                this.dataNum_ = value;
            }
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


        public void OnEndDrag(PointerEventData eventData)
        {
            if(!ScrollOnDrag){
                return;
            }
            Vector2 cellSize = grid.cellSize;
            Vector2 spacing = grid.spacing;
            RectOffset padding = grid.padding;
            Vector3 offset = eventData.position - eventData.pressPosition;
            int count = 1;
            if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                if (!ForceOneStepOnDrag)
                {
                     count = GetInvisibleChildCountByOffset(offset.x, cellSize.x, spacing.x);
                }else
                {
                    count = offset.x >= 0 ? 1 : -1;
                }
                if (grid.startCorner == GridLayoutGroup.Corner.LowerLeft || grid.startCorner == GridLayoutGroup.Corner.UpperLeft)
                {
                     Step(-count, false);
                }
                else
                {
                    Step(count, false);
                }
            }
            else
            {
                if (!ForceOneStepOnDrag)
                {
                    count = GetInvisibleChildCountByOffset(offset.y, cellSize.y,spacing.y);
                }else
                {
                    count = offset.y >= 0 ? 1 : -1;
                }
                if (grid.startCorner == GridLayoutGroup.Corner.LowerLeft || grid.startCorner == GridLayoutGroup.Corner.UpperLeft)
                {
                    Step(-count, false);
                }
                else
                {
                    Step(count, false);
                }

            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if(!ScrollOnDrag){
                return;
            }
            if(OnBeginDragHandler != null)
            {
                OnBeginDragHandler.Invoke();
            }
        }
    }

}
