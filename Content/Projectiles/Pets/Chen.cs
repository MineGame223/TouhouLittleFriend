using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Chen : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawChen(tailFrame, lightColor, 1);
            DrawChen(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawChen(blinkFrame, lightColor, 1);
            DrawChen(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Chen_Cloth"), true);
            DrawChen(clothFrame, lightColor, 1, default, null, true);
            return false;
        }
        private void DrawChen(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
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
            if (blinkFrame < 8)
            {
                blinkFrame = 8;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = 8;
                PetState = 0;
            }
        }
        int tailFrame, tailFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void UpdateTailFrame()
        {
            if (tailFrame < 4)
            {
                tailFrame = 4;
            }
            int count = 6;
            if (++tailFrameCounter > count)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 7)
            {
                tailFrame = 4;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
        private void Meow()
        {
            int count = 7;
            if (Projectile.frame == 6)
                count = 18;

            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame == 6 && Projectile.frameCounter == 0)
            {
                AltVanillaFunction.PlaySound(SoundID.Meowmere, Projectile.Center);
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 0;
                PetState = 0;
                Projectile.netUpdate = true;
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
        Color myColor = new Color(89, 196, 108);
        public override string GetChatText(out string[] text)
        {
            text = new string[3];
            text[1] = ModUtils.GetChatText("Chen", "1");
            text[2] = ModUtils.GetChatText("Chen", "2");
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
            int type1 = ProjectileType<Ran>();
            if (FindChatIndex(out Projectile _, type1, 2, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, type1, 4))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Chen", "3"), myColor, 0, 360);
                p.localAI[2] = 0;
            }
            else if (mainTimer % 640 == 0 && Main.rand.NextBool(7) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
            UpdateClothFrame();
            if (PetState < 2)
                Idel();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YukariBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-120 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;

            ChangeDir(player, true, 130);
            MoveToPoint(point, 17f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
                    {
                        extraAI[0] = 1200;
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Meow();
            }
        }
    }
}


