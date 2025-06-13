using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;
using static TouhouPets.TouhouPets;

namespace TouhouPets
{
    partial class BasicTouhouPet
    {
        /// <summary>
        /// 是否不应该说话
        /// <br/>该属性并不会影响宠物更新常规对话，仅作为是否应当参与聊天的判断条件
        /// <br/>!--该属性会反复重置
        /// </summary>
        internal bool shouldNotTalking;
        /// <summary>
        /// 
        /// </summary>
        internal Color textColor = Color.White;
        /// <summary>
        /// 
        /// </summary>
        internal Color boardColor = Color.Black;
        /// <summary>
        /// 对话文本不透明度
        /// </summary>
        internal float chatOpacity;

        /// <summary>
        /// 对话字体大小
        /// </summary>
        internal float chatScale;

        /// <summary>
        /// 整段对话文本的底部坐标
        /// </summary>
        internal float chatBaseY = 0;

        /// <summary>
        /// 对话的持续剩余时间
        /// </summary>
        internal int chatTimeLeft;

        /// <summary>
        /// 完成一次对话后的间隔，在大于0且 <see cref="chatTimeLeft"/> 小于等于0时会一直减少至0
        /// <br/>该值不为0时，宠物不会发起向其他宠物的对话或接受来自其他宠物的对话
        /// </summary>
        internal int chatCD;

        /// <summary>
        /// 对话文本对应的索引值
        /// </summary>
        internal int chatIndex;

        /// <summary>
        /// 对话文本
        /// </summary>
        internal string chatText;

        /// <summary>
        /// 说话前的延时
        /// </summary>
        internal int chatLag;

        /// <summary>
        /// 打字机式文本显示状态下，打印文本总共需要的时间
        /// </summary>
        internal float totalTimeToType;

        /// <summary>
        /// 打字机式文本显示状态下，打印单个字符所需的时间
        /// </summary>
        internal float timeToType;

        /// <summary>
        /// 主要计时器，从0增加至4800再重置并循环
        /// <br/>由于该计时器并不接受同步，故最好只在本地客户端执行与其相关的操作
        /// </summary>
        internal int mainTimer;

        /// <summary>
        /// 是否开启对话文字震动
        /// <br/>恋恋专用
        /// <br/>!--该属性会反复重置
        /// </summary>
        internal bool textShaking;

        /// <summary>
        /// 对话字典
        /// </summary>
        internal Dictionary<int, string> ChatDictionary = [];

        /// <summary>
        /// 对应聊天室是否启动
        /// </summary>
        internal Dictionary<int, bool> IsChatRoomActive = [];

        /// <summary>
        /// 当前聊天室
        /// </summary>
        internal PetChatRoom currentChatRoom;

        internal bool AllowToUseChatRoom(int indexOfChatRoom)
        {
            if (!IsChatRoomActive.ContainsKey(indexOfChatRoom))
            {
                return false;
            }
            if (FindChatIndex(indexOfChatRoom))
            {
                IsChatRoomActive[indexOfChatRoom] = true;
            }
            return IsChatRoomActive[indexOfChatRoom];
        }

