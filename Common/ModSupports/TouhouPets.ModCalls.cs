using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Common.ModSupports.ModPetRegisterSystem;
using static System.Net.Mime.MediaTypeNames;

namespace TouhouPets
{
    partial class TouhouPets
    {
        // 因为现在有强类型函数支持了所以可以直接用nameof获取函数名，这里函数名全部取自之前的常量值
        // 比如Arg_1即"PetsReactionToBoss", 是获取的成员名，因此CrossModSupport.不会被包含在内
        private const string Arg_1 = nameof(CrossModSupport.PetsReactionToBoss);
        private const string Arg_2 = nameof(CrossModSupport.PetDialog);
        private const string Arg_3 = nameof(CrossModSupport.PetChatRoom);
        private const string Arg_4 = nameof(CrossModSupport.YuyukosReactionToFood);
        private const string Arg_5 = nameof(CrossModSupport.YukaSolutionInfo);

        #region 日志信息
        private const string Warning_NullException = "ModCall填入内容不可为空！";
        private const string Warning_WrongDataType = "填入数据类型错误！已阻止本次载入。";
        private const string Warning_IndexOutOfRange = "填入数值超出索引界限！已阻止本次载入。";
        private const string Warning_NullValue = "检测到空值！已阻止本次载入。";
        private const string Warning_PreventedByConfig = "该项已被模组配置禁止载入。";
        private static string ConsoleMessage(string argName, string msgType) => $"东方小伙伴 ModCall [ {argName} ]：{msgType}";
        #endregion

        /// <summary>
        /// 跨模组添加的对话的列表
        /// </summary>
        public static List<SingleDialogInfo>[] CrossModDialog { get => crossModDialog; set => crossModDialog = value; }
        private static List<SingleDialogInfo>[] crossModDialog = new List<SingleDialogInfo>[(int)TouhouPetID.Count];

        /// <summary>
        /// 跨模组添加的聊天室的列表
        /// </summary>
        public static List<List<ChatRoomInfo>>[] CrossModChatRoomList { get => crossModChatRoomList; set => crossModChatRoomList = value; }
        private static List<List<ChatRoomInfo>>[] crossModChatRoomList = new List<List<ChatRoomInfo>>[(int)TouhouPetID.Count];

        /// <summary>
        /// 跨模组添加的Boss评论的列表
        /// </summary>
        public static List<CommentInfo>[] CrossModBossComment { get => crossModBossComment; set => crossModBossComment = value; }
        private static List<CommentInfo>[] crossModBossComment = new List<CommentInfo>[(int)TouhouPetID.Count];

        /// <summary>
        /// 跨模组添加的可接受食物评论的列表
        /// </summary>
        public static List<CommentInfo> CrossModFoodComment_Accept { get => crossModFoodComment_Accept; set => crossModFoodComment_Accept = value; }
        private static List<CommentInfo> crossModFoodComment_Accept = [];

        /// <summary>
        /// 跨模组添加的不可接受食物评论的列表
        /// </summary>
        public static List<CommentInfo> CrossModFoodComment_Reject { get => crossModFoodComment_Reject; set => crossModFoodComment_Reject = value; }
        private static List<CommentInfo> crossModFoodComment_Reject = [];

        /// <summary>
        /// 跨模组添加的环境溶液的字典
        /// </summary>
        public static Dictionary<int, SprayInfo> CrossModSprayInfo { get => crossModSprayInfo; set => crossModSprayInfo = value; }
        private static Dictionary<int, SprayInfo> crossModSprayInfo = [];

        internal static void ResizeCrossModList(int newSize)
        {
            // 因为要兼容新增宠物，这里要进行一次扩容
            // +1 是因为Count本身也占了一个，在没有扩展模组的情况下不需要扩容
            if (newSize < (int)TouhouPetID.Count + 1) return;
            Array.Resize(ref crossModDialog, newSize);
            Array.Resize(ref crossModChatRoomList, newSize);
            Array.Resize(ref crossModBossComment, newSize);
            for (int i = (int)TouhouPetID.Count; i < newSize; i++)
            {
                CrossModDialog[i] = [];
                CrossModChatRoomList[i] = [];
                CrossModBossComment[i] = [];
            }
        }

