﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Hecatia : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int PlanteState
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        private int blinkFrame, blinkFrameCounter;

        private float[] bodyAlpha = new float[3];
        private Vector2[] plantePos = new Vector2[3];

        private DrawPetConfig drawConfig = new(3);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Hecatia_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            DrawPlantes(pos + new Vector2(0, 4 * Main.essScale), Projectile.GetAlpha(lightColor), effect);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor * bodyAlpha[0], drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor * bodyAlpha[1], drawConfig, 1);
            Projectile.DrawPet(Projectile.frame, lightColor * bodyAlpha[2], drawConfig, 2);
            if (CurrentState == States.Blink)
            {
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
                Projectile.DrawPet(blinkFrame, lightColor * bodyAlpha[0], drawConfig);
                Projectile.DrawPet(blinkFrame, lightColor * bodyAlpha[1], drawConfig, 1);
                Projectile.DrawPet(blinkFrame, lightColor * bodyAlpha[2], drawConfig, 2);
            }
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            });
            return false;
        }
        public override Color ChatTextColor
        {
            get
            {
                return PlanteState switch
                {
                    1 => new Color(79, 215, 239),
                    2 => new Color(255, 249, 137),
                    _ => new Color(255, 120, 120),
                };
            }
        }
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Hecatia";
            indexRange = new Vector2(1, 2);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 666;
            chance = 6;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(2))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Piece>();
            if (FindPet(out Projectile member, type))
            {
                chatRoom.member[0] = member;
                member.ToPetClass().currentChatRoom = chatRoom;
            }
            else
            {
                chatRoom.CloseChatRoom();
                return;
            }
            Projectile hecatia = chatRoom.initiator;
            Projectile piece = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //赫卡提娅：我的穿搭是无可挑剔的...真的会有人不喜欢么？
                piece.CloseCurrentDialog();

                if (hecatia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //皮丝：主人大人的着装当然是最时尚的啦！
                piece.SetChat(ChatSettingConfig, 3, 20);

                if (piece.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWorldState();
            IdleAnimation();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<HecatiaBuff>());

            UpdateTalking();

            ControlMovement();

            if (OwnerIsMyPlayer)
            {
                if (mainTimer == 4798 && !Projectile.isAPreviewDummy)
                {
                    PlanteState++;
                    Projectile.netUpdate = true;
                }
            }
            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                default:
                    Idle();
                    break;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.03f;

            ChangeDir();

            Vector2 point = new Vector2(-55 * Owner.direction, -40 + Owner.gfxOffY);
            MoveToPoint(point, 14.5f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer && mainTimer % 270 == 0)
            {
                CurrentState = States.Blink;
            }
        }
        private void Blink()
        {
            if (blinkFrame < 4)
            {
                blinkFrame = 4;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = 4;
                CurrentState = States.Idle;
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }

        #region 星球更新与绘制
        private int dummyTimer = 0;
        private void DrawPlantes(Vector2 pos, Color color, SpriteEffects effect)
        {
            Texture2D t2 = AltVanillaFunction.GetExtraTexture("HecatiaPlanets");
            int height2 = t2.Height / 3;
            Rectangle rect3 = new Rectangle(0, 0 * height2, t2.Width, height2);
            Rectangle rect4 = new Rectangle(0, 1 * height2, t2.Width, height2);
            Rectangle rect5 = new Rectangle(0, 2 * height2, t2.Width, height2);
            Vector2 orig2 = rect3.Size() / 2;
            //异界 -0
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(plantePos[0].X * -Projectile.spriteDirection, plantePos[0].Y).RotatedBy(Projectile.rotation), rect3, color, Projectile.rotation, orig2, Projectile.scale * 1.12f, effect, 0f);
            //地球 -1
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(plantePos[1].X * -Projectile.spriteDirection, plantePos[1].Y).RotatedBy(Projectile.rotation), rect4, color, Projectile.rotation, orig2, Projectile.scale, effect, 0f);
            //月球 -2
            Main.spriteBatch.TeaNPCDraw(t2, pos + new Vector2(plantePos[2].X * -Projectile.spriteDirection, plantePos[2].Y).RotatedBy(Projectile.rotation), rect5, color, Projectile.rotation, orig2, Projectile.scale, effect, 0f);
        }
        private void UpdateWorldState()
        {
            if (Projectile.isAPreviewDummy)
            {
                dummyTimer++;
                if (dummyTimer >= 120)
                {
                    dummyTimer = 0;
                    PlanteState++;
                }
            }
            //懒得改
            float xSpeed = 1.5f;
            if (PlanteState > 2)
            {
                PlanteState = 0;
            }
            if (PlanteState == 1)
            {
                if (plantePos[1].X > 0)
                    plantePos[1].X--;
                if (plantePos[1].X < 0)
                    plantePos[1].X++;
                if (plantePos[1].Y > -27)
                    plantePos[1].Y--;

                plantePos[2].X += xSpeed;
                if (plantePos[2].Y > -5)
                    plantePos[2].Y--;
                if (plantePos[2].Y < -5)
                    plantePos[2].Y++;

                plantePos[0].X -= xSpeed;
                if (plantePos[0].Y > -5)
                    plantePos[0].Y--;
                if (plantePos[0].Y < -5)
                    plantePos[0].Y++;

                bodyAlpha[0] -= 0.02f;
                bodyAlpha[1] += 0.02f;
                bodyAlpha[2] -= 0.02f;
            }
            else if (PlanteState == 2)
            {
                if (plantePos[2].X > 0)
                    plantePos[2].X--;
                if (plantePos[2].X < 0)
                    plantePos[2].X++;
                if (plantePos[2].Y > -27)
                    plantePos[2].Y--;

                plantePos[0].X += xSpeed;
                if (plantePos[0].Y > -5)
                    plantePos[0].Y--;
                if (plantePos[0].Y < -5)
                    plantePos[0].Y++;

                plantePos[1].X -= xSpeed;
                if (plantePos[1].Y > -5)
                    plantePos[1].Y--;
                if (plantePos[1].Y < -5)
                    plantePos[1].Y++;

                bodyAlpha[0] -= 0.02f;
                bodyAlpha[1] -= 0.02f;
                bodyAlpha[2] += 0.02f;
            }
            else
            {
                if (plantePos[0].X > 0)
                    plantePos[0].X--;
                if (plantePos[0].X < 0)
                    plantePos[0].X++;
                if (plantePos[0].Y > -27)
                    plantePos[0].Y--;

                plantePos[1].X += xSpeed;
                if (plantePos[1].Y > -5)
                    plantePos[1].Y--;
                if (plantePos[1].Y < -5)
                    plantePos[1].Y++;

                plantePos[2].X -= xSpeed;
                if (plantePos[2].Y > -5)
                    plantePos[2].Y--;
                if (plantePos[2].Y < -5)
                    plantePos[2].Y++;

                bodyAlpha[0] += 0.02f;
                bodyAlpha[1] -= 0.02f;
                bodyAlpha[2] -= 0.02f;
            }
            for (int i = 0; i <= 2; i++)
            {
                if (bodyAlpha[i] > 1)
                {
                    bodyAlpha[i] = 1;
                }
                if (bodyAlpha[i] < 0)
                {
                    bodyAlpha[i] = 0;
                }
                if (plantePos[i].X > 20)
                {
                    plantePos[i].X = 20;
                }
                if (plantePos[i].X < -20)
                {
                    plantePos[i].X = -20;
                }
                if (plantePos[i].Y > -5)
                {
                    plantePos[i].Y = -5;
                }
                if (plantePos[i].Y < -27)
                {
                    plantePos[i].Y = -27;
                }
            }
        }
        #endregion
    }
}


