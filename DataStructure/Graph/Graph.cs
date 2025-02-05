// <copyright file="Graph.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Naflim.DevelopmentKit.Algorithms;
using System.Collections;

namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 图结构存储容器
    /// </summary>
    /// <typeparam name="T">存储的元素类型</typeparam>
    public class Graph<T> : IEnumerable<T> where T : notnull
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="origin">源节点</param>
        protected Graph(T origin)
        {
            NodeMap = new Dictionary<T, GraphNode<T>>();
            Origin = new GraphNode<T>(origin);
            NodeMap[origin] = Origin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="origin">源节点</param>
        /// <param name="getNextNodesFunc">获取相邻节点的函数</param>
        public Graph(T origin, Func<T, IEnumerable<T>> getNextNodesFunc)
        {
            NodeMap = new Dictionary<T, GraphNode<T>>();
            Origin = new GraphNode<T>(origin);
            NodeMap[origin] = Origin;

            StartRetrieval(origin, getNextNodesFunc);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="origin">源节点</param>
        public Graph(IGraphNode<T> origin)
        {
            NodeMap = new Dictionary<T, GraphNode<T>>();
            var val = origin.GetValue();
            Origin = new GraphNode<T>(val);
            NodeMap[val] = Origin;

            StartRetrieval(origin);
        }

        /// <summary>
        /// 节点映射表
        /// </summary>
        protected Dictionary<T, GraphNode<T>> NodeMap { get; set; }

        /// <summary>
        /// 根结点
        /// </summary>
        public GraphNode<T> Origin { get; private set; }

        /// <summary>
        /// 节点数
        /// </summary>
        public int Count => NodeMap.Count;

        /// <summary>
        /// 拷贝
        /// </summary>
        /// <returns>拷贝对象</returns>
        public Graph<T> Copy()
        {
            Graph<T> newGraph = new Graph<T>(Origin.Value);
            foreach(var node in NodeMap)
            {
                newGraph.NodeMap[node.Key] = node.Value.Copy();
            }

            newGraph.NodeAssociation();
            return newGraph;
        }

        /// <summary>
        /// 分割图
        /// </summary>
        /// <param name="cutNodes">分割点集合</param>
        /// <returns>分割后的图</returns>
        public List<Graph<T>> Split(IEnumerable<T> cutNodes)
        {
            List<Graph<T>> result = new List<Graph<T>>() { this };

            Queue<T> queue = new Queue<T>(cutNodes);

            while (queue.Count > 0)
            {
                var cutNode = queue.Dequeue();

                if (result.FirstOrDefault(g => g.Contains(cutNode)) is Graph<T> target)
                {
                    result.Remove(target);
                    var splits = target.Split(cutNode);
                    result.AddRange(splits);
                }
            }

            return result;
        }

        /// <summary>
        /// 分割图
        /// </summary>
        /// <param name="cutNode">分割点</param>
        /// <returns>分割后的图</returns>
        /// <exception cref="ArgumentException">割点不在图中</exception>
        public List<Graph<T>> Split(T cutNode)
        {
            if (!NodeMap.ContainsKey(cutNode))
            {
                throw new ArgumentException("割点不在图中", nameof(cutNode));
            }

            List<Graph<T>> result = new List<Graph<T>>();

            var graphNode = NodeMap[cutNode];
            List<GraphNode<T>> nextNodes = new List<GraphNode<T>>(graphNode.NextNodes);

            RemoveNode(cutNode);

            foreach (var nextNode in nextNodes)
            {
                if (result.Any(g => g.Contains(nextNode.Value)))
                    continue;

                Graph<T> newGraph = new Graph<T>(nextNode);
                result.Add(newGraph);
            }

            return result;
        }

        /// <summary>
        /// 获取此节点的相邻节点
        /// </summary>
        /// <param name="node">查询节点</param>
        /// <returns>相邻节点</returns>
        public T[] GetConnectivity(T node)
        {
            if (NodeMap.TryGetValue(node, out var graphNode))
            {
                return graphNode.NextNodes.Select(v => v.Value).ToArray();
            }

            return new T[0];
        }

        /// <summary>
        /// 图中是否包含此节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>结果</returns>
        public bool Contains(T node)
        {
            return NodeMap.ContainsKey(node);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点</param>
        public void AddNode(IGraphNode<T> node)
        {
            var relationNodes = node.GetNextNodes();
            GraphNode<T> graphNode = new GraphNode<T>(node.GetValue());

            foreach (var relationNode in relationNodes)
            {
                if (NodeMap.TryGetValue(relationNode.GetValue(), out var gn))
                {
                    if (!gn.NextNodes.Contains(relationNode))
                        gn.NextNodes.Add(graphNode);

                    graphNode.NextNodes.Add(gn);
                }
            }

            NodeMap[node.GetValue()] = graphNode;
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node">节点</param>
        public void RemoveNode(T node)
        {
            if (!NodeMap.TryGetValue(node, out var graphNode))
                return;

            var relationNodes = graphNode.NextNodes;
            foreach (var relationNode in relationNodes)
            {
                if (NodeMap.TryGetValue(relationNode.Value, out var gn))
                {
                    gn.NextNodes.Remove(graphNode);
                }
            }

            NodeMap.Remove(node);
        }

        /// <summary>
        /// 替换节点
        /// </summary>
        /// <param name="oldNode">旧节点</param>
        /// <param name="newNode">新节点</param>
        public void ReplaceNode(T oldNode, IGraphNode<T> newNode)
        {
            RemoveNode(oldNode);
            AddNode(newNode);
        }

        /// <summary>
        /// 替换节点
        /// </summary>
        /// <param name="oldNode">旧节点</param>
        /// <param name="newNode">新节点</param>
        public void ReplaceNode(T oldNode, T newNode)
        {
            if (NodeMap.TryGetValue(oldNode, out var graphNode))
            {
                graphNode.Value = newNode;
                NodeMap.Remove(oldNode);
                NodeMap[newNode] = graphNode;
            }
        }

        /// <summary>
        /// DFS获取两点间全部路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="filter">过滤器</param>
        /// <returns>全部路径</returns>
        /// <exception cref="ArgumentException">目标点不在图内</exception>
        public List<List<T>> GetAllPathsByDFS(T start, T end, Func<T[], T, bool>? filter = null)
        {
            if (!NodeMap.ContainsKey(start))
            {
                throw new ArgumentException("起点不在图中", nameof(start));
            }

            if (!NodeMap.ContainsKey(end))
            {
                throw new ArgumentException("终点不在图中", nameof(end));
            }

            return GetAllPathsByDFS(new List<T> { start }, start, end, filter);
        }

        /// <summary>
        /// BFS获取两点间全部路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="filter">过滤器</param>
        /// <returns>全部路径</returns>
        /// <exception cref="ArgumentException">目标点不在图内</exception>
        public List<List<T>> GetAllPathsByBFS(T start, T end, Func<T[], T, bool>? filter = null)
        {
            if (!NodeMap.ContainsKey(start))
            {
                throw new ArgumentException("起点不在图中", nameof(start));
            }

            if (!NodeMap.ContainsKey(end))
            {
                throw new ArgumentException("终点不在图中", nameof(end));
            }

            List<List<T>> result = new List<List<T>>();
            List<HashSet<T>> paths = new List<HashSet<T>>();
            HashSet<T> head = new HashSet<T>();
            head.Add(start);
            paths.Add(head);
            while (paths.Count > 0)
            {
                var cache = paths.ToArray();
                paths.Clear();

                foreach (var item in cache)
                {
                    T node = item.Last();
                    if (node.Equals(end))
                    {
                        result.Add(item.ToList());
                        continue;
                    }

                    foreach (var next in GetConnectivity(node))
                    {
                        if (!item.Contains(next) && (filter?.Invoke(item.ToArray(), next) ?? true))
                        {
                            HashSet<T> newPath = new HashSet<T>(item);
                            newPath.Add(next);
                            paths.Add(newPath);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// BFS获取两点间最少连接数路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="filter">过滤器</param>
        /// <returns>最少连接数路径</returns>
        /// <exception cref="ArgumentException">目标点不在图内</exception>
        public List<T> GetShortestPathByBFS(T start, T end, Func<T[], T, bool>? filter = null)
        {
            if (!NodeMap.ContainsKey(start))
            {
                throw new ArgumentException("起点不在图中", nameof(start));
            }

            if (!NodeMap.ContainsKey(end))
            {
                throw new ArgumentException("终点不在图中", nameof(end));
            }

            HashSet<T> accessed = new HashSet<T>();
            List<LinkedList<T>> paths = new List<LinkedList<T>>();
            LinkedList<T> head = new LinkedList<T>();
            head.AddLast(start);
            paths.Add(head);
            while (paths.Count > 0)
            {
                var cache = paths.ToArray();
                paths.Clear();

                foreach (var item in cache)
                {
                    if (item.Last == null)
                        continue;

                    T node = item.Last.Value;
                    accessed.Add(node);
                    if (node.Equals(end))
                    {
                        return item.ToList();
                    }

                    foreach (var next in GetConnectivity(node))
                    {
                        if (!accessed.Contains(next) && (filter?.Invoke(item.ToArray(), next) ?? true))
                        {
                            LinkedList<T> newPath = new LinkedList<T>(item);
                            newPath.AddLast(next);
                            paths.Add(newPath);
                        }
                    }
                }
            }

            return new List<T>();
        }

        /// <summary>
        /// 迪杰斯特拉算法
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="weight">权值</param>
        /// <param name="filter">过滤器</param>
        /// <returns>权值总和最低路径</returns>
        /// <remarks>
        /// Dijkstra算法的实现
        /// 权值不可出现负数
        /// </remarks>
        public List<T> Dijkstra(T start, T end, Func<T, T, double> weight, Func<T, bool>? filter = null)
        {
            if (!NodeMap.TryGetValue(start, out var startNode) || !NodeMap.TryGetValue(end, out var endNode))
                return new List<T>();

            Dictionary<GraphNode<T>, Tuple<double, GraphNode<T>?, bool>> dic = new Dictionary<GraphNode<T>, Tuple<double, GraphNode<T>?, bool>>();
            foreach (var item in NodeMap)
            {
                dic[item.Value] = new Tuple<double, GraphNode<T>?, bool>(double.MaxValue, null, false);
            }

            dic[startNode] = new Tuple<double, GraphNode<T>?, bool>(0, null, true);
            GraphNode<T> pointer = startNode;
            var accessed = new HashSet<GraphNode<T>>();

            while (!pointer.Value.Equals(end))
            {
                pointer = Dijkstra(dic,
                                   accessed,
                                   pointer,
                                   weight,
                                   filter);

                if ((dic[pointer].Item1 == double.MaxValue) && (dic[pointer].Item2 == null))
                {
                    return new List<T>();
                }
            }

            List<T> result = new List<T>();
            while (!pointer.Value.Equals(start))
            {
                result.Add(pointer.Value);
                if (dic[pointer].Item2 is not GraphNode<T> p)
                    break;

                pointer = p;
            }

            result.Add(start);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// 基于Tarjan算法获取图中的割点
        /// </summary>
        /// <returns>割点</returns>
        public List<T> GetCutVertexsByTarjan()
        {
            Tarjan(out List<T> cutVertexs, out _, out _);
            return cutVertexs;
        }

        /// <summary>
        /// 基于Tarjan算法获取图中的桥
        /// </summary>
        /// <returns>桥</returns>
        public List<(T, T)> GetBridgesByTarjan()
        {
            Tarjan(out _, out List<(T, T)> bridge, out _);
            return bridge;
        }

        /// <summary>
        /// Tarjan算法
        /// </summary>
        /// <param name="cutVertexs">割点</param>
        /// <param name="bridge">桥</param>
        /// <param name="dfn">强连通分量dfn</param>
        /// <param name="low">强连通分量low</param>
        public void Tarjan(out List<T> cutVertexs, out List<(T, T)> bridge, out Dictionary<T, int> dfn, out Dictionary<T, int> low)
        {
            Tarjan(out cutVertexs, out bridge, out Dictionary<GraphNode<T>, (GraphNode<T>? prev, int dfn, int low)> tarjanData);
            dfn = new Dictionary<T, int>();
            low = new Dictionary<T, int>();
            foreach (var item in tarjanData)
            {
                dfn[item.Key.Value] = item.Value.dfn;
                low[item.Key.Value] = item.Value.low;
            }
        }

        /// <summary>
        /// 节点关联
        /// </summary>
        /// <remarks>
        /// 统一节点引用
        /// </remarks>
        protected void NodeAssociation()
        {
            HashSet<GraphNode<T>> hash = new HashSet<GraphNode<T>>(NodeMap.Values);

            foreach (var node in NodeMap.Values)
            {
                var nextNodes = node.NextNodes;
                for (int i = 0; i < nextNodes.Count; i++)
                {
                    if (hash.TryGetValue(nextNodes[i], out var target))
                        nextNodes[i] = target;
                }
            }
        }

        /// <summary>
        /// 开始检索图
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="getNextNodesFunc">获取相邻节点方法</param>
        protected void StartRetrieval(T source, Func<T, IEnumerable<T>> getNextNodesFunc)
        {
            Queue<T> queue = new Queue<T>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var pending = queue.Dequeue();
                var pendingNode = NodeMap[pending];
                var nextNodes = getNextNodesFunc(pending);

                foreach (T node in nextNodes)
                {
                    GraphNode<T> graphNode;
                    if (NodeMap.ContainsKey(node))
                    {
                        graphNode = NodeMap[node];
                    }
                    else
                    {
                        graphNode= new GraphNode<T>(node);
                        NodeMap[node]= graphNode;
                        queue.Enqueue(node);
                    }

                    pendingNode.NextNodes.Add(graphNode);
                }
            }

            NodeAssociation();
        }

        /// <summary>
        /// 开始检索图
        /// </summary>
        /// <param name="source">源数据</param>
        protected void StartRetrieval(IGraphNode<T> source)
        {
            Queue<IGraphNode<T>> queue = new Queue<IGraphNode<T>>();
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                var pending = queue.Dequeue();
                var pendingNode = NodeMap[pending.GetValue()];
                var nextNodes = pending.GetNextNodes();

                foreach (var node in nextNodes)
                {
                    var nodeVal = node.GetValue();
                    GraphNode<T> graphNode;
                    if (NodeMap.ContainsKey(nodeVal))
                    {
                        graphNode = NodeMap[nodeVal];
                    }
                    else
                    {
                        graphNode = new GraphNode<T>(nodeVal);
                        NodeMap[nodeVal] = graphNode;
                        queue.Enqueue(node);
                    }

                    pendingNode.NextNodes.Add(graphNode);
                }
            }

            NodeAssociation();
        }

        private void Tarjan(out List<T> cutVertexs, out List<(T, T)> bridge, out Dictionary<GraphNode<T>, (GraphNode<T>? prev, int dfn, int low)> tarjanData)
        {
            cutVertexs = new List<T>();
            bridge = new List<(T, T)>();
            Stack<GraphNode<T>> stack = new Stack<GraphNode<T>>();
            stack.Push(Origin);
            var dic = new Dictionary<GraphNode<T>, (GraphNode<T>? prev, int dfn, int low)>();
            int count = 1;
            dic[Origin] = new(null, count, count);
            int rootSonCount = 0;
            while (stack.Count > 0)
            {
                var now = stack.Peek();
                var next = now.NextNodes.Where(n => !dic.ContainsKey(n)).FirstOrDefault();
                if (next != null)
                {
                    if (now.Equals(Origin))
                        rootSonCount++;

                    count++;
                    dic[next] = (now, count, count);
                    stack.Push(next);
                }
                else
                {
                    stack.Pop();
                    if (now.Equals(Origin))
                        continue;

                    var nowVal = dic[now];
                    int low;
                    var lows = now.NextNodes.Where(n => !n.Equals(dic[now].prev)).Select(n => dic[n].low).ToArray();
                    if (lows.Length == 0)
                    {
                        low = dic[now].low;
                    }
                    else
                    {
                        low = lows.Min();
                        dic[now] = (nowVal.prev, nowVal.dfn, low);
                    }

                    if (nowVal.prev != null && lows.Length > 0 && low >= dic[nowVal.prev].dfn)
                        cutVertexs.Add(now.Value);

                    if (nowVal.prev != null && low > dic[nowVal.prev].dfn)
                        bridge.Add((nowVal.prev.Value, now.Value));
                }
            }

            if (rootSonCount > 1)
                cutVertexs.Add(Origin.Value);

            tarjanData = dic;
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举数</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return NodeMap.Keys.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>一个可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private List<List<T>> GetAllPathsByDFS(List<T> prefix, T start, T end, Func<T[], T, bool>? filter = null)
        {
            List<List<T>> result = new List<List<T>>();

            foreach (var next in GetConnectivity(start))
            {
                if (!prefix.Contains(next))
                {
                    if (next.Equals(end))
                    {
                        List<T> path = new List<T>(prefix);
                        path.Add(next);
                        result.Add(path);
                    }
                    else if (filter?.Invoke(prefix.ToArray(), next) ?? true)
                    {
                        List<T> path = new List<T>(prefix);
                        path.Add(next);
                        result.AddRange(GetAllPathsByDFS(path, next, end, filter));
                    }
                }
            }

            return result;
        }

        private GraphNode<T> Dijkstra(Dictionary<GraphNode<T>, Tuple<double, GraphNode<T>?, bool>> dic,
                           HashSet<GraphNode<T>> accessed,
                           GraphNode<T> pointer,
                           Func<T, T, double> weight,
                           Func<T, bool>? filter = null)
        {
            var nextNodes = pointer.NextNodes
                .Where(next => !accessed.Contains(next) && (filter?.Invoke(next.Value) ?? true));

            foreach (var next in nextNodes)
            {
                var nextItem = dic[next];
                var thisWeight = dic[pointer].Item1 + weight(pointer.Value, next.Value);
                if (thisWeight < nextItem.Item1)
                {
                    dic[next] = new Tuple<double, GraphNode<T>?, bool>(thisWeight, pointer, false);
                }
            }

            accessed.Add(pointer);

            KeyValuePair<GraphNode<T>, Tuple<double, GraphNode<T>?, bool>>? markItem = null;
            foreach (var item in dic)
            {
                if (!item.Value.Item3 && (markItem == null || item.Value.Item1 < markItem.Value.Value.Item1))
                {
                    markItem = item;
                }
            }

            dic[markItem!.Value.Key] = new Tuple<double, GraphNode<T>?, bool>(markItem.Value.Value.Item1, markItem.Value.Value.Item2, true);
            return markItem.Value.Key;
        }
    }
}
