using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Localization;

namespace TouhouPets
{
    partial class TouhouPets
    {
        private const string Arg_1 = "PetsReactionToBoss";
        private const string Arg_2 = "PetDialog";
        private const string Arg_3 = "PetChatRoom";
        private const string Arg_4 = "YuyukosReactionToFood";

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
        }
        private static void NullifyCrossModList()
        {
            CrossModDialog = null;
            CrossModChatRoomList = null;
            CrossModBossComment = null;
            CrossModFoodComment_Accept = null;
            CrossModFoodComment_Reject = null;
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
                }
            }
            return null;
        }
        private object AddBossReaction(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_MarisasReaction)
            {
                Logger.Info(ConsoleMessage(Arg_1, Warning_PreventedByConfig));
                return false;
            }

            if (args[1] is not Mod
                || args[2] is not int and not short
                || args[3] is not int
                || args[4] is not LocalizedText
                || args.Length > 5 && args[5] is not Func<bool> and not null
                || args.Length > 6 && args[6] is not int and not null)
            {
                Logger.Warn(ConsoleMessage(Arg_1, Warning_WrongDataType));
                return false;
            }
            object arg_Mod = args[1];
            object arg_Type = args[2];
            object arg_Index = args[3];
            object arg_Text = args[4];
            object arg_Condi = args.Length > 5 ? args[5] : null;
            object arg_Weight = args.Length > 6 ? args[6] : null;

            if (arg_Mod == null)
            {
                Logger.Warn(ConsoleMessage(Arg_1, $"{Warning_NullValue}，空值对象：添加模组"));
                return false;
            }
            if (arg_Type == null)
            {
                Logger.Warn(ConsoleMessage(Arg_1, $"{Warning_NullValue}，空值对象：对象种类"));
                return false;
            }
            //防止索引值超限
            if ((int)arg_Index >= (int)TouhouPetID.Count)
            {
                Logger.Warn(ConsoleMessage(Arg_1, Warning_IndexOutOfRange));
                return false;
            }
            if (arg_Index == null)
            {
                Logger.Warn(ConsoleMessage(Arg_1, $"{Warning_NullValue}，空值对象：宠物索引"));
                return false;
            }
            if (arg_Text == null)
            {
                Logger.Warn(ConsoleMessage(Arg_1, $"{Warning_NullValue}，空值对象：评价文本"));
                return false;
            }

            int type = arg_Type is short ? (short)arg_Type : (int)arg_Type;
            int id = (int)arg_Index;
            LocalizedText text = (LocalizedText)arg_Text;
            Func<bool> condition = (arg_Condi != null) ? (Func<bool>)arg_Condi : null;
            int weight = (arg_Weight != null) ? (int)arg_Weight : 1;
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

            Mod mod = (Mod)arg_Mod;
            string modName = mod.DisplayNameClean;

            NPC n = new();
            n.SetDefaults(type);

            StringBuilder logInfo = new($"添加成功！" +
                    $"\n添加者：{modName}；宠物索引：{(TouhouPetID)id}；对象种类：{n.FullName}" +
                    $"\n权重：{weight}；评价文本：{text}");

            Logger.Info(ConsoleMessage("宠物Boss评价添加结果", logInfo.ToString()));
            return true;
        }
        private object AddYuyukoReaction(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_YuyukosReaction)
            {
                Logger.Info(ConsoleMessage(Arg_4, Warning_PreventedByConfig));
                return false;
            }

            if (args[1] is not Mod
                || args[2] is not int and not short
                || args[3] is not LocalizedText
                || args[4] is not bool
                || (args.Length > 5 && args[5] is not Func<bool> and not null)
                || (args.Length > 6 && args[6] is not int and not null))
            {
                Logger.Warn(ConsoleMessage(Arg_4, Warning_WrongDataType));
                return false;
            }
            object arg_Mod = args[1];
            object arg_Type = args[2];
            object arg_Text = args[3];
            object arg_Accept = args[4];
            object arg_Condi = args.Length > 5 ? args[5] : null;
            object arg_Weight = args.Length > 6 ? args[6] : null;

            if (arg_Mod == null)
            {
                Logger.Warn(ConsoleMessage(Arg_4, $"{Warning_NullValue}，空值对象：添加对象"));
                return false;
            }
            if (arg_Type == null)
            {
                Logger.Warn(ConsoleMessage(Arg_4, $"{Warning_NullValue}，空值对象：对象种类"));
                return false;
            }
            if (arg_Text == null)
            {
                Logger.Warn(ConsoleMessage(Arg_4, $"{Warning_NullValue}，空值对象：评价文本"));
                return false;
            }
            if (arg_Accept == null)
            {
                Logger.Warn(ConsoleMessage(Arg_4, $"{Warning_NullValue}，空值对象：是否接受"));
                return false;
            }

            int type = arg_Type is short ? (short)arg_Type : (int)arg_Type;
            LocalizedText text = (LocalizedText)arg_Text;
            bool acceptable = (bool)arg_Accept;
            Func<bool> condition = (arg_Condi != null) ? (Func<bool>)arg_Condi : null;
            int weight = (arg_Weight != null) ? (int)arg_Weight : 1;
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

            Mod mod = (Mod)arg_Mod;
            string modName = mod.DisplayNameClean;

            StringBuilder logInfo = new($"添加成功！" +
                    $"\n添加者：{modName}；对象种类：{new Item(type).Name}；是否接受：{acceptable}" +
                    $"\n权重：{weight}；评价文本：{text}");

            Logger.Info(ConsoleMessage("幽幽子食物评价添加结果", logInfo.ToString()));

            return true;
        }
        private object AddCrossModDialog(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_PetDialog)
            {
                Logger.Info(ConsoleMessage(Arg_2, Warning_PreventedByConfig));
                return false;
            }

            if (args[1] is not Mod
                || args[2] is not int
                || args[3] is not LocalizedText
                || (args.Length > 4 && args[4] is not Func<bool> and not null)
                || (args.Length > 5 && args[5] is not int and not null))
            {
                Logger.Warn(ConsoleMessage(Arg_2, Warning_WrongDataType));
                return false;
            }
            object arg_Mod = args[1];
            object arg_Index = args[2];
            object arg_Text = args[3];
            object arg_Condi = args.Length > 4 ? args[4] : null;
            object arg_Weight = args.Length > 5 ? args[5] : null;

            if (arg_Mod == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：添加对象"));
                return false;
            }
            if ((int)arg_Index >= (int)TouhouPetID.Count)
            {
                Logger.Warn(ConsoleMessage(Arg_2, Warning_IndexOutOfRange));
                return false;
            }
            if (arg_Index == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：宠物索引"));
                return false;
            }
            if (arg_Text == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：对话文本"));
                return false;
            }

            int id = (int)arg_Index;
            LocalizedText text = (LocalizedText)arg_Text;
            Func<bool> condition = (arg_Condi != null) ? (Func<bool>)arg_Condi : null;
            int weight = (arg_Weight != null) ? (int)arg_Weight : 1;
            if (weight < 1)
                weight = 1;

            SingleDialogInfo info = new(text, weight, condition);
            CrossModDialog[id].Add(info);

            Mod mod = (Mod)arg_Mod;
            string modName = mod.DisplayNameClean;

            StringBuilder logInfo = new($"添加成功！" +
                    $"\n添加者：{modName}；索引：{(TouhouPetID)id}" +
                    $"\n权重：{weight}；文本：{text}");

            Logger.Info(ConsoleMessage("宠物对话添加结果", logInfo.ToString()));

            return true;
        }
        private object AddCrossModChatRoom(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_PetChatRoom
                || !GetInstance<MiscConfig>().AllowModCall_PetDialog)
            {
                Logger.Info(ConsoleMessage(Arg_3, Warning_PreventedByConfig));
                return false;
            }

            if (args[1] is not Mod
                || args[2] is not List<(int, LocalizedText, int)>)
            {
                Logger.Warn(ConsoleMessage(Arg_3, Warning_WrongDataType));
                return false;
            }
            object arg_Mod = args[1];
            object arg_Info = args[2];

            if (arg_Mod == null)
            {
                Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：添加对象"));
                return false;
            }
            if (arg_Info == null)
            {
                Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：聊天室成员信息列表"));
                return false;
            }

            List<(int petID, LocalizedText chatText, int chatTurn)> infoList = (List<(int, LocalizedText, int)>)arg_Info;

            List<ChatRoomInfo> resultList = [];
            for (int j = 0; j < infoList.Count; j++)
            {
                ChatRoomInfo info = new(
                    (TouhouPetID)infoList[j].petID,
                    infoList[j].chatText,
                    infoList[j].chatTurn
                    );

                resultList.Add(info);
            }

            int id = infoList[0].petID;
            CrossModChatRoomList[id].Add(resultList);
            //注册聊天室活动信息
            IsChatRoomActive[id].Add(infoList[0].chatText, false);

            Mod mod = (Mod)arg_Mod;
            string modName = mod.DisplayNameClean;
            StringBuilder logInfo = new($"添加成功！" +
                   $"\n添加者：{modName}；" +
                   $"{(TouhouPetID)id}的第{CrossModChatRoomList[id].Count}个聊天室；");

            foreach (var j in resultList)
            {
                logInfo.Append($"\n宠物索引：{j.UniqueID}；回合数：{j.ChatTurn}；文本：{j.ChatText}");
            }

            Logger.Info(ConsoleMessage("宠物聊天室添加结果", logInfo.ToString()));

            return true;
        }
    }
}
