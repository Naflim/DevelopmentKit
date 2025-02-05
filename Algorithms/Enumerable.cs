// <copyright file="Enumerable.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Naflim.DevelopmentKit.Algorithms
{
    /// <summary>
    /// 对于linq函数的扩展
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// 分组转字典
        /// </summary>
        /// <typeparam name="TKey">键</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="grups">组</param>
        /// <returns>字典</returns>
        public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> grups)
            where TKey : notnull
        {
            Dictionary<TKey, List<TValue>> dic = new Dictionary<TKey, List<TValue>>();
            foreach (var grup in grups)
            {
                dic[grup.Key] = grup.ToList();
            }

            return dic;
        }

        /// <summary>
        /// 获取序列最小项
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <returns>最小项</returns>
        /// <exception cref="RankException">数组长度为0</exception>
        public static TSource MinItem<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var arr = list.ToArray();
            int len = arr.Length;
            if (len == 0)
            {
                throw new RankException("序列不可为空");
            }

            Tuple<TSource, TKey> result = new Tuple<TSource, TKey>(arr[0], keySelector(arr[0]));

            for (int i = 1; i < len; i++)
            {
                var currentVal = new Tuple<TSource, TKey>(arr[i], keySelector(arr[i]));
                if (currentVal.Item2.CompareTo(result.Item2) == -1)
                {
                    result = currentVal;
                }
            }

            return result.Item1;
        }

        /// <summary>
        /// 获取序列所有最小项
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <returns>所有最小项</returns>
        /// <exception cref="RankException">数组长度为0</exception>
        public static List<TSource> MinItems<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var arr = list.ToArray();
            int len = arr.Length;
            if (len == 0)
            {
                throw new RankException("序列不可为空");
            }

            List<TSource> result = new List<TSource> { arr[0] };
            var minKey = keySelector(arr[0]);

            for (int i = 1; i < len; i++)
            {
                var nowKey = keySelector(arr[i]);
                switch (minKey.CompareTo(nowKey))
                {
                    case 0:
                        result.Add(arr[i]);
                        break;
                    case -1:
                        result = new List<TSource> { arr[i] };
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取序列最大项
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <returns>最大项</returns>
        /// <exception cref="RankException">数组长度为0</exception>
        public static TSource MaxItem<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var arr = list.ToArray();
            int len = arr.Length;
            if (len == 0)
            {
                throw new RankException("序列不可为空");
            }

            Tuple<TSource, TKey> result = new Tuple<TSource, TKey>(arr[0], keySelector(arr[0]));

            for (int i = 1; i < len; i++)
            {
                var currentVal = new Tuple<TSource, TKey>(arr[i], keySelector(arr[i]));
                if (currentVal.Item2.CompareTo(result.Item2) == 1)
                {
                    result = currentVal;
                }
            }

            return result.Item1;
        }

        /// <summary>
        /// 获取序列所有最大项
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <returns>所有最大项</returns>
        /// <exception cref="RankException">数组长度为0</exception>
        public static List<TSource> MaxItems<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var arr = list.ToArray();
            int len = arr.Length;
            if (len == 0)
            {
                throw new RankException("序列不可为空");
            }

            List<TSource> result = new List<TSource> { arr[0] };
            var maxKey = keySelector(arr[0]);

            for (int i = 1; i < len; i++)
            {
                var nowKey = keySelector(arr[i]);
                switch (maxKey.CompareTo(nowKey))
                {
                    case 0:
                        result.Add(arr[i]);
                        break;
                    case 1:
                        result = new List<TSource> { arr[i] };
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 对序列每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">元素的类型</typeparam>
        /// <param name="list">序列</param>
        /// <param name="action">操作</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// 反转序列
        /// </summary>
        /// <typeparam name="T">序列类型</typeparam>
        /// <param name="list">序列</param>
        public static void Reverse<T>(this IList<T> list)
        {
            var arr = list.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                list[i] = arr[arr.Length - i - 1];
            }
        }

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <returns>排序后列表</returns>
        /// <remarks>此方法不会改变数组原有顺序</remarks>
        public static List<TSource> QuickSort<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var sortList = list.ToList();

            //先右后左，通过栈来避免深度递归时的栈溢出问题
            Stack<int> stack = new Stack<int>();
            stack.Push(sortList.Count - 1);
            stack.Push(0);

            while (stack.Count > 0)
            {
                int left = stack.Pop();
                int right = stack.Pop();

                int keyIndex = QuickSort(sortList, keySelector, left, right);

                //先右后左
                if ((keyIndex + 1) < right)
                {
                    stack.Push(right);
                    stack.Push(keyIndex + 1);
                }

                if ((keyIndex - 1) > left)
                {
                    stack.Push(keyIndex - 1);
                    stack.Push(left);
                }
            }

            return sortList;
        }

        /// <summary>
        /// 快速排序同时返回比较键
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <returns>此方法不会改变数组原有顺序</returns>
        public static List<Tuple<TSource, TKey>> QuickSortAlsoReComp<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            List<Tuple<TSource, TKey>> sortList = list.Select(v => new Tuple<TSource, TKey>(v, keySelector(v))).ToList();
            return QuickSort(sortList, v => v.Item2);
        }

        /// <summary>
        /// 添加分组项
        /// </summary>
        /// <typeparam name="TKey">键类别</typeparam>
        /// <typeparam name="TItem">成员类别</typeparam>
        /// <param name="gruops">分组</param>
        /// <param name="key">键</param>
        /// <param name="item">成员</param>
        public static void AddGruopItem<TKey, TItem>(this IDictionary<TKey, List<TItem>> gruops, TKey key, TItem item)
        {
            if (gruops.ContainsKey(key))
            {
                gruops[key].Add(item);
            }
            else
            {
                gruops[key] = new List<TItem> { item };
            }
        }

        /// <summary>
        /// 反射字典
        /// </summary>
        /// <typeparam name="TKey">原字典键类型</typeparam>
        /// <typeparam name="TValue">原字典值类型</typeparam>
        /// <param name="dic">原字典</param>
        /// <returns>反射后字典</returns>
        public static Dictionary<TValue, List<TKey>> Reflex<TKey, TValue>(this IDictionary<TKey, List<TValue>> dic)
            where TValue : notnull
        {
            Dictionary<TValue, List<TKey>> result = new Dictionary<TValue, List<TKey>>();

            foreach (var item in dic)
            {
                foreach (var val in item.Value)
                {
                    result.AddGruopItem(val, item.Key);
                }
            }

            return result;
        }

        /// <summary>
        /// 随机打乱序列元素
        /// </summary>
        /// <typeparam name="T">序列类型</typeparam>
        /// <param name="list">序列</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// 快速排序单次流程
        /// </summary>
        /// <typeparam name="TSource">排序类型</typeparam>
        /// <typeparam name="TKey">类型中的键</typeparam>
        /// <param name="list">列表</param>
        /// <param name="keySelector">用于从元素中提取键的函数</param>
        /// <param name="begin">范围起点</param>
        /// <param name="end">范围终点</param>
        /// <returns>基准元素于排序后的索引</returns>
        /// <remarks>
        /// 此方法会改变数组原有顺序
        /// </remarks>
        private static int QuickSort<TSource, TKey>(IList<TSource> list, Func<TSource, TKey> keySelector, int begin, int end)
            where TKey : IComparable<TKey>
        {
            if (begin > end)
            {
                return -1;
            }

            var pivot = keySelector(list[begin]); //设定基准元素
            int i = begin, j = end;               //保存起始和结尾索引下标

            while (i != j)
            {
                //找到一个小于pivot的值，循环结束j所在位置为小于pivot的元素所在的位置
                while ((keySelector(list[j]).CompareTo(pivot) >= 0) && (j > i))
                {
                    j--;
                }

                //找到一个大于pivot的值，循环结束i所在位置为大于pivot的元素所在的位置
                while ((keySelector(list[i]).CompareTo(pivot) <= 0) && (j > i))
                {
                    i++;
                }

                //交换上面找到的两个值
                if (j > i)
                {
                    TSource tempA = list[i];
                    list[i] = list[j];
                    list[j] = tempA;
                }
            } //循环结束i=j;

            //交换起始的基准元素到i，j所在位置，交换后基准元素左侧都小于基准元素，右侧都大于基准元素
            TSource tempB = list[begin];
            list[begin] = list[i];
            list[i] = tempB;

            return i;
        }
    }
}
