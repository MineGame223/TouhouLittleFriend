using Terraria;
using System;
using static TouhouPets.TouhouPets;
using static TouhouPets.ChatRoomSystem;
using System.Collections.Generic;

namespace TouhouPets
{
    /// <summary>
    /// 聊天室相关静态拓展方法
    /// </summary>
    public static class ChatRoomHelper
    {
        /// <summary>
        /// 宠物是否被允许说话
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns>当 shouldNotTalking 为 false 且 chatCD 小于等于 0 时，返回 true</returns>
        public static bool ShouldPetTalking(this Projectile projectile)
        {
            if (!projectile.IsATouhouPet())
                return false;

            return !projectile.ToPetClass().shouldNotTalking && projectile.ToPetClass().chatCD <= 0;
        }

        /// <summary>
        /// 将 <see cref="Projectile"/> 类转换为 <see cref="BasicTouhouPet"/> 类
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static BasicTouhouPet ToPetClass(this Projectile projectile)
        {
            return projectile.ModProjectile as BasicTouhouPet;
        }

        /// <summary>
        /// 关闭当前聊天室，并将聊天发起者与其中成员的currentChatRoom设为空、chatIndex归零
        /// </summary>
        /// <param name="chatRoom">当前聊天室</param>
        /// <param name="chatCD">聊天冷却时间，归零前宠物之间不会再次发起聊天</param>
        public static void CloseChatRoom(this PetChatRoom chatRoom, int chatCD = 21600)
        {
            if (chatRoom.initiator.IsATouhouPet())
            {
                BasicTouhouPet owner = chatRoom.initiator.ToPetClass();
                owner.currentChatRoom = null;
                owner.chatIndex = 0;
                owner.chatCD = chatCD;

                if (owner.IsChatRoomActive.Count > 0)
                {
                    foreach (int i in owner.IsChatRoomActive.Keys)
                    {
                        owner.IsChatRoomActive[i] = false;
                    }
                }
            }
            foreach (Projectile m in chatRoom.member)
            {
                if (m != null && m.active)
                {
                    if (!m.IsATouhouPet())
                        continue;

                    BasicTouhouPet pet = m.ToPetClass();
                    pet.currentChatRoom = null;
                    pet.chatIndex = 0;
                    pet.chatCD = chatCD;

                    if (pet.IsChatRoomActive.Count > 0)
                    {
                        foreach (int i in pet.IsChatRoomActive.Keys)
                        {
                            pet.IsChatRoomActive[i] = false;
                        }
                    }
                }
            }
            chatRoom.active = false;
        }

        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="initiator">聊天发起者</param>
        /// <returns>以当前宠物为发起者的聊天室的索引值</returns>
        public static int CreateChatRoom(this Projectile initiator)
        {
            if (initiator == null)
                return -1;

            int i = -1;
            for (int l = 0; l < MaxChatRoom; l++)
            {
                if (!ChatRoom[l].active)
                {
                    i = l;
                    break;
                }
            }
            if (i >= 0)
            {
                ChatRoom[i] = new()
                {
                    active = true,
                    initiator = initiator,
                    chatTurn = -1
                };

                initiator.ToPetClass().currentChatRoom = ChatRoom[i];

                return i;
            }
            return MaxChatRoom - 1;
        }

        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="initiator">聊天发起者</param>
        /// <returns>以当前宠物为发起者的聊天室实例</returns>
        public static PetChatRoom CreateChatRoomDirect(this Projectile initiator)
        {
            int i = CreateChatRoom(initiator);
            return ChatRoom[i];
        }

        /// <summary>
        /// 宠物当前是否刚说完一句话
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns>当 chatOpacity 小于 0 且 大于 -0.3 时返回 true</returns>
        public static bool CurrentDialogFinished(this Projectile projectile)
        {
            if (!projectile.IsATouhouPet())
                return false;

            return projectile.ToPetClass().chatOpacity < 0 && projectile.ToPetClass().chatOpacity > -0.3f;
        }

