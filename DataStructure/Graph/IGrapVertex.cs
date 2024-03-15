using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 图中的顶点
    /// </summary>
    /// <typeparam name="TEdge">边类型</typeparam>
    public interface IGrapVertex<TEdge>
    {
        /// <summary>
        /// 获取顶点连接的边
        /// </summary>
        /// <returns>边</returns>
        List<TEdge> GetEdges();
    }
}
