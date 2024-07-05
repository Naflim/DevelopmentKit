using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.Tree
{

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604
#pragma warning disable CS1591

    /// <summary>
    /// 红黑树原型
    /// </summary>
    /// <typeparam name="K">键</typeparam>
    /// <typeparam name="V">值</typeparam>
    public class RBTree<K, V> where K : notnull, IComparable<K>
    {
        private RBNode? root;

        private const bool RED = false;
        private const bool BLACK = true;


        public bool Debug(K key)
        {
            if (GetNode(key) is RBNode node)
            {
                return (node.Left?.Equals(node) ?? false) || (node.Right?.Equals(node) ?? false);
            }

            return false;
        }


        public void Put(K key, V value)
        {
            //如果添加节点为根结点
            if (root == null)
            {
                root = new RBNode(key, value);
                root.Color = BLACK;
                return;
            }

            //寻找插入位置
            var pointer = root;
            RBNode parent = root;
            while (pointer != null)
            {
                parent = pointer;
                var cmp = pointer.Key.CompareTo(key);
                if (cmp > 0)
                {
                    pointer = pointer.Left;
                }
                else if (cmp < 0)
                {
                    pointer = pointer.Right;
                }
                else
                {
                    pointer.Value = value;
                    return;
                }
            }

            var newNode = new RBNode(key, value);
            newNode.Color = RED;

            if (parent.Key.CompareTo(key) > 0)
                parent.Left = newNode;
            else
                parent.Right = newNode;

            newNode.Parent = parent;
            FixAfterPut(newNode);
        }

        public V? Remove(K key)
        {
            if (GetNode(key) is not RBNode node)
                return default;

            V result = node.Value;
            DeleteNode(node);
            return result;
        }

        public V? Find(K key)
        {
            var pointer = root;
            while (pointer != null)
            {
                var com = key.CompareTo(pointer.Key);
                switch (com)
                {
                    case -1:
                        pointer = pointer.Left;
                        break;
                    case 0:
                        return pointer.Value;
                    case 1:
                        pointer = pointer.Right;
                        break;
                }
            }

            return default;
        }

        private RBNode? GetNode(K key)
        {
            var pointer = root;
            while (pointer != null)
            {
                var cmp = pointer.Key.CompareTo(key);
                if (cmp > 0)
                    pointer = pointer.Left;
                else if (cmp < 0)
                    pointer = pointer.Right;
                else
                    return pointer;
            }

            return pointer;
        }

        /// <summary>
        /// 删除操作：
        /// 1.叶子节点直接删除
        /// 2.删除的节点有一个子节点，用子节点替代
        /// 3.删除的节点有两个节点，需要找到前驱节点或后继节点替代
        /// </summary>
        /// <param name="node">删除节点</param>
        private void DeleteNode(RBNode node)
        {
            var hasLeftNode = node.Left != null;
            var hasRightNode = node.Right != null;
            var isBranchNode = hasLeftNode && hasRightNode;
            var parent = node.Parent;
            RBNode? fixAfterNode;

            //如果是情况3需要先将其转为情况2
            if (isBranchNode)
            {
                var standing = GetPredecessor4Son(node);

                if (standing == null)
                    return;

                node.Key = standing.Key;
                node.Value = standing.Value;
                node = standing;
                hasLeftNode = node.Left != null;
                hasRightNode = node.Right != null;
                parent = node.Parent;
            }

            if (parent == null)
            {
                if (hasLeftNode)
                {
                    root = node.Left;
                    root.Color = BLACK;
                    root.Parent = null;
                }

                else if (hasRightNode)
                {
                    root = node.Right;
                    root.Color = BLACK;
                    root.Parent = null;
                }
                else
                {
                    root = null;
                }
            }
            else
            {
                fixAfterNode = hasLeftNode ? node.Left : node.Right;

                //删除节点为叶子节点的情况
                if (fixAfterNode == null)
                {
                    if (node.Color == BLACK)
                        FixAfterRemove(node);

                    if (node.Parent != null)
                    {
                        if (node.Parent.Left?.Equals(node) ?? false)
                            node.Parent.Left = null;
                        else
                            node.Parent.Right = null;

                        node.Parent = null;
                    }
                }
                else
                {
                    if (parent.Left.Equals(node))
                    {
                        parent.Left = fixAfterNode;
                    }
                    else
                    {
                        parent.Right = fixAfterNode;
                    }

                    fixAfterNode.Parent = parent;

                    if (node.Color == BLACK)
                        FixAfterRemove(fixAfterNode);
                }
            }
        }

        /// <summary>
        /// 调整
        /// </summary>
        /// <param name="node">新增节点</param>
        private void FixAfterPut(RBNode node)
        {
            if (root == null)
                return;

            RBNode pointer = node;
            //如果父节点是黑色节点或指针指向根结点则无需调整
            while (pointer.Parent != null && pointer.Parent.Color != BLACK)
            {
                var parent = pointer.Parent;
                if (parent.Parent is not RBNode grandpa)
                    break;

                //判断父节点是否为爷爷节点的左节点
                if (grandpa.Left?.Equals(parent)??false)
                {
                    //获取叔叔节点
                    var uncle = grandpa.Right;

                    //此为加入一个红黑红的子树，直接变色即可
                    if (uncle != null && uncle.Color == RED)
                    {
                        uncle.Color = BLACK;
                        parent.Color = BLACK;
                        grandpa.Color = RED;
                        pointer = grandpa;
                    }
                    //此为加入一个黑红红的子树，需要右旋
                    else
                    {
                        //判断是否为特殊左三"<"子树,如何是将父节点左旋使其成为一个标准左三子树
                        if (parent.Right?.Equals(pointer) ?? false)
                        {
                            pointer = parent;
                            LeftRotate(pointer);
                            parent = pointer.Parent;
                        }

                        parent.Color = BLACK;
                        grandpa.Color = RED;
                        RightRotate(grandpa);
                    }
                }
                else
                {
                    //获取叔叔节点
                    var uncle = grandpa.Left;

                    //此为加入一个红黑红的子树，直接变色即可
                    if (uncle != null && uncle.Color == RED)
                    {
                        uncle.Color = BLACK;
                        parent.Color = BLACK;
                        grandpa.Color = RED;
                        pointer = grandpa;
                    }
                    //此为加入一个黑红红的子树，需要左旋
                    else
                    {
                        //判断是否为特殊右三">"子树,如何是将父节点右使其成为一个标准右三子树
                        if (parent.Left?.Equals(pointer) ?? false)
                        {
                            pointer = parent;
                            RightRotate(pointer);
                            parent = pointer.Parent;
                        }

                        parent.Color = BLACK;
                        grandpa.Color = RED;
                        LeftRotate(grandpa);
                    }
                }
            }

            root.Color = BLACK;
        }

        /// <summary>
        /// 删除后调整
        /// </summary>
        /// <param name="node">删除节点</param>
        private void FixAfterRemove(RBNode node)
        {
            while (node.Parent != null && node.Color == BLACK)
            {
                RBNode broNode;
                //调整节点是删除节点左孩子的情况
                if (node.Equals(node.Parent.Left))
                {
                    broNode = node.Parent.Right;

                    //如果此时兄弟为红色说明这不是真正的兄弟节点
                    if (broNode.Color == RED)
                    {
                        broNode.Color = BLACK;
                        broNode.Parent.Color = RED;
                        LeftRotate(broNode.Parent);
                        //找到真正的兄弟节点
                        broNode = broNode.Parent.Right;
                    }


                    //情况三，找兄弟借，兄弟没的借
                    if (broNode.Left == null &&  broNode.Right == null)
                    {
                        broNode.Color = RED;
                        node = node.Parent;
                    }
                    //情况二，找兄弟借，兄弟有的借
                    else
                    {
                        //左旋右调整
                        if (broNode.Right == null)
                        {
                            broNode.Right.Color = BLACK;
                            broNode.Color = RED;
                            RightRotate(broNode);
                            broNode = broNode.Parent;
                        }

                        broNode.Color = broNode.Parent.Color;
                        broNode.Parent.Color = BLACK;
                        broNode.Right.Color = BLACK;
                        LeftRotate(node.Parent);
                        break;
                    }
                }
                //调整节点是删除节点右孩子的情况
                else
                {
                    broNode = node.Parent.Left;

                    //如果此时兄弟为红色说明这不是真正的兄弟节点
                    if (broNode.Color == RED)
                    {
                        broNode.Color = BLACK;
                        broNode.Parent.Color = RED;
                        RightRotate(broNode.Parent);
                        //找到真正的兄弟节点
                        broNode = broNode.Parent.Left;
                    }


                    //情况三，找兄弟借，兄弟没的借
                    if (broNode.Left == null && broNode.Right == null)
                    {
                        broNode.Color = RED;
                        node = node.Parent;
                    }
                    //情况二，找兄弟借，兄弟有的借
                    else
                    {
                        //左旋右调整
                        if (broNode.Left == null)
                        {
                            broNode.Left.Color = BLACK;
                            broNode.Color = RED;
                            LeftRotate(broNode);
                            broNode = broNode.Parent;
                        }

                        broNode.Color = broNode.Parent.Color;
                        broNode.Parent.Color = BLACK;
                        broNode.Left.Color = BLACK;
                        RightRotate(node.Parent);
                        break;
                    }
                }
            }

            //情况一：替代节点是红色的情况直接染黑补偿黑色节点
            node.Color = BLACK;
        }

        /// <summary>
        /// 围绕p左旋
        /// </summary>
        /// <param name="p">旋转节点</param>
        /// <example>
        ///    p        =>      pr
        ///   / \               /\
        /// pl  pr             p  rr
        ///     /\            / \
        ///    rl rr         pl rl
        /// </example>
        private void LeftRotate(RBNode p)
        {
            if (p.Right == null) return;

            RBNode newRoot = p.Right;

            var parent = p.Parent;

            if (parent == null)
            {
                root = newRoot;
            }
            else
            {
                if (parent.Left?.Equals(p) ?? false)
                {
                    parent.Left = newRoot;
                }
                else if (parent.Right?.Equals(p) ?? false)
                {
                    parent.Right = newRoot;
                }
            }

            newRoot.Parent = parent;

            var rl = newRoot.Left;
            newRoot.Left = p;
            p.Parent = newRoot;
            p.Right = rl;
            if (rl != null)
                rl.Parent = p;
        }

        /// <summary>
        /// 围绕p点右旋
        /// </summary>
        /// <param name="p">旋转节点</param>
        /// <example>
        ///      p        =>      pl
        ///     / \               /\
        ///   pl   pr           ll  p
        ///   /\                   / \
        /// ll  lr                lr  pr
        /// </example>
        private void RightRotate(RBNode p)
        {
            if (p.Left == null) return;

            RBNode newRoot = p.Left;

            var parent = p.Parent;

            if (parent == null)
            {
                root = newRoot;
            }
            else
            {
                if (parent.Left?.Equals(p) ?? false)
                {
                    parent.Left = newRoot;
                }
                else if (parent.Right?.Equals(p) ?? false)
                {
                    parent.Right = newRoot;
                }
            }

            newRoot.Parent = parent;

            var lr = newRoot.Right;
            newRoot.Right = p;
            p.Parent = newRoot;
            p.Left = lr;
            if (lr != null)
                lr.Parent = p;
        }

        private RBNode? GetPredecessor4Son(RBNode p)
        {
            if (p.Left is not RBNode result)
                return null;

            while (result.Right != null)
                result = result.Right;

            return result;
        }

        private RBNode? GetSuccessor4Son(RBNode p)
        {
            if (p.Right is not RBNode result)
                return null;

            while (result.Left != null)
                result = result.Left;

            return result;
        }

        class RBNode
        {
            public RBNode? Parent { get; set; }
            public RBNode? Left { get; set; }
            public RBNode? Right { get; set; }

            public bool Color { get; set; }

            public K Key { get; set; }

            public V Value { get; set; }

            internal RBNode(K key, V value)
            {
                Key = key;
                Value = value;
            }

            internal RBNode(RBTree<K, V>.RBNode? parent, RBTree<K, V>.RBNode? left, RBTree<K, V>.RBNode? right, bool color, K key, V value)
            {
                Parent=parent;
                Left=left;
                Right=right;
                Color=color;
                Key=key;
                Value=value;
            }

            public bool Equals(RBNode other)
            {
                return Key.CompareTo(other.Key) == 0;
            }
        }



        public void PrintTree(int startPos = 10, int nodeGap = 3, int boxLenth = -1, string placeholder = "n")
        {
            if (root == null)
                return;

            // 先序遍历获取所有树节点  并分层存储
            var queue = new Queue<Tuple<RBNode, int>>();
            queue.Enqueue(new Tuple<RBNode, int>(root, 1));
            var dic = new Dictionary<int, List<RBNode>>();    // 存储层号和对应节点
            int maxLength = Math.Max(placeholder.Length, boxLenth);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                var t = item.Item1;
                var l = item.Item2;

                if (dic.ContainsKey(l) == false)
                {
                    // 上一层全部为空
                    if (dic.ContainsKey(l - 1) && dic[l - 1].Any(p => p != null) == false) break;
                    dic[l] = new List<RBNode>();
                }

                dic[l].Add(t);

                if (t != null)
                {
                    maxLength = Math.Max(t.Key.ToString().Length, maxLength);
                }
                queue.Enqueue(new Tuple<RBNode, int>(t?.Left, l + 1));
                queue.Enqueue(new Tuple<RBNode, int>(t?.Right, l + 1));
            }

            // 计算每一层节点起始字符宽度和节点间的宽度
            int height = dic.Last().Key;
            Tuple<int, int>[] locs = new Tuple<int, int>[height];   // 存储起始坐标和节点间距
            locs[locs.Length - 1] = new Tuple<int, int>(startPos, nodeGap);
            for (int i = locs.Length - 2; i >= 0; i--)
            {
                var cur = locs[i + 1];
                int nextStartPos = cur.Item1 + (maxLength + cur.Item2) / 2;
                int nextNodeGap = 2 *  cur.Item2 + maxLength;
                locs[i] = new Tuple<int, int>(nextStartPos, nextNodeGap);
            }


            // PadLeft 填充字符串  绘制二叉树
            int ind = 1;
            foreach (var item in dic.Zip(locs))
            {
                int layer = item.First.Key;
                var nodes = item.First.Value;
                int sp = item.Second.Item1;
                int ng = item.Second.Item2;
                Console.WriteLine();
                Console.Write("".PadLeft(sp, ' '));

                // 绘制节点
                int nc = 1;
                foreach (var n in nodes)
                {
                    string str = string.Empty;
                    bool isLeaf = false;
                    if (n == null)
                    {
                        var pa = dic[ind - 1][(nc - 1) / 2];
                        isLeaf = pa != null && (pa.Left == n || pa.Right == n);
                        if (isLeaf) str = placeholder;
                        else str = "";
                    }
                    else
                    {
                        isLeaf = true;
                        str = n.Key.ToString();
                    }

                    var l = str.Length;
                    var pos = (maxLength - l) / 2;
                    string merge = "".PadLeft(pos, ' ') + str + "".PadRight(maxLength - pos - str.Length, ' ');


                    ChangeColor(n);
                    if (isLeaf == false) ClearColor();
                    Console.Write(merge);
                    ClearColor();

                    if (nc < nodes.Count)   // 不是最后一位
                        Console.Write("".PadLeft(ng, ' '));


                    nc++;
                }

                if (ind == height) break;

                // 绘制连接线
                sp = sp + maxLength / 2;
                int layerCount = (locs[ind].Item2 + maxLength) / 2;  // 保证左右孩子间距的一半等于父节点到子节点的垂线距离
                int charLenth = 1;
                for (int i = 0; i < layerCount; i++)
                {
                    sp--;
                    Console.WriteLine();
                    Console.Write("".PadLeft(sp, ' '));
                    int nodeCount = (int)Math.Pow(2, ind);
                    int charGap = i * 2;
                    for (int j = 0; j < nodeCount; j++)
                    {
                        // 空节点不画下方的连接线
                        bool isEmptyNode = nodes[j / 2] == null;

                        if (j % 2 == 0)
                        {
                            string c = isEmptyNode ? " " : "/";
                            Console.Write(c.PadRight(charGap + 1, ' '));
                        }
                        else
                        {
                            string c = isEmptyNode ? " " : "\\";
                            Console.Write(c);
                            // 不是最后一位
                            if (j < nodeCount - 1)
                                Console.Write("".PadLeft(ng + maxLength - charGap - 2 * charLenth, ' '));
                        }

                    }
                }

                ind++;

            }

        }

        private void ChangeColor(RBNode n)
        {
            if (n == null || n.Color == BLACK)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private void ClearColor()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
