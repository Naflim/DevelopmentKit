namespace Naflim.DevelopmentKit.Algorithms.NP_Hard
{
    /// <summary>
    /// 基于遗传算法(Genetic Algorithm)实现组合优化(Combinatorial Optimization)
    /// </summary>
    /// <typeparam name="T">组合项类型</typeparam>
    public class COByGA<T>
    {
        private IGABuilderForCO<T> builder;
        private T[] geneSamples = null!;
        private List<int[]> genePool = null!;
        private int[] baseGene = null!;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="builder">生成器</param>
        public COByGA(IGABuilderForCO<T> builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// 迭代次数
        /// </summary>
        public int Iterations { get; set; } = 10000;

        /// <summary>
        /// 基因存储数
        /// </summary>
        /// <remarks>
        /// 必须为4的倍数（四组一轮锦标赛）
        /// </remarks>
        public int GeneCount { get; set; } = 100;

        /// <summary>
        /// 变异几率
        /// </summary>
        public double MutationRate { get; set; } = 0.01;

        /// <summary>
        /// 获取最优组合
        /// </summary>
        /// <returns>最优组合</returns>
        public List<T> GetBestCombination()
        {
            geneSamples = builder.GetGeneSamples().ToArray();

            if (geneSamples.Length < 2)
                return geneSamples.ToList();

            InitGenes();

            //开始迭代
            for (int i = 0; i < Iterations; i++)
            {
                List<int[]> genesNext = new List<int[]>(GeneCount);

                while (genePool.Count > 0)
                {
                    //锦标赛抽取四人，决胜两人
                    var winners = Tournament();

                    //赢家培育两株基因
                    var children = Crossover(winners.Item1, winners.Item2);

                    //胜者和他的孩子加入下轮迭代的基因池
                    genesNext.Add(winners.Item1);
                    genesNext.Add(winners.Item2);
                    genesNext.Add(children.Item1);
                    genesNext.Add(children.Item2);
                }

                genePool = genesNext;
            }

            //获取基因池的最优基因
            baseGene = genePool.MaxItem(v => builder.GetScore(GetCombination(v)));

            return GetCombination(baseGene).ToList();
        }

        private void InitGenes()
        {
            genePool = new List<int[]>(GeneCount);
            int geneLen = geneSamples.Length;
            Random r = new Random();

            int[] temp = new int[geneLen];
            for (int i = 0; i < geneLen; i++)
                temp[i] = i;

            for (int i = 0; i < GeneCount; i++)
            {
                int[] newGene = new int[geneLen];
                Array.Copy(temp, newGene, geneLen);
                newGene.Shuffle();
                genePool.Add(newGene);
            }
        }

        /// <summary>
        /// 锦标赛
        /// </summary>
        /// <returns>优胜者</returns>
        /// <remarks>
        /// 抽取四人进行锦标赛
        /// 选择两人作为优胜者
        /// </remarks>
        private (int[], int[]) Tournament()
        {
            Random r = new Random();

            int index = r.Next(genePool.Count);
            int[] player1 = genePool[index];
            genePool.RemoveAt(index);

            index = r.Next(genePool.Count);
            int[] player2 = genePool[index];
            genePool.RemoveAt(index);

            index = r.Next(genePool.Count);
            int[] player3 = genePool[index];
            genePool.RemoveAt(index);

            index = r.Next(genePool.Count);
            int[] player4 = genePool[index];
            genePool.RemoveAt(index);

            var score1 = builder.GetScore(GetCombination(player1));
            var score2 = builder.GetScore(GetCombination(player2));
            int[] winner1 = score1 > score2 ? player1 : player2;

            var score3 = builder.GetScore(GetCombination(player3));
            var score4 = builder.GetScore(GetCombination(player4));
            int[] winner2 = score3 > score4 ? player3 : player4;

            return (winner1, winner2);
        }

        /// <summary>
        /// 交叉变异
        /// </summary>
        /// <param name="gene1">基因1</param>
        /// <param name="gene2">基因2</param>
        /// <returns>交叉变异后的两株新基因</returns>
        private (int[], int[]) Crossover(int[] gene1, int[] gene2)
        {
            Random r = new Random();

            int geneLen = geneSamples.Length;

            int[] child1 = new int[geneLen];
            int[] child2 = new int[geneLen];

            Array.Copy(gene1, child1, geneLen);
            Array.Copy(gene2, child2, geneLen);

            int split1 = r.Next(0, geneLen - 2);
            int split2 = r.Next(split1, geneLen - 1);

            for (int i = 0; i < geneLen; i++)
            {
                if (r.NextDouble() < MutationRate)
                {
                    var pointer = r.Next(geneLen);
                    int temp = child1[i];
                    child1[i] = child1[pointer];
                    child1[pointer] = temp;

                    temp = child2[i];
                    child2[i] = child2[pointer];
                    child2[pointer] = temp;

                    continue;
                }

                if (i >= split1 && i <= split2)
                {
                    var index1 = Array.IndexOf(child1, child2[i]);
                    var index2 = Array.IndexOf(child2, child1[i]);

                    int temp = child1[i];
                    child1[i] = child1[index1];
                    child1[index1] = temp;

                    temp = child2[i];
                    child2[i] = child2[index2];
                    child2[index2] = temp;
                }
            }

            return (child1, child2);
        }

        private T[] GetCombination(int[] gene)
        {
            return gene.Select(v => geneSamples[v]).ToArray();
        }
    }
}
