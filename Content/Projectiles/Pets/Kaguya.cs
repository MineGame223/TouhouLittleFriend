using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Danmaku;
using static TouhouPets.DanmakuFightHelper;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kaguya : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 21;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Fighting)
            {
                DrawDanmakuRing();
            }
            Projectile.DrawStateNormalizeForPet();
            DrawKaguya(hairFrame, lightColor, 1, new Vector2(extraX, extraY));
            DrawKaguya(Projectile.frame, lightColor, 0);
            if (PetState == 1)
                DrawKaguya(blinkFrame, lightColor, 1);
            DrawKaguya(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Kaguya_Cloth"), true);
            DrawKaguya(clothFrame, lightColor, 1, default, null, true);           
            Projectile.DrawStateNormalizeForPet();
            if (Projectile.owner == Main.myPlayer)
            {
                DrawFightState();
            }
            return false;
        }
        private void DrawFightState()
        {
            if (Fighting)
            {
                Main.instance.DrawHealthBar(Projectile.Center.X, Projectile.position.Y + Projectile.height + 10
                , extraAI[2], 360, 0.8f);
            }
            if (PetState < 0)
            {
                if (PetState < -1)
                {
                    string source = "Win: " + PlayerA_Source.ToString();
                    Vector2 pos = new Vector2(Projectile.Center.X - FontAssets.MouseText.Value.MeasureString(source).X / 2, Projectile.Center.Y + 36) - Main.screenPosition;
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, source
                        , pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero, 1f);
                }
                if (PetState == -1 && extraAI[0] <= 0)
                {
                    DrawBattleRound();
                }
            }
        }
        private void DrawDanmakuRing()
        {
            Texture2D t = AltVanillaFunction.ExtraTexture(ExtrasID.CultistRitual);
            Main.instance.LoadProjectile(ProjectileID.CultistRitual);
            Texture2D t2 = AltVanillaFunction.ProjectileTexture(ProjectileID.CultistRitual);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.LightGoldenrodYellow * 0.7f * Main.essScale) * ringAlpha;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(t, pos, rect, clr, -Main.GlobalTimeWrappedHourly * 0.6f, orig, Projectile.scale * (Main.essScale / 2), effect, 0f);
            rect = new Rectangle(0, 0, t2.Width, t2.Height);
            orig = rect.Size() / 2;
            Main.EntitySpriteDraw(t2, pos, rect, clr, Main.GlobalTimeWrappedHourly, orig, Projectile.scale * (Main.essScale / 2), effect, 0f);
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
                PetState = 0;
            }
        }
        private void Idle()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        float extraX, extraY;
        float floatingX, floatingY;
        float ringAlpha;
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 5)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 6)
            {
                clothFrame = 3;
            }

            if (hairFrame < 7)
            {
                hairFrame = 7;
            }
            if (++hairFrameCounter > 5)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }
        }
        private void PlayingGame()
        {
            Projectile.velocity *= 0.3f;
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] < 999)
            {
                if (extraAI[2] == 0)
                {
                    if (Projectile.frame > 9)
                    {
                        Projectile.frame = 8;
                        extraAI[1]++;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            if (Main.rand.NextBool(8))
                            {
                                extraAI[2] = 1;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
                else
                {
                    if (Projectile.frame > 11)
                    {
                        Projectile.frame = 10;
                        extraAI[1]++;
                        extraAI[2]++;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            if (extraAI[2] > Main.rand.Next(2, 5))
                            {
                                extraAI[2] = 0;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
                if (extraAI[1] > 0 && extraAI[1] % 36 == 0 && Main.rand.NextBool(10))
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "4"), 4);
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "5"), 5);
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "3"), 3);
                            break;
                    }
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[0])
                    {
                        extraAI[1] = 0;
                        extraAI[0] = Main.rand.NextBool(10) ? 999 : Main.rand.Next(36, 54);
                        Projectile.netUpdate = true;
                    }
                }
                if (Projectile.velocity.Length() > 7.5f)
                {
                    extraAI[0] = 999;
                }
            }
            else
            {
                if (Projectile.frame > 12)
                {
                    extraAI[2] = 0;
                    Projectile.frame = 0;
                    extraAI[0] = 600;
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
            Projectile.spriteDirection = -1;
            if (extraAI[0] == 0)
            {
                floatingX = 0;
                floatingY = 0;

                Idle();
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
                    RoundTimer = extraAI[1];
                }
            }
            else if (extraAI[0] == 1)
            {
                speed = 3f;
                hairFrameCounter++;
                if (Projectile.frame < 13)
                {
                    Projectile.frame = 13;
                }
                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
                if (Projectile.frame >= 15)
                {
                    Projectile.frame = 15;
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
                    if (extraAI[1] % 20 == 0)
                    {
                        if (Main.rand.NextBool(25) && player.ownedProjectileCounts[ProjectileType<KaguyaWave>()] < 1)
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                                Vector2.Zero
                                , ProjectileType<KaguyaWave>(), Main.rand.Next(12, 20), 0, Projectile.owner);
                        }
                        else
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -50).RotatedByRandom(MathHelper.ToRadians(360)),
                            new Vector2(-Main.rand.Next(5, 9), 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-9, 9)))
                            , ProjectileType<KaguyaBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner, Main.rand.Next(0, 5));
                        }
                    }
                }
                Projectile.HandleHurt(ref extraAI[2]);
                if (Projectile.owner == Main.myPlayer)
                {
                    if (FindPetState(out _, ProjectileType<Moku>(), -3))
                    {
                        PlayerA_Source++;
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
            Vector2 point = new Vector2(200 + floatingX * player.direction, -200 + floatingY);
            MoveToPoint2(point, speed);
        }
        private void Win()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = -1;
            if (Projectile.frame < 17)
            {
                Projectile.frame = 17;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 18)
            {
                Projectile.frame = 17;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[0] == 0)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "-1"));
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "-2"));
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "-3"));
                            break;
                    }
                }
                if (++extraAI[0] > 480 || FindPetState(out _, ProjectileType<Moku>(), -1))
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
            Projectile.spriteDirection = -1;
            if (Projectile.frame < 19)
            {
                Projectile.frame = 19;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 20)
            {
                Projectile.frame = 19;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[0] == 0)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "-4"));
                            break;
                        case 2:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "-5"));
                            break;
                        default:
                            SetChat(myColor, ModUtils.GetChatText("Kaguya", "-6"));
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
        Color myColor = new Color(255, 165, 191);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Kaguya", "1");
            text[2] = ModUtils.GetChatText("Kaguya", "2");
            if (player.HasBuff<MokuBuff>())
            {
                text[7] = ModUtils.GetChatText("Kaguya", "7");
            }
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
            if (ChatIndex >= 3 && ChatIndex <= 5)
            {
                if (mainTimer % 72 == 0)
                {
                    ChatIndex = 0;
                }
            }
            int type1 = ProjectileType<Moku>();
            if (FindChatIndex(out Projectile _, type1, 6, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p, type1, 6, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Kaguya", "6"), myColor, 0, 360);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p1, type1, 3, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Kaguya", "8"), myColor, 8, 600);
            }
            else if (FindChatIndex(out Projectile p2, type1, 4, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Kaguya", "9"), myColor, 9, 600);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(2) && PetState < 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState != 2 && PetState >= 0)
                Idle();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            Projectile.SetPetActive(player, BuffType<KaguyaBuff>());

            if (PetState >= 0)
            {
                UpdateTalking();
                Vector2 point = new Vector2(-60 * player.direction, -30 + player.gfxOffY);
                ChangeDir(player, true);
                MoveToPoint(point, 15f);
            }

            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), 34), MyDustId.YellowFx
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1f, -0.2f)), 100, default
                , Main.rand.NextFloat(0.75f, 1.5f)).noGravity = true;

            if (Projectile.owner == Main.myPlayer)
            {
                if (player.afkCounter >= 600 && player.ownedProjectileCounts[ProjectileType<Moku>()] > 0 && PetState >= 0)
                {
                    if (mainTimer % 60 == 0 && Main.rand.NextBool(2)
                        || FindPetState(out _, ProjectileType<Moku>(), -1))
                    {
                        extraAI[0] = 0;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        PetState = -1;
                        Projectile.netUpdate = true;
                    }
                }
                if (PetState >= 0)
                {
                    if (mainTimer % 270 == 0 && PetState != 2)
                    {
                        PetState = 1;
                        Projectile.netUpdate = true;
                    }
                    if (mainTimer % 360 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0 && Projectile.velocity.Length() < 2f)
                    {
                        extraAI[0] = Main.rand.Next(36, 54);
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
                PlayingGame();
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
            if (player.afkCounter <= 0 && PetState < 0)
            {
                PetState = 0;
            }
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 8 && Projectile.frame <= 11 || Projectile.frame == 15 || Projectile.frame == 18)
            {
                extraX = -2 * Projectile.spriteDirection;
            }
            if (Projectile.frame == 18)
            {
                extraY = -2;
            }
            if (Projectile.frame == 20)
            {
                extraY = 2;
            }
            ringAlpha = MathHelper.Clamp(ringAlpha += 0.05f * (Fighting ? 1 : -1), 0, 1);
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