        /// <summary>
        /// 宠物当前是否没有在说话
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns>当 chatOpacity 小于等于 -0.3 时返回 true</returns>
        public static bool CurrentlyNoDialog(this Projectile projectile)
        {
            if (!projectile.IsATouhouPet())
                return false;

            return projectile.ToPetClass().chatOpacity <= -0.3f;
        }

        /// <summary>
        /// 关闭当前宠物的对话（将chatTimeLeft设为0）
        /// </summary>
        /// <param name="projectile"></param>
        public static void CloseCurrentDialog(this Projectile projectile)
        {
            projectile.ToPetClass().chatTimeLeft = 0;
        }

        #region 设置聊天室
        /// <summary>
        /// 编辑聊天室
        /// </summary>
        /// <param name="chatRoom"></param>
        /// <param name="maxTurn">最大回合数</param>
        /// <param name="infoList">成员信息的元组列表</param>
        public static void ModifyChatRoom(this PetChatRoom chatRoom, List<ChatRoomInfo> infoList, bool isCrossMod = false)
        {
            //成员ID列表
            List<TouhouPetID> member = [];

            //使用弹幕实例的成员信息元组列表
            List<(Projectile pet, int chatIndex, int chatTurn, int targetTurn)> info = [];

            //用于记录实际对话总回合数的计数列表
            List<int> listForCount = [];

            //遍历传入的成员信息列表
            foreach (var i in infoList)
            {
                //若列表中尚不包含与信息列表的对话回合值相等的元素，则加入计数列表
                if (!listForCount.Contains(i.ChatTurn))
                {
                    listForCount.Add(i.ChatTurn);
                }

                //若成员ID列表的元素已被包含其中，则跳过添加
                if (member.Contains(i.UniqueID))
                    continue;

                member.Add(i.UniqueID);
            }

            //以防万一，如果计数列表长度为0，则不执行后续
            if (listForCount.Count <= 0)
                return;

            //设置成员并维持聊天室
            if (!MaintainChatRoom(ref chatRoom, member))
            {
                //因为异常而退出的CD更短
                chatRoom.CloseChatRoom(60);
                return;
            }

            //再次遍历传入的成员信息列表
            foreach (var i in infoList)
            {
                //如果成员信息列表中的成员ID不在先前的成员ID列表中，则不添加到实例列表中
                if (!member.Contains(i.UniqueID))
                {
                    continue;
                }
                info.Add((chatRoom.member[member.IndexOf(i.UniqueID)], i.ChatIndex, i.ChatTurn, i.ChatTurn + 1));
            }

            //根据长度遍历元组列表内容
            for (int i = 0; i < info.Count; i++)
            {
                Projectile pet = info[i].pet;

                //如果加入的弹幕不存在，则不设置对话
                if (pet == null || !pet.active)
                    continue;

                //以防万一，如果加入的弹幕并非东方宠物，则不设置对话
                if (!pet.IsATouhouPet())
                    continue;

                //Item2为对话文本索引值，Item3为回合数
                //若回合数不等于当前回合值则跳过
                if (info[i].chatTurn != chatRoom.chatTurn)
                    continue;

                BasicTouhouPet p = pet.ToPetClass();

                //区分模组对话和原有对话的索引值偏移
                int extraIndex = isCrossMod ? p.crossModDialogStartIndex : 0;

                //仅当索引值大于0时设置要说的话
                if (info[i].chatIndex > 0)
                {
                    int chatIndex = info[i].chatIndex + extraIndex;
                    //起始回合时发起者不设置对话
                    if (info[i].chatTurn != -1 || pet != chatRoom.initiator)
                    {
                        pet.SetChat(p.ChatSettingConfig, chatIndex, 20);
                    }
                }
                //若索引值被设为小于等于0，则关闭当前对话
                else
                {
                    pet.CloseCurrentDialog();
                }

                //若当前对话已说完，则将回合值+1并跳出循环，防止多加
                if (pet.CurrentDialogFinished())
                {
                    chatRoom.chatTurn = info[i].targetTurn;
                    break;
                }
            }
            //超过最大回合数时关闭聊天室
            //由于存在回合-1与回合0，所以总回合数需要 -2 才等于最大回合值
            if (chatRoom.chatTurn > listForCount.Count - 2)
            {
                chatRoom.CloseChatRoom();
            }
        }

