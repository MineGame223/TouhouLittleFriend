using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Luna : BasicTouhouPetNeo
    {
        private static bool IsFullMoon => Main.GetMoonPhase() == MoonPhase.Full
            && !Main.dayTime && !Main.bloodMoon;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Luna_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(wingsFrame, lightColor * 0.7f, drawConfig);

            Projectile.DrawPet(12, lightColor, drawConfig);
            Projectile.DrawPet(12, lightColor, config);
            Projectile.DrawStateNormalizeForPet();

            if (PetState > 0)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        private void Blink(bool alt = false)
        {
            int startFrame = alt ? 5 : 4;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            int count = PetState == 4 ? 9 : 3;
            if (++blinkFrameCounter > count)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (PetState == 4)
            {
                if (blinkFrame > 8)
                {
                    if (Projectile.frame <= 6 && Projectile.frame > 0)
                        blinkFrame = 7;
                    else
                    {
                        if (extraAI[0] == 1)
                        {
                            if (extraAI[1] < 120)
                            {
                                blinkFrame = 10;
                                extraAI[1]++;
                                return;
                            }
                            if (blinkFrame > 10)
                            {
                                blinkFrame = 9;
                                extraAI[1]++;
                            }
                            if (extraAI[1] > 123)
                            {
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    blinkFrame = startFrame;
                                    extraAI[1] = 0;
                                    extraAI[0]++;
                                    Projectile.netUpdate = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (blinkFrame > 6)
                {
                    blinkFrame = startFrame;
                    PetState = alt ? 2 : 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int wingsFrame, wingsFrameCounter;
        int clothFrame, clothFrameCounter;
        private void ReadingPaper()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 3;
                }
                extraAI[1]++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(3200, 7200) || Projectile.velocity.Length() > 4.5f)
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 1200;
                    PetState = 0;
                }
            }
        }
        private void Yawn()
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }
            int count = 8;
            if (Projectile.frame == 6)
            {
                count = 180;
            }
            if (extraAI[0] == 0)
            {
                if (++Projectile.frameCounter > count)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 0;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                Projectile.frame = 0;
            }
            else
            {
                extraAI[0] = 1200;
                PetState = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingsFrame < 8)
            {
                wingsFrame = 8;
            }
            if (++wingsFrameCounter > 6)
            {
                wingsFrameCounter = 0;
                wingsFrame++;
            }
            if (wingsFrame > 11)
            {
                wingsFrame = 8;
            }

            if (++clothFrameCounter > 6)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
        public override Color ChatTextColor => new Color(255, 225, 110);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Luna";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;
            chance = 8;
            whenShouldStop = PetState == 4;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (PetState <= 1)
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                }
                else
                {
                    chat.Add(ChatDictionary[6]);
                    chat.Add(ChatDictionary[7]);
                }
                if (IsFullMoon)
                {
                    chat.Add(ChatDictionary[8]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState < 2)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateTalking()
        {
        }
        private void GenDust()
        {
            int dustID = MyDustId.WhiteTransparent;
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                + new Vector2(Main.rand.NextFloat(-30f, -10f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, Color.Yellow
                , Main.rand.NextFloat(0.5f, 0.9f)).noGravity = true;
            }
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center
                    + new Vector2(Main.rand.NextFloat(20f, 22f) * Projectile.spriteDirection, Main.rand.NextFloat(-5f, 25f)), dustID
                    , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.2f)), 100, Color.LightGoldenrodYellow
                    , Main.rand.NextFloat(0.5f, 0.9f)).noGravity = true;
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(true, 200);

            Vector2 point = new Vector2(50 * player.direction, -40 + player.gfxOffY);
            if (player.HasBuff<TheThreeFairiesBuff>())
            {
                point = new Vector2(60 * player.direction, -70 + player.gfxOffY);
                point += new Vector2(0, -40).RotatedBy(MathHelper.ToRadians(360 / 3 * 2) + Main.GlobalTimeWrappedHourly);
            }
            MoveToPoint(point, 7.5f);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.52f, 1.50f, 1.15f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<LunaBuff>());
            Projectile.SetPetActive(player, BuffType<TheThreeFairiesBuff>());

            UpdateTalking();
            ControlMovement(player);
            GenDust();

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 2)
                    {
                        PetState = 3;
                    }
                    else if (PetState == 0)
                    {
                        PetState = 1;
                    }
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState == 0)
                {
                    if (Main.dayTime)
                    {
                        if (mainTimer % 600 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && chatTimeLeft <= 0)
                        {
                            PetState = 4;
                            int chance = Main.rand.Next(2);
                            switch (chance)
                            {
                                case 1:
                                    Projectile.SetChat(ChatSettingConfig, 4);
                                    break;
                                default:
                                    Projectile.SetChat(ChatSettingConfig, 5);
                                    break;
                            }
                            Projectile.netUpdate = true;
                        }
                    }
                    else if (Projectile.velocity.Length() < 2f)
                    {
                        if (mainTimer % 120 == 0 && Main.rand.NextBool(12) && extraAI[0] <= 0)
                        {
                            if (blinkFrame < 5)
                                blinkFrame = 5;
                            PetState = 2;
                            Projectile.netUpdate = true;
                        }
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
            else if (PetState == 2 || PetState == 3)
            {
                ReadingPaper();
                if (PetState == 3)
                    Blink(true);
            }
            else if (PetState == 4)
            {
                Yawn();
                Blink();
            }
        }
    }
}


