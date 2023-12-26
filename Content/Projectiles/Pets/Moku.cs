using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Danmaku;
using static TouhouPets.DanmakuFightHelper;

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
            if (Fighting)
            {
                DrawDanmakuRing();
            }
            DrawWings(Projectile.GetAlpha(Color.White) * 0.3f);
            Projectile.DrawStateNormalizeForPet();

            DrawMoku(hairFrame, lightColor, 1, new Vector2(extraX, extraY));
            DrawMoku(hairFrame, lightColor, 1, new Vector2(extraX, extraY), AltVanillaFunction.GetExtraTexture("Moku_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();

            DrawMoku(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 3)
                DrawMoku(blinkFrame, lightColor, 1);
            DrawMoku(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Moku_Cloth"), true);
            Projectile.DrawStateNormalizeForPet();
            if (Projectile.owner == Main.myPlayer && PetState < 0)
            {
                DrawFightState();
            }
            return false;
        }
        private void DrawFightState()
        {
            if (Fighting)
            {
                Main.instance.DrawHealthBar(Projectile.Center.X, Projectile.position.Y + Projectile.height + 16
                    , extraAI[2], 360, 0.8f);
            }
            if (PetState < -1)
            {
                string source = "Win: " + PlayerB_Source.ToString();
                Vector2 pos = new Vector2(Projectile.Center.X - FontAssets.MouseText.Value.MeasureString(source).X / 2, Projectile.Center.Y + 36) - Main.screenPosition;
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, source
                    , pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
            }
        }
        private void DrawWings(Color lightColor)
        {
            for (int i = 0; i < 7; i++)
            {
                DrawMoku(wingFrame, lightColor, 1, new Vector2(Main.rand.NextFloat(-1f, 1f)) + new Vector2(extraX, extraY), null, true);
            }
        }
        private void DrawDanmakuRing()
        {
            Main.spriteBatch.QuickToggleAdditiveMode(true);
            Main.instance.LoadFlameRing();
            Texture2D t = TextureAssets.FlameRing.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height / 3);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.White) * ringAlpha;
            float scale = Projectile.scale * DanmakuRingScale;
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(t, pos + new Vector2(Main.rand.NextFloat(-1.3f, 1.3f)), rect, clr * 0.34f, Main.GlobalTimeWrappedHourly, orig, scale * 0.65f, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(Main.rand.NextFloat(-1.3f, 1.3f)), rect, clr * 0.3f, -Main.GlobalTimeWrappedHourly, orig, scale * 0.5f, SpriteEffects.FlipHorizontally, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false);
        }
        private void DrawMoku(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
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

        float extraX, extraY;
        float floatingX, floatingY;
        float ringAlpha;
        int[] abilityCD;
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
        private bool Fighting => PetState == -1 && extraAI[0] > 0;
        private void Battle()
        {
            float speed = 15f;
            chatFuncIsOccupied = true;
            Player player = Main.player[Projectile.owner];
            Projectile.spriteDirection = 1;
            if (extraAI[0] == 0)
            {
                floatingX = 0;
                floatingY = 0;

                Projectile.frame = 0;
                if (extraAI[1] == 0)
                {
                    Round++;
                }
                extraAI[1]++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > 360)
                    {
                        extraAI[2] = 360;
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                speed = 4f;
                Projectile.velocity *= 0.5f;
                hairFrameCounter += 2;
                if (Projectile.frame < 7)
                {
                    Projectile.frame = 7;
                }
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 8;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] >= 3600)
                    {
                        extraAI[1] = 0;
                    }
                    if (extraAI[1] % 120 == 0)
                    {
                        floatingX = Main.rand.Next(-50, 50);
                        floatingY = Main.rand.Next(-50, 50);
                        Projectile.netUpdate = true;
                    }
                    if (extraAI[1] % (30 * MathHelper.Clamp(extraAI[2] / 360, 0.5f, 1)) == 0)
                    {
                        if (Main.rand.NextBool(30) && abilityCD[0] <= 0
                            && player.ownedProjectileCounts[ProjectileType<MokuFireball>()] < 1)
                        {
                            abilityCD[0] = 180;
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                                new Vector2(Main.rand.Next(7, 9), 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-3, 3)))
                                , ProjectileType<MokuFireball>(), Main.rand.Next(12, 20), 0, Projectile.owner);
                        }
                        else if (Main.rand.NextBool(25) && abilityCD[1] <= 0)
                        {
                            abilityCD[1] = 180;
                            for (int i = -3; i <= 3; i++)
                            {
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                                    new Vector2(5, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(5, 6) * i))
                                    , ProjectileType<MokuBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner);
                            }
                        }
                        else
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Main.rand.Next(-50, -50)).RotatedByRandom(MathHelper.ToRadians(360)),
                                new Vector2(Main.rand.Next(4, 8), 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-9, 9)))
                                , ProjectileType<MokuBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner);
                        }
                    }
                }
                Projectile.HandleHurt(ref extraAI[2], false);
                if (Projectile.owner == Main.myPlayer)
                {
                    if (FindPetState(out _, ProjectileType<Kaguya>(), -3))
                    {
                        PlayerB_Source++;
                        CombatText.NewText(Projectile.getRect(), Color.Yellow, "WIN!", true, false);
                        PetState = -2;
                        extraAI[0] = 0;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                    else if (extraAI[2] <= 0)
                    {
                        Projectile.FailEffect();
                        CombatText.NewText(Projectile.getRect(), Color.Gray, "lose...", true, false);
                        PetState = -3;
                        extraAI[0] = 0;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            Vector2 point = new Vector2(-200 + floatingX * player.direction, -200 + floatingY);
            MoveToPoint2(point, speed);
        }
        private void Win()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = 1;
            if (Projectile.frame < 10)
            {
                Projectile.frame = 10;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 10;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[0] == 0)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Moku", "-1"));
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Moku", "-2"));
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Moku", "-3"));
                            break;
                    }
                }
                if (++extraAI[0] > 480 || FindPetState(out _, ProjectileType<Kaguya>(), -1))
                {
                    extraAI[0] = 0;
                    PetState = -1;
                    Projectile.netUpdate = true;
                }
            }
        }
        private void Lose()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = 1;
            if (Projectile.frame < 12)
            {
                Projectile.frame = 12;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 12;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[0] == 0)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Moku", "-4"));
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Moku", "-5"));
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Moku", "-6"));
                            break;
                    }
                }
                if (++extraAI[0] > 480 || FindPetState(out _, ProjectileType<Kaguya>(), -1))
                {
                    extraAI[0] = 0;
                    PetState = -1;
                    Projectile.netUpdate = true;
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
            int type1 = ProjectileType<Kaguya>();
            if (FindChatIndex(out Projectile _, type1, 3, 5, 0)
                || FindChatIndex(out Projectile _, type1, 7, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p1, type1, 7))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Moku", "3"), myColor, 3, 600);
            }
            else if (FindChatIndex(out Projectile p2, type1, 8, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Moku", "4"), myColor, 4, 600);
            }
            else if (FindChatIndex(out Projectile p3, type1, 9, default, 1, true))
            {
                SetChatWithOtherOne(p3, ModUtils.GetChatText("Moku", "5"), myColor, 0, 360);
                p3.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p, type1, 3, 5))
            {
                if (mainTimer % 64 == 0 && Main.rand.NextBool(5) && FindPetState(out _, ProjectileType<Kaguya>(), 2))
                    SetChatWithOtherOne(p, ModUtils.GetChatText("Moku", "6"), myColor, 6, 360);
            }
            else if (mainTimer % 960 == 0 && Main.rand.NextBool(2))
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        private void UpdateMiscData()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 7 && Projectile.frame <= 8)
            {
                extraX = 2 * Projectile.spriteDirection;
            }
            if (Projectile.frame == 11)
            {
                extraY = -2;
            }
            if (Projectile.frame == 13 || Projectile.frame == 8)
            {
                extraY = 2;
            }
            ringAlpha = MathHelper.Clamp(ringAlpha += 0.05f * (Fighting ? 1 : -1), 0, 1);
            if (!Fighting)
            {
                abilityCD[0] = 0;
                abilityCD[1] = 0;
            }
            else
            {
                if (abilityCD[0] > 0)
                    abilityCD[0]--;
                if (abilityCD[1] > 0)
                    abilityCD[1]--;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            abilityCD = new int[2];
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.15f, 1.84f, 0.87f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MokuBuff>());
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;
            if (PetState >= 0)
            {
                UpdateTalking();
                Vector2 point = new Vector2(70 * player.direction, -30 + player.gfxOffY);
                ChangeDir(player);
                MoveToPoint(point, 15f);
            }

            if (Main.rand.NextBool(7))
                Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), MyDustId.Fire
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;

            if (Projectile.owner == Main.myPlayer)
            {
                if (player.afkCounter >= 600 && PetState >= 0)
                {
                    if (mainTimer % 60 == 0 && Main.rand.NextBool(2)
                        || FindPetState(out _, ProjectileType<Kaguya>(), -1))
                    {
                        InitializeFightData();
                        extraAI[0] = 0;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        PetState = -1;
                        Projectile.netUpdate = true;
                    }
                }
                if (PetState >= 0)
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
            else if (PetState == -1)
            {
                Battle();
            }
            else if (PetState == -2)
            {
                Win();
            }
            else if (PetState == -3)
            {
                Lose();
            }
            if (PetState < 0 && (player.afkCounter <= 0
                || !player.HasBuff<KaguyaBuff>() || player.ownedProjectileCounts[ProjectileType<Kaguya>()] < 0))
            {
                PetState = 0;
            }
            UpdateMiscData();
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            floatingX = reader.ReadSingle();
            floatingY = reader.ReadSingle();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(floatingX);
            writer.Write(floatingY);
        }
    }
}


