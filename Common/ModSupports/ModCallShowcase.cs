using System;
using Terraria.Localization;
using Terraria;
using System.Collections.Generic;
using Terraria.ID;

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

            //玩家位于海边
            Func<bool> condi_beach = delegate () { return Main.LocalPlayer.ZoneBeach; };

            //时间处于夜晚
            Func<bool> condi_night = delegate () { return !Main.dayTime; };

            //时间处于夜晚且玩家位于太空高度
            Func<bool> condi_night_space = delegate () { return !Main.dayTime && Main.LocalPlayer.ZoneSkyHeight; };

            //时间处于白天
            Func<bool> condi_day = delegate () { return Main.dayTime; };

            LocalizedText cirno1 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的，只会在海边出现！");
            LocalizedText cirno2 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的，只会在夜晚出现！");
            LocalizedText cirno3 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的，只会在夜晚的太空出现！");
            LocalizedText cirno4 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的！");

            LocalizedText junko1 = Language.GetText($"你知道吗？其实这句话是由 {nameof(TouhouPets)} 通过Mod.Call添加的。");
            LocalizedText reisen1 = Language.GetText("是吗...为什么要告诉我？");
            LocalizedText junko2 = Language.GetText("没什么，只是想让你知道。");
            LocalizedText junko3 = Language.GetText("这铁打的...");
            LocalizedText reisen2 = Language.GetText("啊这...");

            LocalizedText lunasa1 = Language.GetText("下一次的演出是什么时候？在哪儿？");
            LocalizedText merlin1 = Language.GetText("好像是在下周一的太阳花田吧？");
            LocalizedText lunasa2 = Language.GetText("要去那个花之妖怪的领地吗...");
            LocalizedText lyrica1 = Language.GetText("沟通的事情就交给姐姐们吧~ 人家要帮忙准备器具哦。");

            LocalizedText yuyuko1 = Language.GetText("汉堡是最棒的宵夜！");
            LocalizedText yuyuko2 = Language.GetText("你喜欢肯德基还是麦当劳？");
            LocalizedText yuyuko3 = Language.GetText("疯狂星期四？那是什么？");

            LocalizedText yuyuko4 = Language.GetText("当生活给了你柠檬...不，这个模组不让我吃。");
            LocalizedText yuyuko5 = Language.GetText("你有更小的几率在试图喂我柠檬时看到这句话哦。");

            LocalizedText youmu1 = Language.GetText("巨大石像来犯！");
            LocalizedText youmu2 = Language.GetText("哪怕是石头、观楼剑也劈得开！");

            LocalizedText youmu3 = Language.GetText("无论你是妖精女皇还是什么，都算作敌人！");
            LocalizedText youmu4 = Language.GetText("这秒杀一切的气场？！...要小心啊！");

            //宠物的独特ID值，详细见 TouhouPetUniqueID.cs
            int cirno = 1;//琪露诺
            int junko = 13;//纯狐
            int reisen = 39;//铃仙
            int youmu = 58;//妖梦
            int lunasa = 23;//露娜萨
            int merlin = 27;//梅露兰
            int lyrica = 24;//莉莉卡

            //为琪露诺添加四句常规对话，分别会在不同的条件下说出
            //参数分别为：Call类型、添加模组、宠物索引、文本、条件、权重
            //“添加模组”参数用于日志信息
            //条件和权重为选填项，默认分别为 true 和 1
            mod.Call("PetDialog", mod, cirno, cirno1, condi_beach);
            mod.Call("PetDialog", mod, cirno, cirno2, condi_night);
            mod.Call("PetDialog", mod, cirno, cirno3, condi_night_space);
            mod.Call("PetDialog", mod, cirno, cirno4);

            //为纯狐和铃仙添加一个聊天室
            //先将聊天室所需的第一句对话按照常规对话的形式加入
            mod.Call("PetDialog", mod, junko, junko1);
            //聊天室信息列表
            //元组中的三个参数分别为：宠物索引、文本索引、回合数
            //对话的回合值都是从 -1 开始的
            List<(int, LocalizedText, int)> chatRoom1 = new()
            {
                (junko, junko1, -1),
                (reisen, reisen1, 0),
                (junko, junko2, 1),
                //这里表示纯狐和铃仙在第二回合时会同时说话
                (reisen, junko3, 2),
                (junko, junko3, 2),
                (reisen, reisen2, 3),
            };
            //添加聊天室
            //下面参数分别为：Call类型、添加模组、聊天室信息列表
            mod.Call("PetChatRoom", mod, chatRoom1);

            //为骚灵三姐妹添加一个聊天室
            mod.Call("PetDialog", mod, lunasa, lunasa1);
            List<(int, LocalizedText, int)> chatRoom2 = new()
            {
                (lunasa, lunasa1, -1),
                (merlin, merlin1, 0),
                (lunasa, lunasa2, 1),
                (lyrica, lyrica1, 2),
            };
            mod.Call("PetChatRoom", mod, chatRoom2);

            //下面这些Call添加的文本都不会被计入宠物的对话字典中

            //为妖梦添加两句关于石巨人的评论，其中第二句拥有更高的权重
            //参数分别为：Call类型、添加模组、Boss种类、宠物索引、文本、条件、权重
            //条件和权重为选填项，默认分别为 true 和 1
            //理论上讲可以对模组内已有的同种类评价进行覆盖
            //但这是让你用来适配自己模组的Boss的，请不要随便覆盖已有内容
            //若多个模组对同一种类的Boss添加了评价，则这些评价会共存
            mod.Call("PetsReactionToBoss", mod, NPCID.Golem, youmu, youmu1);
            mod.Call("PetsReactionToBoss", mod, NPCID.Golem, youmu, youmu2, null, 2);

            //为妖梦添加两句关于光之女皇的评论
            //分别会在夜晚和白天时说出
            mod.Call("PetsReactionToBoss", mod, NPCID.HallowBoss, youmu, youmu3, condi_night);
            mod.Call("PetsReactionToBoss", mod, NPCID.HallowBoss, youmu, youmu4, condi_day);

            //为幽幽子添加三句接受汉堡评论的话，其中第一句只会在夜晚说出
            //参数分别为：Call类型、添加模组、食物种类、文本、是否接受该食物、条件、权重
            //是否接受该食物、条件和权重均为选填项，默认分别为true, true 和 1
            //理论上讲可以对模组内已有的同种类评价进行覆盖
            //但这是让你用来适配自己模组的食物的，请不要随便覆盖已有内容
            //若多个模组对同一种类的食物添加了评价，则这些评价会共存
            mod.Call("YuyukosReactionToFood", mod, ItemID.Burger, yuyuko1, true, condi_night);
            mod.Call("YuyukosReactionToFood", mod, ItemID.Burger, yuyuko2);
            mod.Call("YuyukosReactionToFood", mod, ItemID.Burger, yuyuko3);

            //为幽幽子添加两句拒绝柠檬评论的话，其中第一句拥有更高的权重
            //若同一食物同时存在拒绝与接受的条件，则拒绝的优先级更高
            mod.Call("YuyukosReactionToFood", mod, ItemID.Lemon, yuyuko4, false, null, 3);
            mod.Call("YuyukosReactionToFood", mod, ItemID.Lemon, yuyuko5, false);
        }
    }
}
