using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    public static partial class YuyukoComment
    {
        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Yuyuko";

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
        private static WeightedRandom<LocalizedText> commentCollection = new();
        /// <summary>
        /// 更新评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="foodType">食物种类</param>
        /// <param name="feeded">是否主动投喂</param>
        public static void UpdateComment(this Projectile projectile, int foodType, bool feeded = false)
        {
            if (!projectile.IsATouhouPet())
                return;
            BasicTouhouPet yuyuko = projectile.AsTouhouPet();

            //若发现食物，则进行评价。反之会随机选取一个抱怨文本
            if (foodType > 0)
            {
                //分列表读取以实现覆盖效果
                //由于跨模组评价会被优先读取，因此可以对原版已有评价进行覆盖
                if (!projectile.Comment_CrossMod(foodType)
                    && !projectile.Comment_Vanilla(foodType))
                {
                    //若获取到的食物的类别不属于上述列表内容，则启用默认评价
                    projectile.DefaultComment(feeded);
                }
            }
            else if (!yuyuko.FindBoss)
            {
                projectile.SetChat(yuyuko.ChatDictionary[Main.rand.Next(8, 10 + 1)]);
            }
        }

        /// <summary>
        /// 当幽幽子没有吃到指定食物时给予的评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="feeded">是否主动投喂</param>
        private static void DefaultComment(this Projectile projectile, bool feeded = false)
        {
            if (!projectile.IsATouhouPet())
                return;
            BasicTouhouPet yuyuko = projectile.AsTouhouPet();

            //如果是主动投喂
            if (feeded)
            {
                projectile.SetChat(yuyuko.ChatDictionary[15], 60);
            }
            else
            {
                projectile.SetChat(yuyuko.ChatDictionary[Main.rand.Next(5, 7 + 1)], 60);
            }
        }

        /// <summary>
        /// 关于原版食物的评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="foodType">食物种类</param>
        private static bool Comment_Vanilla(this Projectile projectile, int foodType)
        {
            if (!projectile.IsATouhouPet())
                return false;

            //以防万一（？）
            if (acceptIDList_Vanilla.Count <= 0)
                return false;

            if (acceptIDList_Vanilla.Contains(foodType))
            {
                //清酒和啤酒的评价是一样的
                if (foodType == ItemID.Sake)
                    foodType = ItemID.Ale;

                projectile.SetChat(Language.GetText($"{Path}.Food_{acceptIDList_Vanilla.IndexOf(foodType) + 1}"), 60);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 跨模组食物的评价
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="foodType">食物种类</param>
        private static bool Comment_CrossMod(this Projectile projectile, int foodType)
        {
            //若列表不存在内容，则不执行后续
            if (CrossModFoodComment_Accept.Count <= 0)
                return false;

            WeightedRandom<LocalizedText> result = new();
            //遍历食物评价信息列表并选取评价
            foreach (var info in CrossModFoodComment_Accept)
            {
                if (foodType != info.ObjectType)
                    continue;

                if (info.CommentContent.Count <= 0)
                    continue;
             
                foreach (var j in info.CommentContent)
                {
                    if (j.Condition())
                        result.Add(j.DialogText, j.Weight);
                }
            }
            if (result.elements.Count > 0)
            {
                projectile.SetChat(result, 60);
                return true;
            }
            return false;
        }
    }
}