        private static void InitializeCrossModList()
        {
            //需要对列表进行初始化
            for (int i = 0; i < (int)TouhouPetID.Count; i++)
            {
                CrossModDialog[i] = [];
                CrossModChatRoomList[i] = [];
                CrossModBossComment[i] = [];
            }
            CrossModFoodComment_Accept = [];
            CrossModFoodComment_Reject = [];
            CrossModSprayInfo = [];
        }
        private static void NullifyCrossModList()
        {
            CrossModDialog = null;
            CrossModChatRoomList = null;
            CrossModBossComment = null;
            CrossModFoodComment_Accept = null;
            CrossModFoodComment_Reject = null;
            CrossModSprayInfo = null;
        }
        public override object Call(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length == 0)
            {
                throw new ArgumentException(ConsoleMessage("参数", Warning_NullException));
            }

            if (args[0] is string content)
            {
                switch (content)
                {
                    case Arg_1:
                        return AddBossReaction(args);

                    case Arg_2:
                        return AddCrossModDialog(args);

                    case Arg_3:
                        return AddCrossModChatRoom(args);

                    case Arg_4:
                        return AddYuyukoReaction(args);

                    case Arg_5:
                        return AddCrossModSolution(args);
                }
            }
            return null;
        }
        private object AddBossReaction(params object[] args)
        {
            if (!AllowCall_MarisasReaction)
            {
                Logger.Info(ConsoleMessage(Arg_1, Warning_PreventedByConfig));
                return false;
            }
            int argsLength = args.Length;
            // 如果我们只匹配一种类型，可以检测的同时进行声明，如果不是Mod就进入下面那个return，如果是我们就可以把声明的mod在外面拿出来用
            // 有模式匹配检测类型我们就不需要再检测null了，因为null不是任何一种类型
            if (args[1] is not Mod mod 
                || args[2] is not int or short
                || args[3] is not int id
                || args[4] is not LocalizedText text
                || argsLength > 5 && args[5] is not Func<bool> and not null
                || argsLength > 6 && args[6] is not int and not null)
            {
                Logger.Warn(ConsoleMessage(Arg_1, Warning_WrongDataType));
                return false;
            }

            // 虽然我觉得应该很少会有人塞short，但是如果要兼容应该就是得这样写了
            int type = 0;
            if (args[2] is int intValue)
                type = intValue;
            else if (args[2] is short shortValue)
                type = shortValue;

            // 这里是之前那种写法的等效写法，更简洁省事
            Func<bool> condition = (argsLength > 5 && args[5] is Func<bool> func) ? func : null;
            int weight = (argsLength > 6 && args[6] is int value) ? value : 1;

            // 具体的实现转发到这个函数内部了，这样逻辑更清晰，而且也方便强引用
            CrossModSupport.PetsReactionToBoss(mod, type, id, text, condition, weight);

            return true;
        }
        private object AddCrossModDialog(params object[] args)
        {
            if (!AllowCall_PetDialog)
            {
                Logger.Info(ConsoleMessage(Arg_2, Warning_PreventedByConfig));
                return false;
            }
            int argsLength = args.Length;
            if (args[1] is not Mod mod
                || args[2] is not int id
                || args[3] is not LocalizedText text
                || (argsLength > 4 && args[4] is not Func<bool> and not null)
                || (argsLength > 5 && args[5] is not int and not null))
            {
                Logger.Warn(ConsoleMessage(Arg_2, Warning_WrongDataType));
                return false;
            }

            Func<bool> condition = (argsLength > 4 && args[4] is Func<bool> func) ? func : null;
            int weight = (argsLength > 5 && args[5] is int value) ? value : 1;

            CrossModSupport.PetDialog(mod, id, text, condition, weight);

            return true;
        }
        private object AddCrossModChatRoom(params object[] args)
        {
            if (!AllowCall_PetChatRoom
                || !AllowCall_PetDialog)
            {
                Logger.Info(ConsoleMessage(Arg_3, Warning_PreventedByConfig));
                return false;
            }

            if (args[1] is not Mod mod
                || args[2] is not List<(int, LocalizedText, int)> infoList)
            {
                Logger.Warn(ConsoleMessage(Arg_3, Warning_WrongDataType));
                return false;
            }

            CrossModSupport.PetChatRoom(mod, infoList);

