using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Daiyousei : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawDaiyousei(wingFrame, lightColor);
            DrawDaiyousei(Projectile.frame, lightColor);
            DrawDaiyousei(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Daiyousei_Cloth"), true);
            return false;
        }
        private void DrawDaiyousei(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, frame * height, t.Width, height);
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
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        private void UpdateWingFrame()
        {
            if (wingFrame < 3)
            {
                wingFrame = 3;
            }
            if (++wingFrameCounter > 4)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 6)
            {
                wingFrame = 3;
            }
        }
        Color myColor = new Color(71, 228, 63);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            text[1] = ModUtils.GetChatText("Daiyousei", "1");
            text[2] = ModUtils.GetChatText("Daiyousei", "2");
            if (player.ownedProjectileCounts[ProjectileType<Cirno>()] > 0)
            {
                text[3] = ModUtils.GetChatText("Daiyousei", "3");
                text[4] = ModUtils.GetChatText("Daiyousei", "4");
            }
            if (player.ZoneGraveyard)
            {
                text[5] = ModUtils.GetChatText("Daiyousei", "5");
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 5)
                        {
                            weight = 5;
                        }
                        if (i == 3 || i == 4)
                        {
                            weight = 5;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type1 = ProjectileType<Cirno>();
            if (FindChatIndex(out Projectile _, type1, 4, default, 0)
                || FindChatIndex(out Projectile _, type1, 7, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p, type1, 4))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Daiyousei", "7"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p1, type1, 7))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Daiyousei", "6"), myColor, 6);
            }
            else if (FindChatIndex(out Projectile p2, type1, 9, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Daiyousei", "8"), myColor, 0);
                p2.localAI[2] = 0;
            }
            if (mainTimer % 960 == 0 && Main.rand.NextBool(9) && mainTimer > 0)
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
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<DaiyouseiBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-40 * player.direction, -30 + player.gfxOffY);
            if (player.ownedProjectileCounts[ProjectileType<Cirno>()] > 0)
            {
                point = new Vector2(80 * player.direction, -30 + player.gfxOffY);
            }
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;

            ChangeDir(player, player.ownedProjectileCounts[ProjectileType<Cirno>()] <= 0);
            MoveToPoint(point, 9f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
            }
            if (PetState == 0)
            {
                Projectile.frame = 0;
            }
            else if (PetState == 1)
            {
                Blink();
            }
        }
    }
}


