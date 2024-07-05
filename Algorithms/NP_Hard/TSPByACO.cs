// <copyright file="TSPByACO.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Naflim.DevelopmentKit.Algorithms.NP_Hard
{
    /// <summary>
    /// 基于蚁群算法(Ant Colony Optimization)解决旅行商问题(Traveling Salesman Problem)
    /// </summary>
    /// <typeparam name="T">城市类</typeparam>
    public class TSPByACO<T>
        where T : class
    {
        /// <summary>
        /// 信息素
        /// </summary>
        private readonly double[,] pheromones;

        /// <summary>
        /// 城市
        /// </summary>
        private readonly T[] cities;

        /// <summary>
        /// 城市索引
        /// </summary>
        private readonly Dictionary<T, int> citiesIndex;

        /// <summary>
        /// 距离函数
        /// </summary>
        private readonly Func<T, T, double> distanceFunction;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// 蚁群
        /// </summary>
        private Ant[] antColony = new Ant[20];

        /// <summary>
        /// 最佳路径
        /// </summary>
        private List<T>? bestTour;

        /// <summary>
        /// 最佳路径长度
        /// </summary>
        private double bestTourLength = -1;

        /// <summary>
        /// 起点
        /// </summary>
        private T? start;

        /// <summary>
        /// 终点
        /// </summary>
        private T? end;

        /// <summary>
        /// 中继点
        /// </summary>
        private List<T>? relay;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cities">城市集合</param>
        /// <param name="distanceFunction">距离函数</param>
        public TSPByACO(IEnumerable<T> cities, Func<T, T, double> distanceFunction)
        {
            this.distanceFunction = distanceFunction;
            this.cities = cities.ToArray();
            AntsCount = this.cities.Length * 4;

            int len = this.cities.Length;
            pheromones = new double[len, len];
            citiesIndex = new Dictionary<T, int>();

            for (int i = 0; i < len; i++)
            {
                citiesIndex[this.cities[i]] = i;
            }

            InitPherom();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cities">城市集合</param>
        /// <param name="distanceFunction">距离函数</param>
        /// <param name="antsCount">蚂蚁数量</param>
        public TSPByACO(IEnumerable<T> cities, Func<T, T, double> distanceFunction, int antsCount)
        {
            this.distanceFunction = distanceFunction;
            this.cities = cities.ToArray();
            AntsCount = antsCount;

            int len = this.cities.Length;
            pheromones = new double[len, len];
            citiesIndex = new Dictionary<T, int>();

            for (int i = 0; i < len; i++)
            {
                citiesIndex[this.cities[i]] = i;
            }

            InitPherom();
        }

        /// <summary>
        /// 信息素的权重
        /// </summary>
        public double ALPHA { get; set; } = 1;

        /// <summary>
        /// 距离的权重
        /// </summary>
        public double BETA { get; set; } = 1;

        /// <summary>
        /// 挥发系数
        /// </summary>
        public double RHO { get; set; } = .5;

        /// <summary>
        /// 迭代次数
        /// </summary>
        public int Iterations { get; set; } = 200;

        /// <summary>
        /// 信息素常量
        /// </summary>
        public int PheromConst { get; set; } = 100;

        /// <summary>
        /// 蚂蚁数量
        /// </summary>
        public int AntsCount
        {
            get => antColony.Length;

            set => antColony = new Ant[value];
        }

        /// <summary>
        /// 获取最优路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="bestTourLength">最优路径长度</param>
        /// <returns>最优路径</returns>
        public List<T> GetBestTour(T start, T end, out double bestTourLength)
        {
            this.start = start;
            this.end = end;
            relay = new List<T>(cities);
            relay.Remove(start);
            relay.Remove(end);

            InitAnts();
            Calculate();
            bestTourLength = this.bestTourLength;
            return bestTour ?? new List<T>();
        }

        /// <summary>
        /// 获取最优路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="bestTourLength">最优路径长度</param>
        /// <returns>最优路径</returns>
        /// <remarks>
        /// 此路径最终会回到起点
        /// </remarks>
        public List<T> GetBestTour(T start, out double bestTourLength)
        {
            this.start = start;
            end = null;
            relay = new List<T>(cities);
            relay.Remove(start);

            InitAnts();
            Calculate();
            bestTourLength = this.bestTourLength;
            return bestTour ?? new List<T>();
        }

        /// <summary>
        /// 获取最优路径
        /// </summary>
        /// <returns>最优路径</returns>
        /// <param name="bestTourLength">最优路径长度</param>
        /// <remarks>
        /// 此路径最终会回到起点
        /// </remarks>
        public List<T> GetBestTour(out double bestTourLength)
        {
            start = null;
            end = null;
            end = start;
            relay = new List<T>(cities);

            InitAnts();
            Calculate();
            bestTourLength = this.bestTourLength;
            return bestTour ?? new List<T>();
        }

        /// <summary>
        /// 初始化蚁群
        /// </summary>
        private void InitAnts()
        {
            for (int i = 0; i < AntsCount; i++)
            {
                Ant ant;
                if (start == null)
                {
                    var citiesIndex = random.Next(0, cities.Length);
                    var city = cities[citiesIndex];
                    ant = new Ant(city);
                }
                else
                {
                    ant = new Ant(start);
                }

                antColony[i] = ant;
            }
        }

        /// <summary>
        /// 初始化信息素
        /// </summary>
        private void InitPherom()
        {
            int len = cities.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    pheromones[i, j] = 1.0 / len;
                    pheromones[j, i] = 1.0 / len;
                }
            }
        }

        /// <summary>
        /// 执行算法
        /// </summary>
        private void Calculate()
        {
            for (int k = 0; k < Iterations; k++)
            {
                for (int i = 0; i < cities.Length; i++)
                {
                    if (AntsStop())
                    {
                        EvaporatePheromones();
                        UpdatePheromones();
                        UpdateBestTour();
                        InitAnts();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 移动一步并判断蚂蚁是否走完行程
        /// </summary>
        /// <returns>是否走完行程</returns>
        private bool AntsStop()
        {
            //移动的蚂蚁数
            int moved = 0;
            for (int i = 0; i < AntsCount; i++)
            {
                //当前蚂蚁是否走完行程
                if (antColony[i].HaveBeen.Count != cities.Length)
                {
                    GoToNextCity(antColony[i]);
                    moved++;
                }
            }

            if (moved == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 前往下座城市
        /// </summary>
        /// <param name="currentAnt">当前蚂蚁</param>
        /// <remarks>
        /// 根据路径的吸引力（信息素）选择下一个城市及其可见性（距离）
        /// </remarks>
        private void GoToNextCity(Ant currentAnt)
        {
            //概率系数的分母
            double sumProb = 0;

            //当前城市
            var currentCity = currentAnt.CurrentLocation;
            var currentCityIndex = citiesIndex[currentCity];

            //叠加所有未行走的城市计算系数
            for (int i = 0; i < cities.Length; i++)
            {
                var city = cities[i];

                //不能叠加终点
                if (!currentAnt.HaveBeen.Contains(city) && !city.Equals(end))
                {
                    sumProb += Math.Pow(pheromones[currentCityIndex, i], ALPHA) *
                               Math.Pow(1.0 / distanceFunction(currentCity, city), BETA);
                }
            }

            //目的地城市索引
            int destinationCityIndex = 0;
            T destinationCity = cities[destinationCityIndex];
            int count = 0;

            /*
             * 循环直到蚂蚁选择一个城市
             * 400为循环的阈值
             */
            while (count < 400)
            {
                //目的地城市
                destinationCity = cities[destinationCityIndex];

                //蚂蚁没有去过这个城市,蚂蚁永远最后走终点
                if (!currentAnt.HaveBeen.Contains(destinationCity) && !destinationCity.Equals(end))
                {
                    //计算移动概率
                    var moveProb = Math.Pow(pheromones[currentCityIndex, destinationCityIndex], ALPHA) *
                                    Math.Pow(1.0 / distanceFunction(currentCity, destinationCity), BETA) / sumProb;

                    //Console.WriteLine(moveProb);

                    //避免局部最优的随机量
                    var randMove = random.NextDouble();

                    //中标，选择进城
                    if (randMove < moveProb)
                    {
                        break;
                    }
                }

                destinationCityIndex++;

                //重置城市计数
                if (destinationCityIndex >= cities.Length)
                {
                    destinationCityIndex = 0;
                }

                count++;
            }

            //到达阈值，随机选择一个未走过的城市
            if (count == 400)
            {
                List<T> notVisited = new List<T>(cities);
                Debug.Assert(start != null && end != null);
                notVisited.Remove(start);
                notVisited.Remove(end);
                notVisited.RemoveAll(v => currentAnt.HaveBeen.Contains(v));
                var randIndex = random.Next(notVisited.Count);
                destinationCityIndex = citiesIndex[notVisited[randIndex]];
            }

            /*
             * 更新下一个地点和旅游行程
             * 前往城市
             */

            currentAnt.HaveBeen.Add(destinationCity);
            currentAnt.Tour.Add(destinationCity);
            currentAnt.TourNumber++;
            currentAnt.DistanceTraveled += distanceFunction(currentAnt.CurrentLocation, destinationCity);
            currentAnt.CurrentLocation = destinationCity;

            //计算旅途是否达到终点
            Debug.Assert(relay != null);
            int relayLen = relay.Count;
            if (start != null)
            {
                relayLen++;
            }

            //中继点是否走完
            if (currentAnt.TourNumber == relayLen)
            {
                if (end != null) //如果有终点走终点
                {
                    currentAnt.HaveBeen.Add(end);
                    currentAnt.Tour.Add(end);
                    currentAnt.TourNumber++;
                    currentAnt.DistanceTraveled += distanceFunction(currentAnt.CurrentLocation, end);
                    currentAnt.CurrentLocation = end;
                }
                else //没有计算回到起点的距离
                {
                    currentAnt.DistanceTraveled += distanceFunction(currentAnt.CurrentLocation, currentAnt.Tour[0]);
                }
            }
        }

        /// <summary>
        /// 蒸发信息素
        /// </summary>
        /// <remarks>
        /// 减少当前信息素的因子（1.0-RHO）所以信息素的较大值将以较高的速率减少,
        /// 通过这种方法可以使算法避免陷入局部最优解
        /// </remarks>
        private void EvaporatePheromones()
        {
            //handles both cases of [i,k] and [k,i] so dont need to code pheromones 2x
            for (int i = 0; i < cities.Length; i++)
            {
                for (int k = 0; k < cities.Length; k++)
                {
                    pheromones[i, k] *= 1.0 - RHO;

                    //pheromone levels should always be at base levels
                    if (pheromones[i, k] < 1.0 / cities.Length)
                    {
                        pheromones[i, k] = 1.0 / cities.Length;
                    }
                }
            }
        }

        /// <summary>
        /// 更新信息素
        /// </summary>
        /// <remarks>
        /// 在每只蚂蚁完成其旅程后，更新所有边缘的信息素
        /// </remarks>
        private void UpdatePheromones()
        {
            int lenK = cities.Length;

            //如果存在终点说明最优路径不考虑回来
            if (end != null)
            {
                lenK -= 1;
            }

            for (int i = 0; i < AntsCount; i++)
            {
                var ant = antColony[i];
                for (int k = 0; k < lenK; k++)
                {
                    //不存在终点的情况，如果city+1=numcities，则city是最后一个城市，而destination是起始城市

                    //边起点
                    int from = citiesIndex[ant.Tour[k]];

                    //边终点
                    int to = citiesIndex[ant.Tour[(k + 1) % cities.Length]];

                    pheromones[from, to] += PheromConst / ant.DistanceTraveled;
                    pheromones[to, from] = pheromones[from, to];
                }
            }
        }

        /// <summary>
        /// 更新全局最优路径
        /// </summary>
        private void UpdateBestTour()
        {
            double bestLocalTour = antColony[0].DistanceTraveled;
            int saveIndex = 0;

            //检查此迭代中的最佳行程长度
            for (int i = 1; i < AntsCount; i++)
            {
                if (antColony[i].DistanceTraveled < bestLocalTour)
                {
                    bestLocalTour = antColony[i].DistanceTraveled;
                    saveIndex = i;
                }
            }

            //将最佳局部长度与全局长度进行比较并相应更新
            if (bestLocalTour < bestTourLength || bestTourLength == -1)
            {
                bestTour = antColony[saveIndex].Tour;
                bestTourLength = bestLocalTour;
            }
        }

        /// <summary>
        /// 蚂蚁
        /// </summary>
        private class Ant
        {
            public Ant(T startPos)
            {
                CurrentLocation = startPos;
                DistanceTraveled = 0;
                HaveBeen = new HashSet<T> { startPos };
                Tour = new List<T> { startPos };
                TourNumber = 1;
            }

            /// <summary>
            /// 蚂蚁所处位置
            /// </summary>
            public T CurrentLocation { get; set; }

            /// <summary>
            /// 旅行编号
            /// </summary>
            /// <remarks>
            /// 标记第几次行走
            /// </remarks>
            public int TourNumber { get; set; }

            /// <summary>
            /// 蚂蚁去过的城市
            /// </summary>
            public HashSet<T> HaveBeen { get; }

            /// <summary>
            /// 蚂蚁的旅行轨迹
            /// </summary>
            public List<T> Tour { get; }

            /// <summary>
            /// 旅途长度
            /// </summary>
            public double DistanceTraveled { get; set; }
        }
    }
}
