using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naflim.DevelopmentKit.DataStructure.Graph
{
    /// <summary>
    /// 图中的边
    /// </summary>
    /// <typeparam name="TVertex">点类型</typeparam>
    public interface IGrapEdge<TVertex>
    {
        /// <summary>
        /// 获取边上的两个顶点
        /// </summary>
        /// <returns>顶点</returns>
        List<TVertex> GetVertexs();
    }
}
