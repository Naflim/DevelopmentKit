using Naflim.DevelopmentKit.DataStructure.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IGraphNode<T>[] GetNextNodes()
        {
            return NextNodes.ToArray();
        }

        public T GetValue()
        {
            return Value;
        }

        public override bool Equals(object? obj)
        {
            if(obj == this)
                return true;

            if (obj is GraphNode<T> node)
                return node.Value?.Equals(Value) ?? false;

            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? base.GetHashCode();
        }
    }
}
