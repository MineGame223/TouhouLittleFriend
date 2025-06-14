using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    public static class YuyukoComment
    {
        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Yuyuko";

        private static int startIndex = 0;

        private readonly static List<int> foodIDList_Vanilla = [
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

        public static void UpdateComment(this Projectile yuyuko, int foodType, bool feeded = false)
        {
            if (foodType > 0)
            {
                yuyuko.Comment_Vanilla(foodType, feeded);
                yuyuko.Comment_CrossMod(foodType, feeded);
            }
            else if (yuyuko.CurrentDialogFinished())
            {
                yuyuko.SetChat(Main.rand.Next(8, 10 + 1));
            }
        }
        private static void DefaultComment(this Projectile yuyuko, bool feeded = false)
        {
            if (feeded)
            {
                yuyuko.SetChat(15, 60);
            }
            else
            {
                yuyuko.SetChat(Main.rand.Next(5, 7 + 1), 60);
            }
        }
        public static void RegisterComment_Vanilla(this Yuyuko yuyuko)
        {
            int index = yuyuko.ChatDictionary.Count;
            startIndex = index + 1;

            for (int i = 0; i < foodIDList_Vanilla.Count; i++)
            {
                yuyuko.ChatDictionary.TryAdd(startIndex + i, Language.GetTextValue($"{Path}.Food_{i + 1}"));
            }
        }
        private static void Comment_Vanilla(this Projectile yuyuko, int foodType, bool feeded = false)
        {
            //以防万一（？）
            if (foodIDList_Vanilla.Count <= 0)
                return;

            int index = startIndex;

            yuyuko.CloseCurrentDialog();
            if (!foodIDList_Vanilla.Contains(foodType))
            {
                yuyuko.DefaultComment(feeded);
                return;
            }

            //清酒和啤酒的评价是一样的
            if (foodType == ItemID.Sake)
                foodType = ItemID.Ale;

            yuyuko.SetChat(index + foodIDList_Vanilla.IndexOf(foodType), 60);
        }
        private static void Comment_CrossMod(this Projectile yuyuko, int foodType, bool feeded = false)
        {
            //以防万一
            if (CrossModFoodComment.Count <= 0)
                return;

            yuyuko.CloseCurrentDialog();
            if (!foodIDList_Vanilla.Contains(foodType))
            {
                yuyuko.DefaultComment(feeded);
                return;
            }
            foreach (var i in CrossModFoodComment)
            {
                if (i.ObjectType != foodType)
                    continue;

                yuyuko.SetChat(i.CommentText.Value, 60);
            }
        }
    }
}
