namespace Assets.Scripts.Utils.DoublyCircularLinkedList
{
    /// <summary>
    /// 双向循环链表节点
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class DCL_Node<T>
    {

        /// <summary>
        /// 节点数据
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 上一个节点引用
        /// </summary>
        public DCL_Node<T> Next { get; set; }

        /// <summary>
        /// 下一个节点引用
        /// </summary>
        public DCL_Node<T> Prev { get; set; }


        public DCL_Node(T value)
        {
            Value = value;
            Next = this;
            Prev = this;
        }
    }
}
