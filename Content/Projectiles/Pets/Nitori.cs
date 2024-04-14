using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Nitori : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Nitori_Cloth");
        readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Nitori_Glow");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(backFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(backFrame, Color.White * 0.7f,
                drawConfig with
                {
                    AltTexture = glowTex,
                }, 1);

            Projectile.DrawPet(backFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 13)
            {
                blinkFrame = 13;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 15)
            {
                blinkFrame = 13;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int backFrame, backFrameCounter;
        private void UpdateBackFrame()
        {
            if (++backFrameCounter > 4)
            {
                backFrameCounter = 0;
                backFrame++;
            }
            if (backFrame > 3)
            {
                backFrame = 0;
            }
        }
        private void Breakdown()
        {
            if (extraAI[0] == 0)
            {
                if (++backFrameCounter > 4)
                {
                    backFrameCounter = 0;
                    backFrame++;
                }
                if (backFrame > 7)
                {
                    backFrame = 4;
                    extraAI[1]++;
                }
                if (Main.rand.NextBool(8 - extraAI[1]))
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(-6 * Projectile.spriteDirection, -8)
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-6, -3) * 0.75f)
                        , 100, default, Main.rand.NextFloat(1.5f, 2.25f)).noGravity = true;
                }
                if (extraAI[1] > 6)
                {
                    extraAI[1] = 4;
                    extraAI[1] = 0;
                    extraAI[0]++;
                }
            }
            else if (extraAI[0] == 1)
            {
                if (++backFrameCounter > 4)
                {
                    backFrameCounter = 0;
                    backFrame++;
                }
                if (backFrame > 7)
                {
                    backFrame = 4;
                }
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 4;
                    extraAI[1]++;
                }
                if (Main.rand.NextBool(4 - extraAI[1]))
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(-6 * Projectile.spriteDirection, -8)
                        , MyDustId.Smoke
                        , new Vector2(Main.rand.Next(-3, 3) * 0.75f, Main.rand.Next(-6, -3) * 0.75f)
                        , 100, Color.Black, Main.rand.NextFloat(1.5f, 2.25f)).noGravity = true;
                    if (Main.rand.NextBool(2))
                    {
                        Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                            , Projectile.Center + new Vector2(0, -24)
                            , new Vector2(Main.rand.Next(-3, 3) * 0.15f, Main.rand.Next(-3, -1) * 0.15f)
                            , Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.25f, 0.5f));
                    }
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > 2)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 2)
            {
                backFrame = 8;
                if (extraAI[1] == 0)
                {
                    for (int i = 0; i < 25; i++)
                        Dust.NewDustPerfect(Projectile.Center
                            , MyDustId.Smoke
                            , new Vector2(Main.rand.Next(-6, 6) * 0.75f, Main.rand.Next(-6, 6) * 0.75f)
                            , 20, Color.Black, Main.rand.NextFloat(2.5f, 4.25f)).noGravity = true;
                    for (int i = 0; i < 10; i++)
                        Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                            , Projectile.Center + new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, 8))
                            , new Vector2(Main.rand.Next(-3, 3) * 0.15f, Main.rand.Next(-3, 3) * 0.15f)
                            , Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1), Main.rand.NextFloat(0.75f, 1.25f));
                    AltVanillaFunction.PlaySound(SoundID.Item14, Projectile.position);
                }
                else if (Main.rand.NextBool(2))
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(-8, 8))
                            , MyDustId.Smoke
                            , new Vector2(Main.rand.Next(-1, 1) * 0.75f, Main.rand.Next(-4, -2) * 0.75f)
                            , 90, Color.Black, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                }
                extraAI[1]++;
                if (extraAI[1] > 120 && extraAI[1] <= 144)
                    Projectile.frame = extraAI[1] % 6 == 0 ? 8 : 9;
                else
                    Projectile.frame = 8;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > 240)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 3)
            {
                if (Main.rand.NextBool(3))
                {
                    for (int i = 0; i < 4; i++)
                        Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-14, 14), Main.rand.Next(-8, 8))
                            , MyDustId.Smoke
                            , new Vector2(Main.rand.Next(-6, 6) * 0.85f, Main.rand.Next(-6, 6) * 0.85f)
                            , 90, Color.Black, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                }
                int maxTime = 6;
                backFrame = 9;
                extraAI[1]++;
                if (extraAI[1] <= maxTime * 6)
                    Projectile.frame = extraAI[1] % maxTime == 0 ? 10 : 11;
                else
                    Projectile.frame = 12;
                if (extraAI[1] > maxTime * 6 + 6)
                {
                    extraAI[1] = 0;
                    extraAI[0] = 2400;
                    PetState = 0;
                }
            }
        }
        private void Idel()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        public override Color ChatTextColor => new Color(74, 165, 255);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Nitori";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 9;
            whenShouldStop = PetState == 2;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            if (PetState != 2)
            {
                Idel();
                UpdateBackFrame();
            }
            else if (extraAI[0] == 0)
                Idel();
        }
        private void UpdateTalking()
        {
        }
        public override void AI()
        {
            if (backFrame != 8)
            {
                Lighting.AddLight(Projectile.Center + new Vector2(0, 20), 1.95f, 1.64f, 0.67f);
            }
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<NitoriBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(60 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            if (PetState == 2 && extraAI[0] == 2)
            {
                Projectile.rotation = 0;
            }

            ChangeDir();
            MoveToPoint(point, 12.5f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState < 1)
                {
                    if (mainTimer % 900 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0 && chatTimeLeft <= 0)
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
                Breakdown();
            }
        }
    }
}


