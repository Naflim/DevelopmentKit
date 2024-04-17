using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 支持异步读取的图结构存储容器
    /// </summary>
    /// <typeparam name="T">存储的元素类型</typeparam>
    public class GraphAsync<T> : Graph<T> where T : notnull, IGraphNode<T>
    {
        private ConcurrentDictionary<T, List<T>>? _nodeMapAsync;

        public GraphAsync(T origin) : base(origin)
        {
        }

        /// <summary>
        /// 开始检索图
        /// </summary>
        public void StartRetrieval()
        {
            StartRetrieval(Origin.Value);
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
                _nodeMapAsync = new ConcurrentDictionary<T, List<T>>();
                CountdownEvent countdown = new CountdownEvent(1);
                ThreadPool.GetMaxThreads(out int works, out int coms);

                ThreadPool.QueueUserWorkItem(AddNextNodeAsync,
                                             new Tuple<T, CountdownEvent>(Origin.Value,
                                                 countdown));
                countdown.Wait(180000);

                HashSet<T> hash = new HashSet<T>(_nodeMapAsync.Keys);

                NodeMap.Clear();
                foreach (var item in _nodeMapAsync)
                {
                    GraphNode<T> graphNode = new GraphNode<T>(item.Key);
                    foreach (var val in item.Value)
                    {
                        GraphNode<T> gn = new GraphNode<T>(val);
                        graphNode.NextNodes.Add(gn);
                    }

                    NodeMap[item.Key] = graphNode;
                }

                NodeAssociation();
            }, TaskCreationOptions.LongRunning);
        }

        private void AddNextNodeAsync(object? state)
        {
            try
            {
                Debug.Assert(_nodeMapAsync != null, "异步节点表未初始化！");
                if (!(state is Tuple<T, CountdownEvent> date))
                {
                    return;
                }

                var node = date.Item1;
                var countdown = date.Item2;
                _nodeMapAsync[node] = node.GetNextNodes().Select(n => n.GetValue()).ToList();
                var nextNodes = _nodeMapAsync[node].Where(n => !_nodeMapAsync.ContainsKey(n)).ToArray();

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
    }
}