        /// <summary>
        /// 设置聊天成员并维持聊天室，若成员列表无内容或任意成员不存在则会关闭聊天室
        /// </summary>
        /// <param name="chatRoom">需要被设置和维持的聊天室实例</param>
        /// <param name="member">所需成员的独特标识符的列表</param>
        /// <returns>当聊天室不能被维持时，返回 false 并关闭聊天室</returns>
        private static bool MaintainChatRoom(ref PetChatRoom chatRoom, List<TouhouPetID> member)
        {
            //若聊天室不存在，返回false
            if (chatRoom == null || !chatRoom.active)
                return false;

            //若成员列表不存在或长度为0，返回false
            if (member == null || member.Count <= 0)
                return false;

            //若成员列表不存在或长度为0，返回false
            if (chatRoom.initiator == null || !chatRoom.initiator.active)
                return false;

            //以防万一，若成员发起者不是东方宠物，返回false
            if (!chatRoom.initiator.IsATouhouPet())
                return false;

            BasicTouhouPet initiator = chatRoom.initiator.ToPetClass();

            //若成员发起者不是成员列表第一位，返回false
            if (initiator.UniqueID != member[0])
                return false;

            Projectile[] memberArray = chatRoom.member;

            for (int i = 0; i < member.Count; i++)
            {
                //若列表长度大于成员数组长度，则不设置成员聊天室
                if (i > memberArray.Length)
                    continue;

                //这之前不许使用memberArray[i]，因为是空值
                //若成员为空，则为其添加通过独特标识ID查找到的实例并设置聊天室（以便只执行一次下方的遍历）
                if (memberArray[i] == null)
                {
                    if (chatRoom.initiator.ToPetClass().FindPetByUniqueID(out Projectile p, member[i]))
                    {
                        memberArray[i] = p;
                        //以防万一，若发现该成员不是东方宠物，则不设置成员聊天室
                        if (i > 0 && memberArray[i].IsATouhouPet())
                        {
                            memberArray[i].ToPetClass().currentChatRoom = chatRoom;
                        }
                    }
                }

                //若成员依旧为空或不存在，返回false
                if (memberArray[i] == null || !memberArray[i].active)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 设置对话
        private static void SetChat_Inner(Projectile projectile, ChatSettingConfig config, int lag = 0, int index = -1, string text = null)
        {
            //若被设置对象并非东方宠物，则不再执行后续
            if (!projectile.IsATouhouPet())
                return;

            BasicTouhouPet pet = projectile.ToPetClass();

            string chat = string.Empty;

            //采用索引值设置，若当前索引值不等于目标索引值则进行设置
            if (index > 0 && pet.chatIndex != index)
            {
                chat = pet.ChatDictionary[index];
                pet.chatIndex = index;
            }
            //采用文本设置
            else if (text != null)
            {
                chat = text;
                //匹配字典中相同的对话并据此给chatIndex赋值
                foreach (int k in pet.ChatDictionary.Keys)
                {
                    if (text.Equals(pet.ChatDictionary[k]))
                    {
                        pet.chatIndex = k;
                    }
                    else
                    {
                        pet.chatIndex = -1;
                    }
                }
            }

            //若都没有设置上则不执行后续
            if (chat == string.Empty)
                return;

            //若宠物非玩家本人召唤、或宠物有话还没说完，则不再执行后续
            if (projectile.owner != Main.myPlayer || pet.chatTimeLeft > 0)
                return;

            //自动处理单个字符剩余时间
            if (chat.Length > 10 && !config.AutoHandleTimeLeft)
            {
                if (config.TimeLeftPerWord > 10)
                {
                    config.TimeLeftPerWord = 10;
                }
            }
            //自动设置打字所需时间
            if (config.TyperModeUseTime == -1)
            {
                config.TyperModeUseTime = Math.Clamp(chat.Length * 5, 0, 150);
            }

            pet.chatBaseY = -24;
            pet.chatScale = 0f;
            pet.chatText = chat;
            pet.chatTimeLeft = Math.Clamp(chat.Length * config.TimeLeftPerWord, 0, 420);
            pet.timeToType = 0;
            pet.totalTimeToType = config.TyperModeUseTime;
            pet.chatLag = lag;

            pet.textColor = config.TextColor;
            pet.boardColor = config.TextBoardColor;

            //Main.NewText($"Index: {index}", Main.DiscoColor);
        }

        /// <summary>
        /// 设置宠物要说的话
        /// <br/>当 ChatTimeLeft 大于0时不输出结果
        /// </summary>
        /// <param name="index">对话索引值</param>
        /// <param name="config">对话属性配置</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChat(this Projectile projectile, ChatSettingConfig config, int index, int lag = 0)
        {
            SetChat_Inner(projectile, config, lag, index);
        }

        /// <summary>
        /// 设置宠物要说的话，自动调用ChatSettingConfig
        /// <br/>当 ChatTimeLeft 大于0时不输出结果
        /// </summary>
        /// <param name="index">对话索引值</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChat(this Projectile projectile, int index, int lag = 0)
        {
            if (!projectile.IsATouhouPet())
                return;

            SetChat_Inner(projectile, projectile.ToPetClass().ChatSettingConfig, lag, index);
        }

        /// <summary>
        /// 设置宠物要说的话，采用直接输入文本的形式
        /// <br/>当 ChatTimeLeft 大于0时不输出结果
        /// </summary>
        /// <param name="text">对话文本，若宠物的聊天字典中存在匹配的文本，则自动为对话索引赋值</param>
        /// <param name="config">对话属性配置</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChat(this Projectile projectile, ChatSettingConfig config, string text, int lag = 0)
        {
            SetChat_Inner(projectile, config, lag, -1, text);
        }

        /// <summary>
        /// 设置宠物要说的话，采用直接输入文本的形式，自动调用ChatSettingConfig
        /// <br/>当 ChatTimeLeft 大于0时不输出结果
        /// </summary>
        /// <param name="text">对话文本，若宠物的聊天字典中存在匹配的文本，则自动为对话索引赋值</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChat(this Projectile projectile, string text, int lag = 0)
        {
            SetChat_Inner(projectile, projectile.ToPetClass().ChatSettingConfig, lag, -1, text);
        }

        /// <summary>
        /// 设置宠物要说的话，无视当前索引值必须不等于目标索引值的限制
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="index">对话索引值</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChatForcely(this Projectile projectile, int index, int lag = 0)
        {
            if (!projectile.IsATouhouPet())
                return;

            BasicTouhouPet pet = projectile.ToPetClass();

            pet.chatIndex = -1;
            SetChat_Inner(projectile, projectile.ToPetClass().ChatSettingConfig, lag, index);
        }

        /// <summary>
        /// 设置宠物要说的话，无视当前索引值必须不等于目标索引值的限制
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="text">对话文本，若宠物的聊天字典中存在匹配的文本，则自动为对话索引赋值</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChatForcely(this Projectile projectile, string text, int lag = 0)
        {
            if (!projectile.IsATouhouPet())
                return;

            BasicTouhouPet pet = projectile.ToPetClass();

            pet.chatIndex = -1;
            SetChat_Inner(projectile, projectile.ToPetClass().ChatSettingConfig, lag, -1, text);
        }
        #endregion
    }
}
