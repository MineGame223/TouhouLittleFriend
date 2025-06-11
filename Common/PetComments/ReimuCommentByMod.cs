using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets
{
    partial class ReimuComment
    {
        private static List<int> modPetTypeList = [];
        private static List<string> modChatList = [];
        public static List<string> ModChatList { get => modChatList; set => modChatList = value; }
        public static List<int> ModPetTypeList { get => modPetTypeList; set => modPetTypeList = value; }
        public static void RegisterComment_ByMod(this Reimu reimu)
        {
            if (modPetTypeList == null || modChatList == null)
            {
                return;
            }

            int index = reimu.ChatDictionary.Count;
            startIndex[(int)DictionaryID.Mods] = index + 1;
            for (int i = 1; i <= modChatList.Count; i++)
            {
                reimu.ChatDictionary.TryAdd(index + i, modChatList[i - 1]);
            }
        }
        public static void PetChat_ByMod(this Reimu reimu, WeightedRandom<string> chat)
        {
            if (modPetTypeList == null || modChatList == null)
            {
                return;
            }
            int index = startIndex[(int)DictionaryID.Mods];
            foreach (Projectile pet in Main.ActiveProjectiles)
            {
                if (!Main.projPet[pet.type])
                    continue;

                for (int i = 0; i < modPetTypeList.Count; i++)
                {
                    reimu.AddDialogToPets(chat, pet, modPetTypeList[i], index + i);
                }
            }
        }
    }
}
