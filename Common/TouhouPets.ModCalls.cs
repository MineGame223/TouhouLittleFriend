using System;

namespace TouhouPets
{
    partial class TouhouPets
    {
        private const string Arg_1 = "MarisasReactionToBoss";
        private const string Arg_2 = "ReimusReactionToOtherPet";

        private const string Warning_NullException = "东方小伙伴 ModCall：ModCall填入内容不可为空！";
        private static string Warning_WrongDataType(string msgType)
        {
            return $"东方小伙伴 ModCall [{msgType}]：填入数据类型错误！";
        }
        public override object Call(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length == 0)
            {
                throw new ArgumentException(Warning_NullException);
            }

            if (args[0] is string content)
            {
                switch (content)
                {
                    case Arg_1:
                        if (args[1] is not int || args[2] is not string)
                        {
                            Logger.Warn(Warning_WrongDataType(Arg_1));
                            return false;
                        }
                        MarisaComment.ModNPCTypeList.Add((int)args[1]);
                        MarisaComment.ModChatList.Add((string)args[2]);
                        return true;

                    case Arg_2:
                        if (args[1] is not int || args[2] is not string)
                        {
                            Logger.Warn(Warning_WrongDataType(Arg_2));
                            return false;
                        }
                        ReimuComment.ModPetTypeList.Add((int)args[1]);
                        ReimuComment.ModChatList.Add((string)args[2]);
                        return true;
                }
            }
            return null;
        }
    }
}
