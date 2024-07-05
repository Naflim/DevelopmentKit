using Naflim.DevelopmentKit.DataStructure.Tree;

namespace Naflim.DevelopmentKit.Algorithms.Tree
{
    /// <summary>
    /// 树结构扩展方法
    /// </summary>
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
                    var childNodes = list[i].GetChildNodes();
                    if (childNodes != null)
                        newList.AddRange(childNodes);
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

            if (childNodes == null)
                return;

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

            if (childNodes != null)
            {
                foreach (var child in childNodes)
                {
                    PostorderTraversal(child, action);
                }
            }

            action(treeNode.GetValue());
        }

        /// <summary>
        /// 获取节点所处路由
        /// </summary>
        /// <typeparam name="T">节点类型</typeparam>
        /// <param name="treeNode">节点</param>
        /// <param name="getName">获取节点名称函数</param>
        /// <returns>节点所处路由</returns>
        public static string GetPath<T>(this ITreeNode<T> treeNode, Func<ITreeNode<T>, string> getName)
        {
            List<string> pathNodes = new List<string>();
            ITreeNode<T>? nowNode = treeNode;

            while (nowNode != null)
            {
                pathNodes.Add(getName(nowNode));
                nowNode = nowNode.GetParentNode();
            }

            pathNodes.Reverse();

            string path = string.Join("/", pathNodes);
            return $"/{path}";
        }
    }
}
