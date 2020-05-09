using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleFramework;
namespace User{
	public class UserLoopLayout : UIBehaviour {
        public bool alwaysScrollToEndForNew = false;
        public Func<Transform, int, LuaBehaviour> onWillCreateItem;
	    public Action<LuaTable, int> onWillDestroyItem;
	    public Action<LuaTable, int> onInitializeItem;
        private double destroyAllListenId = -1;
		public int maxDataCount {
			get { return maxDataCount_; }
		}


		private Vector3[] scrollPos;
		private UserScrollRect scrollRect;
		private RectTransform scrollTrans;
		private RectTransform viewRectTrans;

	    private int maxDataCount_ = 0;
	    private int itemMaxCount_ = 0;
	    private int pageCount_ = 0;
	    private int dataIndex_ = 0;

	    private float oldViewHeight = 0;
	    private Vector2 oldViewPos = Vector2.zero;


		private List<TempItem> itemList;

		private class TempItem {
			public GameObject go;
			public RectTransform trans;
			public LuaTable luaTable;
			public Vector2 pos;
			public int dataIndex;

			public TempItem() {

			}
		}

		private enum ScrollTo{
			None,
			Top,
			Bottom
		}
		private Vector2 pivot = Vector2.zero;

		

	    public void DrawList(int maxCount, int pageCount, int dataIndex) {
			this.ClearList ();
	        this.maxDataCount_ = maxCount;
	        this.pageCount_ = pageCount;
	        this.dataIndex_ = dataIndex;
	        int index = Mathf.Min(maxCount, dataIndex + pageCount);
	        for (int i = dataIndex; i < index; i++) {
	            Create(i, true);
	        }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(StartScrolling(ScrollTo.Bottom));
            }
        }
		public void DrawListAndScrollToBegin(int maxCount, int pageCount, int dataIndex) {
			this.ClearList ();
			this.maxDataCount_ = maxCount;
			this.pageCount_ = pageCount;
			this.dataIndex_ = dataIndex;
			int index = Mathf.Min(maxCount, dataIndex + pageCount);
			for (int i = dataIndex; i < index; i++) {
				Create(i, true);
			}
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(StartScrolling(ScrollTo.Top));
            }
        }


		[ContextMenu("ClearList")]
		public void ClearList(){
			if (itemList != null) {
				for (int i = 0; i < itemList.Count; i++) {
					if (onWillDestroyItem != null) {
						onWillDestroyItem (itemList [i].luaTable, itemList [i].dataIndex);
					}
				}
				itemList.Clear ();
			}
		}

		private void ScrollToEnd(){
            this.scrollRect.enabled = true;
            this.scrollRect.StopMovement();
			this.scrollRect.verticalNormalizedPosition = 0f;
		}

		private void ScrollToBegin(){
            this.scrollRect.enabled = true;
            this.scrollRect.StopMovement();
            this.scrollRect.verticalNormalizedPosition = 1f;
		}

		private bool WillSetScrollToEnd(){
            ScrollTo to = ScrollTo.None;
            if( viewRectTrans.rect.height <= scrollTrans.rect.height)
            {
                to = ScrollTo.Bottom;
            }
            else if (this.pivot.y == 1)
            {
                if (this.scrollRect.verticalNormalizedPosition > 0.99f)
                {
                    to = ScrollTo.Bottom;
                }
            }else if (this.pivot.y == 0)
            {
			    if (this.scrollRect.verticalNormalizedPosition < 0.01f )
                {
				    to = ScrollTo.Bottom;
			    }

            }
            
            if(to != ScrollTo.None)
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(StartScrolling(to));
                }
                return true;
            }
            return false;
		}


		public void AddItem(int maxCount,int pageCount, int createCount) {
			createCount = Mathf.Min (createCount, maxCount - maxDataCount_);
			if(createCount <= 0) {
				return;
			}
				

			int endrealIndex = GetEndRealIndex ();

            if(endrealIndex < this.maxDataCount_)
            {
                createCount += this.maxDataCount_ - endrealIndex;
            }

			this.maxDataCount_ = maxCount;
			this.pageCount_ = pageCount;
            bool drawItem = alwaysScrollToEndForNew;
            if (alwaysScrollToEndForNew)
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(StartScrolling(ScrollTo.Bottom));
                }
            }
            else
            {
                drawItem = this.WillSetScrollToEnd();
            }
			if (drawItem) {
                this.StopScroll();
				for (int i = endrealIndex; i < endrealIndex + createCount; i++) {
					Create(i, true);
				}
				this.TryRemoveOverflowItems ();
			}
	        
	    }
		[ContextMenu("ScrollBottom")]
	    public void ScrollBottom() {
			int endrealIndex = GetEndRealIndex ();

			int left_count = maxDataCount_ - endrealIndex;

			int create_count = Mathf.Min (left_count, pageCount_);
	 
			int start_index = maxDataCount_ - create_count;

            this.StopScroll();

			if (endrealIndex  < maxDataCount_) {
				for (int i = start_index; i < maxDataCount_; i++) {
	                Create(i, true);
	            }
				
	        }

			this.TryRemoveOverflowItems ();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(StartScrolling(ScrollTo.Bottom));
            }
	    }

		private bool TryRemoveOverflowItems(){
			if (itemList == null) {
				return false;
			}
			int remove_count = Mathf.Max (0, itemList.Count - pageCount_);

			for( int i = remove_count-1;i >= 0;i --){
				var tempItem = itemList [i];
				if(this.onWillDestroyItem !=null){
					this.onWillDestroyItem (tempItem.luaTable, tempItem.dataIndex);
				}
				itemList.RemoveAt (i);
			}
            if (remove_count > 0)
            {
                return true;
            }
            return false;

		}


		public int GetEndRealIndex(){
			if (itemList == null || itemList.Count == 0)
				return 0;
			return itemList [itemList.Count - 1].dataIndex + 1;
		}

		public int GetStartRealIndex(){
			if (itemList == null || itemList.Count == 0)
				return 0;
			return itemList [0].dataIndex + 1;
		}




	    void Awake() {
	        itemList = new List<TempItem>();
	        scrollRect = GetComponentInParent<UserScrollRect>();
	        scrollTrans = transform.parent as RectTransform;
	        viewRectTrans = GetComponent<RectTransform>();
	        scrollRect.onValueChanged.AddListener(OnValueChange);
			pivot = viewRectTrans.pivot;
            InitEvent();
	    }

        private void InitEvent()
        {
            RemoveEvent();
            destroyAllListenId = EventDispatcher.AddEventListener(sw.game.evt.UIEventType.DESTROY_ALL_NODE, OnDestroyAllNode);
        }

        private void RemoveEvent()
        {
            if (destroyAllListenId >= 0)
            {
                EventDispatcher.RemoveEventListener(sw.game.evt.UIEventType.DESTROY_ALL_NODE, destroyAllListenId);
                destroyAllListenId = -1;
            }
        }

        private void OnDestroyAllNode(object[] args)
        {
            onWillDestroyItem = null;
        }

	    // Use this for initialization
	    void Start() {
	        scrollPos = new Vector3[4];
	        scrollTrans.GetLocalCorners(scrollPos);
	    }

		protected override void OnRectTransformDimensionsChange ()
		{
			if(viewRectTrans != null && scrollTrans != null) {
				float viewHeight = viewRectTrans.rect.height;
				float scrollHeight = scrollTrans.rect.height;
				if(viewHeight < scrollHeight) {
					if (this.pivot.y != 1) {
						this.pivot.y = 1;
						viewRectTrans.pivot = this.pivot;
						viewRectTrans.anchorMin = this.pivot;
						viewRectTrans.anchorMax = this.pivot;
						viewRectTrans.anchoredPosition = Vector2.zero;
					}
				} else {
					if (this.pivot.y != 0) {
						this.pivot.y = 0;
						viewRectTrans.pivot = this.pivot;
						viewRectTrans.anchorMin = this.pivot;
						viewRectTrans.anchorMax = this.pivot;
						viewRectTrans.anchoredPosition = Vector2.zero;
					}
				}
			}

			base.OnRectTransformDimensionsChange ();
	
		}

        private IEnumerator StartScrolling(ScrollTo to)
        {
            yield return new WaitForEndOfFrame();
            if (to == ScrollTo.Bottom)
            {
                this.ScrollToEnd();
            }
            else if (to == ScrollTo.Top)
            {
                this.ScrollToBegin();
            }
        }


	    public void BottomCreate() {
			var endrealIndex = this.GetEndRealIndex ();

			if (endrealIndex >= maxDataCount_) {
				return;
			}
	        StopScroll();

			var minIndex = Mathf.Min(maxDataCount_, endrealIndex + pageCount_);
			for (int i = endrealIndex; i < minIndex; i++) {
	            Create(i, true);
	        }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SetEase(true));
            }
	    }

	    private void StopScroll() {
	        oldViewHeight = viewRectTrans.rect.height;
	        oldViewPos = viewRectTrans.anchoredPosition;

	        scrollRect.canDrag = false;
	        scrollRect.enabled = false;
	    }

	    private void TopCreate() {

			int startRealIndex = this.GetStartRealIndex ();

			if (startRealIndex <= 1) {
	            return;
	        }
			
	        StopScroll();
			var topRealIndex = startRealIndex - 1;
			var maxIndex = Mathf.Max(0, topRealIndex - pageCount_);
			for (int i = topRealIndex - 1; i >= maxIndex; i--) {
	            Create(i, false);
	        }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SetEase(false));
            }
	    }

	    void Create(int dataIndex, bool isBottom) {
	        if (onWillCreateItem == null) {
	            return;
	        }

			LuaBehaviour lb = onWillCreateItem(this.transform, dataIndex);
			GameObject obj = lb.gameObject;
	        if (obj == null) {
	            return;
	        }
	        var item = new TempItem();
	        item.go = obj;
			item.luaTable = lb.GetSelf();
	        item.trans = obj.GetComponent<RectTransform>();
	        item.dataIndex = dataIndex;
	        if (isBottom) {
	            itemList.Add(item);
	        } else {
				item.trans.SetAsFirstSibling();
	            itemList.Insert(0, item);
	        }
			onInitializeItem(item.luaTable, dataIndex);
	    }

	    
	    private void OnValueChange(Vector2 vec2) {
            if(this.pivot.y == 0)
            {
	            if (vec2.y <= 0f) {
	                //向下
	                BottomCreate();
	            } else if (vec2.y >= 1f) {
	                //向上
	                TopCreate();
	            }
            }else
            {
                if (vec2.y <= 0f)
                {
                    //向上
                    TopCreate();
                }
                else if (vec2.y >= 1f)
                {
                    //向下
                    BottomCreate();
                }
            }
	    }

	    private IEnumerator SetEase(bool isBottom) {
	        yield return new WaitForEndOfFrame();

	        var h = viewRectTrans.rect.height - oldViewHeight;
	        if (isBottom) {
	            var topH = h * (1 - viewRectTrans.pivot.y);
	            viewRectTrans.anchoredPosition = oldViewPos - new Vector2(0, topH);
	        } else {
	            var topH = h * (viewRectTrans.pivot.y);
	            viewRectTrans.anchoredPosition = oldViewPos + new Vector2(0, topH);
	        }
	        scrollRect.StopMovement();
	        scrollRect.enabled = true;

	    }

		void OnDestroy(){
			this.ClearList ();
            RemoveEvent();
		}

	}
}