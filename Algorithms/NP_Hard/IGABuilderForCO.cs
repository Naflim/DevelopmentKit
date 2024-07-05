namespace Naflim.DevelopmentKit.Algorithms.NP_Hard
{
    /// <summary>
    /// 应用于组合优化问题的遗传算法生成器
    /// </summary>
    /// <typeparam name="T">组合优化序列类型</typeparam>
    public interface IGABuilderForCO<T>
    {
        /// <summary>
        /// 获取基因分数
        /// </summary>
        /// <param name="gene">基因</param>
        /// <returns>基因分数</returns>
        double GetScore(IList<T> gene);

        /// <summary>
        ///  获取基因样本
        /// </summary>
        /// <returns>基因样本</returns>
        IEnumerable<T> GetGeneSamples();
    }
}
