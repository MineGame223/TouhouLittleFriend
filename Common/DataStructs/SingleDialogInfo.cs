using System;
using Terraria.Localization;

namespace TouhouPets
{
    /// <summary>
    /// 单句对话的结构体
    /// </summary>
    /// <param name="dialogText"></param>
    /// <param name="weight"></param>
    /// <param name="condition"></param>
    public struct SingleDialogInfo(LocalizedText dialogText, int weight, Func<bool> condition = null)
    {
        /// <summary>
        /// 对话文本
        /// </summary>
        public LocalizedText DialogText = dialogText;
        /// <summary>
        /// 对话权重
        /// </summary>
        public int Weight = weight;
        /// <summary>
        /// 对话出现的条件，默认为 true
        /// </summary>
        public Func<bool> Condition = condition ?? delegate () { return true; };
    }
}
