
namespace NComputerVision.GraphicsLib
{
    using System;

    /// <summary>
    /// foamliu, 2009/02/24, 基本集.
    /// 
    /// </summary>
    [Serializable()]
    public enum HaarLikeFeatureType
    {
        A,
        B,
        C,
        D,
    }

    /// <summary>
    /// foamliu, 2009/02/24, 扩展集.
    /// 
    /// </summary>
    [Serializable()]
    public enum HaarLikeExtendedFeatureType
    {
        // 边缘特征
        Edge_A,
        Edge_B,
        Edge_C,
        Edge_D,

        // 线段特征
        Line_A,
        Line_B,
        Line_C,
        Line_D,
        Line_E,
        Line_F,
        Line_G,
        Line_H,

        // 中心特征
        Center_A,
        Center_B,
    }
    
}
