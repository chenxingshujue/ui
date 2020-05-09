using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using LuaInterface;
using SimpleFramework;
namespace User
{
    public abstract class UIList : UIBehaviour
    {
        // Use this for initialization
        public UnityEvent OnListDrawCompleted = new UnityEvent();
        public GridAndLoop.OnInitializeItem onInitializeItem;
        public System.Action<LuaTable, int, int> OnLuaInitializeItem;
        public System.Action<GameObject, int, int> OnRendererClick;
        public System.Action<GameObject, int, int, bool> OnRendererSelect;
        public System.Action<LuaTable, int, int> OnLuaRendererClick;
        public System.Action<LuaTable, int, int, bool> OnLuaRendererSelect;
        public System.Func<RectTransform, int, GameObject> OnWillCreateItem;
        public System.Action<GameObject, int, int> OnWillDestroyItem;
        public System.Func<int, bool> OnWillSelectItem;
        public System.Action<int,int, int> OnSelectChange;
        public RectTransform listContainer;
        public bool DontDestroyItem = true;
        [NonSerialized]
        protected int drawIndex = 0;
        [NonSerialized]
        protected List<GameObject> rendererList = new List<GameObject>();
        [NonSerialized]
        protected List<LuaTable> rendererLuaList = new List<LuaTable>();
        [NonSerialized]
        protected List<GameObject> rendererCacheList = new List<GameObject>();
        [NonSerialized]
        protected bool DrawCompleted;
        [NonSerialized]
        protected int drawedCount;
        [NonSerialized]
        int count_;
        [NonSerialized]
        protected bool scrollToSelected_ = true;
        public virtual int count
        {
            set
            {
                count_ = value;
            }
            get{
                return count_;
            }
        }
        [NonSerialized]
        private float delay;
        [NonSerialized]
        private float tempTime;
        [NonSerialized]
        private int selectedIndex_ = -1;
        [NonSerialized]
        private int preSelectedIndex_ = -1;

	    public int selectedIndex {
		    get{ 
			    return selectedIndex_;
		    }
		    protected set{
                if (selectedIndex_ != value)
                {
                    preSelectedIndex_ = selectedIndex_;
			        selectedIndex_ = value;
                }
		    }
	    }
	    public int preSelectedIndex {
		    get{ 
			    return preSelectedIndex_;
		    }
	    }

        private double destroyAllListenId = -1;

        override protected void Awake()
        {
            base.Awake();
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
            OnWillDestroyItem = null;
        }

        public virtual void DrawListDelay(int count_, float delay_ = 0)
        {
            Clear(count_);
            this.count_ = count_;
            delay = delay_;
        }
        public virtual void DrawList(int count_)
        {
            this.scrollToSelected_ = true;
            Clear(count_);
            this.count_ = count_;
            delay = 0;
        }

        public virtual void DrawListAndSelect(int count_, int realindex_)
        {
            this.DrawList(count_);
            this.SetSelectIndex(realindex_);
        }


        [ContextMenu("Redraw")]
        public virtual void Redraw()
        {
            this.scrollToSelected_ = false;
            this.DrawList(this.GetRealCount());
        }


        // Update is called once per frame
        protected virtual void Update()
        {
		    if (drawIndex < count_) {
			    tempTime += Time.deltaTime;
			    int DrawCountPerFrame;
			    if(delay ==0){
                    DrawCountPerFrame = count_;
			    }else{
                    DrawCountPerFrame = Mathf.FloorToInt(tempTime / delay);
                }
			    for (int i=0;i<DrawCountPerFrame;i++){
                    tempTime -= DrawCountPerFrame * delay;
				    if(Draw()) {
                        GameObject renderer = rendererList[drawedCount];
                        DrawRenderer(renderer, drawedCount);
                        if (ShouldPointerClick())
                        {
                            PointerClick trigger = renderer.GetComponent<PointerClick>();
						    if( trigger ==null){
                                trigger = renderer.AddComponent<PointerClick>();
                            }
                            trigger.OnClick.RemoveAllListeners();
                            trigger.OnClick.AddListener(OnPointClick);
                        }
                        drawedCount++;
                    }
                    drawIndex++;
                    if (drawIndex >= count_)
                    {
                        DrawCompleted = true;
                        DrawSelect(true, scrollToSelected_);
                        StartCoroutine(OnDrawCompleted());
                        break;
                    }
                }
            }

        }

        protected virtual bool ShouldPointerClick()
        {
            return OnRendererClick != null || OnRendererSelect != null || OnLuaRendererClick != null || OnLuaRendererSelect != null;
        }

        protected virtual void DrawRenderer(GameObject renderer, int index)
        {
            if (onInitializeItem != null) onInitializeItem(renderer, index, Index2RealIndex(index));
            if (OnLuaInitializeItem != null) OnLuaInitializeItem(this.GetRendererLua(index), index, Index2RealIndex(index));
        }

