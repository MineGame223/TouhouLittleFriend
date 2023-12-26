using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Koakuma : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            TouhouPetPlayer lp = Main.LocalPlayer.GetModPlayer<TouhouPetPlayer>();
            Projectile.Name = Language.GetTextValue("Mods.TouhouPets.Projectiles.Koakuma.DisplayName", NumberToCNCharacter.GetNumberText(lp.koakumaNumber));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawKoakuma(hairFrame, lightColor, 0);
            DrawKoakuma(wingFrame, lightColor, 1);
            DrawKoakuma(Projectile.frame, lightColor, 0);
            DrawKoakuma(earFrame, lightColor, 1);
            if (PetState == 1)
                DrawKoakuma(blinkFrame, lightColor, 0);
            DrawKoakuma(Projectile.frame, lightColor, 0, AltVanillaFunction.GetExtraTexture("Koakuma_Cloth"), true);
            return false;
        }
        private void DrawKoakuma(int frame, Color lightColor, int columns = 0, Texture2D tex = null, bool entitySpriteDraw = false)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
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
            if (blinkFrame < 4)
            {
                blinkFrame = 4;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 6)
            {
                blinkFrame = 4;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int earFrame, earFrameCounter;
        int hairFrame, hairFrameCounter;
        bool EarActive
        {
            get => Projectile.ai[2] == 0;
            set => Projectile.ai[2] = value ? 0 : 1;
        }
        private void UpdateWingFrame()
        {
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > 5)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 9)
            {
                wingFrame = 4;
            }
        }
        private void UpdateHairFrame()
        {
            if (hairFrame < 6)
            {
                hairFrame = 6;
            }
            if (++hairFrameCounter > 7)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 9)
            {
                hairFrame = 6;
            }
        }
        private void UpdateEarsFrame()
        {
            if (++earFrameCounter > 5 && EarActive)
            {
                earFrameCounter = 0;
                earFrame++;
            }
            if (earFrame > 3)
            {
                earFrame = 0;
                EarActive = false;
            }
        }
        Color myColor = new Color(224, 78, 78);
        public override string GetChatText(out string[] text)
        {
            TouhouPetPlayer lp = Main.player[Projectile.owner].GetModPlayer<TouhouPetPlayer>();
            text = new string[11];
            text[1] = ModUtils.GetChatText("Koakuma", "1", NumberToCNCharacter.GetNumberText(lp.koakumaNumber));
            text[2] = ModUtils.GetChatText("Koakuma", "2");
            text[3] = ModUtils.GetChatText("Koakuma", "3");
            if (FindPetState(out _, ProjectileType<Patchouli>(), 0, 1))
            {
                text[4] = ModUtils.GetChatText("Koakuma", "4");
            }
            if (FindPetState(out _, ProjectileType<Patchouli>(), 2))
            {
                text[6] = ModUtils.GetChatText("Koakuma", "6");
            }
            WeightedRandom<string> chat = new();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (i == 4 || i == 6)
                            weight = 4;
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            int type = ProjectileType<Patchouli>();
            if (FindChatIndex(out Projectile p1, type, 16, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Koakuma", "5"), myColor, 5, 360);
            }
            else if (FindChatIndex(out Projectile p2, type, 19, 35, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Koakuma", "7"), myColor, 0, 360);
                p2.localAI[2] = 0;
            }
            else if (mainTimer % 666 == 0 && Main.rand.NextBool(6) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateEarsFrame();
            UpdateHairFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<KoakumaBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.022f;

            ChangeDir(player, true);
            MoveToPoint(point, 9f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    if (Main.rand.NextBool(4))
                        EarActive = true;
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


