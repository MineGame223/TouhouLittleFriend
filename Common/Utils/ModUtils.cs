using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace TouhouPets
{
    /// <summary>
    /// 各种静态拓展方法
    /// </summary>
    public static class ModUtils
    {
        #region 模组配置快捷引用
        public static bool CompatibilityMode => GetInstance<MiscConfig_ServerSide>().CompatibilityMode;
        public static bool AllowCall_MarisasReaction => GetInstance<MiscConfig_ServerSide>().AllowModCall_MarisasReaction;
        public static bool AllowCall_YuyukosReaction => GetInstance<MiscConfig_ServerSide>().AllowModCall_YuyukosReaction;
        public static bool AllowCall_PetDialog => GetInstance<MiscConfig_ServerSide>().AllowModCall_PetDialog;
        public static bool AllowCall_PetChatRoom => GetInstance<MiscConfig_ServerSide>().AllowModCall_PetChatRoom;

        public static bool EnableCustomMusicMode => GetInstance<MiscConfig_ClientSide>().EnableCustomMusicMode;
        public static bool PetInvisWhenMouseHover => GetInstance<MiscConfig_ClientSide>().PetInvisWhenMouseHover;
        public static bool PetLightOnPlayer => GetInstance<MiscConfig_ClientSide>().PetLightOnPlayer;
        public static List<ItemDefinition> YuyukoBanList => GetInstance<MiscConfig_ClientSide>().YuyukoBanList;

        public static bool SpecialAbility_Hina => GetInstance<PetAbilitiesConfig>().SpecialAbility_Hina;
        public static bool SpecialAbility_Letty => GetInstance<PetAbilitiesConfig>().SpecialAbility_Letty;
        public static bool SpecialAbility_Minoriko => GetInstance<PetAbilitiesConfig>().SpecialAbility_Minoriko;
        public static bool SpecialAbility_MokuAndKaguya => GetInstance<PetAbilitiesConfig>().SpecialAbility_MokuAndKaguya;
        public static bool SpecialAbility_Murasa => GetInstance<PetAbilitiesConfig>().SpecialAbility_Murasa;
        public static bool SpecialAbility_Prismriver => GetInstance<PetAbilitiesConfig>().SpecialAbility_Prismriver;
        public static bool SpecialAbility_Satori => GetInstance<PetAbilitiesConfig>().SpecialAbility_Satori;
        public static bool SpecialAbility_Star => GetInstance<PetAbilitiesConfig>().SpecialAbility_Star;
        public static bool SpecialAbility_Tenshin => GetInstance<PetAbilitiesConfig>().SpecialAbility_Tenshin;
        public static bool SpecialAbility_Tewi => GetInstance<PetAbilitiesConfig>().SpecialAbility_Tewi;
        public static bool SpecialAbility_Wriggle => GetInstance<PetAbilitiesConfig>().SpecialAbility_Wriggle;
        public static bool SpecialAbility_Yuka => GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuka;
        public static bool SpecialAbility_Yuyuko => GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuyuko;

        public static float PetChatFrequency => GetInstance<PetDialogConfig>().ChatFrequency;
        public static bool TyperStylePetDialog => GetInstance<PetDialogConfig>().TyperStyleChat;

        public static bool PetCanDropFromBoss => GetInstance<PetObtainConfig>().PetCanDropFromBoss;
        public static bool ObtainPetByFishing => GetInstance<PetObtainConfig>().ObtainPetByFishing;
        public static bool AllowGapToSpawn => GetInstance<PetObtainConfig>().AllowGapToSpawn;
        #endregion

        /// <summary>
        /// 检查一个弹幕是否属于BasicTouhouPet类或派生
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static bool IsATouhouPet(this Projectile projectile,out BasicTouhouPet pet)
        {
            if (projectile.ModProjectile is BasicTouhouPet pet2) 
            {
                pet = pet2;
                return true;
            }
            pet = null;
            return false;
        }

        /// <summary>
        /// 检查一个弹幕是否属于BasicTouhouPet类或派生
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static bool IsATouhouPet(this Projectile projectile)
        {
            return projectile.ModProjectile is BasicTouhouPet;
        }

        /// <summary>
        /// 将 <see cref="Projectile"/> 类转换为 <see cref="BasicTouhouPet"/> 类
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static BasicTouhouPet AsTouhouPet(this Projectile projectile)
        {
            return projectile.ModProjectile as BasicTouhouPet;
        }

        /// <summary>
        /// 查找其他模组的指定NPC
        /// </summary>
        /// <param name="modName">模组类名字符串</param>
        /// <param name="target">需要对比的目标NPC</param>
        /// <param name="npcName">指定NPC类名字符串</param>
        /// <returns></returns>
        public static bool HasModAndFindNPC(string modName, NPC target, string npcName)
        {
            if (ModLoader.TryGetMod(modName, out Mod result))
            {
                if (result.TryFind(npcName, out ModNPC n) && target.type == n.Type)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否为特定语言
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static bool IsSpecificLanguage(GameCulture.CultureName lang)
        {
            return Language.ActiveCulture.LegacyId == (int)lang;
        }

        /// <summary>
        /// 将输入价格转换为货币单位价格的文本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CoinValue(int value)
        {
            int p = value % (int)Math.Pow(100, 4) / (int)Math.Pow(100, 3);
            int g = value % (int)Math.Pow(100, 3) / (int)Math.Pow(100, 2);
            int s = value % (int)Math.Pow(100, 2) / 100;
            int c = value % 100;
            string textP = p <= 0 ? "" : $"[i/s{p}:{ItemID.PlatinumCoin}]";
            string textG = g <= 0 ? "" : $"[i/s{g}:{ItemID.GoldCoin}]";
            string textS = s <= 0 ? "" : $"[i/s{s}:{ItemID.SilverCoin}]";
            string textC = c <= 0 ? "" : $"[i/s{c}:{ItemID.CopperCoin}]";
            return textP + textG + textS + textC;
        }

        /// <summary>
        /// 在Buff内生成宠物并设置Buff时间
        /// </summary>
        /// <param name="player"></param>
        /// <param name="buffIndex"></param>
        /// <param name="petType">宠物ID</param>
        /// <param name="buffTime">Buff持续时间，默认18000</param>
        public static void SpawnPetAndSetBuffTime(this Player player, int buffIndex, int petType, int buffTime = 18000)
        {
            player.buffTime[buffIndex] = buffTime;
            bool flag = petType != -1 && player.ownedProjectileCounts[petType] <= 0;
            if (flag && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, petType, 0, 0f, player.whoAmI);
            }
        }

        /// <summary>
        /// 在已有物品描述后插入新描述(允许添加变量)
        /// </summary>
        public static void InsertTooltipLine(this List<TooltipLine> tooltips, string text)
        {
            int index = tooltips.FindLastIndex((TooltipLine x) =>
            x.Name.StartsWith("Tooltip") && x.Mod == "Terraria");

            if (index == -1)
            {
                index = tooltips.FindIndex((TooltipLine x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            }
            if (index == -1)
                return;

            tooltips.Insert(index + 1, new TooltipLine(TouhouPets.Instance, "EachLine" + index.ToString(), text));
        }
        /// <summary>
        /// 获取对话文本
        /// </summary>
        /// <param name="tag">角色对应标签</param>
        /// <param name="index">文本对应编号</param>
        /// <returns></returns>
        public static LocalizedText GetChatText(string tag, string index, params object[] args)
        {
            return Language.GetText($"Mods.{nameof(TouhouPets)}.Chat_{tag}.Chat{index}").WithFormatArgs(args);
        }
        /// <summary>
        /// 获取对话文本
        /// </summary>
        /// <param name="tag">角色对应标签</param>
        /// <param name="index">文本对应编号</param>
        /// <returns></returns>
        public static LocalizedText GetChatText(string tag, int index, params object[] args)
        {
            return Language.GetText($"Mods.{nameof(TouhouPets)}.Chat_{tag}.Chat{index}").WithFormatArgs(args);
        }

        /// <summary>
        /// 额外设置宠物物品基础属性
        /// </summary>
        /// <param name="item"></param>
        /// <param name="width">物品宽</param>
        /// <param name="height">物品高</param>
        /// <param name="rare">物品稀有度，默认为橙色</param>
        /// <param name="value">物品购买价值，默认为2金</param>
        public static void DefaultToVanitypetExtra(this Item item, int width, int height, int rare = ItemRarityID.Orange, int value = 20000)
        {
            item.width = width;
            item.height = height;
            item.rare = rare;
            item.value = value;
        }

        /// <summary>
        /// 维持宠物弹幕的存在时间
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="owner"></param>
        /// <param name="buffType">宠物对应buff种类</param>
        public static void SetPetActive(this Projectile projectile, Player owner, int buffType)
        {
            if (!owner.dead && owner.HasBuff(buffType))
            {
                projectile.timeLeft = 2;
            }
        }
    }
}
