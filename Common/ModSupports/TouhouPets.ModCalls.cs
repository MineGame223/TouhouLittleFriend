using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace TouhouPets
{
    public partial class TouhouPets
    {
        private const string Arg_1 = "MarisasReactionToBoss";
        private const string Arg_2 = "PetDialog";

        #region 日志信息
        private const string Warning_NullException = "ModCall填入内容不可为空！";
        private const string Warning_WrongDataType = "填入数据类型错误！已阻止本次载入。";
        private const string Warning_IndexOutOfRange = "填入数值超出索引界限！已阻止本次载入。";
        private const string Warning_NullValue = "检测到空值！已阻止本次载入。";
        private static string ConsoleMessage(string argName, string msgType) => $"东方小伙伴 ModCall [{argName}]：{msgType}";
        #endregion

        private static List<CrossModDialogInfo>[] crossModDialog = new List<CrossModDialogInfo>[(int)TouhouPetID.Count];
        public static List<CrossModDialogInfo>[] CrossModDialog { get => crossModDialog; set => crossModDialog = value; }

        
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
                }
            }
            return null;
        }
        private object AddMarisasReaction(params object[] args)
        {
            if (args[1] is not int || args[2] is not string)
            {
                Logger.Warn(ConsoleMessage(Arg_1, Warning_WrongDataType));
                return false;
            }
            MarisaComment.ModNPCTypeList.Add((int)args[1]);
            MarisaComment.ModChatList.Add((string)args[2]);
            return true;
        }
        private object AddCrossModDialog(params object[] args)
        {
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

            CrossModDialogInfo info = new CrossModDialogInfo() with
            {
                dialogText = text,
                condition = condition,
                weight = weight,
            };

            CrossModDialog[id].Add(info);

            for (int i = 0; i < CrossModDialog[id].Count; i++)
            {
                if (i < CrossModDialog[id].Count - 1)
                    continue;

                Logger.Info(ConsoleMessage("添加结果"
                    , $"对话添加成功！" +
                    $"索引：{id}；" +
                    $"内容：{CrossModDialog[id][i].dialogText}；" +
                    $"权重：{CrossModDialog[id][i].weight}"));
            }

            return true;
        }
        private object AddCrossModChatRoom(params object[] args)
        {
            return true;
        }
    }
}
