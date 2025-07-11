using System;
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

        /// <summary>
        /// 对对话数组进行扩容
        /// <br></br>
        /// <br>在<see cref="ModPetRegisterSystem.PostSetupContent"/>中调用</br>
        /// </summary>
        /// <param name="newSize">扩容后的大小</param>
        internal static void ResizeChatSetting(int newSize) 
        {
            // 因为要兼容新增宠物，这里要进行一次扩容
            // +1 是因为Count本身也占了一个，在没有扩展模组的情况下不需要扩容
            if (newSize <= (int)TouhouPetID.Count + 1) return;
            Array.Resize(ref chatDictionry, newSize);
            Array.Resize(ref isChatRoomActive, newSize);
            for (int i = (int)TouhouPetID.Count; i < newSize; i++)
            {
                ChatDictionry[i] = [];
                IsChatRoomActive[i] = [];
            }
        }

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
