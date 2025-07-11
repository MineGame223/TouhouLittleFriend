using Terraria.Localization;
using TouhouPets.Common.ModSupports.ModPetRegisterSystem;

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

        /// <summary>
        /// 新建一个聊天室信息
        /// </summary>
        /// <param name="uniqueIDExtended">当前语句所属宠物的拓展独特ID</param>
        /// <param name="chatText">当前语句文本</param>
        /// <param name="chatTurn">当前语句所属的回合值</param>
        public ChatRoomInfo(int uniqueIDExtended, LocalizedText chatText, int chatTurn) : this((TouhouPetID)uniqueIDExtended, chatText, chatTurn) { }

        /// <summary>
        /// 新建一个聊天室信息
        /// </summary>
        /// <typeparam name="T">当前语句所属的宠物</typeparam>
        /// <param name="chatText">当前语句文本</param>
        /// <param name="chatTurn">当前语句所属的回合值</param>
        /// <returns></returns>
        public static ChatRoomInfo NewInfo<T>(LocalizedText chatText, int chatTurn) where T : BasicTouhouPet 
        {
            return new(ModTouhouPetLoader.UniqueID<T>(), chatText, chatTurn);
        }
    }
}
