﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    /// <summary>
    /// 假面骑士赤蛮骑
    /// </summary>
    public class Sekibanki : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Posing,
            AfterPosing,
            BeforeHenshin,
            Henshin,
            AfterHenshin,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int ActionCD
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private int RandomCount
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private int CloseEyesTimer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private bool ShouldCloseEyes => CloseEyesTimer > 0;
        private bool IsIdleState => PetState <= 1;

        private int blinkFrame, blinkFrameCounter;
        private int headFrame, headFrameCounter;
        private int headBaseY, headAdjX, headAdjY;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sekibanki_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 17;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 4, 7)
                .WhenSelected(11, 3, 6);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Sekibanki;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Vector2 headPosAdj = new Vector2(0, 2 * headBaseY) + new Vector2(headAdjX, headAdjY);
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            DrawPetConfig config2 = drawConfig with
            {
                PositionOffset = headPosAdj,
            };

            Projectile.DrawPet(headFrame, lightColor, config2, 1);
            Projectile.DrawPet(headFrame, lightColor,
                config with
                {
                    PositionOffset = headPosAdj,
                }, 1);
            Projectile.ResetDrawStateForPet();

            if (CurrentState == States.Blink || ShouldCloseEyes)
                Projectile.DrawPet(blinkFrame, lightColor, config2, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 105, 105),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sekibanki";
            indexRange = new Vector2(1, 14);
        }
        public override void PostRegisterChat()
        {
            IsChatRoomActive.Add(ChatDictionary[1], false);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 777;//777
            chance = 7;//7
            whenShouldStop = !IsIdleState || chatCD > 0;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                for (int i = 11; i <= 14; i++)
                {
                    chat.Add(ChatDictionary[i]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateHeadFrame();
            UpdateHeadPosition();
        }
        private void UpdateTalking()
        {
            if (AllowToUseChatRoom(ChatDictionary[1]))
            {
                Chatting2(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
            };
        }
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID seki = TouhouPetID.Sekibanki;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(seki,ChatDictionary[3], -1),//赤蛮奇：一直以来我都穿着斗篷，
                new ChatRoomInfo(seki,ChatDictionary[4], 0),//赤蛮奇：因为我不喜欢被人类认出是妖怪。
                new ChatRoomInfo(seki,ChatDictionary[5], 1),//赤蛮奇：人类不喜欢妖怪，我也不怎么想亲近人类...
                new ChatRoomInfo(seki,ChatDictionary[6], 2),//赤蛮奇：...好吧，除了你。
            ];

            return list;
        }
        private void Chatting2(PetChatRoom chatRoom)
        {
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //赤蛮奇：独来独往...
                if (Projectile.CurrentlyNoDialog())
                {
                    if (Main.rand.NextBool(10))
                    {
                        chatRoom.chatTurn++;
                        if (OwnerIsMyPlayer)
                        {
                            CurrentState = States.BeforeHenshin;
                        }
                    }
                    else
                    {
                        chatRoom.CloseChatRoom(60);
                    }
                }
            }
            else if (turn == 0)
            {
                //赤蛮奇：...我将超越一切！
                Projectile.SetChatForChatRoom(ChatDictionary[7], 20);

                if (Projectile.CurrentlyNoDialog())
                {
                    chatRoom.chatTurn++;
                    if (OwnerIsMyPlayer)
                    {
                        CurrentState = States.Henshin;
                    }
                }
            }
            else if (turn == 1)
            {
                //赤蛮奇：超——变——身——！！！
                Projectile.SetChatForChatRoom(ChatDictionary[8], 20);

                if (Projectile.CurrentlyNoDialog())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //赤蛮奇：......
                Projectile.SetChatForChatRoom(ChatDictionary[9], 20);

                if (Projectile.CurrentlyNoDialog())
                {
                    chatRoom.chatTurn++;
                    if (OwnerIsMyPlayer)
                    {
                        CurrentState = States.AfterHenshin;
                    }
                }
            }
            else if (turn == 3)
            {
                //赤蛮奇：...呃，你什么都没听见
                Projectile.SetChatForChatRoom(ChatDictionary[10], 20);

                if (Projectile.CurrentlyNoDialog())
                {
                    chatRoom.chatTurn++;
                }
            }
            else
            {
                chatRoom.CloseChatRoom(12000);
            }
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<SekibankiBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;
                case States.Posing:
                    Posing();
                    break;
                case States.AfterPosing:
                    AfterPosing();
                    break;
                case States.BeforeHenshin:
                    BeforeHenshin();
                    break;
                case States.Henshin:
                    Henshin();
                    break;
                case States.AfterHenshin:
                    AfterHenshin();
                    break;
                default:
                    Idle();
                    break;
            }
            if (IsIdleState)
            {
                IdleAnimation();
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            if (CloseEyesTimer > 0)
            {
                blinkFrame = 12;
                CloseEyesTimer--;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            ChangeDir();

            Vector2 point = new Vector2(-40 * Owner.direction, -30 + Owner.gfxOffY);
            MoveToPoint(point, 7f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0 && !ShouldCloseEyes)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 555 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(9))
                    {
                        RandomCount = Main.rand.Next(5, 10);
                        CurrentState = States.Posing;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 11)
            {
                blinkFrame = 11;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 11;
                CurrentState = States.Idle;
            }
        }
        private void Posing()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 6)
            {
                Projectile.frame = 9;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 11;
                Timer++;
            }
            if (OwnerIsMyPlayer)
            {
                if (Timer > RandomCount)
                {
                    Timer = 0;
                    CurrentState = States.AfterPosing;
                }
            }
        }
        private void AfterPosing()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 16)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 3600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void BeforeHenshin()
        {
            IdleAnimation();
            CloseEyesTimer = 2;
        }
        private void Henshin()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 7;
            }
        }
        private void AfterHenshin()
        {
            if (Projectile.frame < 15)
                Projectile.frame = 15;

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 16)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    CloseEyesTimer = 480;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateHeadFrame()
        {
            if (++headFrameCounter > 6)
            {
                headFrameCounter = 0;
                headFrame++;
            }
            if (Projectile.frame >= 9 && Projectile.frame <= 13)
            {
                if (headFrame > 10)
                {
                    headFrame = 5;
                }
            }
            else
            {
                if (headFrame > 5)
                {
                    if (headFrame > 10 || Projectile.isAPreviewDummy)
                    {
                        headFrame = 0;
                    }
                }
                else if (headFrame > 4)
                {
                    headFrame = 0;
                }
            }
        }
        private void UpdateHeadPosition()
        {
            if (Projectile.frame >= 9 && Projectile.frame <= 13)
            {
                if (headBaseY > -15)
                    headBaseY--;
            }
            else
            {
                if (headBaseY < 0)
                    headBaseY += 2;
            }
            if (headBaseY > 0)
            {
                headBaseY = 0;
            }
            headAdjX = 0;
            headAdjY = 0;
            if (Projectile.frame >= 5 && Projectile.frame <= 16)
            {
                headAdjY = -2;
                if (Projectile.frame >= 10 && Projectile.frame <= 15)
                {
                    headAdjX = -2 * Projectile.spriteDirection;
                }
            }
        }
    }
}


