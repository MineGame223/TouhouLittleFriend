﻿using Terraria;
using TouhouPets.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System;
using static TouhouPets.TouhouPets;
using static TouhouPets.ChatRoomSystem;

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
        /// <returns>当 shouldNotTalking 为 true 且 chatCD 小于等于 0 时，返回 true</returns>
        public static bool ShouldPetTalking(this Projectile projectile)
        {
            return !projectile.ToPetClass().shouldNotTalking && projectile.ToPetClass().chatCD <= 0;
        }
        /// <summary>
        /// 将 <see cref="Projectile"/> 类转换为 <see cref="BasicTouhouPet"/> 类
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static BasicTouhouPet ToPetClass(this Projectile projectile)
        {
            return Main.projectile[projectile.whoAmI].ModProjectile as BasicTouhouPet;
        }
        /// <summary>
        /// 关闭当前聊天室，并将聊天发起者与其中成员的currentChatRoom设为空、chatIndex归零
        /// </summary>
        /// <param name="chatRoom">当前聊天室</param>
        /// <param name="chatCD">聊天冷却时间，归零前宠物之间不会再次发起聊天</param>
        public static void CloseChatRoom(this PetChatRoom chatRoom, int chatCD = 21600)
        {
            BasicTouhouPet owner = ToPetClass(chatRoom.initiator);
            owner.currentChatRoom = null;
            owner.chatIndex = 0;
            owner.chatCD = chatCD;
            foreach (Projectile m in chatRoom.member)
            {
                if (m != null && m.active)
                {
                    ToPetClass(m).currentChatRoom = null;
                    ToPetClass(m).chatIndex = 0;
                    ToPetClass(m).chatCD = chatCD;
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
                ChatRoom[i] = new PetChatRoom();
                ChatRoom[i].active = true;
                ChatRoom[i].initiator = initiator;
                ChatRoom[i].chatTurn = -1;

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
        /// 宠物当前是否已说完话或尚未准备说话
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns>当 chatTimeLeft 小于等于 1 时返回 true</returns>
        public static bool CurrentDialogFinished(this Projectile projectile)
        {
            return projectile.ToPetClass().chatTimeLeft <= 1;
        }
        /// <summary>
        /// 关闭当前宠物的对话（将chatTimeLeft设为0）
        /// </summary>
        /// <param name="projectile"></param>
        public static void CloseCurrentDialog(this Projectile projectile)
        {
            projectile.ToPetClass().chatTimeLeft = 0;
        }
        /// <summary>
        /// 设置宠物要说的话
        /// <br/>当 <see cref="ChatTimeLeft"/> 或 <see cref="ChatCD"/> 大于0时不输出结果
        /// </summary>
        /// <param name="index">对话索引值</param>
        /// <param name="config">对话属性配置</param>
        /// <param name="lag">说话前的延时</param>
        /// <param name="color">文本颜色</param>
        public static void SetChat(this Projectile projectile, ChatSettingConfig config, int index, int lag = 0, Color color = default)
        {
            BasicTouhouPet pet = projectile.ToPetClass();
            if (projectile.owner != Main.myPlayer || pet.chatTimeLeft > 0)
            {
                return;
            }
            if (color == default)
            {
                color = pet.ChatTextColor;
            }
            string chat = pet.ChatDictionary[index];
            if (chat.Length > 10 && !config.AutoHandleTimeLeft)
            {
                if (config.TimeLeftPerWord > 10)
                {
                    config.TimeLeftPerWord = 10;
                }
            }
            if (config.TyperModeUseTime == -1)
            {
                config.TyperModeUseTime = Math.Clamp(chat.Length * 5, 0, 150);
            }
            pet.chatIndex = index;

            pet.chatBaseY = -24;
            pet.chatScale = 0f;
            pet.chatText = chat;
            pet.chatTimeLeft = Math.Clamp(chat.Length * config.TimeLeftPerWord, 0, 420);
            pet.timeToType = 0;
            pet.totalTimeToType = config.TyperModeUseTime;
            pet.chatColor = color;
            pet.chatLag = lag;

            //Main.NewText($"Index: {index}", Main.DiscoColor);
        }
    }
}
