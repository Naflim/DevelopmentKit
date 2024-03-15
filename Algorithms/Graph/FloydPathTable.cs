// <copyright file="FloydPathTable.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Naflim.DevelopmentKit.DataStructure.Graph;

namespace Naflim.DevelopmentKit.Algorithms.Graph
{
    /// <summary>
    /// 基于弗洛伊德算法的最短路径表
    /// </summary>
    /// <typeparam name="T">图节点类型</typeparam>
    public class FloydPathTable<T>
        where T : class, IGraphNode<T>
    {
        /// <summary>
        /// 节点
        /// </summary>
        private readonly T[] nodes;

        /// <summary>
        /// 节点所在位置
        /// </summary>
        private readonly Dictionary<T, int> indexMap;

        /// <summary>
        /// 权重函数
        /// </summary>
        private readonly Func<T, T, double> weightFunc;

        /// <summary>
        /// 权重矩阵
        /// </summary>
        private double[,]? weightDp;

        /// <summary>
        /// 路由矩阵
        /// </summary>
        private int[,]? pathDp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="graph">图数据</param>
        /// <param name="weightFunc">权重函数</param>
        public FloydPathTable(Graph<T> graph, Func<T, T, double> weightFunc)
        {
            Graph = graph;
            this.weightFunc = weightFunc;
            nodes = graph.ToArray();
            indexMap = new Dictionary<T, int>();
            for (int i = 0; i < nodes.Length; i++)
            {
                indexMap[nodes[i]] = i;
            }
        }

        /// <summary>
        /// 图数据
        /// </summary>
        public Graph<T> Graph { get; set; }

        /// <summary>
        /// 创建路径表
        /// </summary>
        public void CreatePathTable()
        {
            InitDp();
            Floyd();
        }

        /// <summary>
        /// 获取最短路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>最短路径</returns>
        /// <exception cref="ArgumentException">目标点不在表中</exception>
        public List<T> GetShortestPath(T start, T end)
        {
            if (!indexMap.ContainsKey(start))
            {
                throw new ArgumentException("起点不在表中", nameof(start));
            }

            if (!indexMap.ContainsKey(end))
            {
                throw new ArgumentException("终点不在表中", nameof(end));
            }

            if (GetMinWeight(start, end) == double.MaxValue)
            {
                return new List<T>();
            }

            HashSet<T> path = new HashSet<T>();
            AddShortestPath(path, indexMap[start], indexMap[end]);
            return path.ToList();
        }

        /// <summary>
        /// 获取最短路径的权重
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>最短路径的权重</returns>
        public double GetMinWeight(T start, T end)
        {
            return weightDp?[indexMap[start], indexMap[end]] ?? -1;
        }

        private void InitDp()
        {
            int len = nodes.Length;
            weightDp = new double[len, len];
            pathDp = new int[len, len];

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    weightDp[i, j] = double.MaxValue;
                    pathDp[i, j] = -1;
                }
            }

            for (int i = 0; i < len; i++)
            {
                weightDp[i, i] = 0;
                var currentNode = nodes[i];
                var nextNodes = Graph.GetConnectivity(currentNode);
                foreach (var node in nextNodes)
                {
                    var weigth = weightFunc(currentNode, node);
                    var index = indexMap[node];
                    weightDp[i, index] = weigth;
                }
            }
        }

        private void Floyd()
        {
            Debug.Assert(weightDp != null && pathDp != null);
            int len = nodes.Length;
            for (int k = 0; k < len; k++)
            {
                for (int i = 0; i < len; i++)
                {
                    for (int j = 0; j < len; j++)
                    {
                        if ((weightDp[i, k] != double.MaxValue) && (weightDp[k, j] != double.MaxValue))
                        {
                            var newWeight = weightDp[i, k] + weightDp[k, j];
                            if (newWeight < weightDp[i, j])
                            {
                                weightDp[i, j] = newWeight;
                                pathDp[i, j] = k;
                            }
                        }
                    }
                }
            }
        }

        private void AddShortestPath(HashSet<T> list, int startIndex, int endIndex)
        {
            if (startIndex == endIndex)
            {
                return;
            }

            Debug.Assert(pathDp != null);

            var poiniter = pathDp[startIndex, endIndex];
            if (poiniter == -1)
            {
                list.Add(nodes[startIndex]);
                list.Add(nodes[endIndex]);
            }
            else
            {
                AddShortestPath(list, startIndex, poiniter);
                AddShortestPath(list, poiniter, endIndex);
            }
        }
    }
}
