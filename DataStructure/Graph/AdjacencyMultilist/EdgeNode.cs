// <copyright file="EdgeNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Naflim.DevelopmentKit.DataStructure.Graph.AdjacencyMultilist
{
    /// <summary>
    /// 边表结点
    /// </summary>
    /// <typeparam name="TVertex">点类型</typeparam>
    /// <typeparam name="TEdge">边类型</typeparam>
    public class EdgeNode<TVertex, TEdge>
        where TVertex : IGrapVertex<TEdge>
        where TEdge : IGrapEdge<TVertex>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="edge">边储存数据</param>
        /// <param name="vexI">边起点索引</param>
        /// <param name="vexJ">边终点索引</param>
        public EdgeNode(TEdge edge, int vexI, int vexJ)
        {
            Edge = edge;
            IVex = vexI;
            JVex = vexJ;
        }

        /// <summary>
        /// 边储存数据
        /// </summary>
        public TEdge Edge { get; set; }

        /// <summary>
        /// 边起点索引
        /// </summary>
        public int IVex { get; set; }

        /// <summary>
        /// 边起点所连接的下一条边结点
        /// </summary>
        public EdgeNode<TVertex, TEdge>? Link { get; set; }

        /// <summary>
        /// 边终点索引
        /// </summary>
        public int JVex { get; set; }

        /// <summary>
        /// 边终点所连接的下一条边结点
        /// </summary>
        public EdgeNode<TVertex, TEdge>? JLink { get; set; }
    }
}
