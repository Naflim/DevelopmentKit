// <copyright file="Tree.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections;

namespace Naflim.DevelopmentKit.DataStructure.Tree
{
    /// <summary>
    /// 树结构存储容器
    /// </summary>
    /// <typeparam name="T">存储的元素类型</typeparam>
    public class Tree<T> : IEnumerable<T> where T : notnull
    {
        private readonly Dictionary<T, TreeNode<T>> _nodeMap;

        private readonly List<HashSet<T>> _levels;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="root">根结点</param>
        public Tree(ITreeNode<T> root)
        {
            _nodeMap = new Dictionary<T, TreeNode<T>>();
            _levels = new List<HashSet<T>>();

            var rootVal = root.GetValue();
            Root = new TreeNode<T>(rootVal);
            _nodeMap[rootVal] = Root;

            List<ITreeNode<T>> level = new List<ITreeNode<T>> { root };

            while (level.Count > 0)
            {
                _levels.Add(new HashSet<T>(level.Select(v => v.GetValue())));
                List<ITreeNode<T>> newLevel = new List<ITreeNode<T>>();
                for (int i = 0; i < level.Count; i++)
                {
                    var childNodes = level[i].GetChildNodes();
                    if (childNodes == null)
                        continue;

                    foreach (var childNode in childNodes)
                    {
                        newLevel.Add(childNode);
                        var childVal = childNode.GetValue();
                        TreeNode<T> node = new TreeNode<T>(childVal);
                        _nodeMap[childVal] = node;

                        var parentNode = _nodeMap[level[i].GetValue()];
                        node.ParentNode = parentNode;
                        parentNode?.ChildNodes?.Add(node);
                    }
                }

                level = newLevel;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="root">根结点</param>
        /// <param name="getChildNodesFunc">子节点获取函数</param>
        public Tree(T root, Func<T, IEnumerable<T>> getChildNodesFunc)
        {
            _nodeMap = new Dictionary<T, TreeNode<T>>();
            _levels = new List<HashSet<T>>();

            Root = new TreeNode<T>(root);
            _nodeMap[root] = Root;

            List<T> level = new List<T> { root };

            while (level.Count > 0)
            {
                _levels.Add(new HashSet<T>(level));
                List<T> newLevel = new List<T>();
                for (int i = 0; i < level.Count; i++)
                {
                    var childNodes = getChildNodesFunc(level[i]);
                    if (childNodes == null)
                        continue;

                    foreach (var childNode in childNodes)
                    {
                        newLevel.Add(childNode);
                        TreeNode<T> node = new TreeNode<T>(childNode);
                        _nodeMap[childNode] = node;

                        var parentNode = _nodeMap[level[i]];
                        node.ParentNode = parentNode;
                        parentNode?.ChildNodes?.Add(node);
                    }
                }

                level = newLevel;
            }
        }

        /// <summary>
        /// 根结点
        /// </summary>
        public TreeNode<T> Root { get; private set; }

        /// <summary>
        /// 树层级数
        /// </summary>
        public int LevelCount => _levels.Count;

        /// <summary>
        /// 树中是否包含此节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>结果</returns>
        public bool Contains(T node)
        {
            return _nodeMap.ContainsKey(node);
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="node">节点数据</param>
        /// <returns>树节点</returns>
        /// <exception cref="ArgumentException">节点不在树中</exception>
        public TreeNode<T> GetNode(T node)
        {
            if (!_nodeMap.ContainsKey(node))
                throw new ArgumentException("节点不在树中", nameof(node));

            return _nodeMap[node];
        }

        /// <summary>
        /// 获取父项
        /// </summary>
        /// <param name="node">节点数据</param>
        /// <returns>父项</returns>
        /// <exception cref="ArgumentException">节点不在树中</exception>
        public T? GetParentItem(T node)
        {
            if (!_nodeMap.ContainsKey(node))
                throw new ArgumentException("节点不在树中", nameof(node));

            var parentNode = _nodeMap[node].ParentNode;

            if(parentNode == null)
                return default;

            return parentNode.Value;
        }

        /// <summary>
        /// 获取子项
        /// </summary>
        /// <param name="node">节点数据</param>
        /// <returns>子项</returns>
        /// <exception cref="ArgumentException">节点不在树中</exception>
        public T[] GetChildItems(T node)
        {
            if (!_nodeMap.ContainsKey(node))
                throw new ArgumentException("节点不在树中", nameof(node));

            return _nodeMap[node].ChildNodes.Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// 获取层级
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>层级</returns>
        public int GetLevel(T node)
        {
            if (!_nodeMap.ContainsKey(node))
                throw new ArgumentException("节点不在树中", nameof(node));

            return _levels.FindIndex(v => v.Contains(node));
        }

        /// <summary>
        /// 获取同层级项
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>同层级项</returns>
        public T[] GetLevelItems(T node)
        {
            if (!_nodeMap.ContainsKey(node))
                throw new ArgumentException("节点不在树中", nameof(node));

            return _levels.First(v => v.Contains(node)).ToArray();
        }

        /// <summary>
        /// 获取层级项
        /// </summary>
        /// <param name="level">层级</param>
        /// <returns>层级项</returns>
        public T[] GetLevelItems(int level)
        {
            return _levels[level].ToArray();
        }

        /// <summary>
        /// 获取兄弟项
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>兄弟项</returns>
        /// <exception cref="ArgumentException">节点不在树中</exception>
        public T[] GetSiblingItems(T node)
        {
            if (!_nodeMap.ContainsKey(node))
                throw new ArgumentException("节点不在树中", nameof(node));

            var parent = GetParentItem(node);

            if(parent == null)
                return Array.Empty<T>();

            List<T> siblingItems = new List<T>(GetChildItems(parent));
            siblingItems.Remove(node);
            return siblingItems.ToArray();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _nodeMap.Keys.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>一个可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
