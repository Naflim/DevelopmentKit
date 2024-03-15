// <copyright file="ITreeNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Naflim.DevelopmentKit.DataStructure.Tree
{
    /// <summary>
    /// 树节点
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    public interface ITreeNode<out T>
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        T GetValue();

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <returns>父节点</returns>
        ITreeNode<T>? GetParentNode();

        /// <summary>
        /// 获取子节点
        /// </summary>
        /// <returns>子节点</returns>
        ITreeNode<T>[] GetChildNodes();
    }
}