            return true;
        }
        private object AddYuyukoReaction(params object[] args)
        {
            if (!AllowCall_YuyukosReaction)
            {
                Logger.Info(ConsoleMessage(Arg_4, Warning_PreventedByConfig));
                return false;
            }
            int argsLength = args.Length; // 因为是不变的所以获取一次就可以了
            if (args[1] is not Mod mod    // 类型检测同时声明变量
                || args[2] is not int or short
                || args[3] is not LocalizedText text
                || (argsLength > 4 && args[4] is not bool and not null)
                || (argsLength > 5 && args[5] is not Func<bool> and not null)
                || (argsLength > 6 && args[6] is not int and not null))
            {
                Logger.Warn(ConsoleMessage(Arg_4, Warning_WrongDataType));
                return false;
            }

            int type = 0;
            if (args[2] is int intValue)
                type = intValue;
            else if (args[2] is short shortValue)
                type = shortValue;

            // 要么没填，要么填了但是是null，这两种都是取true，再要么就取填入的bool值
            bool acceptable = argsLength <= 4 || args[4] is not bool acc || acc;

            // 如果填了并且类型对就是填入的值，否则取null
            Func<bool> condition = (argsLength > 5 && args[5] is Func<bool> func) ? func : null;

            // 如果填了并且类型对就是填入的值，否则取1
            int weight = (argsLength > 6 && args[6] is int value) ? value : 1;

            CrossModSupport.YuyukosReactionToFood(mod, type, text, acceptable, condition, weight);

            return true;
        }
        private object AddCrossModSolution(params object[] args)
        {
            if (args[1] is not Mod mod
                || args[2] is not int itemType
                || args[3] is not int sprayType
                || args[4] is not int and not Func<int>)
            {
                Logger.Warn(ConsoleMessage(Arg_5, Warning_WrongDataType));
                return false;
            }

            int dustType = -1;
            Func<int> dustTypeDelegate = null;
            if (args[4] is int type)
                dustType = type;
            else if (args[4] is Func<int> func)
                dustTypeDelegate = func;

            CrossModSupport.YukaSolutionInfo(mod, itemType, sprayType, dustType, dustTypeDelegate);
            return true;
        }




