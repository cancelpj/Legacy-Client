namespace Parser
{
    public class Items2400
    {
        /// <summary>
        /// 纤芯
        /// </summary>
        public Items2400Line Core { get; set; }

        /// <summary>
        /// 包层
        /// </summary>
        public Items2400Line[] Clad { get; set; }

        /// <summary>
        /// 涂敷层1
        /// </summary>
        public Items2400Line[] Coat1 { get; set; }

        /// <summary>
        /// 涂敷层2
        /// </summary>
        public Items2400Line[] Coat2 { get; set; }

        /// <summary>
        /// 应力区1
        /// </summary>
        public Items2400Line[] Rod1 { get; set; }

        /// <summary>
        /// 应力区2
        /// </summary>
        public Items2400Line[] Rod2 { get; set; }

        /// <summary>
        /// 到包层直线度
        /// </summary>
        public decimal PlanarityToClad { get; set; }

        /// <summary>
        /// 到纤芯直线度
        /// </summary>
        public decimal PlanarityToCore { get; set; }
    }

    public class Items2400Line
    {
        /// <summary>
        /// 直径 单位:um
        /// </summary>
        public string Diameter { get; set; }

        /// <summary>
        /// 不圆度 单位:%
        /// </summary>
        public string Noncirc { get; set; }

        /// <summary>
        /// 同心度 单位:um
        /// </summary>
        public string Conc { get; set; }
    }
}