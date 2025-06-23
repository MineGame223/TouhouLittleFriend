using System;
using Terraria.Localization;
using Terraria.Utilities;

namespace TouhouPets
{
    /// <summary>
    /// 评价语句的结构体
    /// </summary>
    /// <param name="type"></param>
    /// <param name="commentText"></param>
    /// <param name="condition"></param>
    public struct CommentInfo(int type, WeightedRandom<LocalizedText> commentText, Func<bool> condition = null)
    {
        /// <summary>
        /// 评价对象的种类
        /// </summary>
        public int ObjectType = type;
        /// <summary>
        /// 评价文本合集
        /// </summary>
        public WeightedRandom<LocalizedText> CommentText = commentText;
        /// <summary>
        /// 评价出现的条件，默认为 true
        /// </summary>
        public Func<bool> Condition = condition ?? delegate () { return true; };
    }
}