        public static class CrossModSupport
        {
            public static void PetsReactionToBoss(Mod mod, int type, int id, LocalizedText text, Func<bool> condition = null, int weight = 1)
            {
                if (weight < 1) weight = 1;

                List<CommentInfo> bossComment = CrossModBossComment[id];
                //在列表中查找对象种类与条件方法和当前信息相等的元素
                var existingItem = bossComment.FirstOrDefault(
                    x => x.ObjectType == type
                );

                //若查找到对象，则对对象进行重置，否则按新元素加入列表中
                if (existingItem.CommentContent != null)
                {
                    //当前对象的索引值
                    int index = bossComment.IndexOf(existingItem);
                    var mergedList = new List<SingleDialogInfo>();

                    //由于遍历不可对其中的合集元素进行修改
                    //因此需要将原有的文本加入新的随机选择器中，并在后续修改
                    foreach (var c in existingItem.CommentContent)
                        mergedList.Add(c);

                    mergedList.Add(new SingleDialogInfo(text, weight, condition));
                    bossComment[index] = new CommentInfo(type, mergedList);
                }
                else
                {
                    List<SingleDialogInfo> resultContent = [new SingleDialogInfo(text, weight, condition)];
                    bossComment.Add(new CommentInfo(type, resultContent));
                }

                string modName = mod.DisplayNameClean;

                NPC n = new();
                n.SetDefaults(type);

                StringBuilder logInfo = new($"添加成功！" +
                        $"\n添加者：{modName}；宠物索引：{(TouhouPetID)id}；对象种类：{n.FullName}" +
                        $"\n权重：{weight}；评价文本：{text}");

                Instance.Logger.Info(ConsoleMessage("宠物Boss评价添加结果", logInfo.ToString()));
            }
            public static void PetDialog(Mod mod, int id, LocalizedText text, Func<bool> condition = null, int weight = 1)
            {
                if (weight < 1) weight = 1;

                SingleDialogInfo info = new(text, weight, condition);
                CrossModDialog[id].Add(info);

                string modName = mod.DisplayNameClean;

                StringBuilder logInfo = new($"添加成功！" +
                        $"\n添加者：{modName}；索引：{(TouhouPetID)id}" +
                        $"\n权重：{weight}；文本：{text}");

                Instance.Logger.Info(ConsoleMessage("宠物对话添加结果", logInfo.ToString()));
            }
            public static void PetChatRoom(Mod mod, List<(int petID, LocalizedText chatText, int chatTurn)> infoList)
            {
                List<ChatRoomInfo> resultList
                    = [.. from info in infoList select new ChatRoomInfo((TouhouPetID)info.petID, info.chatText, info.chatTurn)];
                // 这里使用了查询语句和集合表达式
                // from info是在声明范围变量info，相当于来自infoList的每一个元素
                // in infoList就是由infoList提供每一个info
                // select后面的部分就是由info计算出来的值，一个info对应一个
                // 还可以使用where语句来对info作出限制，只查询符合条件的info
                // 最后用集合表达式把查询结果转List

                int id = infoList[0].petID;
                CrossModChatRoomList[id].Add(resultList);
                //注册聊天室活动信息
                IsChatRoomActive[id].Add(infoList[0].chatText, false);

                string modName = mod.DisplayNameClean;
                StringBuilder logInfo = new($"添加成功！" +
                       $"\n添加者：{modName}；" +
                       $"{(TouhouPetID)id}的第{CrossModChatRoomList[id].Count}个聊天室；");

                foreach (var j in resultList)
                {
                    logInfo.Append($"\n宠物索引：{j.UniqueID}；回合数：{j.ChatTurn}；文本：{j.ChatText}");
                }

                Instance.Logger.Info(ConsoleMessage("宠物聊天室添加结果", logInfo.ToString()));

            }
            public static void YuyukosReactionToFood(Mod mod, int type, LocalizedText text, bool acceptable = true, Func<bool> condition = null, int weight = 1)
            {
                if (weight < 1) weight = 1;

                List<(CommentInfo info, bool accept)> foodComment = [];
                //在列表中查找对象种类与条件方法和当前信息相等的元素
                var existingItem = foodComment.FirstOrDefault(
                    x => x.info.ObjectType == type
                    && x.accept == acceptable
                );

                //若查找到对象，则对对象进行重置，否则按新元素加入列表中
                if (existingItem.info.CommentContent != null)
                {
                    //当前对象的索引值
                    int index = foodComment.IndexOf(existingItem);
                    var mergedList = new List<SingleDialogInfo>();

                    //由于遍历不可对其中的合集元素进行修改
                    //因此需要将原有的文本加入新的随机选择器中，并在后续修改
                    foreach (var c in existingItem.info.CommentContent)
                        mergedList.Add(c);

                    mergedList.Add(new SingleDialogInfo(text, weight, condition));
                    foodComment[index] = (new CommentInfo(type, mergedList), acceptable);
                }
                else
                {
                    List<SingleDialogInfo> resultContent = [new SingleDialogInfo(text, weight, condition)];
                    foodComment.Add((new CommentInfo(type, resultContent), acceptable));
                }

                //遍历食物评价信息列表并根据接受与否进行分类
                foreach (var (info, accept) in foodComment)
                {
                    if (accept)
                        CrossModFoodComment_Accept.Add(info);
                    else
                        CrossModFoodComment_Reject.Add(info);
                }

                string modName = mod.DisplayNameClean;

                StringBuilder logInfo = new($"添加成功！" +
                        $"\n添加者：{modName}；对象种类：{new Item(type).Name}；是否接受：{acceptable}" +
                        $"\n权重：{weight}；评价文本：{text}");

                Instance.Logger.Info(ConsoleMessage("幽幽子食物评价添加结果", logInfo.ToString()));
            }
            public static void YukaSolutionInfo(Mod mod, int itemType, int sprayType, int dustType, Func<int> dustTypeDelegate = null)
            {
                if (itemType > ItemID.None && !CrossModSprayInfo.ContainsKey(itemType))
                    CrossModSprayInfo.Add(itemType, new SprayInfo(sprayType, dustType, dustTypeDelegate));

                string modName = mod.DisplayNameClean;
                StringBuilder logInfo = new($"添加成功！添加者：{modName}");

                Instance.Logger.Info(ConsoleMessage("环境溶液添加结果", logInfo.ToString()));
            }
        }
    }
}
