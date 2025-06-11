using System;
using System.Collections.Generic;

namespace TouhouPets
{
    public partial class TouhouPets
    {
        private const string Arg_1 = "MarisasReactionToBoss";
        private const string Arg_2 = "ReimusReactionToOtherPet";
        private const string Arg_3 = "PetDialog";

        private const string Warning_NullException = "ModCall填入内容不可为空！";
        private const string Warning_WrongDataType = "填入数据类型错误！已阻止本次载入。";
        private const string Warning_IndexOutOfRange = "填入数值超出索引界限！已阻止本次载入。";
        private const string Warning_NullValue = "检测到空值！已阻止本次载入。";
        private static string ConsoleMessage(string argName, string msgType)
        {
            return $"东方小伙伴 ModCall [{argName}]：{msgType}";
        }

        private static List<string>[] crossModChatText = new List<string>[(int)TouhouPetID.Count];
        private static List<Func<bool>>[] crossModChatCondition = new List<Func<bool>>[(int)TouhouPetID.Count];
        private static List<int>[] crossModChatWeight = new List<int>[(int)TouhouPetID.Count];

        /// <summary>
        /// 来自其他模组的对话文本的列表数组
        /// </summary>
        public static List<string>[] CrossModChatText { get => crossModChatText; set => crossModChatText = value; }
        /// <summary>
        /// 来自其他模组的对话条件的列表数组
        /// </summary>
        public static List<Func<bool>>[] CrossModChatCondition { get => crossModChatCondition; set => crossModChatCondition = value; }
        /// <summary>
        /// 来自其他模组的对话权重的列表数组
        /// </summary>
        public static List<int>[] CrossModChatWeight { get => crossModChatWeight; set => crossModChatWeight = value; }
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
                        if (args[1] is not int || args[2] is not string)
                        {
                            Logger.Warn(ConsoleMessage(Arg_1, Warning_WrongDataType));
                            return false;
                        }
                        MarisaComment.ModNPCTypeList.Add((int)args[1]);
                        MarisaComment.ModChatList.Add((string)args[2]);
                        return true;

                    case Arg_2:
                        if (args[1] is not int || args[2] is not string)
                        {
                            Logger.Warn(ConsoleMessage(Arg_2, Warning_WrongDataType));
                            return false;
                        }
                        ReimuComment.ModPetTypeList.Add((int)args[1]);
                        ReimuComment.ModChatList.Add((string)args[2]);
                        return true;

                    case Arg_3:
                        if (args[1] is not int || args[2] is not string
                            || args[3] is not Func<bool> || args[4] is not int)
                        {
                            Logger.Warn(ConsoleMessage(Arg_3, Warning_WrongDataType));
                            return false;
                        }
                        if ((int)args[1] >= (int)TouhouPetID.Count)
                        {
                            Logger.Warn(ConsoleMessage(Arg_3, Warning_IndexOutOfRange));
                            return false;
                        }
                        if (args[1] == null)
                        {
                            Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：宠物索引"));
                            return false;
                        }
                        if (args[2] == null)
                        {
                            Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：对话文本"));
                            return false;
                        }
                        if (args[3] == null)
                        {
                            Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：对话条件"));
                            return false;
                        }
                        if (args[4] == null)
                        {
                            Logger.Warn(ConsoleMessage(Arg_3, $"{Warning_NullValue}，空值对象：对话权重"));
                            return false;
                        }
                        if ((int)args[4] <= 0)
                        {
                            args[4] = (int)0;
                        }

                        crossModChatText[(int)args[1]].Add((string)args[2]);
                        crossModChatCondition[(int)args[1]].Add((Func<bool>)args[3]);
                        crossModChatWeight[(int)args[1]].Add((int)args[4]);

                        for (int i = 0; i < crossModChatText[(int)args[1]].Count; i++)
                        {
                            if (i != crossModChatText[(int)args[1]].Count - 1)
                                continue;

                            Logger.Info(ConsoleMessage("添加结果"
                                , $"对话添加成功！内容：{crossModChatText[(int)args[1]][i]}；权重：{crossModChatWeight[(int)args[1]][i]}"));
                        }

                        return true;
                }
            }
            return null;
        }
    }
}
