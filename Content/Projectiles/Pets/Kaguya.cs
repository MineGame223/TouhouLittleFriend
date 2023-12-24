using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

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
            DrawKaguya(hairFrame, lightColor, 1, new Vector2(extraX, extraY));
            DrawKaguya(Projectile.frame, lightColor, 0, new Vector2(extraX, extraY));
            if (PetState == 1)
                DrawKaguya(blinkFrame, lightColor, 1, new Vector2(extraX, extraY));
            DrawKaguya(Projectile.frame, lightColor, 0, new Vector2(extraX, extraY), AltVanillaFunction.GetExtraTexture("Kaguya_Cloth"), true);
            DrawKaguya(clothFrame, lightColor, 1, new Vector2(extraX, extraY), null, true);
            return false;
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
        Color myColor = new Color(255, 165, 191);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Kaguya", "1");
            text[2] = ModUtils.GetChatText("Kaguya", "2");
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
            if (PetState != 2)
                Idle();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<KaguyaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-60 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;
            ChangeDir(player, true);

            MoveToPoint(point, 15f);

            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), 28), MyDustId.YellowFx
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1f, -0.2f)), 100, default
                , Main.rand.NextFloat(0.75f, 1.5f)).noGravity = true;

            if (Projectile.owner == Main.myPlayer)
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
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 8 && Projectile.frame <= 11)
            {
                extraX = 2;
            }
        }
    }
}


