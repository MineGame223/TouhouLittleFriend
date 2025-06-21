using Terraria.Localization;
using Terraria.Utilities;

namespace TouhouPets
{
    public struct CommentInfo(int type, WeightedRandom<LocalizedText> commentText)
    {
        /// <summary>
        /// 评价对象的种类
        /// </summary>
        public int ObjectType = type;
        /// <summary>
        /// 评价文本合集
        /// </summary>
        public WeightedRandom<LocalizedText> CommentText = commentText;
    }
}
