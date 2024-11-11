using Assets.Scripts.Utils.DoublyCircularLinkedList;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ProductCenterProMax
{
    /// <summary>
    /// 火车轮播展示
    /// </summary>
    public class DisplayTrain<T> where T : MonoBehaviour
    {
        public DCL_Node<T> LeftEnd;             // 左端
        public DCL_Node<T> RightEnd;            // 右端
        public LinkedList<T> Train = new();     // 轮播展示火车
        public int Count { get => Train.Count; private set { } }
        public Vector2 Offset { get; set; }      //图片位移偏差
        public Vector2 ItemSize { get; set; }      //Item的尺寸

        /// <summary>
        /// 初始化调用，在最右侧挂载一个展示车厢
        /// </summary>
        /// <param name="newNode">展示内容</param>
        public void InitAdd(DCL_Node<T> newNode)
        {
            Train.AddLast(newNode.Value);

            if (LeftEnd == null && RightEnd == null)
            {
                LeftEnd = newNode;
                RightEnd = newNode;
            }
            else
            {
                RightEnd = newNode;
            }
        }

        /// <summary>
        /// 向展示火车左侧挂载新的展示车厢
        /// </summary>
        /// <param name="itemList">新的展示车厢</param>
        public void AddLeft(List<DCL_Node<T>> itemList)
        {
            int i = 0;
            foreach (DCL_Node<T> item in itemList)
            {
                Train.AddFirst(item.Value);
                if (i == itemList.Count - 1)
                    LeftEnd = item;
                i++;
            }
        }

        /// <summary>
        /// 向展示火车右侧挂载新的展示车厢
        /// </summary>
        /// <param name="itemList">新的展示车厢</param>
        public DCL_Node<T> AddRight(List<DCL_Node<T>> itemList)
        {
            int i = 0;
            foreach (DCL_Node<T> item in itemList)
            {
                Train.AddLast(item.Value);
                if (i == itemList.Count - 1)
                    RightEnd = item;
                i++;
            }
            return RightEnd;
        }

        /// <summary>
        /// 卸载火车左侧指定数量的展示车厢
        /// </summary>
        /// <param name="step">卸载的数量</param>
        /// <returns>被卸载的车厢</returns>
        public DCL_Node<T> RemoveLeft(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Train.RemoveFirst();
                LeftEnd = LeftEnd.Next;
            }
            return LeftEnd.Prev;
        }

        /// <summary>
        /// 卸载火车右侧指定数量的展示车厢
        /// </summary>
        /// <param name="num">卸载的数量</param>
        /// <returns>被卸载的车厢</returns>
        public DCL_Node<T> RemoveRight(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Train.RemoveLast();
                RightEnd = RightEnd.Prev;
            }
            return RightEnd.Next;
        }
        
        //TODO: 封装获取正中间展示元素API
        public T GetMiddleItem()
        {
            int middleIndex = Count / 2 + 1;
            int i = 0;
            T targetItem = Train.First.Value;
            foreach (var item in Train)
            {
                targetItem = item;
                if (i == middleIndex)
                    break;
                i++;
            }
            return targetItem;
        }
    }
}
