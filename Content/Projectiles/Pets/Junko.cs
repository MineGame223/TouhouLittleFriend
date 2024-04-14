using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Junko : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        float auraValue;
        private void DrawTail()
        {
            Texture2D tex = tailTex;
            int width = tex.Width / drawConfig.TextureRow;
            int height = tex.Height / 4;
            Rectangle rect = new Rectangle(0, tailFrame * height, width, height);

            Vector2 extraPos = new Vector2(-4 * Projectile.spriteDirection, 20).RotatedBy(Projectile.rotation);
            Vector2 pos = Projectile.DefaultDrawPetPosition() + extraPos;
            Vector2 orig = new Vector2(rect.Width / 2, rect.Height);

            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            for (int i = -4; i <= 4; i++)
            {
                Color clr = Projectile.GetAlpha(Color.White * (1 - Math.Abs(i * 0.12f)));
                float rotOffset = MathHelper.ToRadians(3 * i * (float)Math.Sin(Main.GlobalTimeWrappedHourly));
                float rot = MathHelper.ToRadians(30 * i) + rotOffset;
                float rot2 = MathHelper.ToRadians(20 * i * 1.1f) + rotOffset;
                Vector2 scale = new Vector2(Projectile.scale * 0.7f, (Projectile.scale * (1f - Math.Abs(i * 0.2f))) + (auraValue * 0.7f));

                Main.EntitySpriteDraw(tex, pos, rect, clr * 0.5f, Projectile.rotation + rot2, orig, scale, SpriteEffects.None);
                Main.EntitySpriteDraw(tex, pos, rect, clr, Projectile.rotation + rot, orig, scale, SpriteEffects.None);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
        }

        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Junko_Cloth");
        readonly Texture2D tailTex = AltVanillaFunction.GetExtraTexture("Junko_Tail");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTail();

            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
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
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int tailFrame, tailFrameCounter;
        private void Wrath()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (extraAI[0] == 0)
            {
                if (++Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 5;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(48, 72))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 2400;
                    PetState = 0;
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
        private void Idel()
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
        public override Color ChatTextColor => new Color(254, 159, 75);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Junko";
            indexRange = new Vector2(1, 3);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 1000;
            chance = 10;
            whenShouldStop = PetState > 1;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (!Main.dayTime && Main.cloudAlpha <= 0 && Main.GetMoonPhase() == MoonPhase.Full)
                {
                    chat.Add(ChatDictionary[1]);
                }
                chat.Add(ChatDictionary[2]);
                if (FindPet(ProjectileType<Reisen>()))
                {
                    chat.Add(ChatDictionary[3]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(3))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Reisen>();
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
            Projectile junko = chatRoom.initiator;
            Projectile reisen = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //纯狐：乌冬酱~最近还好嘛？
                reisen.CloseCurrentDialog();

                if (junko.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //铃仙：嗯嗯...还、还好吧...
                reisen.SetChat(ChatSettingConfig, 11, 20);

                if (reisen.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
            if (PetState != 2)
                Idel();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.38f + 5 * auraValue, 1.41f + 5 * auraValue, 2.55f + 5 * auraValue);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<JunkoBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(45 * player.direction, -65 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.018f;

            ChangeDir();
            MoveToPoint(point, 17f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 450 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 0)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Wrath();
            }
            if (PetState == 2)
            {
                auraValue += 0.08f;
            }
            else
            {
                auraValue -= 0.01f;
            }
            auraValue = MathHelper.Clamp(auraValue, 0, 1);
        }
    }
}


