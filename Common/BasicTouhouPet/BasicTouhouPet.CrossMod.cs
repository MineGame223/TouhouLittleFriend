using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    partial class BasicTouhouPet
    {
        /// <summary>
        /// 模组添加的对话索引起始的地方
        /// </summary>
        internal int crossModDialogStartIndex;

        #region 对话注册
        /// <summary>
        /// 注册跨模组常规对话
        /// </summary>
        private void RegisterCrossModChat()
        {
            if (CrossModDialog == null)
                return;

            int lastIndex = ChatDictionary.Count;

            int id = (int)UniqueID;
            if (id <= (int)TouhouPetID.None)
                return;

            var dialogList = CrossModDialog[id];
            if (dialogList.Count <= 0)
                return;

            crossModDialogStartIndex = lastIndex + 1;

            for (int i = 0; i < dialogList.Count; i++)
            {
                string text = dialogList[i].DialogText.Value;
                int startIndex = crossModDialogStartIndex + i;

                ChatDictionary.Add(startIndex, text);
            }
        }

        /// <summary>
        /// 注册跨模组聊天室
        /// </summary>
        private void RegisterCrossModChatRoom()
        {
            //若没有模组注册聊天室列表，则不执行后续
            if (CrossModChatRoomList == null)
                return;

            int id = (int)UniqueID;
            if (id <= (int)TouhouPetID.None)
                return;

            var listRoom = CrossModChatRoomList[id];
            if (listRoom.Count <= 0)
                return;

            foreach (var i in listRoom)
            {
                int startIndex = crossModDialogStartIndex + i[0].ChatIndex;
                IsChatRoomActive.TryAdd(startIndex, false);
            }
        }
        #endregion

        #region 对话更新
        /// <summary>
        /// 将跨模组常规对话加入被传入的随机选择器中
        /// </summary>
        /// <param name="chatText"></param>
        private void GetCrossModChat(ref WeightedRandom<string> chatText)
        {
            //若没有模组进行注册则不执行后续
            if (CrossModDialog == null)
                return;

            int id = (int)UniqueID;
            if (id <= (int)TouhouPetID.None)
                return;

            var dialogList = CrossModDialog[id];
            if (dialogList.Count <= 0)
                return;

            for (int i = 0; i < dialogList.Count; i++)
            {
                SingleDialogInfo info = dialogList[i];
                //若符合注册时写入的条件，则加入随机选择器
                if (info.Condition())
                {
                    chatText.Add(ChatDictionary[crossModDialogStartIndex + i], info.Weight);
                }
            }
        }

        /// <summary>
        /// 更新跨模组聊天室
        /// </summary>
        private void UpdateCrossModChatRoom()
        {
            //遍历模组聊天室列表并根据条件执行聊天方法
            foreach (var infoList in CrossModChatRoomList[(int)UniqueID])
            {
                //若发现模组聊天室信息列表中的第一个ChatRoomInfo中的索引值不在允许名单内，则不设置并维持聊天室
                if (!AllowToUseChatRoom(crossModDialogStartIndex + infoList[0].ChatIndex))
                    continue;

                PetChatRoom room = currentChatRoom ?? Projectile.CreateChatRoomDirect();

                //这里需要额外加上模组对话索引的起始值
                room.ModifyChatRoom(infoList, true);
            }
        }

        /// <summary>
        /// 跨模组Boss评价
        /// </summary>
        /// <param name="bossType"></param>
        /// <param name="uniqueID"></param>
        private bool BossChat_CrossMod(int bossType, TouhouPetID uniqueID)
        {
            int id = (int)uniqueID;
            List<CommentInfo> comments = CrossModBossComment[id];
            //若列表不存在内容，则不执行后续
            if (comments == null || comments.Count <= 0)
            {
                return false;
            }
            foreach (var i in comments)
            {
                if (bossType == i.ObjectType)
                {
                    Projectile.SetChat(i.CommentText.Get().Value);
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
