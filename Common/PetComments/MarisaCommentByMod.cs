using System.Collections.Generic;
using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    partial class MarisaComment
    {
        private static List<int> modNPCTypeList = [];
        private static List<string> modChatList = [];
        public static List<string> ModChatList { get => modChatList; set => modChatList = value; }
        public static List<int> ModNPCTypeList { get => modNPCTypeList; set => modNPCTypeList = value; }

        public static void RegisterComment_ByMod(this Marisa marisa)
        {
            if (modNPCTypeList == null || modChatList == null)
            {
                return;
            }

            int index = marisa.ChatDictionary.Count;
            startIndex[(int)DictionaryID.Mods] = index + 1;
            for (int i = 1; i <= modChatList.Count; i++)
            {
                marisa.ChatDictionary.TryAdd(index + i, modChatList[i - 1]);
            }
        }
        public static void BossChat_ByMod(this Projectile marisa, ChatSettingConfig config, NPC boss)
        {
            if (modNPCTypeList == null || modChatList == null)
            {
                return;
            }

            int index = startIndex[(int)DictionaryID.Mods];
            for (int i = 0; i < modNPCTypeList.Count; i++)
            {
                if (boss.type == modNPCTypeList[i])
                {
                    marisa.SetChat(config, index + i);
                    break;
                }
            }
        }
    }
}
