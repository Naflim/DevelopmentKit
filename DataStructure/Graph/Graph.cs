// <copyright file="Graph.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Naflim.DevelopmentKit.Algorithms;
using System.Collections;
using System.Collections.Concurrent;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604
#pragma warning disable CS8618
#pragma warning disable CS8625

namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 图结构存储容器
    /// </summary>
    /// <typeparam name="T">存储的元素类型</typeparam>
    public class Graph<T>
        : IEnumerable<T>
        where T : class, IGraphNode<T>
    {
        /// <summary>
        /// 连接关系表
        /// </summary>
        private Dictionary<T, List<T>> connectivity;

        /// <summary>
        /// 线程安全的连接关系表
        /// </summary>
        /// <remarks>用作异步存储</remarks>
        private ConcurrentDictionary<T, List<T>> connectivityAsync;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="origin">源节点</param>
        public Graph(T origin)
        {
            Origin = origin;
        }

        /// <summary>
        /// 源节点
        /// </summary>
        public T Origin { get; set; }

        /// <summary>
        /// 开始检索图
        /// </summary>
        public void StartRetrieval()
        {
            connectivity = new Dictionary<T, List<T>>();
            AddNextNode(Origin);
        }

        /// <summary>
        /// 以异步形式检索图结构
        /// </summary>
        /// <returns>异步操作</returns>
        /// <remarks>
        /// BFS,检索分支时将新分支分配给线程池安排检索
        /// </remarks>
        public Task StartRetrievalAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                connectivityAsync = new ConcurrentDictionary<T, List<T>>();
                CountdownEvent countdown = new CountdownEvent(1);
                ThreadPool.GetMaxThreads(out int works, out int coms);

                ThreadPool.QueueUserWorkItem(AddNextNodeAsync,
                                             new Tuple<T, CountdownEvent>(Origin,
                                                 countdown));
                countdown.Wait(180000);
                connectivity = new Dictionary<T, List<T>>(connectivityAsync);
            },
                                         TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 节点关联
        /// </summary>
        public void NodeAssociation()
        {
            HashSet<T> hash = new HashSet<T>(connectivity.Keys);
            foreach (var item in connectivity)
            {
                connectivity[item.Key] = item.Value.Select(v =>
                {
                    if (hash.TryGetValue(v, out T target))
                        return target;

                    return v;
                }).ToList();
            }
        }

        /// <summary>
        /// 获取此节点的相邻节点
        /// </summary>
        /// <param name="node">查询节点</param>
        /// <returns>相邻节点</returns>
        public T[] GetConnectivity(T node)
        {
            if (connectivity.ContainsKey(node))
            {
                return connectivity[node].ToArray();
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
            return connectivity.ContainsKey(node);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点</param>
        public void AddNode(T node)
        {
            var relationNodes = node.GetNextNodes();
            foreach (var relationNode in relationNodes)
            {
                if (connectivity.ContainsKey(relationNode.GetValue()) && !connectivity[relationNode.GetValue()].Contains(relationNode))
                {
                    connectivity[relationNode.GetValue()].Add(node);
                }
            }

            connectivity[node] = relationNodes.Select(n => n.GetValue()).ToList();
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node">节点</param>
        public void RemoveNode(T node)
        {
            var relationNodes = node.GetNextNodes();
            foreach (var relationNode in relationNodes)
            {
                if (connectivity.ContainsKey(relationNode.GetValue()))
                {
                    connectivity[relationNode.GetValue()].Remove(node);
                }
            }

            connectivity.Remove(node);
        }

        /// <summary>
        /// 替换节点
        /// </summary>
        /// <param name="oldNode">就节点</param>
        /// <param name="newNode">新节点</param>
        public void ReplaceNode(T oldNode, T newNode)
        {
            RemoveNode(oldNode);
            AddNode(newNode);
        }

        /// <summary>
        /// DFS获取两点间全部路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="filter">过滤器</param>
        /// <returns>全部路径</returns>
        /// <exception cref="ArgumentException">目标点不在图内</exception>
        public List<List<T>> GetAllPathsByDFS(T start, T end, Func<T[], T, bool> filter = null)
        {
            if (!connectivity.ContainsKey(start))
            {
                throw new ArgumentException("起点不在图中", nameof(start));
            }

            if (!connectivity.ContainsKey(end))
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
        public List<List<T>> GetAllPathsByBFS(T start, T end, Func<T[], T, bool> filter = null)
        {
            if (!connectivity.ContainsKey(start))
            {
                throw new ArgumentException("起点不在图中", nameof(start));
            }

            if (!connectivity.ContainsKey(end))
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
            if (!connectivity.ContainsKey(start))
            {
                throw new ArgumentException("起点不在图中", nameof(start));
            }

            if (!connectivity.ContainsKey(end))
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
        public List<T> Dijkstra(T start, T end, Func<T, T, double> weight, Func<T, bool> filter = null)
        {
            Dictionary<T, Tuple<double, T, bool>> dic = new Dictionary<T, Tuple<double, T, bool>>();
            foreach (var item in connectivity)
            {
                dic[item.Key] = new Tuple<double, T, bool>(double.MaxValue, null, false);
            }

            dic[start] = new Tuple<double, T, bool>(0, null, true);
            T pointer = start;
            var accessed = new HashSet<T>();

            while (!pointer.Equals(end))
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
            while (!pointer.Equals(start))
            {
                result.Add(pointer);
                pointer = dic[pointer].Item2;
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
            Tarjan(out cutVertexs, out bridge, out Dictionary<T, (T prev, int dfn, int low)> tarjanData);
            dfn = new Dictionary<T, int>();
            low = new Dictionary<T, int>();
            foreach (var item in tarjanData)
            {
                dfn[item.Key] = item.Value.dfn;
                low[item.Key] = item.Value.low;
            }
        }

        private void Tarjan(out List<T> cutVertexs, out List<(T, T)> bridge, out Dictionary<T, (T prev, int dfn, int low)> tarjanData)
        {
            cutVertexs = new List<T>();
            bridge = new List<(T, T)>();
            Stack<T> stack = new Stack<T>();
            stack.Push(Origin);
            var dic = new Dictionary<T, (T prev, int dfn, int low)>();
            int count = 1;
            dic[Origin] = new(null, count, count);
            int rootSonCount = 0;
            while (stack.Count > 0)
            {
                var now = stack.Peek();
                var next = GetConnectivity(now).Where(n => !dic.ContainsKey(n)).FirstOrDefault();
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
                    var lows = GetConnectivity(now).Where(n => !n.Equals(dic[now].prev)).Select(n => dic[n].low).ToArray();
                    if (lows.Length == 0)
                    {
                        low = dic[now].low;
                    }
                    else
                    {
                        low = lows.Min();
                        dic[now] = (nowVal.prev, nowVal.dfn, low);
                    }

                    if (lows.Length > 0 && low >= dic[nowVal.prev].dfn)
                        cutVertexs.Add(now);

                    if (low > dic[nowVal.prev].dfn)
                        bridge.Add((nowVal.prev, now));
                }
            }

            if (rootSonCount > 1)
                cutVertexs.Add(Origin);

            tarjanData = dic;
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举数</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return connectivity.Keys.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>一个可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddNextNode(T node)
        {
            if (connectivity.ContainsKey(node))
            {
                return;
            }

            var nextNodes = node.GetNextNodes();
            var nexts = nextNodes.Select(n => n.GetValue()).ToList();
            connectivity[node] = nexts;
            foreach (var item in nexts)
            {
                AddNextNode(item);
            }
        }

        private void AddNextNodeAsync(object? state)
        {
            try
            {
                if (!(state is Tuple<T, CountdownEvent> date))
                {
                    return;
                }

                var node = date.Item1;
                var countdown = date.Item2;
                connectivityAsync[node] = node.GetNextNodes().Select(n => n.GetValue()).ToList();
                var nextNodes = connectivityAsync[node].Where(n => !connectivityAsync.ContainsKey(n)).ToArray();

                if (nextNodes.Length == 0)
                {
                    countdown.Signal();
                    return;
                }

                for (int i = 1; i < nextNodes.Length; i++)
                {
                    countdown.TryAddCount();

                    ThreadPool.QueueUserWorkItem(AddNextNodeAsync,
                                                 new Tuple<T, CountdownEvent>(nextNodes[i], countdown));
                }

                AddNextNodeAsync(new Tuple<T, CountdownEvent>(nextNodes[0], countdown));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<List<T>> GetAllPathsByDFS(List<T> prefix, T start, T end, Func<T[], T, bool> filter = null)
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

        private T Dijkstra(Dictionary<T, Tuple<double, T, bool>> dic,
                           HashSet<T> accessed,
                           T pointer,
                           Func<T, T, double> weight,
                           Func<T, bool> filter = null)
        {
            var nextNodes = GetConnectivity(pointer)
                .Where(next => !accessed.Contains(next) && (filter?.Invoke(next) ?? true));

            foreach (var next in nextNodes)
            {
                var nextItem = dic[next];
                var thisWeight = dic[pointer].Item1 + weight(pointer, next);
                if (thisWeight < nextItem.Item1)
                {
                    dic[next] = new Tuple<double, T, bool>(thisWeight, pointer, false);
                }
            }

            accessed.Add(pointer);

            var markItem = dic.Where(i => !i.Value.Item3).MinItem(v => v.Value.Item1);
            dic[markItem.Key] = new Tuple<double, T, bool>(markItem.Value.Item1, markItem.Value.Item2, true);

            return markItem.Key;
        }
    }
}
