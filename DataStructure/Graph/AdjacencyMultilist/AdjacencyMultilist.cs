// <copyright file="AdjacencyMultilist.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#pragma warning disable SA1305

namespace Naflim.DevelopmentKit.DataStructure.Graph.AdjacencyMultilist
{
    /// <summary>
    /// 邻接多重表
    /// </summary>
    /// <typeparam name="TVertex">顶点</typeparam>
    /// <typeparam name="TEdge">边</typeparam>
    /// <remarks>
    /// 允许出现单顶点边
    /// 顶点与边类型需重写好Equals方法
    /// </remarks>
    public class AdjacencyMultilist<TVertex, TEdge>
        where TVertex : IGrapVertex<TEdge>
        where TEdge : IGrapEdge<TVertex>
    {
        private readonly List<VertexNode<TVertex, TEdge>> list;

        private readonly List<TVertex> vertexs;

        private readonly List<TEdge> edges;

        private readonly Dictionary<TEdge, Tuple<int, int>> edgeMap;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vertexs">顶点集合</param>
        /// <param name="edges">边集合</param>
        public AdjacencyMultilist(IEnumerable<TVertex> vertexs, IEnumerable<TEdge> edges)
        {
            this.vertexs = vertexs.ToList();
            this.edges = edges.ToList();
            list = this.vertexs.Select(v => new VertexNode<TVertex, TEdge>(v)).ToList();
            edgeMap = new Dictionary<TEdge, Tuple<int, int>>();

            InitEdgeNode();
        }

        /// <summary>
        /// 邻接多重表
        /// </summary>
        public VertexNode<TVertex, TEdge>[] Table => list.ToArray();

        /// <summary>
        /// 储存的所有顶点
        /// </summary>
        public TVertex[] Vertexs => vertexs.ToArray();

        /// <summary>
        /// 储存的所有边
        /// </summary>
        public TEdge[] Edges => edges.ToArray();

        /// <summary>
        /// 顶点是否存在于表中
        /// </summary>
        /// <param name="vertex">顶点</param>
        /// <returns>结果</returns>
        public bool ContainsVertex(TVertex vertex)
        {
            foreach (var item in list)
            {
                if (item.Vertex.Equals(vertex))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 顶点于表中的索引
        /// </summary>
        /// <param name="vertex">顶点</param>
        /// <returns>索引</returns>
        public int VertexIndexOf(TVertex vertex)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Vertex.Equals(vertex))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 边的顶点于表中的位置
        /// </summary>
        /// <param name="edge">边</param>
        /// <returns>索引</returns>
        public Tuple<int, int> EdgeIndexOf(TEdge edge)
        {
            if (edgeMap.ContainsKey(edge))
            {
                return edgeMap[edge];
            }

            return new Tuple<int, int>(-1, -1);
        }

        /// <summary>
        /// 删除边
        /// </summary>
        /// <param name="edge">边</param>
        public void RemoveEdge(TEdge edge)
        {
            if (edges.Contains(edge))
            {
                edges.Remove(edge);
                edgeMap.Remove(edge);
            }
            else
            {
                return;
            }

            var vertexs = edge.GetVertexs();

            //获取顶点的下标
            int vIndex = -1, oIndex = -1;

            for (int i = 0; i < vertexs.Count; i++)
            {
                if (i == 0)
                {
                    vIndex = VertexIndexOf(vertexs[i]);
                }
                else if (i == 1)
                {
                    oIndex = VertexIndexOf(vertexs[i]);
                }
                else
                {
                    break;
                }
            }

            if (vIndex != -1)
            {
                //顶点A指针的前驱
                EdgeNode<TVertex, TEdge>? preCursor = null;
                var vertex = list[vIndex];
                var cursor = vertex.FirstEdge;

                while (cursor != null)
                {
                    //通过遍历顶点A的所有边，找到边的前驱指针
                    if (cursor.Edge.Equals(edge))
                    {
                        break;
                    }

                    preCursor = cursor;

                    if (cursor.IVex == vIndex)
                    {
                        cursor = cursor.Link;
                    }
                    else
                    {
                        cursor = cursor.JLink;
                    }
                }

                if (cursor != null)
                {
                    if (preCursor != null)
                    {
                        if ((preCursor.IVex == vIndex) && (cursor.IVex == vIndex))
                        {
                            preCursor.Link = cursor.Link;
                        }
                        else if ((preCursor.IVex == vIndex) && (cursor.JVex == vIndex))
                        {
                            preCursor.Link = cursor.JLink;
                        }
                        else if ((preCursor.JVex == vIndex) && (cursor.IVex == vIndex))
                        {
                            preCursor.JLink = cursor.Link;
                        }
                        else
                        {
                            preCursor.JLink = cursor.JLink;
                        }
                    }
                    else
                    {
                        if (cursor.IVex == vIndex)
                        {
                            vertex.FirstEdge = cursor.Link;
                        }
                        else
                        {
                            vertex.FirstEdge = cursor.JLink;
                        }
                    }
                }
            }

            if (oIndex != -1)
            {
                //顶点B指针的前驱
                EdgeNode<TVertex, TEdge>? preCursor = null;
                var vertex = list[oIndex];
                var cursor = vertex.FirstEdge;

                while (cursor != null)
                {
                    //通过遍历顶点A的所有边，找到边的前驱指针
                    if (cursor.Edge.Equals(edge))
                    {
                        break;
                    }

                    preCursor = cursor;

                    if (cursor.IVex == oIndex)
                    {
                        cursor = cursor.Link;
                    }
                    else
                    {
                        cursor = cursor.JLink;
                    }
                }

                if (cursor != null)
                {
                    if (preCursor != null)
                    {
                        if ((preCursor.IVex == oIndex) && (cursor.IVex == oIndex))
                        {
                            preCursor.Link = cursor.Link;
                        }
                        else if ((preCursor.IVex == oIndex) && (cursor.JVex == oIndex))
                        {
                            preCursor.Link = cursor.JLink;
                        }
                        else if ((preCursor.JVex == oIndex) && (cursor.IVex == oIndex))
                        {
                            preCursor.JLink = cursor.Link;
                        }
                        else
                        {
                            preCursor.JLink = cursor.JLink;
                        }
                    }
                    else
                    {
                        if (cursor.IVex == oIndex)
                        {
                            vertex.FirstEdge = cursor.Link;
                        }
                        else
                        {
                            vertex.FirstEdge = cursor.JLink;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 替换边
        /// </summary>
        /// <param name="original">原边</param>
        /// <param name="newEdge">新边</param>
        public void ReplaceEdge(TEdge original, TEdge newEdge)
        {
            var index = edges.IndexOf(original);
            if (index == -1)
            {
                return;
            }

            edges[index] = newEdge;
            edgeMap.Remove(original);

            var vertexs = original.GetVertexs();

            foreach (var vertex in vertexs)
            {
                var i = VertexIndexOf(vertex);
                if (i != -1)
                {
                    var cursor = list[i].FirstEdge;

                    while (cursor != null)
                    {
                        //通过遍历顶点A的所有边，找到边的前驱指针
                        if (cursor.Edge.Equals(original))
                        {
                            cursor.Edge = newEdge;
                            edgeMap[newEdge] = new Tuple<int, int>(cursor.IVex, cursor.JVex);
                            break;
                        }

                        if (cursor.IVex == i)
                        {
                            cursor = cursor.Link;
                        }
                        else
                        {
                            cursor = cursor.JLink;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 删除顶点
        /// </summary>
        /// <param name="vertex">顶点</param>
        public void RemoveVertex(TVertex vertex)
        {
            int vIndex = VertexIndexOf(vertex);
            if (vIndex == -1)
            {
                return;
            }

            var cursor = list[vIndex].FirstEdge;
            while (cursor != null)
            {
                if (cursor.IVex == vIndex)
                {
                    cursor.IVex = -1;
                    edgeMap[cursor.Edge] = new Tuple<int, int>(-1, cursor.JVex);
                    cursor = cursor.Link;
                }
                else
                {
                    cursor.JVex = -1;
                    edgeMap[cursor.Edge] = new Tuple<int, int>(cursor.IVex, -1);
                    cursor = cursor.JLink;
                }
            }

            vertexs.Remove(vertex);
            list.RemoveAt(vIndex);
        }

        /// <summary>
        /// 获取边相邻的边
        /// </summary>
        /// <param name="edge">参照边</param>
        /// <returns>相邻边</returns>
        public List<TEdge> GetAdjacentEdge(TEdge edge)
        {
            List<TEdge> result = new List<TEdge>();
            if (!edgeMap.ContainsKey(edge))
            {
                return result;
            }

            var indexs = edgeMap[edge];
            var cursor = indexs.Item1 < 0 ? null : list[indexs.Item1].FirstEdge;

            while (cursor != null)
            {
                if (!cursor.Edge.Equals(edge))
                {
                    result.Add(cursor.Edge);
                }

                if (cursor.IVex == indexs.Item1)
                {
                    cursor = cursor.Link;
                }
                else
                {
                    cursor = cursor.JLink;
                }
            }

            cursor = indexs.Item2 < 0 ? null : list[indexs.Item2].FirstEdge;

            while (cursor != null)
            {
                if (!cursor.Edge.Equals(edge))
                {
                    result.Add(cursor.Edge);
                }

                if (cursor.IVex == indexs.Item2)
                {
                    cursor = cursor.Link;
                }
                else
                {
                    cursor = cursor.JLink;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取顶点相邻的点
        /// </summary>
        /// <param name="vertex">参照点</param>
        /// <returns>相邻点</returns>
        public List<TVertex> GetAdjacentVertex(TVertex vertex)
        {
            List<TVertex> result = new List<TVertex>();

            int index = VertexIndexOf(vertex);
            if (index == -1)
            {
                return result;
            }

            var cursor = list[index].FirstEdge;

            while (cursor != null)
            {
                if (cursor.IVex == index)
                {
                    if (cursor.JVex != -1)
                    {
                        result.Add(list[cursor.JVex].Vertex);
                    }

                    cursor = cursor.Link;
                }
                else
                {
                    if (cursor.IVex != -1)
                    {
                        result.Add(list[cursor.IVex].Vertex);
                    }

                    cursor = cursor.JLink;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取顶点相邻的边
        /// </summary>
        /// <param name="vertex">参照点</param>
        /// <returns>顶点相邻的边</returns>
        public List<TEdge> GetEdges(TVertex vertex)
        {
            List<TEdge> result = new List<TEdge>();

            int index = VertexIndexOf(vertex);
            if (index == -1)
            {
                return result;
            }

            var cursor = list[index].FirstEdge;

            while (cursor != null)
            {
                if (cursor.IVex == index)
                {
                    result.Add(cursor.Edge);
                    cursor = cursor.Link;
                }
                else
                {
                    result.Add(cursor.Edge);
                    cursor = cursor.JLink;
                }
            }

            return result;
        }

        /// <summary>
        /// 通过查表获取边相邻顶点
        /// </summary>
        /// <param name="edge">边</param>
        /// <returns>顶点</returns>
        public List<TVertex> GetVertexs(TEdge edge)
        {
            List<TVertex> result = new List<TVertex>();
            if (!edgeMap.ContainsKey(edge))
            {
                return result;
            }

            var indexs = edgeMap[edge];

            if (indexs.Item1 >= 0)
            {
                result.Add(list[indexs.Item1].Vertex);
            }

            if (indexs.Item2 >= 0)
            {
                result.Add(list[indexs.Item2].Vertex);
            }

            return result;
        }

        /// <summary>
        /// 获取边相邻的顶点与边
        /// </summary>
        /// <param name="edge">边</param>
        /// <returns>相邻的顶点与边</returns>
        public Dictionary<TVertex, List<TEdge>> GetConnectors(TEdge edge)
        {
            Dictionary<TVertex, List<TEdge>> result = new Dictionary<TVertex, List<TEdge>>();
            if (!edgeMap.ContainsKey(edge))
            {
                return result;
            }

            var indexs = edgeMap[edge];
            EdgeNode<TVertex, TEdge>? cursor = null;
            List<TEdge>? addList = null;
            if (indexs.Item1 >= 0)
            {
                cursor = list[indexs.Item1].FirstEdge;
                addList = result[list[indexs.Item1].Vertex] = new List<TEdge>();
            }

            while (cursor != null)
            {
                Debug.Assert(addList != null);

                if (!cursor.Edge.Equals(edge))
                {
                    addList.Add(cursor.Edge);
                }

                if (cursor.IVex == indexs.Item1)
                {
                    cursor = cursor.Link;
                }
                else
                {
                    cursor = cursor.JLink;
                }
            }

            if (indexs.Item2 >= 0)
            {
                cursor = list[indexs.Item2].FirstEdge;
                addList = result[list[indexs.Item2].Vertex] = new List<TEdge>();
            }

            while (cursor != null)
            {
                Debug.Assert(addList != null);

                if (!cursor.Edge.Equals(edge))
                {
                    addList.Add(cursor.Edge);
                }

                if (cursor.IVex == indexs.Item2)
                {
                    cursor = cursor.Link;
                }
                else
                {
                    cursor = cursor.JLink;
                }
            }

            return result;
        }

        /// <summary>
        /// 初始化边节点
        /// </summary>
        private void InitEdgeNode()
        {
            for (int i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];
                var edgeVertexs = edge.GetVertexs();

                bool hasVertex = false;
                foreach (var v in edgeVertexs)
                {
                    if (ContainsVertex(v))
                    {
                        hasVertex = true;
                        break;
                    }
                }

                if (hasVertex)
                {
                    //获取顶点的下标
                    int vIndex = -1, oIndex = -1;

                    for (int j = 0; j < edgeVertexs.Count; j++)
                    {
                        if (j == 0)
                        {
                            vIndex = VertexIndexOf(edgeVertexs[j]);
                        }
                        else if (j == 1)
                        {
                            oIndex = VertexIndexOf(edgeVertexs[j]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    edgeMap[edge] = new Tuple<int, int>(vIndex, oIndex);

                    EdgeNode<TVertex, TEdge> edgeNode = new EdgeNode<TVertex, TEdge>(edge, vIndex, oIndex);

                    if (vIndex != -1)
                    {
                        var vertex = list[vIndex];
                        if (vertex.FirstEdge == null)
                        {
                            vertex.FirstEdge = edgeNode;
                        }
                        else
                        {
                            var nextEdge = vertex.FirstEdge;
                            edgeNode.Link = nextEdge;
                            vertex.FirstEdge = edgeNode;
                        }
                    }

                    if (oIndex != -1)
                    {
                        var vertex = list[oIndex];
                        if (vertex.FirstEdge == null)
                        {
                            vertex.FirstEdge = edgeNode;
                        }
                        else
                        {
                            var nextEdge = vertex.FirstEdge;
                            edgeNode.JLink = nextEdge;
                            vertex.FirstEdge = edgeNode;
                        }
                    }
                }
            }
        }
    }
}
