using System;
using System.Collections.Generic;
using System.Text;
using NComputerVision.DataStructures;

namespace NComputerVision.Common
{
    public interface IConvexHullAlgorithm
    {
        /// <summary>
        /// Find convex hull for the given set of points.
        /// </summary>
        /// 
        /// <param name="points">Set of points to search convex hull for.</param>
        /// 
        /// <returns>Returns set of points, which form a convex hull for the given <paramref name="points"/>.</returns>
        /// 
        List<IntPoint> FindHull(List<IntPoint> points);
    }
}
