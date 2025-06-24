using System.Collections.Generic;

namespace TouhouPets
{
    /// <summary>
    /// 评价语句的结构体
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public struct CommentInfo(int type, List<SingleDialogInfo> content)
    {
        /// <summary>
        /// 评价对象的种类
        /// </summary>
        public int ObjectType = type;
        /// <summary>
        /// 评价文本合集
        /// </summary>
        public List<SingleDialogInfo> CommentContent = content;
    }
}
