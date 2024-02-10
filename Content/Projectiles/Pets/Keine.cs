using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Keine : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawKeine(auraFrame, Color.White, null, true);
            Projectile.DrawStateNormalizeForPet();

            DrawKeine(hairFrame, lightColor);
            DrawKeine(Projectile.frame, lightColor);

            DrawKeine(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Keine_Cloth"), true);
            DrawKeine(clothFrame, lightColor, null, true);
            return false;
        }
        private void DrawKeine(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            int width = t.Width / 2;
            Rectangle rect = new Rectangle(width * (UseAlternatePhase ? 1 : 0), frame * height, width, height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (entitySpriteDraw)
                Main.EntitySpriteDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
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
                if (PetState == 3)
                {
                    PetState = 2;
                }
                else
                {
                    PetState = 0;
                }
            }
        }
        int hairFrame, hairFrameCounter;
        int clothFrame, clothFrameCounter;
        int auraFrame, auraFrameCounter;
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 4)
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
            if (++hairFrameCounter > 4)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }

            if (auraFrame < 11)
            {
                auraFrame = 11;
            }
            if (++auraFrameCounter > 4)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 14)
            {
                auraFrame = 11;
            }
        }
        Color myColor => UseAlternatePhase ? new Color(69, 172, 105) : new Color(97, 103, 255);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            text[1] = ModUtils.GetChatText("Keine", "1");
            text[2] = ModUtils.GetChatText("Keine", "2");
            text[3] = ModUtils.GetChatText("Keine", "3");
            if (UseAlternatePhase)
            {
                text[4] = ModUtils.GetChatText("Keine", "4");
                text[5] = ModUtils.GetChatText("Keine", "5");
                text[6] = ModUtils.GetChatText("Keine", "6");
            }
            if (player.HasBuff<MokuBuff>())
            {
                text[7] = ModUtils.GetChatText("Keine", "7");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;

                        if (i <= 3 && UseAlternatePhase)
                            weight = 0;

                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Moku>();
            if (FindChatIndex(out _, type1, 9, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, type1, 9))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Keine", "9"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, type1, 7, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Keine", "8"), myColor, 8);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(9) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        internal static bool UseAlternatePhase => Main.bloodMoon || (Main.GetMoonPhase() == MoonPhase.Full && !Main.dayTime);
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.007f;

            ChangeDir(player, true);

            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            MoveToPoint(point, 12f);
        }
        private void Transform()
        {
            if (PetState == 0 && UseAlternatePhase)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.RedBubble, 0, 0
                        , 100, default, Main.rand.NextFloat(1.7f, 2.5f)).noGravity = true;
                }
                PetState = 2;
            }
            if (PetState == 2 && !UseAlternatePhase)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.BlueMagic, 0, 0
                        , 100, default, Main.rand.NextFloat(1.7f, 2.5f)).noGravity = true;
                }
                PetState = 0;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<KeineBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
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
            }
            if (PetState == 0 || PetState == 2)
            {
                Projectile.frame = 0;
            }
            else if (PetState == 1 || PetState == 3)
            {
                Blink();
            }
            Transform();
        }
    }
}


