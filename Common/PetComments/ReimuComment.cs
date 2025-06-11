using TouhouPets.Content.Projectiles.Pets;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using Terraria;
using System.Collections.Generic;

namespace TouhouPets
{
    public static class ReimuComment
    {
        private enum DictionaryID : int
        {
            MyMod,
            Count,
        }
        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Reimu";
        private const int MaxTouhouComment = 29;

        private static Dictionary<int, string> commentDictionary = [];
        private static int[] startIndex = new int[(int)DictionaryID.Count];
        private static void AddDialogToPets(this Reimu reimu, WeightedRandom<string> chat, Projectile target, int petType, int chatIndex)
        {
            if (target.type == petType && target.owner == reimu.Owner.whoAmI)
            {
                chat.Add(reimu.ChatDictionary[chatIndex]);
            }
        }
        public static void RegisterComments(this Reimu reimu)
        {
            for (int i = 1; i <= MaxTouhouComment; i++)
            {
                commentDictionary[i] = Language.GetTextValue($"{Path}.LightPet_{i}");
            }

            int index = reimu.ChatDictionary.Count;
            startIndex[(int)DictionaryID.MyMod] = index + 1;

            foreach (var comment in commentDictionary)
            {
                reimu.ChatDictionary.TryAdd(index + comment.Key, comment.Value);
            }
        }
        public static void Comment_TouhouLightPet(this Reimu reimu, WeightedRandom<string> chat)
        {
            int index = startIndex[(int)DictionaryID.MyMod];

            foreach (Projectile pet in Main.ActiveProjectiles)
            {
                reimu.AddDialogToPets(chat, pet, ProjectileType<Cirno>(), index);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Doremy>(), index + 1);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Eirin>(), index + 2);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Flandre>(), index + 3);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Iku>(), index + 4);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Junko>(), index + 5);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Kokoro>(), index + 6);

                reimu.AddDialogToPets(chat, pet, ProjectileType<Meirin>(), index + 8);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Moku>(), index + 9);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Nitori>(), index + 10);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Patchouli>(), index + 11);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Sanae>(), index + 12);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Satori>(), index + 13);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Shinki>(), index + 14);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Sizuha>(), index + 15);
                reimu.AddDialogToPets(chat, pet, ProjectileType<StarPet>(), index + 16);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Sunny>(), index + 17);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Luna>(), index + 18);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Utsuho>(), index + 19);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Wakasagihime>(), index + 20);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Wriggle>(), index + 21);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Youmu>(), index + 22);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Lily>(), index + 23);
                reimu.AddDialogToPets(chat, pet, ProjectileType<AliceOld>(), index + 24);

                reimu.AddDialogToPets(chat, pet, ProjectileType<Lunasa>(), index + 26);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Merlin>(), index + 27);
                reimu.AddDialogToPets(chat, pet, ProjectileType<Lyrica>(), index + 28);
            }

            if (reimu.Owner.HasBuff<TheThreeFairiesBuff>())
            {
                chat.Add(reimu.ChatDictionary[index + 7]);
            }
            if (reimu.Owner.HasBuff<PoltergeistBuff>())
            {
                chat.Add(reimu.ChatDictionary[index + 25]);
            }
        }
    }
}
