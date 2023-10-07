using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Youmu : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawYoumu(8, Color.White * 0.5f, 1, null, true
                    , new Vector2(Main.rand.Next(-10, 11) * 0.15f, Main.rand.Next(-10, 11) * 0.15f));
            }
            Projectile.DrawStateNormalizeForPet();
            if (Projectile.frame != 4)
                DrawYoumu(10, lightColor);
            DrawYoumu(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 4)
                DrawYoumu(blinkFrame, lightColor);

            DrawYoumu(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Youmu_Cloth"), true);
            DrawYoumu(clothFrame, lightColor, 1, null, true);
            DrawYoumu(clothFrame + 4, lightColor, 1, null, true);
            Projectile.DrawStateNormalizeForPet();
            if (Projectile.frame >= 1 && Projectile.frame <= 4)
            {
                DrawYoumu(handFrame, lightColor);
                DrawYoumu(handFrame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Youmu_Cloth"), true);
            }
            return false;
        }
        private void DrawYoumu(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false, Vector2 posAdjForGhost = default)
        {
            if (posAdjForGhost == default)
                posAdjForGhost = Vector2.Zero;
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale) + shake + posAdjForGhost;
            Rectangle rect = new Rectangle(t.Width / 2 * columns, frame * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Blink()
        {
            int startFrame = (PetState == 4 ? 6 : 5);
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = 5;
                PetState = (PetState == 4 ? 3 : 0);
            }
        }
        int handFrame;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        Vector2 shake;
        private void Scared()
        {
            extraAI[1]++;
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (extraAI[1] % 180 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 1;
            }
            handFrame = 8;
            shake = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0);
        }
        private void UpdateClothFrame()
        {
            int count = 4;
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
        Color myColor = new Color(188, 248, 248);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Youmu", "1");
            text[2] = ModUtils.GetChatText("Youmu", "2");
            if (PetState == 3)
                text[3] = ModUtils.GetChatText("Youmu", "3");
            if (PetState == 2)
            {
                text[4] = ModUtils.GetChatText("Youmu", "4");
                text[5] = ModUtils.GetChatText("Youmu", "5");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (PetState == 2 && i <= 3)
                        {
                            weight = 0;
                        }
                        if (PetState >= 3 && i != 3)
                        {
                            weight = 0;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Yuyuko>();
            if (FindChatIndex(out Projectile _, type1, 1, default, 0)
                || FindChatIndex(out Projectile _, type1, 2, default, 0)
                  || FindChatIndex(out Projectile _, type1, 4, default, 0)
                  || FindChatIndex(out Projectile _, type1, 8, default, 0)
                  || FindChatIndex(out Projectile _, type1, 9, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p, type1, 9))
            {
                if (PetState < 2)
                    SetChatWithOtherOne(p, ModUtils.GetChatText("Youmu", "6"), myColor, 6, 600);
            }
            else if (FindChatIndex(out Projectile p1, type1, 10, default, 1, true))
            {
                if (PetState < 2)
                    SetChatWithOtherOne(p1, ModUtils.GetChatText("Youmu", "7"), myColor, 0, 600);
                p1.ai[0] = 0;
            }
            else if (FindChatIndex(out Projectile p2, type1, 4)
                || FindChatIndex(out p2, type1, 8))
            {
                if (PetState < 2)
                    SetChatWithOtherOne(p2, ModUtils.GetChatText("Youmu", "8"), myColor, 0, 360);
                p2.ai[0] = 0;
            }
            else if (FindChatIndex(out Projectile p3, type1, 1))
            {
                if (PetState < 2)
                    SetChatWithOtherOne(p3, ModUtils.GetChatText("Youmu", "9"), myColor, 0, 360);
                p3.ai[0] = 0;
            }
            else if (FindChatIndex(out Projectile p4, type1, 2))
            {
                if (PetState < 2)
                    SetChatWithOtherOne(p4, ModUtils.GetChatText("Youmu", "10"), myColor, 0, 360);
                p4.ai[0] = 0;
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(12))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void AI()
        {
            float lightPlus = 0.7f;
            Lighting.AddLight(Projectile.Center, 1.83f * lightPlus, 2.02f * lightPlus, 1.99f * lightPlus);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YoumuBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(54 * player.direction, -34 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Yuyuko>()] > 0)
            {
                point = new Vector2(-50 * player.direction, -50 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.001f;

            ChangeDir(player, player.ownedProjectileCounts[ProjectileType<Yuyuko>()] > 0);
            MoveToPoint(point, 15f * (PetState == 2 ? 0.5f : 1));
            bool hasBoss = false;
            if (player.active && !player.dead)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && (n.target == player.whoAmI || Vector2.Distance(n.Center, player.Center) <= 1280))
                    {
                        if (n.boss)
                        {
                            hasBoss = true;
                            Projectile.spriteDirection = (n.position.X > Projectile.position.X) ? 1 : -1;
                        }
                    }
                }
            }
            if (hasBoss)
            {
                PetState = 3;
            }
            else
            {
                if (PetState == 3)
                {
                    PetState = 0;
                }
                if (player.ZoneGraveyard)
                {
                    if (PetState != 2 && extraAI[1] > 0)
                        extraAI[1] = 0;
                    PetState = 2;
                }
                else if (PetState == 2)
                {
                    PetState = 0;
                    extraAI[1] = 0;
                }
            }

            shake = Vector2.Zero;
            if (mainTimer % 270 == 0)
            {
                if (PetState < 2)
                    PetState = 1;
                else if (PetState == 3)
                    PetState = 4;
            }
            if (PetState == 0)
            {
                Projectile.frame = 0;
            }
            else if (PetState == 1)
            {
                Projectile.frame = 0;
                Blink();
            }
            else if (PetState == 2)
            {
                Scared();
            }
            else if (PetState == 3 || PetState == 4)
            {
                Projectile.frame = 4;
                handFrame = 9;
                if (PetState == 4)
                {
                    Blink();
                }
            }
        }
    }
}


