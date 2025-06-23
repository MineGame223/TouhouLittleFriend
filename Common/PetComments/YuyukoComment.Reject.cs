using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    partial class YuyukoComment
    {
        private readonly static List<int> rejectIDList_Vanilla = [
            ItemID.JojaCola,//请不要喂我垃圾...
            ];

        /// <summary>
        /// 更新拒绝评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="foodType">食物种类</param>
        /// <param name="giveComment">是否给出评价</param>
        public static bool IsFoodOnRejectList(this Projectile projectile, int foodType, bool giveComment = false)
        {
            if (!projectile.IsATouhouPet())
                return false;

            //若发现食物，则进行评价。
            if (foodType > 0)
            {
                //若物品类别位于黑名单中，则直接拒绝并返回
                foreach (var i in GetInstance<MiscConfig_ClientSide>().YuyukoBanList)
                {
                    if (foodType == i.Type)
                    {
                        if (giveComment)
                            projectile.SetChat(projectile.AsTouhouPet().ChatDictionary[14]);

                        return true;
                    }
                }
                //分列表读取以实现覆盖效果
                //由于跨模组评价会被优先读取，因此可以对原版已有评价进行覆盖
                if (projectile.Reject_CrossMod(foodType, giveComment)
                    || projectile.Reject_Vanilla(foodType, giveComment))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 关于原版食物的拒绝评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="foodType">食物种类</param>
        /// <param name="giveComment">是否给出评价</param>
        private static bool Reject_Vanilla(this Projectile projectile, int foodType, bool giveComment = false)
        {
            if (!projectile.IsATouhouPet())
                return false;

            //以防万一（？）
            if (rejectIDList_Vanilla.Count <= 0)
                return false;

            if (rejectIDList_Vanilla.Contains(foodType))
            {
                if (giveComment)
                {
                    projectile.SetChat(Language.GetText($"{Path}.Food_Reject_{rejectIDList_Vanilla.IndexOf(foodType) + 1}"));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 跨模组食物的拒绝评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="foodType">食物种类</param>
        /// <param name="giveComment">是否给出评价</param>
        private static bool Reject_CrossMod(this Projectile projectile, int foodType, bool giveComment = false)
        {
            if (CrossModFoodComment.Count <= 0)
                return false;

            //遍历食物评价信息列表并选取评价
            foreach (var (info, accept) in CrossModFoodComment)
            {
                if (!accept && foodType == info.ObjectType)
                {
                    if (giveComment && info.Condition())
                    {
                        projectile.SetChat(info.CommentText);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
