using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Junko : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Wrath,
            AfterWrath,
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

        private int blinkFrame, blinkFrameCounter;
        private int tailFrame, tailFrameCounter;
        private float auraValue;

        private DrawPetConfig drawConfig = new(1);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Junko_Cloth");
        private readonly Texture2D tailTex = AltVanillaFunction.GetExtraTexture("Junko_Tail");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Junko;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawTail();
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        private void DrawTail()
        {
            Texture2D tex = tailTex;
            int width = tex.Width / drawConfig.TextureRow;
            int height = tex.Height / 4;
            Rectangle rect = new(0, tailFrame * height, width, height);

            Vector2 extraPos = new Vector2(-4 * Projectile.spriteDirection, 20).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.DefaultDrawPetPosition() + extraPos;
            Vector2 orig = new(rect.Width / 2, rect.Height);

            Main.spriteBatch.QuickEndAndBegin(true, Projectile.isAPreviewDummy, BlendState.Additive);
            for (int i = -4; i <= 4; i++)
            {
                Color clr = Projectile.GetAlpha(Color.White * (1 - Math.Abs(i * 0.12f))) * mouseOpacity;
                float rotOffset = MathHelper.ToRadians(3 * i * (float)Math.Sin(Main.GlobalTimeWrappedHourly));
                float rot = MathHelper.ToRadians(30 * i) + rotOffset;
                float rot2 = MathHelper.ToRadians(20 * i * 1.1f) + rotOffset;
                Vector2 scale = new(Projectile.scale * 0.7f, (Projectile.scale * (1f - Math.Abs(i * 0.2f))) + (auraValue * 0.7f));

                Main.EntitySpriteDraw(tex, pos, rect, clr * 0.5f, Projectile.rotation + rot2, orig, scale, SpriteEffects.None);
                Main.EntitySpriteDraw(tex, pos, rect, clr, Projectile.rotation + rot, orig, scale, SpriteEffects.None);
            }
            Main.spriteBatch.QuickEndAndBegin(false, Projectile.isAPreviewDummy);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(254, 159, 75),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Junko";
            indexRange = new Vector2(1, 4);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 1000;//1000
            chance = 10;//10
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new();
            {
                if (!Main.dayTime && Main.cloudAlpha <= 0 && Main.GetMoonPhase() == MoonPhase.Full)
                {
                    chat.Add(ChatDictionary[1]);
                }
                if (FindPet(ProjectileType<Reisen>()))
                {
                    chat.Add(ChatDictionary[3]);
                }
            }
            return chat;
        }
        public override void OnFindBoss(NPC boss, bool noReaction)
        {
            if (boss.type == NPCID.MoonLordCore)
            {
                Projectile.SetChat(ChatDictionary[4]);
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
            TouhouPetID junko = TouhouPetID.Junko;
            TouhouPetID reisen = TouhouPetID.Reisen;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(junko, ChatDictionary[3], -1), //纯狐：乌冬酱~最近还好嘛？
                new ChatRoomInfo(reisen, GetChatText("Reisen",11), 0),//铃仙：嗯嗯...还、还好吧...
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            float lightStrength = 5 * auraValue;
            rgb = new Vector3(2.38f + lightStrength, 1.41f + lightStrength, 2.55f + lightStrength);
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<JunkoBuff>());

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Wrath:
                    shouldNotTalking = true;
                    Wrath();
                    break;

                case States.AfterWrath:
                    shouldNotTalking = true;
                    AfterWrath();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            if (IsIdleState)
            {
                IdleAnimation();
            }
            UpdateAuraValue();
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.018f;

            ChangeDir();

            Vector2 point = new Vector2(45 * Owner.direction, -65 + Owner.gfxOffY);
            MoveToPoint(point, 17f);
        }
        private void UpdateAuraValue()
        {
            if (CurrentState == States.Wrath)
            {
                auraValue += 0.08f;
            }
            else
            {
                auraValue -= 0.01f;
            }
            auraValue = MathHelper.Clamp(auraValue, 0, 1);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 450 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        RandomCount = Main.rand.Next(48, 72);
                        CurrentState = States.Wrath;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 9)
            {
                blinkFrame = 9;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 11)
            {
                blinkFrame = 9;
                CurrentState = States.Idle;
            }
        }
        private void Wrath()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 5;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterWrath;
            }
        }
        private void AfterWrath()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 2400;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateTailFrame()
        {
            if (++tailFrameCounter > 6)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 3)
            {
                tailFrame = 0;
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
    }
}


