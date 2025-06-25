using Terraria.Localization;

namespace TouhouPets
{
    /// <summary>
    /// 聊天室信息的结构体
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <param name="chatText"></param>
    /// <param name="chatTurn"></param>
    public struct ChatRoomInfo(TouhouPetID uniqueID, LocalizedText chatText, int chatTurn)
    {
        /// <summary>
        /// 当前语句所属宠物的独特ID
        /// </summary>
        public TouhouPetID UniqueID = uniqueID;
        /// <summary>
        /// 当前语句文本
        /// </summary>
        public LocalizedText ChatText = chatText;
        /// <summary>
        /// 当前语句所属的回合值
        /// </summary>
        public int ChatTurn = chatTurn;
    }
}
