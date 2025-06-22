using Terraria.Localization;

namespace TouhouPets
{
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
