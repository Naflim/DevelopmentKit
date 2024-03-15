using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.DataStructure.Graph.AdjacencyMultilist
{
    /// <summary>
    /// 顶点表结点
    /// </summary>
    /// <typeparam name="TVertex">点类型</typeparam>
    /// <typeparam name="TEdge">边类型</typeparam>
    public class VertexNode<TVertex, TEdge>
        where TVertex : IGrapVertex<TEdge>
        where TEdge : IGrapEdge<TVertex>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vertex">点存储数据</param>
        public VertexNode(TVertex vertex)
        {
            Vertex = vertex;
        }

        /// <summary>
        /// 顶点储存的数据
        /// </summary>
        public TVertex Vertex { get; set; }

        /// <summary>
        /// 指向的第一个顶点
        /// </summary>
        public EdgeNode<TVertex, TEdge>? FirstEdge { get; set; }
    }
}
