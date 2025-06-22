namespace TouhouPets
{
    public struct ChatRoomInfo(TouhouPetID uniqueID, int chatIndex, int chatTurn)
    {
        /// <summary>
        /// 当前语句所属宠物的独特ID
        /// </summary>
        public TouhouPetID UniqueID = uniqueID;
        /// <summary>
        /// 当前语句的索引值
        /// </summary>
        public int ChatIndex = chatIndex;
        /// <summary>
        /// 当前语句所属的回合值
        /// </summary>
        public int ChatTurn = chatTurn;
    }
}
