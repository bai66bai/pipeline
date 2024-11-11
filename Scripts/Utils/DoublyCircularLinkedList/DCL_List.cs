using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Utils.DoublyCircularLinkedList
{
    /// <summary>
    /// 双向循环链表
    /// </summary>
    /// <typeparam name="T">节点数据类型</typeparam>
    public class DCL_List<T> : IEnumerable<T>
    {
        /// <summary>
        /// 头节点
        /// </summary>
        public DCL_Node<T> Head;

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count { get; private set; }

        public DCL_List()
        {
            Head = null;
            Count = 0;
        }
        
        /// <summary>
        /// 尾部添加新节点
        /// </summary>
        /// <param name="value">新节点数据</param>
        public DCL_Node<T> Add(T value)
        {
            DCL_Node<T> newNode = new(value);
            if (Head == null)
            {
                Head = newNode;
            }
            else
            {
                newNode.Prev = Head.Prev;
                newNode.Next = Head;
                Head.Prev.Next = newNode;
                Head.Prev = newNode;
            }
            Count++;
            return newNode;
        }

        /// <summary>
        /// 从指定节点，向指定方向，遍历指定步数
        /// </summary>
        /// <param name="startNode">开始遍历节点</param>
        /// <param name="direction">遍历方向</param>
        /// <param name="steps">遍历步数</param>
        /// <returns>遍历得到的DCL_Node序列</returns>
        public List<DCL_Node<T>> TraverseFromNode(DCL_Node<T> startNode, TraversalDirection direction, int steps)
        {
            List<DCL_Node<T>> result = new();
            if (startNode == null) return result;

            DCL_Node<T> currentNode = startNode;
            for (int i = 0; i < Math.Abs(steps); i++)
            {
                
                currentNode = direction == TraversalDirection.Forward ? currentNode.Next : currentNode.Prev;
                result.Add(currentNode);
            }

            return result;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            DCL_Node<T> current = Head;
            if (current != null)
            {
                do
                {
                    yield return current.Value;
                    current = current.Next;
                } while (current != Head);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