        #region 文本绘制方法
        private void DrawChatText(Vector2 pos, float alpha, int maxWidth = 210)
        {
            string text = chatText;

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Color color = textColor;
            Color bColor = boardColor;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float totalScale = 0.9f;
            string _text = text;
            bool typerStyle = GetInstance<PetDialogConfig>().TyperStyleChat;

            if (typerStyle)
            {
                int textLength = (int)Math.Clamp(timeToType / (totalTimeToType / text.Length), 0, text.Length);
                _text = text.Remove(textLength);
            }
            string[] array = DrawUtils.MyWordwrapString(_text, font, maxWidth, 10, out int chatLine);
            chatLine++;
            for (int i = 0; i < chatLine; i++)
            {
                if (array[i] != null)
                {
                    if (typerStyle)
                    {
                        chatBaseY = 0;
                        chatScale = 1;
                    }
                    Vector2 offset = font.MeasureString(array[i]);
                    Vector2 orig = offset / 2;
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, font
                            , array[i], pos.X
                            , pos.Y + orig.Y / 2 + i * 30 * totalScale - (chatLine - 1) * 30 * totalScale - chatBaseY
                            , color * alpha, bColor * alpha
                            , orig, chatScale * totalScale);
                }
            }
        }
        private void DrawChatText_Koishi(Vector2 pos, float alpha, int maxWidth = 240)
        {
            string text = chatText;

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Color color = textColor;
            Color bColor = boardColor;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float totalScale = 0.9f;
            string[] array = DrawUtils.MyWordwrapString(text, font, maxWidth, 10, out int chatLine);

            chatLine++;
            for (int i = 0; i < chatLine; i++)
            {
                if (array[i] != null)
                {
                    Vector2 offset = font.MeasureString(array[i]);
                    Vector2 orig = offset / 2;
                    char[] array2 = text.ToCharArray();
                    int textLength = (int)Math.Clamp(timeToType / (totalTimeToType / text.Length), 0, text.Length);
                    for (int l = 0; l < array.Length - text.Length + textLength; l++)
                    {
                        string chara = array2[l].ToString();
                        float xOffset = FontAssets.MouseText.Value.MeasureString(text.Remove(l)).X;
                        DrawUtils.MyDrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                            , chara, pos.X + xOffset + Main.rand.Next(-2, 2)
                            , pos.Y + orig.Y / 2 + i * 30 * totalScale - (chatLine - 1) * 30 * totalScale + Main.rand.Next(-2, 2)
                            , color * alpha, bColor * alpha, orig, chatScale * totalScale);
                    }
                }
            }
        }
        #endregion

        #region 对话注册方法
        /// <summary>
        /// 完整的注册对话方法
        /// </summary>
        private void RegisterChat_Full()
        {
            string name = string.Empty;
            Vector2 indexRange = Vector2.Zero;

            //通过该方法设置字符标签与索引范围
            RegisterChat(ref name, ref indexRange);

            //若标签为空则不执行后续
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            //将对话文本加入字典
            for (int i = (int)indexRange.X; i <= (int)indexRange.Y; i++)
            {
                string chatText = ModUtils.GetChatText(name, i.ToString());
                if (string.IsNullOrEmpty(chatText))
                {
                    chatText = "这是一段空对话，你怎么找出来的？";
                }
                ChatDictionary.Add(i, chatText);
            }

            //仅当聊天室列表存在内容时进行注册
            if (RegisterChatRoom().Count > 0)
            {
                foreach (var infoList in RegisterChatRoom())
                {
                    IsChatRoomActive.Add(infoList[0].ChatIndex, false);
                }
            }

            PostRegisterChat();

            RegisterCrossModChat();
            RegisterCrossModChatRoom();

            //增加一个空位，防止WeightedRandom无法读取最后一个索引
            int lastIndex = ChatDictionary.Count;
            ChatDictionary.Add(lastIndex + 1, string.Empty);
        }
        
        #endregion

        #region 对话更新方法
        /// <summary>
        /// 随机选取常规对话并让设置宠物说话
        /// </summary>
        private void UpdateRegularDialog()
        {
            //正在聊天室中时不执行后续
            if (currentChatRoom != null || mainTimer <= 0)
                return;

            int time = 0;
            int chance = 0;
            bool stop = true;

            //设置说话间隔、说话几率以及说话条件
            SetRegularDialog(ref time, ref chance, ref stop);

            if (mainTimer % time == 0 && Main.rand.NextBool(chance) && !stop)
            {
                WeightedRandom<string> chatText = RegularDialogText();

                //将跨模组的常规对话加入随机选择器
                GetCrossModChat(ref chatText);

                string result = chatText.Get();

                //发现空对话时不执行后续
                if (string.IsNullOrEmpty(result))
                    return;

                //将获取结果与对话字典中存在的语句进行匹配并设置说话
                for (int i = 1; i < ChatDictionary.Count; i++)
                {
                    if (!result.Equals(ChatDictionary[i]))
                        continue;

                    Projectile.SetChat(ChatSettingConfig, i);
                }
            }
        }
        /// <summary>
        /// 更新聊天室
        /// </summary>
        private void UpdateChatRoom()
        {
            //遍历聊天室列表并根据条件执行聊天方法
            foreach (var infoList in RegisterChatRoom())
            {
                //若发现聊天室信息列表中的第一个ChatRoomInfo中的索引值不在允许名单内，则不设置并维持聊天室
                if (!AllowToUseChatRoom(infoList[0].ChatIndex))
                    continue;

                PetChatRoom room = currentChatRoom ?? Projectile.CreateChatRoomDirect();
                room.ModifyChatRoom(infoList);
            }
        }
        /// <summary>
        /// 更新对话文本的状态
        /// </summary>
        private void UpdateChatState()
        {
            if (chatLag > 0)
            {
                chatLag--;
            }
            else
            {
                chatLag = 0;
            }
            if (chatTimeLeft > 0)
            {
                if (chatLag <= 0)
                {
                    chatBaseY += 1.2f;
                    chatScale += 0.05f;
                    chatOpacity += 0.1f;
                    if (timeToType >= totalTimeToType)
                        chatTimeLeft--;
                }
            }
            else
            {
                chatBaseY = 0;
                chatScale = 1;
                chatTimeLeft = 0;
                chatOpacity -= 0.05f;
            }

            if (chatOpacity > 0)
                timeToType++;

            if (chatCD > 0)
                chatCD--;

            chatBaseY = Math.Clamp(chatBaseY, -24, 0);
            chatScale = Math.Clamp(chatScale, 0, 1);
            chatOpacity = Math.Clamp(chatOpacity, 0, 1);
            timeToType = Math.Clamp(timeToType, 0, totalTimeToType);

            textShaking = false;
            shouldNotTalking = false;
        }
        /// <summary>
        /// 完整的对话更新方法
        /// </summary>
        private void UpdateChat_Full()
        {
            //仅在本地端、且Config允许说话时执行
            if (!OwnerIsMyPlayer || !GetInstance<PetDialogConfig>().CanPetChat)
                return;

            UpdateChatState();

            if (UpdateFindBoss())
            {
                shouldNotTalking = true;
                return;
            }

            UpdateRegularDialog();

            UpdateChatRoom();
            UpdateCrossModChatRoom();
        }
        #endregion

        #region 对话相关查找方法
        /// <summary>
        /// 查找自身的<see cref="chatIndex"/>
        /// </summary>
        /// <param name="minIndex">最小索引值</param>
        /// <param name="maxIndex">最大索引值，默认等于最小索引值</param>
        /// <returns></returns>
        internal bool FindChatIndex(int minIndex, int maxIndex = 0)
        {
            if (maxIndex <= minIndex)
            {
                maxIndex = minIndex;
            }
            if (chatIndex >= minIndex && chatIndex <= maxIndex)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 对话重写函数
        /// <summary>
        /// 注册对话文本及其索引值
        /// <br/>仅在本地端更新
        /// </summary>
        /// <param name="name">对话所属宠物的名字</param>
        /// <param name="indexRange">对话索引的范围</param>
        public virtual void RegisterChat(ref string name, ref Vector2 indexRange) { }
        /// <summary>
        /// 完成基本对话注册后执行的内容
        /// </summary>
        public virtual void PostRegisterChat() { }
        /// <summary>
        /// 设置常规对话文本
        /// <br/>仅在本地端更新
        /// </summary>
        /// <param name="timePerDialog">每次说话机会的间隔</param>
        /// <param name="chance">说话的几率（1 / chance）</param>
        /// <param name="whenShouldStop">何时应当停止说话</param>
        public virtual void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop) { }
        /// <summary>
        /// 常规对话
        /// <br/>仅在本地端更新
        /// </summary>
        /// <returns></returns>
        public virtual WeightedRandom<string> RegularDialogText() => null;
        /// <summary>
        /// 注册聊天室列表
        /// </summary>
        /// <returns></returns>
        public virtual List<List<ChatRoomInfo>> RegisterChatRoom() => [];
        #endregion
    }
}