        protected virtual void OnPointClick(BaseEventData eventData)
        {
            if(!DrawCompleted){
                return;
            }
            PointerEventData evt = eventData as PointerEventData;
            //		Debug.Log ("OnPointClick evt"+evt +"\tevt.selectedObject"+evt.selectedObject+"\tEventSystem.current.currentSelectedGameObject"+EventSystem.current.currentSelectedGameObject);
            int index = rendererList.IndexOf(evt.pointerPress);
            int realindex = Index2RealIndex(index);
		    if (index >= 0) {
                if (OnRendererClick != null)
                {
                    OnRendererClick(rendererList[index], index, realindex);
                }
                if (OnLuaRendererClick != null)
                {
					OnLuaRendererClick(rendererLuaList[index], index,realindex);
                }
		    }
            if (OnRendererSelect != null || OnLuaRendererSelect != null)
            {
                bool can_select = this.SetSelectIndex(realindex);
				if (can_select) {
                    DrawSelect(false, false);
                }
            }

        }

        protected virtual int Index2RealIndex(int index)
        {
            return index;
        }

	    public virtual void Select(int realindex)
	    {
			bool can_select = this.SetSelectIndex (realindex);
			if (can_select && DrawCompleted) {
                DrawSelect(true,selectedIndex >=0);
		    }
	    }
		protected virtual bool SetSelectIndex(int realindex){
			if (realindex < 0) {
				realindex = -1;
			}
			if (realindex >= this.GetRealCount()) {
				return false;
			}
			bool can_select = true;
			if (this.OnWillSelectItem != null) {
				can_select = this.OnWillSelectItem (realindex);
			}
			if (can_select) {
				selectedIndex = realindex;
			}
			return can_select;
		}

		protected virtual int GetRealCount(){
			return this.count_;
		}

        protected virtual IEnumerator OnDrawCompleted()
        {
            yield return new WaitForEndOfFrame();
            if (OnListDrawCompleted != null)
                OnListDrawCompleted.Invoke();
        }

        protected virtual void DrawSelect(bool update, bool scrollToSelected)
        {
            int index = -1;
			if (OnRendererSelect != null || OnLuaRendererSelect != null || this.OnSelectChange != null)
            {
                for (int i = 0; i < count_; i++)
                {
                    int realindex = Index2RealIndex(i);
                    if (realindex >= 0)
                    {
                        if (OnRendererSelect != null)
                        {
                            OnRendererSelect(rendererList[i], i, realindex, selectedIndex == realindex);
                        }
                        if (OnLuaRendererSelect != null)
                        {
                            OnLuaRendererSelect(rendererLuaList[i], i, realindex, selectedIndex == realindex);
                        }
                        if(selectedIndex == realindex)
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


        virtual protected bool Draw()
        {
            if (listContainer == null) return false;
            GameObject obj;
			if (this.DontDestroyItem && drawIndex < this.rendererCacheList.Count) {
				obj = this.rendererCacheList[drawIndex];
				obj.SetActive (true);
			} else {
                if (OnWillCreateItem != null)
                {
                    obj = OnWillCreateItem(listContainer, drawIndex);
                }
                else
                {
                    obj = CreateItem(listContainer);
                }
                if (this.DontDestroyItem)
                {
                    rendererCacheList.Add(obj);
                }
            }

            if (obj != null)
            {
                obj.SetActive(true);
                LuaBehaviour lb = obj.GetComponent<LuaBehaviour>();
                LuaTable lua = null;
                if (lb != null)
                {
                    lua = lb.GetSelf();
                }

                rendererLuaList.Add(lua);
                rendererList.Add(obj);
                return true;
            }
            return false;
        }

        abstract protected GameObject CreateItem(RectTransform parent);
        protected virtual void ClearList(int count_)
        {
            if (this.DontDestroyItem)
            {
                for (int i = count_; i < rendererCacheList.Count; i++)
                {
                    rendererCacheList[i].SetActive(false);
                }
            }
            rendererList.Clear();
            rendererLuaList.Clear();

        }

        protected virtual void Clear(int count_)
        {
            DrawCompleted = false;
            drawIndex = 0;
            ClearList(count_);
            drawedCount = 0;
            this.count_ = 0;
            tempTime = 0;
        }
        protected virtual void DestroyList()
        {
			if (this.DontDestroyItem == false) {
				if (OnWillDestroyItem != null) {
					for (int i = 0; i < rendererList.Count; i++) {
						OnWillDestroyItem (rendererList [i], i, Index2RealIndex (i));
					}
				} else {
					for (int i = 0; i < rendererList.Count; i++) {
						UnityEngine.Object.Destroy (rendererList [i]);
					}
				}
			}
	    }
        public virtual Vector3 GetRendererPositionSameRoot(int index)
        {
            return rendererList[index].transform.localPosition + this.transform.localPosition;
        }
        public GameObject GetRenderer(int index)
        {
            if (index < 0 || index >= rendererList.Count)
                return null;
            return rendererList[index];
        }
        public LuaTable GetRendererLua(int index)
        {
            if (index < 0 || index >= rendererList.Count)
                return null;
            return rendererLuaList[index];
        }
        override protected void OnDestroy()
        {
            base.OnDestroy();
            RemoveEvent();
            DestroyList();
            rendererList = null;
            rendererCacheList = null;
            rendererLuaList = null;
            onInitializeItem = null;
            OnListDrawCompleted = null;
            OnRendererClick = null;
            OnRendererSelect = null;
            OnLuaInitializeItem = null;
            OnLuaRendererClick = null;
            OnLuaRendererSelect = null;
        }
    }

}
