using System.Collections.Generic;

namespace Parser
{
    /// <summary>
    /// PK2200 采集项
    /// </summary>
    public class Items2200 : ItemsBase
    {
        /// <summary>
        /// T 端截止波长
        /// </summary>
        public decimal CutoffT { get; set; }

        /// <summary>
        /// B 端截止波长
        /// </summary>
        public decimal CutoffB { get; set; }

        /// <summary>
        /// T 端模场直径
        /// </summary>
        public Dictionary<int, decimal> MFDsT { get; set; } = new Dictionary<int, decimal>();

        /// <summary>
        /// B 端模场直径
        /// </summary>
        public Dictionary<int, decimal> MFDsB { get; set; } = new Dictionary<int, decimal>();

        /// <summary>
        /// 衰减
        /// </summary>
        public Dictionary<int, decimal> Attenuations { get; set; } = new Dictionary<int, decimal>();
    }
}