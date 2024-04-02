using Terraria;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.TouhouPets;
using static TouhouPets.ChatRoomSystem;
using Microsoft.Xna.Framework;
using System;

namespace TouhouPets
{
    /// <summary>
    /// 聊天室相关静态拓展方法
    /// </summary>
    public static class ChatRoomHelper
    {
        /// <summary>
        /// 将 <see cref="Projectile"/> 类转换为 <see cref="BasicTouhouPetNeo"/> 类
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static BasicTouhouPetNeo ToPetClass(this Projectile projectile)
        {
            return Main.projectile[projectile.whoAmI].ModProjectile as BasicTouhouPetNeo;
        }
        /// <summary>
        /// 关闭当前聊天室，并将聊天发起者与其中成员的currentChatRoom设为空、chatIndex归零
        /// </summary>
        /// <param name="chatRoom"></param>
        public static void CloseChatRoom(this PetChatRoom chatRoom)
        {
            BasicTouhouPetNeo owner = ToPetClass(chatRoom.initiator);
            owner.currentChatRoom = null;
            owner.chatIndex = 0;
            foreach (Projectile m in chatRoom.member)
            {
                if (m != null && m.active)
                {
                    ToPetClass(m).currentChatRoom = null;
                    ToPetClass(m).chatIndex = 0;
                }
            }
            chatRoom.active = false;
        }
        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="initiator"></param>
        /// <returns></returns>
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
        /// <param name="initiator"></param>
        /// <returns></returns>
        public static PetChatRoom CreateChatRoomDirect(this Projectile initiator)
        {
            int i = CreateChatRoom(initiator);
            return ChatRoom[i];
        }
        /// <summary>
        /// 宠物当前对话是否已说完
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
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
            BasicTouhouPetNeo pet = projectile.ToPetClass();
            pet.chatTimeLeft = 0;
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
            BasicTouhouPetNeo pet = projectile.ToPetClass();
            if (projectile.owner != Main.myPlayer || pet.chatTimeLeft > 0 || pet.chatCD > 0)
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
        }
    }
}
