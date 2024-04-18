namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 图节点
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class GraphNode<T> : IGraphNode<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">数据</param>
        public GraphNode(T value) 
        {
            Value = value;
            NextNodes = new List<GraphNode<T>>();
        }

        /// <summary>
        /// 数据
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 相邻节点
        /// </summary>
        internal List<GraphNode<T>> NextNodes { get; set; }

        /// <summary>
        /// 获取此节点相连的其他节点
        /// </summary>
        /// <returns>相连的其他节点</returns>
        public IGraphNode<T>[] GetNextNodes()
        {
            return NextNodes.ToArray();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public T GetValue()
        {
            return Value;
        }

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns>拷贝对象</returns>
        public GraphNode<T> Copy()
        {
            GraphNode<T> newNode = new GraphNode<T>(Value);
            newNode.NextNodes = new List<GraphNode<T>>(NextNodes);
            return newNode;
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns>如果指定的对象等于当前对象，则为 true，否则为 false。</returns>
        public override bool Equals(object? obj)
        {
            if(obj == this)
                return true;

            if (obj is GraphNode<T> node)
                return node.Value?.Equals(Value) ?? false;

            return false;
        }

        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns>当前对象的哈希代码。</returns>
        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? base.GetHashCode();
        }
    }
}
