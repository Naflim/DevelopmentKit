namespace Naflim.DevelopmentKit.DataStructure.Tree
{
    /// <summary>
    /// 树节点
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class TreeNode<T> : ITreeNode<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">数据</param>
        public TreeNode(T value)
        {
            Value = value;
            ChildNodes = new List<TreeNode<T>>();
        }

        /// <summary>
        /// 数据
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        internal TreeNode<T>? ParentNode { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        internal List<TreeNode<T>> ChildNodes { get; set; }

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <returns>子节点</returns>
        public ITreeNode<T>[] GetChildNodes()
        {
            return ChildNodes.ToArray();
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <returns>父节点</returns>
        public ITreeNode<T>? GetParentNode()
        {
            return ParentNode;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public T GetValue()
        {
            return Value;
        }
    }
}
