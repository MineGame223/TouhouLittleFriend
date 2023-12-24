using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Moku : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawWings(Projectile.GetAlpha(Color.White) * 0.3f);
            Projectile.DrawStateNormalizeForPet();

            DrawKaguya(hairFrame, lightColor, 1);
            DrawKaguya(hairFrame, lightColor, 1, default, AltVanillaFunction.GetExtraTexture("Moku_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();

            DrawKaguya(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 3)
                DrawKaguya(blinkFrame, lightColor, 1);
            DrawKaguya(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Moku_Cloth"), true);
            return false;
        }
        private void DrawWings(Color lightColor)
        {
            for (int i = 0; i < 7; i++)
            {
                DrawKaguya(wingFrame, lightColor, 1, new Vector2(Main.rand.NextFloat(-1f, 1f)), null, true);
            }
        }
        private void DrawKaguya(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            if (extraPos == default)
                extraPos = Vector2.Zero;
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale) + extraPos;
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
                PetState = PetState == 3 ? 2 : 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int wingFrame, wingFrameCounter;
        int hairFrame, hairFrameCounter;
        private void UpdateMiscFrame()
        {
            if (wingFrame < 9)
            {
                wingFrame = 9;
            }
            if (++wingFrameCounter > 5)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 14)
            {
                wingFrame = 9;
            }

            if (hairFrame < 3)
            {
                hairFrame = 3;
            }
            if (++hairFrameCounter > 6)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 8)
            {
                hairFrame = 3;
            }
        }
        private void Fire()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] < 1)
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 5;
                    for (int i = 0; i < 6; i++)
                        Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.spriteDirection == -1 ? -24 : 14, 6)
                                , 1, 1, Main.rand.NextBool(2) ? MyDustId.Fire : MyDustId.YellowGoldenFire,
                                Main.rand.Next(-4, 4), Main.rand.Next(-16, -8)
                                , 100, Color.White, Main.rand.NextFloat(1.3f, 2.2f)).noGravity = true;
                    if (Main.rand.NextBool(64))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Projectile.spriteDirection == -1 ? -24 : 14, 6),
                            new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-5, -2)), ProjectileID.GreekFire1, 0, 0, Projectile.owner);
                    }

                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[0]++;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 900;
                    PetState = 0;
                }
            }
        }
        Color myColor = new Color(200, 200, 200);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Moku", "1");
            text[2] = ModUtils.GetChatText("Moku", "2");
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
            if (mainTimer % 720 == 0 && Main.rand.NextBool(9))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.15f, 1.84f, 0.87f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MokuBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(70 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;
            ChangeDir(player);

            MoveToPoint(point, 15f);

            if (Main.rand.NextBool(7))
                Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), MyDustId.Fire
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState <= 2)
                {
                    if (PetState == 2)
                    {
                        PetState = 3;
                    }
                    else
                    {
                        PetState = 1;
                    }
                    Projectile.netUpdate = true;
                }
                if (mainTimer % 720 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0 && PetState < 2)
                {
                    extraAI[2] = Main.rand.Next(240, 480);
                    PetState = 2;
                    Projectile.netUpdate = true;
                }
            }
            if (PetState == 0)
            {
                Projectile.frame = 0;
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Projectile.frame = 0;
                Blink();
            }
            else if (PetState == 2 || PetState == 3)
            {
                Fire();
                if (PetState == 3)
                    Blink();
            }
        }
    }
}


