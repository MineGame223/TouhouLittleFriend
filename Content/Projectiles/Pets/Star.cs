using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Star : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Star_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 extraPos = new Vector2(extraX, extraY);
            DrawPetConfig config = drawConfig with
            {
                PositionOffset = extraPos,
            };

            Projectile.DrawPet(wingsFrame, lightColor * 0.7f, config, 1);

            Projectile.DrawPet(hairFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawPet(clothFrame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
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
        int wingsFrame, wingsFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        float extraX, extraY;
        private void StarMagic()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                }

                if (extraAI[1] % 2 == 0)
                {
                    Projectile starTrail = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position,
                        Vector2.Zero, ProjectileType<StarTrail>(), 0, 0, Projectile.owner
                        , Main.rand.Next(30, 60), Main.rand.Next(50, 100 + extraAI[1]), Main.rand.Next(0, 360));
                    starTrail.localAI[2] = Projectile.whoAmI;
                    starTrail.netUpdate = true;
                }

                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(120, 180))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 5)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 10800;
                    PetState = 0;
                }
            }
        }
        private void UpdateExtraPos()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 5)
            {
                extraY = -2;
                if (Projectile.frame >= 2 && Projectile.frame <= 4)
                {
                    extraX = -2 * Projectile.spriteDirection;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 8)
            {
                wingsFrame = 8;
            }
            if (++wingsFrameCounter > 5)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 11)
            {
                wingsFrame = 8;
            }

            if (hairFrame < 4)
            {
                hairFrame = 4;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 7)
            {
                hairFrame = 4;
            }

            if (++clothFrameCounter > 8)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
        Color myColor = new Color(135, 143, 237);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Star", "1");
            text[2] = ModUtils.GetChatText("Star", "2");
            text[3] = ModUtils.GetChatText("Star", "3");
            if (Main.dayTime)
                text[4] = ModUtils.GetChatText("Star", "4");
            else if (player.ZoneForest && Main.cloudAlpha == 0 && !Main.bloodMoon)
                text[5] = ModUtils.GetChatText("Star", "5");

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
            int type = ProjectileType<Sunny>();
            int type2 = ProjectileType<Luna>();
            if (FindChatIndex(out Projectile _, type, 12, 14, 0)
                || FindChatIndex(out Projectile _, type2, 9, 10, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p1, type, 12, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Star", "6"), myColor, 6);
                p1.localAI[2] = 0;
                p1.localAI[1] = 4800;
            }
            else if (FindChatIndex(out Projectile p2, type, 14, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Star", "7"), myColor, 7);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(7) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState < 2)
            {
                Projectile.frame = 0;
            }
        }
        private void GenDust()
        {
            int dustID = MyDustId.BlueMagic;
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            if (Projectile.velocity.Length() > 4)
            {
                ParticleOrchestraSettings settings = new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)),
                    MovementVector = Vector2.Zero,
                };
                if (Main.rand.NextBool(3))
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
                //if (Main.rand.NextBool(6))
                //Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), Vector2.Normalize(Projectile.velocity) * -2, Main.rand.Next(16, 18), Main.rand.NextFloat(0.9f, 1.1f));
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(player, true, 200);

            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 1) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 7.5f);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.35f, 1.43f, 2.37f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<StarBuff>());
            Projectile.SetPetActive(player, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();
            ControlMovement(player);
            GenDust();

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && !Main.dayTime
                        && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.cloudAlpha == 0 && !Main.bloodMoon)
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
                StarMagic();
            }
            UpdateExtraPos();
        }
    }
}


