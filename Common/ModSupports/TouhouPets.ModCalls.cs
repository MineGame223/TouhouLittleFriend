using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace TouhouPets
{
    partial class TouhouPets
    {
        private const string Arg_1 = "MarisasReactionToBoss";
        private const string Arg_2 = "PetDialog";
        private const string Arg_3 = "PetChatRoom";
        private const string Arg_4 = "YuyukosReactionToFood";

        #region 日志信息
        private const string Warning_NullException = "ModCall填入内容不可为空！";
        private const string Warning_WrongDataType = "填入数据类型错误！已阻止本次载入。";
        private const string Warning_IndexOutOfRange = "填入数值超出索引界限！已阻止本次载入。";
        private const string Warning_NullValue = "检测到空值！已阻止本次载入。";
        private const string Warning_PreventedByConfig = "该项已被模组配置禁止载入。";
        private static string ConsoleMessage(string argName, string msgType) => $"东方小伙伴 ModCall [{argName}]：{msgType}";
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
        private static List<ChatRoomInfo>[] crossModChatRoom = new List<ChatRoomInfo>[(int)TouhouPetID.Count];

        /// <summary>
        /// 跨模组添加的Boss评论的列表
        /// </summary>
        public static List<CommentInfo> CrossModBossComment { get => crossModBossComment; set => crossModBossComment = value; }
        private static List<CommentInfo> crossModBossComment = [];

        /// <summary>
        /// 跨模组添加的食物评论的列表
        /// </summary>
        public static List<CommentInfo> CrossModFoodComment { get => crossModFoodComment; set => crossModFoodComment = value; }
        private static List<CommentInfo> crossModFoodComment = [];
        private static void InitializCrossModList()
        {
            //需要对列表进行初始化
            for (int i = 0; i < (int)TouhouPetID.Count; i++)
            {
                CrossModDialog[i] = [];

                CrossModChatRoomList[i] = [];
                crossModChatRoom[i] = [];
            }
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
                        return AddMarisasReaction(args);

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
        private object AddMarisasReaction(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_MarisasReaction)
            {
                Logger.Info(ConsoleMessage(Arg_1, Warning_PreventedByConfig));
                return false;
            }
            if (args[1] is not int || args[2] is not LocalizedText)
            {
                Logger.Warn(ConsoleMessage(Arg_1, Warning_WrongDataType));
                return false;
            }
            if (args[1] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：对象种类"));
                return false;
            }
            if (args[2] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：评价文本"));
                return false;
            }

            int npcType = (int)args[1];
            LocalizedText text = (LocalizedText)args[2];

            CommentInfo info = new(npcType, text);

            CrossModBossComment.Add(info);
            return true;
        }
        private object AddYuyukoReaction(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_YuyukosReaction)
            {
                Logger.Info(ConsoleMessage(Arg_1, Warning_PreventedByConfig));
                return false;
            }
            if (args[1] is not int || args[2] is not LocalizedText)
            {
                Logger.Warn(ConsoleMessage(Arg_1, Warning_WrongDataType));
                return false;
            }
            if (args[1] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：对象种类"));
                return false;
            }
            if (args[2] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：评价文本"));
                return false;
            }

            int npcType = (int)args[1];
            LocalizedText text = (LocalizedText)args[2];

            CommentInfo info = new(npcType, text);

            CrossModFoodComment.Add(info);
            return true;
        }
        private object AddCrossModDialog(params object[] args)
        {
            if (!GetInstance<MiscConfig>().AllowModCall_PetDialog)
            {
                Logger.Info(ConsoleMessage(Arg_2, Warning_PreventedByConfig));
                return false;
            }
            if (args[1] is not int || args[2] is not LocalizedText
                || args[3] is not Func<bool> || args[4] is not int)
            {
                Logger.Warn(ConsoleMessage(Arg_2, Warning_WrongDataType));
                return false;
            }
            if ((int)args[1] >= (int)TouhouPetID.Count)
            {
                Logger.Warn(ConsoleMessage(Arg_2, Warning_IndexOutOfRange));
                return false;
            }
            if (args[1] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：宠物索引"));
                return false;
            }
            if (args[2] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：对话文本"));
                return false;
            }
            if (args[3] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：对话条件"));
                return false;
            }
            if (args[4] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_2, $"{Warning_NullValue}，空值对象：对话权重"));
                return false;
            }

            int id = (int)args[1];
            LocalizedText text = (LocalizedText)args[2];
            Func<bool> condition = (Func<bool>)args[3];
            int weight = (int)args[4];

            if (weight < 1)
                weight = 1;

            SingleDialogInfo info = new(text, condition, weight);
            CrossModDialog[id].Add(info);

            for (int i = 0; i < CrossModDialog[id].Count; i++)
            {
                if (i < CrossModDialog[id].Count - 1)
                    continue;

                Logger.Info(ConsoleMessage("宠物对话添加结果"
                    , $"添加成功！" +
                    $"索引：{(TouhouPetID)id}；" +
                    $"内容：{CrossModDialog[id][i].DialogText}；" +
                    $"权重：{CrossModDialog[id][i].Weight}"));
            }

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
            if (args[1] is not int || args[2] is not List<(int, int, int)>)
            {
                Logger.Warn(ConsoleMessage(Arg_3, Warning_WrongDataType));
                return false;
            }
            if (args[1] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：宠物索引"));
                return false;
            }
            if (args[2] == null)
            {
                Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：聊天室成员信息列表"));
                return false;
            }

            int id = (int)args[1];
            List<(int, int, int)> infoList = (List<(int, int, int)>)args[2];


            for (int j = 0; j < infoList.Count; j++)
            {
                ChatRoomInfo info = new(
                    (TouhouPetID)infoList[j].Item1,
                    infoList[j].Item2,
                    infoList[j].Item3
                    );

                crossModChatRoom[id].Add(info);
            }

            CrossModChatRoomList[id].Add(crossModChatRoom[id]);

            for (int i = 0; i < CrossModChatRoomList[id].Count; i++)
            {
                if (i < CrossModChatRoomList[id].Count - 1)
                    continue;

                foreach (var j in crossModChatRoom[id])
                {
                    Logger.Info(ConsoleMessage("宠物聊天室添加结果"
                    , $"添加成功！" +
                    $"第{i + 1}个聊天室；" +
                    $"索引：{(TouhouPetID)id}；" +
                    $"宠物ID：{j.UniqueID}；" +
                    $"索引值：{j.ChatIndex}；" +
                    $"回合数：{j.ChatTurn}"));
                }
            }

            return true;
        }
    }
}
