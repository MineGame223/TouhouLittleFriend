using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Hina : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 17;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHina(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawHina(blinkFrame, lightColor);
            DrawHina(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Hina_Cloth"), true);
            return false;
        }
        private void DrawHina(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private void Blink()
        {
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                PetState = 0;
            }
        }
        private void Idle()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        private void Turning()
        {
            if (Projectile.frame < 6)
            {
                Projectile.frame = 6;
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 13)
                {
                    Projectile.frame = 6;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(10, 20))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 13)
                {
                    extraAI[2] = 0;
                    Projectile.frame = 0;
                    extraAI[0] = 300;
                    PetState = 0;
                }
            }
        }
        Color myColor = new Color(70, 226, 164);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Hina", "1");
            text[2] = ModUtils.GetChatText("Hina", "2");
            if (player.HasBuff<NitoriBuff>())
            {
                text[4] = ModUtils.GetChatText("Hina", "4");
                text[7] = ModUtils.GetChatText("Hina", "7");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i >= 4)
                        {
                            weight = 10;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Nitori>();
            if (FindChatIndex(out Projectile p, type1, 4, ignoreCD: true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Hina", "5"), myColor, 5);
            }
            else if (FindChatIndex(out p, type1, 5, ignoreCD: true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Hina", "6"), myColor, 6);
            }
            else if (FindChatIndex(out p, type1, 7, ignoreCD: true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Hina", "8"), myColor, 0);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(7))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            if (PetState != 2)
                Idle();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<HinaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-40 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;
            if (PetState != 2)
            {
                ChangeDir(player, true);
            }

            MoveToPoint(point, 13f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 200 == 0 && Main.rand.NextBool(2) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                        if (Main.rand.NextBool(3) && ChatCD <= 0)
                            SetChat(myColor, ModUtils.GetChatText("Hina", "3"), 3, 60, 30);
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
                Turning();
            }
        }
    }
}


