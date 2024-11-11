using Assets.Scripts.Utils.DoublyCircularLinkedList;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ProductCenterProMax
{
    public class Swiper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public Vector2 Offset;      //图片位移偏差
        public Vector2 ItemSize;    //Item的尺寸

        public TCPClient client;

        private readonly DCL_List<SwiperItem> dclList = new();          // 双向循环链表轨道
        private readonly DisplayTrain<SwiperItem> displayTrain = new(); // 轮播展示列车

        void Start()
        {
            displayTrain.Offset = Offset;
            displayTrain.ItemSize = ItemSize;

            SwiperItem current;
            for (int i = 0; i < transform.childCount; i++)
            {
                current = transform.GetChild(i).GetComponent<SwiperItem>();
                var newNode = dclList.Add(current);

                if (current.id <= 4)
                {
                    displayTrain.InitAdd(newNode);
                }
            }
            UpdateLayer();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                SwipeOnce(SwipeDirection.Left);
            else if (Input.GetKeyDown(KeyCode.D))
                SwipeOnce(SwipeDirection.Right);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        private int countOnDrag = 0;
        private bool hasChange = false;
        public void OnDrag(PointerEventData eventData)
        {
            if (countOnDrag % 18 == 0 
                && Math.Abs(eventData.delta.x) > 2)
            {
                hasChange = true;
                SwipeOnce(eventData.delta.x > 0 ? SwipeDirection.Left : SwipeDirection.Right);
            }
            countOnDrag++;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!hasChange)
            {
                float currentPosition = eventData.position.x;
                float endPosition = eventData.pressPosition.x;

                SwipeOnce(endPosition > currentPosition
                            ? SwipeDirection.Right
                            : SwipeDirection.Left);
            }
            hasChange = false;
        }


        /// <summary>
        /// 向指定方向滑动一次
        /// </summary>
        /// <param name="direction">滑动方向</param>
        private void SwipeOnce(SwipeDirection direction)
        {
            DCL_Node<SwiperItem> RemovedItem;
            if (direction == SwipeDirection.Left)
            {
                client.SendMessage($"left:1");
                List<DCL_Node<SwiperItem>> itemList = dclList.TraverseFromNode(displayTrain.LeftEnd, TraversalDirection.Backward, 1);
                displayTrain.AddLeft(itemList);
                RemovedItem = displayTrain.RemoveRight(1);
            }
            else
            {
                client.SendMessage($"right:1");
                List<DCL_Node<SwiperItem>> itemList = dclList.TraverseFromNode(displayTrain.RightEnd, TraversalDirection.Forward, 1);
                displayTrain.AddRight(itemList);
                RemovedItem = displayTrain.RemoveLeft(1);
            }

            UpdateLayer();
            UpdateDisplayOnce(RemovedItem.Value, direction);
        }


        /// <summary>
        /// 更新当前展示的显示层级(越靠中间层级越高)
        /// </summary>
        public void UpdateLayer()
        {
            var topLayerPosition = (displayTrain.Count / 2) + 1;
            int i = 1;
            foreach (var item in displayTrain.Train)
            {
                item.gameObject.transform.SetSiblingIndex(topLayerPosition - Math.Abs(topLayerPosition - i));
                i++;
            }
        }

        /// <summary>
        /// 更新渲染
        /// </summary>
        public void UpdateDisplayOnce(SwiperItem removedItem, SwipeDirection direction)
        {
            Vector2 localPos;                       //图片刚开始的中心点
            Vector2 startpos = -(ItemSize + Offset) + new Vector2(-350, -263);//开始的位置

            removedItem.SlidePosAndScale(new Vector2(direction == SwipeDirection.Left
                ? 1420  
                : -1420, 0), Vector3.one * 0.9f);

            int i = 0;
            foreach (var item in displayTrain.Train)
            {
                localPos = startpos + (ItemSize + Offset) * i;
                localPos.y = 0;
                if (i is 2)
                {
                    item.SlidePosAndScale(localPos, Vector3.one * 1.2f);
                }
                else if (i is 1 or 3)
                {
                    item.SlidePosAndScale(localPos, Vector3.one * 1.1f);
                }
                else if (direction == SwipeDirection.Left && i is 0)
                {
                    item.SetPosAndScale(new Vector2(-1420, 0), Vector3.one * 0.9f);
                    item.SlidePosAndScale(localPos * 0.95f, Vector3.one * 0.9f);
                }
                else if (direction == SwipeDirection.Right && i is 4)
                {
                    item.SetPosAndScale(new Vector2(1420, 0), Vector3.one * 0.9f);
                    item.SlidePosAndScale(localPos * 0.95f, Vector3.one * 0.9f);
                }
                else
                {
                    item.SlidePosAndScale(localPos * 0.94f, Vector3.one * 0.9f);
                }
                i++;
            }
        }

    }
}
