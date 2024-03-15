using Naflim.DevelopmentKit.DataStructure.Tree;

namespace Naflim.DevelopmentKit.Algorithms.Tree
{
    public static class TreeUtils
    {

        /// <summary>
        /// 层级遍历
        /// </summary>
        /// <typeparam name="T">元素的类型</typeparam>
        /// <param name="treeNode">树节点</param>
        /// <param name="action">操作</param>
        public static void LevelTraversal<T>(this ITreeNode<T> treeNode, Action<T> action)
        {
            List<ITreeNode<T>> list = new List<ITreeNode<T>> { treeNode };

            while (list.Count > 0)
            {
                List<ITreeNode<T>> newList = new List<ITreeNode<T>>();
                for (int i = 0; i < list.Count; i++)
                {
                    action(list[i].GetValue());
                    newList.AddRange(list[i].GetChildNodes());
                }

                list = newList;
            }
        }

        /// <summary>
        /// 前序遍历
        /// </summary>
        /// <typeparam name="T">元素的类型</typeparam>
        /// <param name="treeNode">树节点</param>
        /// <param name="action">操作</param>
        public static void PreorderTraversal<T>(this ITreeNode<T> treeNode, Action<T> action)
        {
            action(treeNode.GetValue());
            var childNodes = treeNode.GetChildNodes();

            foreach (var child in childNodes)
            {
                PreorderTraversal(child, action);
            }
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        /// <typeparam name="T">元素的类型</typeparam>
        /// <param name="treeNode">树节点</param>
        /// <param name="action">操作</param>
        public static void PostorderTraversal<T>(this ITreeNode<T> treeNode, Action<T> action)
        {
            var childNodes = treeNode.GetChildNodes();

            foreach (var child in childNodes)
            {
                PostorderTraversal(child, action);
            }

            action(treeNode.GetValue());
        }
    }
}
