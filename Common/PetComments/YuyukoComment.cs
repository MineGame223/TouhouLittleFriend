using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    public static partial class YuyukoComment
    {
        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Yuyuko";

        private static int startIndex = 0;

        private static readonly List<int> acceptIDList_Vanilla = [
            ItemID.ChocolateChipCookie,//现烤的最好吃！
            ItemID.GrubSoup,//奇特的丛林美食，富含蛋白质！
            ItemID.Sashimi,//是家乡的味道呢...但是冥界并没有海吧？
            ItemID.Burger,//向传奇商业食物致敬！
            ItemID.Fries,//没有番茄酱或者炸鱼的薯条是没有灵魂的...
            ItemID.GoldenDelight,//呜呼！金美味！
            ItemID.ShuckedOyster,//壳什么的一起吃掉就好啦！
            ItemID.Apple,//一天一个苹果，医生...欸我需要医生吗？
            ItemID.Cherry, //这不会爆炸，对吧？
            ItemID.Pizza,//我已经把菠萝都藏起来了...
            ItemID.Escargot,//能不能做成派呢？
            ItemID.ChickenNugget,//没有碎骨更好吃！
            ItemID.Ale,//人生得意须尽欢，莫使金樽空对月。干了！
            ItemID.Sake,
            ];

        /// <summary>
        /// 更新评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="foodType">食物种类</param>
        /// <param name="feeded">是否主动投喂</param>
        public static void UpdateComment(this Projectile yuyuko, int foodType, bool feeded = false)
        {
            if (!yuyuko.IsATouhouPet())
                return;

            //若发现食物，则进行评价。反之会随机选取一个抱怨文本
            if (foodType > 0)
            {
                //分列表读取以实现覆盖效果
                //由于跨模组评价会被优先读取，因此可以对原版已有评价进行覆盖
                if (!yuyuko.Comment(foodType))
                {
                    //若获取到的食物的类别不属于上述列表内容，则启用默认评价
                    yuyuko.DefaultComment(feeded);
                }
            }
            else if (!yuyuko.AsTouhouPet().FindBoss)
            {
                int index = Main.rand.Next(8, 10 + 1);
                yuyuko.SetChat(index);
            }
        }

        /// <summary>
        /// 当幽幽子没有吃到指定食物时给予的评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="feeded">是否主动投喂</param>
        private static void DefaultComment(this Projectile yuyuko, bool feeded = false)
        {
            //如果是主动投喂
            if (feeded)
            {
                yuyuko.SetChat(15, 60);
            }
            else
            {
                yuyuko.SetChat(Main.rand.Next(5, 7 + 1), 60);
            }
        }

        /// <summary>
        /// 注册原版食物评价
        /// </summary>
        /// <param name="yuyuko"></param>
        public static void RegisterComment_Vanilla(this Yuyuko yuyuko)
        {
            //记录起始索引值
            int index = yuyuko.ChatDictionary.Count;
            startIndex = index + 1;

            //以ID列表的长度为索引，注册相应对话
            for (int i = 0; i < acceptIDList_Vanilla.Count; i++)
            {
                yuyuko.ChatDictionary.TryAdd(startIndex + i, Language.GetTextValue($"{Path}.Food_{i + 1}"));
            }
        }

        /// <summary>
        /// 关于原版食物的评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="foodType">食物种类</param>
        private static bool Comment_Vanilla(this Projectile yuyuko, int foodType)
        {
            //以防万一（？）
            if (acceptIDList_Vanilla.Count <= 0)
                return false;

            if (acceptIDList_Vanilla.Contains(foodType))
            {
                //清酒和啤酒的评价是一样的
                if (foodType == ItemID.Sake)
                    foodType = ItemID.Ale;

                int finalIndex = startIndex + acceptIDList_Vanilla.IndexOf(foodType);
                //不是很必要的双重保险
                if (yuyuko.AsTouhouPet().ChatDictionary.ContainsKey(finalIndex))
                {
                    yuyuko.SetChat(finalIndex, 60);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 总食物的评价
        /// </summary>
        /// <param name="yuyuko"></param>
        /// <param name="foodType">食物种类</param>
        private static bool Comment(this Projectile yuyuko, int foodType)
        {
            //以防万一
            if (CrossModFoodComment.Count <= 0)
                return yuyuko.Comment_Vanilla(foodType);

            //遍历食物评价信息列表并选取评价
            foreach (var (info, accept, cover) in CrossModFoodComment)
            {
                if (accept && foodType == info.ObjectType)
                {
                    if (!cover && foodType < ItemID.Count && Main.rand.NextBool(2))
                        return yuyuko.Comment_Vanilla(foodType);

                    yuyuko.SetChat(info.CommentText.Get().Value, 60);
                    return true;
                }
            }
            return false;
        }
    }
}
