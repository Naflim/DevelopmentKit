using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 图节点
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    public interface IGraphNode<T>
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        T GetValue();

        /// <summary>
        /// 获取此节点相连的其他节点
        /// </summary>
        /// <returns>相连的其他节点</returns>
        IGraphNode<T>[] GetNextNodes();
    }
}
