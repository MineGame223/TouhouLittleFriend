﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Alice : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Puppet,
            Puppet2,
            AfterPuppet,
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
        private bool IsIdleState => CurrentState <= States.Blink;

        private int auraFrame, auraFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Alice_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Alice;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawAura();
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            return false;
        }
        private void DrawAura()
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            for (int i = 0; i < 3; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -1.4f);
                Color clr = (Color.White * 0.3f).ModifiedAlphaColor();
                Projectile.DrawPet(auraFrame, clr,
                    config with
                    {
                        PositionOffset = spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly
                        + MathHelper.TwoPi / 3 * i)
                    }
                    , 1);
            }
            Projectile.DrawPet(auraFrame, Color.White * 0.7f, config, 1);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(185, 228, 255),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Alice";
            indexRange = new Vector2(1, 17);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 1200;//1200
            chance = 4;//4
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Marisa>()))
                {
                    chat.Add(ChatDictionary[4]);
                }
                if (FindPet(ProjectileType<AliceOld>()))
                {
                    chat.Add(ChatDictionary[16]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateAuraFrame();
            UpdateClothFrame();
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2(),
            };
        }
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID alice = TouhouPetID.Alice;
            TouhouPetID marisa = TouhouPetID.Marisa;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(alice, ChatDictionary[4], -1),//爱丽丝：我说，你上次偷走的我的蘑菇什么时候能还我？
                new ChatRoomInfo(marisa, GetChatText("Marisa",12), 0),//魔理沙：哎呀，人家的事情那能叫偷嘛？那叫借啦！
                new ChatRoomInfo(alice, ChatDictionary[5], 1),//爱丽丝：别在这里耍嘴皮子了！给我个期限啊。
                new ChatRoomInfo(marisa, GetChatText("Marisa",13), 2),//魔理沙：放心，死了以后保证还给你！
                new ChatRoomInfo(alice, ChatDictionary[6], 3),//爱丽丝：...你还是别还了...下次不许再偷了！
                new ChatRoomInfo(marisa, GetChatText("Marisa",14), 4),//魔理沙：下次一定！
                new ChatRoomInfo(alice, ChatDictionary[7], 5),//爱丽丝：你！......
            ];

            return list;
        }

        private List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID alice = TouhouPetID.Alice;
            TouhouPetID alice_L = TouhouPetID.AliceLegacy;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(alice, ChatDictionary[16], -1),//爱丽丝：你是谁？总感觉有点眼熟...
                new ChatRoomInfo(alice_L, GetChatText("AliceOld",6), 0),//爱丽丝：不知道哦，可能是过去的你吧。
                new ChatRoomInfo(alice, ChatDictionary[17], 1),//爱丽丝：过去的...我？过去的时候...呃啊，弄不明白...
            ];

            return list;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<AliceBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Puppet:
                    shouldNotTalking = true;
                    Puppet();
                    break;

                case States.Puppet2:
                    shouldNotTalking = true;
                    Puppet2();
                    break;

                case States.AfterPuppet:
                    shouldNotTalking = true;
                    AfterPuppet();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.006f;

            ChangeDir();

            Vector2 point = new Vector2(-50 * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 12f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 600 == 0 && currentChatRoom == null && ActionCD <= 0
                    && Projectile.velocity.Length() <= 3f)
                {
                    if (Main.rand.NextBool(12))
                    {
                        RandomCount = Main.rand.Next(400, 600);
                        CurrentState = States.Puppet;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 10;
                CurrentState = States.Idle;
            }
        }
        private void Puppet()
        {
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 2 && Projectile.frameCounter % 4 == 0 && Projectile.frameCounter <= 7)
            {
                float posX = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                float posY = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                if (OwnerIsMyPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                        , new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), ProjectileType<AliceDoll_Proj>(), 0, 0
                        , Main.myPlayer, Projectile.whoAmI, posX, posY);
                }
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 3;
            }
            if (OwnerIsMyPlayer)
            {
                Timer++;
                if (Timer > RandomCount)
                {
                    Timer = 0;
                    RandomCount = Main.rand.Next(400, 600);
                    CurrentState = States.Puppet2;
                }
            }
        }
        private void Puppet2()
        {
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 6 && Projectile.frameCounter % 4 == 0 && Projectile.frameCounter <= 7)
            {
                float posX = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                float posY = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                if (OwnerIsMyPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                        , new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), ProjectileType<AliceDoll_Proj>(), 0, 0
                        , Main.myPlayer, Projectile.whoAmI, posX, posY);
                }
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 7;
            }
            if (OwnerIsMyPlayer)
            {
                Timer++;
                if (Timer > RandomCount)
                {
                    Timer = 0;
                    CurrentState = States.AfterPuppet;
                }
            }
        }
        private void AfterPuppet()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 9)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 320;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateAuraFrame()
        {
            int count = 3;
            if (auraFrame < 4)
            {
                auraFrame = 4;
            }
            if (++auraFrameCounter > count)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 7)
            {
                auraFrame = 4;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 6;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
    }
}


