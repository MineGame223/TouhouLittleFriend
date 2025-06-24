using System;
using Terraria.Localization;

namespace TouhouPets
{
    /// <summary>
    /// 评价内容的记录结构体
    /// </summary>
    /// <param name="CommentText"></param>
    /// <param name="Condition"></param>
    public record struct CommentContent(LocalizedText CommentText, Func<bool> Condition = null)
    {
        /// <summary>
        /// 评价文本
        /// </summary>
        public LocalizedText CommentText = CommentText;
        /// <summary>
        /// 评价出现的条件，默认为 true
        /// </summary>
        public Func<bool> Condition = Condition ?? delegate () { return true; };
    }
}
