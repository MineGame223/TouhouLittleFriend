using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    partial class YuyukoComment
    {
        private static int startIndex_Reject = 0;

        private readonly static List<int> rejectIDList_Vanilla = [
            ItemID.JojaCola,//请不要喂我垃圾...
            ];

        /// <summary>
        /// 更新拒绝评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="foodType">食物种类</param>
        /// <param name="giveComment">是否给出评价</param>
        public static bool IsFoodOnRejectList(this Projectile yuyuko, int foodType, bool giveComment = false)
        {
            //若发现食物，则进行评价。
            if (foodType > 0)
            {
                //若物品类别位于黑名单中，则直接拒绝并返回
                foreach (var i in GetInstance<MiscConfig_ClientSide>().YuyukoBanList)
                {
                    if (foodType == i.Type)
                    {
                        if (giveComment)
                            yuyuko.SetChat(14);

                        return true;
                    }
                }
                //分列表读取以实现覆盖效果
                //由于跨模组评价会被优先读取，因此可以对原版已有评价进行覆盖
                if (yuyuko.Reject_CrossMod(foodType, giveComment)
                    || yuyuko.Reject_Vanilla(foodType, giveComment))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 注册原版食物拒绝评价
        /// </summary>
        /// <param name="yuyuko"></param>
        public static void RegisterRejectComment_Vanilla(this Yuyuko yuyuko)
        {
            //记录起始索引值
            int index = yuyuko.ChatDictionary.Count;
            startIndex_Reject = index + 1;

            //以ID列表的长度为索引，注册相应对话
            for (int i = 0; i < rejectIDList_Vanilla.Count; i++)
            {
                yuyuko.ChatDictionary.TryAdd(startIndex_Reject + i, Language.GetTextValue($"{Path}.Food_Reject_{i + 1}"));
            }
        }

        /// <summary>
        /// 关于原版食物的拒绝评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="foodType">食物种类</param>
        private static bool Reject_Vanilla(this Projectile yuyuko, int foodType, bool giveComment = false)
        {
            //以防万一（？）
            if (rejectIDList_Vanilla.Count <= 0)
                return false;

            if (rejectIDList_Vanilla.Contains(foodType))
            {
                if (giveComment)
                {
                    int finalIndex = startIndex_Reject + rejectIDList_Vanilla.IndexOf(foodType);
                    //不是很必要的双重保险
                    if (yuyuko.AsTouhouPet().ChatDictionary.ContainsKey(finalIndex))
                    {
                        yuyuko.SetChat(finalIndex, 60);
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 关于跨模组食物的拒绝评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="foodType">食物种类</param>
        private static bool Reject_CrossMod(this Projectile yuyuko, int foodType, bool giveComment = false)
        {
            //以防万一
            if (CrossModFoodComment.Count <= 0)
                return false;

            //遍历食物评价信息列表并选取评价
            foreach (var (info, accept) in CrossModFoodComment)
            {
                if (!accept && foodType == info.ObjectType)
                {
                    if (giveComment)
                        yuyuko.SetChat(info.CommentText.Get().Value);

                    return true;
                }
            }
            return false;
        }
    }
}
