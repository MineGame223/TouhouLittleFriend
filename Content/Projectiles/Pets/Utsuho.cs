using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Utsuho : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Utsuho_Cloth");
        readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Utsuho_Glow");
        readonly Texture2D eyeTex = AltVanillaFunction.GetGlowTexture("Utsuho_Glow_Eye");
        readonly Texture2D sunTex = AltVanillaFunction.GetExtraTexture("UtsuhoSun");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawSun();

            Projectile.DrawPet(wingFrame, lightColor, config);
            Projectile.DrawPet(wingFrame, Color.White * 0.7f,
                config with
                {
                    AltTexture = glowTex,
                });
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawStateNormalizeForPet();

            DrawEye();
            return false;
        }
        private void DrawEye()
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = eyeTex,
            };
            for (int i = -1; i <= 1; i++)
            {
                Vector2 eyePos = new Vector2(0f, 2 * i).RotatedBy(MathHelper.PiOver2 * i);
                Projectile.DrawPet(Projectile.frame, Color.White * 0.3f,
                    config with
                    {
                        PositionOffset = eyePos.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly),
                    });
            }
            Projectile.DrawPet(Projectile.frame, Color.White, config);
        }
        private void DrawSun()
        {
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(sunTex, pos + sunPos + new Vector2(Main.rand.Next(-10, 11) * 0.2f, Main.rand.Next(-10, 11) * 0.2f), null, Projectile.GetAlpha(Color.White) * 0.5f, -mainTimer * 0.09f, sunTex.Size() / 2, Projectile.scale * 1.02f, SpriteEffects.None, 0f);
            }
            Main.EntitySpriteDraw(sunTex, pos + sunPos, null, Projectile.GetAlpha(Color.White), mainTimer * 0.05f, sunTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
        }
        private void Blink()
        {
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        Vector2 sunPos;
        private void Fire()
        {
            if (extraAI[1] > 0)
            {
                extraAI[1]--;
            }
            if (Projectile.frame == 3 && Projectile.frameCounter >= 12)
            {
                Projectile.frame = 4;
                extraAI[1] = 60;
            }
            if (extraAI[1] > 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(26 * Projectile.spriteDirection, 6), MyDustId.OrangeFire2, new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-3, 3) * 0.75f), 100, default, Main.rand.NextFloat(0.5f, 1.25f)).noGravity = true;
            }

            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            if (extraAI[1] <= 0)
                Projectile.frameCounter++;

            if (Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 5 && Projectile.frameCounter == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 center = Projectile.Center;
                    Vector2 vel = (new Vector2(6 * Projectile.spriteDirection, -4) * Main.rand.NextFloat(0.5f, 2f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30)));
                    Dust.NewDustDirect(center, 4, 4, MyDustId.Smoke, vel.X, vel.Y, 100, default, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
                    vel = (new Vector2(6 * Projectile.spriteDirection, -4) * 2f * Main.rand.NextFloat(0.5f, 2f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10)));
                    Dust.NewDustDirect(center, 4, 4, MyDustId.OrangeFire2, vel.X, vel.Y, 100, default, Main.rand.NextFloat(1.4f, 1.9f)).noGravity = true;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.velocity.X = 9 * -Projectile.spriteDirection;
                    Projectile.velocity.Y = 4;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.frame >= 5)
            {
                Projectile.velocity *= 0.85f;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
                extraAI[1] = 0;
                extraAI[0] = 600;
                PetState = 0;
            }
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 7)
            {
                wingFrame = 7;
            }
            if (++wingFrameCounter > 6)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 10)
            {
                wingFrame = 7;
            }
        }
        Color myColor = new Color(228, 184, 75);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            text[1] = ModUtils.GetChatText("Utsuho", "1");
            text[2] = ModUtils.GetChatText("Utsuho", "2");
            text[3] = ModUtils.GetChatText("Utsuho", "3");
            if (player.ZoneUnderworldHeight)
            {
                text[4] = ModUtils.GetChatText("Utsuho", "4");
            }
            if (player.ownedProjectileCounts[ProjectileType<Satori>()] > 0)
            {
                text[5] = ModUtils.GetChatText("Utsuho", "5");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 4)
                        {
                            weight = 3;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type3 = ProjectileType<Rin>();
            if (FindChatIndex(out Projectile _, type3, 5, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type3, 7, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Utsuho", "6"), myColor, 6, 600);
                p1.localAI[2] = 0;
            }
            if (mainTimer % 720 == 0 && Main.rand.NextBool(7))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
        }
        public override void AI()
        {
            sunPos = new Vector2(0, -40);

            Lighting.AddLight(Projectile.Center + sunPos, 1.95f, 1.64f, 0.67f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<UtsuhoBuff>());
            Projectile.SetPetActive(player, BuffType<KomeijiBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(0, -60 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Rin>()] > 0)
            {
                point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
                if (player.HasBuff<KomeijiBuff>())
                    point = new Vector2(70 * player.direction, 0 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (Projectile.frame < 5)
            {
                ChangeDir(player);
            }

            if (Projectile.frame < 5)
                MoveToPoint(point, 13.5f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 300 == 0 && Main.rand.NextBool(7) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
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
                Blink();
            }
            else if (PetState == 2)
            {
                Fire();
            }
            if (extraAI[0] >= 480)
            {
                if (Main.rand.NextBool(4))
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(24 * Projectile.spriteDirection, 20), MyDustId.Smoke, new Vector2(0, Main.rand.Next(-4, -2) * 0.5f), 100, default, Main.rand.NextFloat(0.5f, 1.25f)).noGravity = true;
            }
        }
    }
}


