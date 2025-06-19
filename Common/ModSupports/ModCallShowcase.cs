using System;
using Terraria.Localization;
using Terraria;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Utilities;

namespace TouhouPets
{
    public static class ModCallShowcase
    {
        /// <summary>
        /// Mod.Call示范
        /// <br>整段内容会在 TouhouPets.cs 的 PostSetUpContent 中添加</br>
        /// </summary>
        /// <param name="mod"></param>
        public static void SetModCall(this Mod mod)
        {
            //使用方法委托作为判定条件参数

            //使用此条件则表示永远不会出现在常规对话中
            Func<bool> notRegular = delegate () { return false; };

            //使用此条件则表示会一直出现在常规对话中
            Func<bool> isRegular = delegate () { return true; };

            //玩家位于海边
            Func<bool> condi_1 = delegate () { return Main.LocalPlayer.ZoneBeach; };

            //时间处于夜晚
            Func<bool> condi_2 = delegate () { return !Main.dayTime; };

            //时间处于夜晚且玩家位于太空高度
            Func<bool> condi_3 = delegate () { return !Main.dayTime && Main.LocalPlayer.ZoneSkyHeight; };

            LocalizedText text_1 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 给所有宠物添加的，只会在海边出现");
            LocalizedText text_2 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 给所有宠物添加的，只会在夜晚出现");
            LocalizedText text_3 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 给所有宠物添加的，只会在夜晚的太空出现");

            LocalizedText text_4 = Language.GetText($"你知道吗？其实这句话是由 {nameof(TouhouPets)} 通过Mod.Call添加的。");
            LocalizedText text_5 = Language.GetText($"是吗...为什么要告诉我？");
            LocalizedText text_6 = Language.GetText($"没什么，只是想让你知道。");
            LocalizedText text_7 = Language.GetText($"这铁打的...");
            LocalizedText text_8 = Language.GetText($"啊这...");

            LocalizedText comment_m1 = Language.GetText($"这句话覆盖了原本对独眼巨鹿的评价！");
            LocalizedText comment_m2 = Language.GetText($"哇！鹿！");
            LocalizedText comment_m3 = Language.GetText($"你知道吗？这句话会在独眼巨鹿出现时有1/3的几率出现。");
            WeightedRandom<LocalizedText> marisa_1 = new();
            marisa_1.Add(comment_m1);
            marisa_1.Add(comment_m2);
            marisa_1.Add(comment_m3);

            LocalizedText comment_y1 = Language.GetText($"这句话覆盖了原本对汉堡的评价！");
            LocalizedText comment_y2 = Language.GetText($"你喜欢肯德基还是麦当劳？");
            WeightedRandom<LocalizedText> yuyuko_1 = new();
            yuyuko_1.Add(comment_y1);
            yuyuko_1.Add(comment_y2);

            LocalizedText comment_y3 = Language.GetText($"当生活给了你柠檬...不，这个模组不让我吃。");
            LocalizedText comment_y4 = Language.GetText($"我能看见的只有那棵黄黄的柠檬树。");
            LocalizedText comment_y5 = Language.GetText($"你有更大的几率在试图喂我柠檬时看到这句话哦。");
            WeightedRandom<LocalizedText> yuyuko_2 = new();
            yuyuko_2.Add(comment_y3);
            yuyuko_2.Add(comment_y4);
            yuyuko_2.Add(comment_y5, 3);

            LocalizedText comment_ym1 = Language.GetText($"巨大石像来犯！");
            LocalizedText comment_ym2 = Language.GetText($"就算是石头、观楼剑也劈得开！");
            WeightedRandom<LocalizedText> youmu_1 = new();
            youmu_1.Add(comment_ym1);
            youmu_1.Add(comment_ym2);

            //遍历全部TouhouPetID表并为所有宠物添加上面的三句对话
            for (int i = 1; i < (int)TouhouPetID.Count; i++)
            {
                //参数分别为：Call类型、宠物索引、文本、条件、权重、添加模组
                //内部索引值由添加顺序决定、从0开始，此处可视为0、1、2
                //最后一个参数为添加方模组的实例，用于日志信息
                //虽然很想做成选填项，但TML不允许
                mod.Call("PetDialog", i, text_1, condi_1, 1, mod);
                mod.Call("PetDialog", i, text_2, condi_2, 1, mod);
                mod.Call("PetDialog", i, text_3, condi_3, 1, mod);
            }

            //宠物的独特ID值，详细见 TouhouPetUniqueID.cs
            int junko = 13;//纯狐
            int reisen = 39;//铃仙

            //先将聊天室所需的对话按照常规对话的形式加入，注意需要将仅在聊天室里出现的对话排除在常规对话之外
            mod.Call("PetDialog", junko, text_4, isRegular, 1, mod);//索引为3
            mod.Call("PetDialog", junko, text_6, notRegular, 1, mod);//索引为4，且这句话不会出现在常规对话里，下同
            mod.Call("PetDialog", junko, text_7, notRegular, 1, mod);//索引为5

            mod.Call("PetDialog", reisen, text_5, notRegular, 1, mod);//索引为3，且这句话不会出现在常规对话里，下同
            mod.Call("PetDialog", reisen, text_7, notRegular, 1, mod);//索引为4
            mod.Call("PetDialog", reisen, text_8, notRegular, 1, mod);//索引为5

            //聊天室信息列表
            List<(int, int, int)> chatRoom1 = new()
            {
                (junko,3,-1),
                (reisen,3,0),
                (junko,4,1),
                //这里表示纯狐和铃仙在第二回合时会同时说话
                (reisen,4,2),
                (junko,5,2),
                (reisen,5,3),
            };

            //第二个参数的值一定要和聊天列表里第一个元素的Item1值相同
            mod.Call("PetChatRoom", junko, chatRoom1, mod);

            //为魔理沙添加三句覆盖原版独眼巨鹿评论的话
            //参数分别为：Call类型、Boss种类、宠物类别名称、文本、添加模组
            mod.Call("PetReactionToBoss", NPCID.Deerclops, "Marisa", marisa_1, mod);

            //为妖梦添加两句关于原版石巨人评论的话
            mod.Call("PetReactionToBoss", NPCID.Golem, "Youmu", youmu_1, mod);

            //为幽幽子添加两句接受并覆盖原版汉堡评论的话
            //参数分别为：Call类型、食物种类、文本、是否接受该食物、添加模组
            mod.Call("YuyukosReactionToFood", ItemID.Burger, yuyuko_1, true, mod);

            //为幽幽子添加三句拒绝柠檬评论的话
            //若同一食物同时存在拒绝与接受的条件，则拒绝的优先级更高
            mod.Call("YuyukosReactionToFood", ItemID.Lemon, yuyuko_2, false, mod);
        }
    }
}
