using Terraria;
using System;
using static TouhouPets.TouhouPets;
using static TouhouPets.ChatRoomSystem;
using System.Collections.Generic;
using Terraria.Localization;

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

            return !projectile.AsTouhouPet().shouldNotTalking && projectile.AsTouhouPet().chatCD <= 0;
        }

        /// <summary>
        /// 关闭当前聊天室，并将聊天发起者与其中成员的currentChatRoom设为空
        /// </summary>
        /// <param name="chatRoom">当前聊天室</param>
        /// <param name="chatCD">聊天冷却时间，归零前宠物之间不会再次发起聊天</param>
        public static void CloseChatRoom(this PetChatRoom chatRoom, int chatCD = 21600)
        {
            if (chatRoom.initiator.IsATouhouPet())
            {
                BasicTouhouPet owner = chatRoom.initiator.AsTouhouPet();
                owner.currentChatRoom = null;
                owner.chatCD = chatCD;

                if (owner.IsChatRoomActive.Count > 0)
                {
                    foreach (var i in owner.IsChatRoomActive.Keys)
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

                    BasicTouhouPet pet = m.AsTouhouPet();
                    pet.currentChatRoom = null;
                    pet.chatCD = chatCD;
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

                initiator.AsTouhouPet().currentChatRoom = ChatRoom[i];

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
        /// 宠物当前是否没有在说话
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns>当 chatTimeLeft 小于等于 0 时返回 true</returns>
        public static bool CurrentlyNoDialog(this Projectile projectile)
        {
            if (!projectile.IsATouhouPet())
                return false;

            return projectile.AsTouhouPet().chatTimeLeft <= 0;
        }

        /// <summary>
        /// 关闭当前宠物的对话（将chatTimeLeft设为0）
        /// </summary>
        /// <param name="projectile"></param>
        public static void CloseCurrentDialog(this Projectile projectile)
        {
            projectile.AsTouhouPet().chatTimeLeft = 0;
        }

        #region 设置聊天室
        /// <summary>
        /// 编辑聊天室
        /// </summary>
        /// <param name="chatRoom"></param>
        /// <param name="maxTurn">最大回合数</param>
        /// <param name="infoList">成员信息的元组列表</param>
        public static void ModifyChatRoom(this PetChatRoom chatRoom, List<ChatRoomInfo> infoList)
        {
            //成员ID列表
            List<TouhouPetID> member = [];

            //使用弹幕实例的成员信息元组列表
            List<(Projectile pet, LocalizedText text, int chatTurn)> info = [];

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
            if (!MaintainChatRoom(ref chatRoom, member) && chatRoom != null)
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
                info.Add((chatRoom.member[member.IndexOf(i.UniqueID)], i.ChatText, i.ChatTurn));
            }

            //根据长度遍历元组列表内容
            foreach ((Projectile pet, LocalizedText text, int chatTurn) in info)
            {
                //如果加入的弹幕不存在，则不设置对话
                if (pet == null || !pet.active)
                    continue;

                //以防万一，如果加入的弹幕并非东方宠物，则不设置对话
                if (!pet.IsATouhouPet())
                    continue;

                //若回合数不等于当前回合值则跳过
                if (chatTurn != chatRoom.chatTurn)
                    continue;

                //起始回合时发起者不设置对话
                if (chatTurn != -1 || pet != chatRoom.initiator)
                {
                    pet.SetChatForChatRoom(text, 20);
                }

                //若当前对话已说完，则将回合值+1并跳出循环，防止多加
                if (pet.CurrentlyNoDialog())
                {
                    chatRoom.chatTurn++;
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

            BasicTouhouPet initiator = chatRoom.initiator.AsTouhouPet();

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
                    if (chatRoom.initiator.AsTouhouPet().FindPetByUniqueID(out Projectile p, member[i]))
                    {
                        memberArray[i] = p;
                        //以防万一，若发现该成员不是东方宠物，则不设置成员聊天室
                        if (i > 0 && memberArray[i].IsATouhouPet())
                        {
                            memberArray[i].AsTouhouPet().currentChatRoom = chatRoom;
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
        private static void SetChat_Inner(Projectile projectile, ChatSettingConfig config, int lag = 0, LocalizedText text = null, bool forcely = true)
        {
            //若被设置对象并非东方宠物，则不再执行后续
            if (!projectile.IsATouhouPet())
                return;

            BasicTouhouPet pet = projectile.AsTouhouPet();

            //若宠物非玩家本人召唤、或宠物有话还没说完，则不再执行后续
            if (projectile.owner != Main.myPlayer || pet.chatTimeLeft > 0)
                return;

            //若文本为空则不执行后续
            if (text == null)
                return;

            //若目标文本键等于当前文本键、且并非强制赋值，则不会进行设置
            //此举是为了避免重复设置导致聊天室内宠物无法结束说话
            if (text.Equals(pet.chatText) && !forcely)
                return;

            //自动处理单个字符剩余时间
            if (text.Value.Length > 10 && !config.AutoHandleTimeLeft)
            {
                if (config.TimeLeftPerWord > 10)
                {
                    config.TimeLeftPerWord = 10;
                }
            }
            //自动设置打字所需时间
            if (config.TyperModeUseTime == -1)
            {
                config.TyperModeUseTime = Math.Clamp(text.Value.Length * 5, 0, 150);
            }

            pet.chatBaseY = -24;
            pet.chatScale = 0f;
            pet.chatText = text;
            pet.chatTimeLeft = Math.Clamp(text.Value.Length * config.TimeLeftPerWord, 0, 420);
            pet.timeToType = 0;
            pet.totalTimeToType = config.TyperModeUseTime;
            pet.chatLag = lag;

            pet.textColor = config.TextColor;
            pet.boardColor = config.TextBoardColor;

            //Main.NewText($"Index: {index}", Main.DiscoColor);
        }

        /// <summary>
        /// 设置宠物要说的话
        /// <br/>当 ChatTimeLeft 大于 0 时，不输出结果
        /// </summary>
        /// <param name="text">对话文本</param>
        /// <param name="config">对话属性配置</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChat(this Projectile projectile, ChatSettingConfig config, LocalizedText text, int lag = 0)
        {
            SetChat_Inner(projectile, config, lag, text);
        }

        /// <summary>
        /// 设置宠物要说的话，自动调用ChatSettingConfig
        /// <br/>当 ChatTimeLeft 大于 0 时，不输出结果
        /// </summary>
        /// <param name="text">对话文本</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChat(this Projectile projectile, LocalizedText text, int lag = 0)
        {
            SetChat_Inner(projectile, projectile.AsTouhouPet().ChatSettingConfig, lag, text);
        }

        /// <summary>
        /// 设置宠物要说的话，自动调用ChatSettingConfig
        /// <br>会检测 text 参数是否等于宠物当前的对话文本键，若相等则不继续设置</br>
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="text">对话文本</param>
        /// <param name="lag">说话前的延时</param>
        public static void SetChatForChatRoom(this Projectile projectile, LocalizedText text, int lag = 0)
        {
            if (!projectile.IsATouhouPet())
                return;

            SetChat_Inner(projectile, projectile.AsTouhouPet().ChatSettingConfig, lag, text, false);
        }
        #endregion
    }
}
