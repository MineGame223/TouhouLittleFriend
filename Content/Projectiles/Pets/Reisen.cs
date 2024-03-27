using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Reisen : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawReisen(hairFrame, lightColor, 1);
            DrawReisen(earFrame, lightColor);
            DrawReisen(Projectile.frame, lightColor);
            DrawReisen(Projectile.frame, Projectile.GetAlpha(Color.White * 0.6f), 0, AltVanillaFunction.GetGlowTexture("Reisen_Glow"));
            if (PetState == 1)
            {
                DrawReisen(blinkFrame, lightColor, 1);
                DrawReisen(blinkFrame, Projectile.GetAlpha(Color.White * 0.6f), 1, AltVanillaFunction.GetGlowTexture("Reisen_Glow"));
            }
            DrawReisen(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Reisen_Cloth"), true);
            DrawReisen(clothFrame, lightColor, 1, null, true);
            return false;
        }
        private void DrawReisen(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
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
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 2)
            {
                blinkFrame = 0;
                PetState = 0;
            }
        }
        int earFrame, earFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            int count = 6;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }

            if (PetState < 3)
            {
                if (earFrame < 7)
                {
                    earFrame = 7;
                }
                count = 6;
                if (++earFrameCounter > count)
                {
                    earFrameCounter = 0;
                    earFrame++;
                }
                if (earFrame > 10)
                {
                    earFrame = 7;
                }
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            count = 6;
            if (++hairFrameCounter > count)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }
        }
        private void Shooting()
        {
            int count = 7;

            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame >= 2 && extraAI[1] > 0)
                {
                    Projectile.frame = 2;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]--;
                    if (extraAI[1] <= 0)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                PetState = 0;
                extraAI[0] = 900;
            }
        }
        private void Nerves()
        {
            earFrame = 11;
            extraAI[1]++;
            if (extraAI[0] == 0)
            {
                Projectile.frame = 5;
                if (extraAI[1] > 270)
                {
                    Projectile.frame = 6;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[1] > 3)
            {
                extraAI[1] = 0;
                extraAI[0] = 0;
                Projectile.frame = 5;
            }
        }
        Color myColor = new Color(255, 10, 10);
        public override string GetChatText(out string[] text)
        {
            text = new string[20];
            text[1] = ModUtils.GetChatText("Reisen", "1");
            text[2] = ModUtils.GetChatText("Reisen", "2");
            text[3] = ModUtils.GetChatText("Reisen", "3");
            text[4] = ModUtils.GetChatText("Reisen", "4");
            text[5] = ModUtils.GetChatText("Reisen", "5");
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (mainTimer % 600 == 0 && Main.rand.NextBool(8))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState < 2)
                Projectile.frame = 0;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<ReisenBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.014f;

            ChangeDir(player, true);
            MoveToPoint(point, 20f);
            if (Projectile.owner == Main.myPlayer && PetState == 0)
            {
                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(5) && extraAI[0] <= 0)
                    {
                        extraAI[1] = Main.rand.Next(300, 600);
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
                Shooting();
            }
            else if (PetState == 3)
            {
                Nerves();
            }
            if (PetState <= 1 && player.ownedProjectileCounts[ProjectileType<Junko>()] > 0)
            {
                PetState = 3;
            }
            if (PetState == 3 && player.ownedProjectileCounts[ProjectileType<Junko>()] <= 0)
            {
                PetState = 0;
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1] = 0;
                    extraAI[0] = 0;
                    Projectile.netUpdate = true;
                }
            }
        }
    }
}


