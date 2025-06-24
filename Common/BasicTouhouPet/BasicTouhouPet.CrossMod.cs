using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.Utilities;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    partial class BasicTouhouPet
    {
        /// <summary>
        /// 将跨模组常规对话加入被传入的随机选择器中
        /// </summary>
        /// <param name="chatText"></param>
        private void GetCrossModChat(ref WeightedRandom<LocalizedText> chatText)
        {
            //以防万一
            if (CrossModDialog == null)
                return;

            int id = (int)UniqueID;
            if (id <= (int)TouhouPetID.None || id >= (int)TouhouPetID.Count)
                return;

            //若没有模组进行注册则不执行后续
            var dialogList = CrossModDialog[id];
            if (dialogList.Count <= 0)
                return;

            foreach(var info in dialogList)
            {
                //若符合注册时写入的条件，则加入随机选择器
                if (!info.Condition())
                    continue;

                chatText.Add(info.DialogText, info.Weight);
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
                if (!AllowToUseChatRoom(infoList[0].ChatText))
                    continue;

                PetChatRoom room = currentChatRoom ?? Projectile.CreateChatRoomDirect();

                room.ModifyChatRoom(infoList);
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
                if (bossType != i.ObjectType)
                    continue;

                if (i.CommentContent.Count <= 0)
                    continue;

                WeightedRandom<LocalizedText> result = new();
                foreach (var j in i.CommentContent)
                {
                    if (j.Condition())
                        result.Add(j.DialogText, j.Weight);
                }
                if (result.elements.Count > 0)
                {
                    Projectile.SetChat(result);
                    return true;
                }
            }
            return false;
        }
    }
}
