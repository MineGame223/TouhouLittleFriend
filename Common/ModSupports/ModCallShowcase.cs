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

            //使用此条件则表示会一直出现在常规对话中
            Func<bool> isRegular = delegate () { return true; };

            //玩家位于海边
            Func<bool> condi_1 = delegate () { return Main.LocalPlayer.ZoneBeach; };

            //时间处于夜晚
            Func<bool> condi_2 = delegate () { return !Main.dayTime; };

            //时间处于夜晚且玩家位于太空高度
            Func<bool> condi_3 = delegate () { return !Main.dayTime && Main.LocalPlayer.ZoneSkyHeight; };

            LocalizedText text_1 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的，只会在海边出现！");
            LocalizedText text_2 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的，只会在夜晚出现！");
            LocalizedText text_3 = Language.GetText($"这句话是由 {nameof(TouhouPets)} 添加的，只会在夜晚的太空出现！");

            LocalizedText text_4 = Language.GetText($"你知道吗？其实这句话是由 {nameof(TouhouPets)} 通过Mod.Call添加的。");
            LocalizedText text_5 = Language.GetText("是吗...为什么要告诉我？");
            LocalizedText text_6 = Language.GetText("没什么，只是想让你知道。");
            LocalizedText text_7 = Language.GetText("这铁打的...");
            LocalizedText text_8 = Language.GetText("啊这...");

            WeightedRandom<LocalizedText> yuyuko_1 = new();
            {
                yuyuko_1.Add(Language.GetText("这句话加入到了对汉堡的评价！"));
            }

            WeightedRandom<LocalizedText> yuyuko_2 = new();
            {
                yuyuko_2.Add(Language.GetText("你喜欢肯德基还是麦当劳？"));
                yuyuko_2.Add(Language.GetText("疯狂星期四？那是什么？"));
            }

            WeightedRandom<LocalizedText> yuyuko_3 = new();
            {
                yuyuko_3.Add(Language.GetText("正常来说你看不到这句话，除非你注释掉了下面拒绝食用柠檬的Call。"));
            }

            WeightedRandom<LocalizedText> yuyuko_4 = new();
            {
                yuyuko_4.Add(Language.GetText("当生活给了你柠檬...不，这个模组不让我吃。"));
                yuyuko_4.Add(Language.GetText("我能看见的只有那棵黄黄的柠檬树。"));
                yuyuko_4.Add(Language.GetText("你有更大的几率在试图喂我柠檬时看到这句话哦。"), 3);
            }

            WeightedRandom<LocalizedText> yuyuko_5 = new();
            {
                yuyuko_5.Add(Language.GetText("你已经看不到原有评论了哦。"));
                yuyuko_5.Add(Language.GetText("吃起来有点咯牙..."));
            }

            WeightedRandom<LocalizedText> youmu_1 = new();
            {
                youmu_1.Add(Language.GetText("巨大石像来犯！"));
                youmu_1.Add(Language.GetText("就算是石头、观楼剑也劈得开！"));
            }        

            //宠物的独特ID值，详细见 TouhouPetUniqueID.cs
            int cirno = 1;//琪露诺
            int junko = 13;//纯狐
            int reisen = 39;//铃仙
            int youmu = 58;//妖梦

            //参数分别为：Call类型、宠物索引、文本、条件、权重、添加模组
            //内部索引值由添加顺序决定、从0开始，此处可视为0、1、2
            //最后一个参数为添加方模组的实例，用于日志信息
            mod.Call("PetDialog", cirno, text_1, condi_1, 1, mod);
            mod.Call("PetDialog", cirno, text_2, condi_2, 1, mod);
            mod.Call("PetDialog", cirno, text_3, condi_3, 1, mod);

            //先将聊天室所需的第一句对话按照常规对话的形式加入
            mod.Call("PetDialog", junko, text_4, isRegular, 1, mod);

            //聊天室信息列表
            //元组中的三个参数分别为：宠物索引、文本索引、回合数
            //对话的回合值都是从 -1 开始的
            List<(int, LocalizedText, int)> chatRoom1 = new()
            {
                (junko, text_4, -1),
                (reisen, text_5, 0),
                (junko, text_6, 1),
                //这里表示纯狐和铃仙在第二回合时会同时说话
                (reisen, text_7, 2),
                (junko, text_7, 2),
                (reisen, text_8, 3),
            };

            //添加聊天室
            //下面参数分别为：Call类型、聊天室信息列表、添加模组
            mod.Call("PetChatRoom", chatRoom1, mod);

            //下面这些Call添加的文本都不会被计入宠物的对话字典中，也不会被赋予对话索引值，全部使用-1

            //为妖梦添加两句关于石巨人评论的话
            //参数分别为：Call类型、Boss种类、宠物索引、文本、添加模组
            //理论上讲可以对模组内已有的同种类评价进行覆盖
            //但这是让你用来适配自己模组的Boss的，请不要随便覆盖已有内容
            //若多个模组对同一种类的Boss添加了评价，则这些评价会共存
            mod.Call("PetsReactionToBoss", NPCID.Golem, youmu, youmu_1, mod);

            //为幽幽子添加两句接受汉堡评论的话
            //参数分别为：Call类型、食物种类、文本、是否接受该食物、添加模组
            //这是让你用来适配自己模组的食物的，请不要随便覆盖已有内容
            mod.Call("YuyukosReactionToFood", ItemID.Burger, yuyuko_1, true, mod);

            //再次为幽幽子添加一句接受汉堡评论的话，如果多次为同一个食物添加文本则会合并
            mod.Call("YuyukosReactionToFood", ItemID.Burger, yuyuko_2, true, mod);

            //为幽幽子添加一句接受并覆盖原有柠檬评论的话
            //参数分别为：Call类型、食物种类、文本、是否接受该食物、添加模组、是否覆盖原版评论
            //模组添加的评价不受覆盖影响
            mod.Call("YuyukosReactionToFood", ItemID.Lemon, yuyuko_3, true, mod, true);

            //为幽幽子添加三句拒绝柠檬评论的话
            //若同一食物同时存在拒绝与接受的条件，则拒绝的优先级更高
            mod.Call("YuyukosReactionToFood", ItemID.Lemon, yuyuko_4, false, mod);

            //为幽幽子添加两句接受并覆盖原有金怡口评论的话
            mod.Call("YuyukosReactionToFood", ItemID.GoldenDelight, yuyuko_5, true, mod, true);
        }
    }
}
