using System.Collections.Generic;
using Terraria.Localization;

namespace TouhouPets
{
    partial class TouhouPets
    {
        private static PetChatRoom[] chatRoom;
        public static PetChatRoom[] ChatRoom { get => chatRoom; set => chatRoom = value; }

        private static Dictionary<int, LocalizedText>[] chatDictionry = new Dictionary<int, LocalizedText>[(int)TouhouPetID.Count];
        public static Dictionary<int, LocalizedText>[] ChatDictionry { get => chatDictionry; set => chatDictionry = value; }

        private static Dictionary<LocalizedText, bool>[] isChatRoomActive = new Dictionary<LocalizedText, bool>[(int)TouhouPetID.Count];
        public static Dictionary<LocalizedText, bool>[] IsChatRoomActive { get => isChatRoomActive; set => isChatRoomActive = value; }

        private static void InitializeChatSetting()
        {
            ChatRoom = new PetChatRoom[ChatRoomSystem.MaxChatRoom];

            //需要对字典进行初始化
            for (int i = 0; i < (int)TouhouPetID.Count; i++)
            {
                ChatDictionry[i] = [];
                IsChatRoomActive[i] = [];
            }           
        }
        private static void NullifyChatSetting()
        {
            ChatRoom = null;
            ChatDictionry = null;
            IsChatRoomActive = null;
        }
    }
}
