using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    public class ChatRoomSystem : ModSystem
    {
        public const int MaxChatRoom = 50;
        public override void OnModLoad()
        {
            for (int i = 0; i < MaxChatRoom; i++)
            {
                ChatRoom[i] = new PetChatRoom();
            }
        }
        public override void OnModUnload()
        {
            for (int i = 0; i < MaxChatRoom; i++)
            {
                ChatRoom[i] = null;
            }
        }
    }
}
