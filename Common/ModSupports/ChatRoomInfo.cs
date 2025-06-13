namespace TouhouPets
{
    public struct ChatRoomInfo(TouhouPetID uniqueID, int chatIndex, int chatTurn)
    {
        public TouhouPetID UniqueID = uniqueID;
        public int ChatIndex = chatIndex;
        public int ChatTurn = chatTurn;
    }
}
