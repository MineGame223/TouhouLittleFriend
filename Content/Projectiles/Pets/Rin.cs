using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Rin : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(3);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Rin_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };

            Projectile.DrawPet(swingFrame + 8, lightColor, drawConfig, 1);
            Projectile.DrawPet(swingFrame + 4, lightColor, drawConfig, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(swingFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(swingFrame + 4, lightColor, config, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(swingFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, Color.White * 0.8f, drawConfig, 2);
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 10;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int swingFrame, swingFrameCounter;
        private void Playing()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 3;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[2] = 0;
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    PetState = 0;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (++swingFrameCounter > 6)
            {
                swingFrameCounter = 0;
                swingFrame++;
            }
            if (swingFrame > 3)
            {
                swingFrame = 0;
            }
        }
        Color myColor = new Color(227, 59, 59);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[11];
            text[1] = ModUtils.GetChatText("Rin", "1");
            text[2] = ModUtils.GetChatText("Rin", "2");
            text[3] = ModUtils.GetChatText("Rin", "3");
            if (player.ZoneUnderworldHeight)
            {
                text[4] = ModUtils.GetChatText("Rin", "4");
            }
            if (player.ownedProjectileCounts[ProjectileType<Satori>()] > 0)
            {
                text[5] = ModUtils.GetChatText("Rin", "5");
                text[6] = ModUtils.GetChatText("Rin", "6");
            }
            if (player.ownedProjectileCounts[ProjectileType<Utsuho>()] > 0)
            {
                text[7] = ModUtils.GetChatText("Rin", "7");
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
            if (mainTimer % 640 == 0 && Main.rand.NextBool(6) && PetState != 2)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        private void GenDust(Player player)
        {
            int dustID = MyDustId.CyanBubble;

            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(-28, 8), dustID
                , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 1.5f));
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cPet, player);

            d = Dust.NewDustPerfect(Projectile.Center + new Vector2(28, 8), dustID
                , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2.5f, -1.2f)), 100, default
                , Main.rand.NextFloat(0.5f, 1.5f));
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cPet, player);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<RinBuff>());
            Projectile.SetPetActive(player, BuffType<KomeijiBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            if (player.HasBuff<KomeijiBuff>())
                point = new Vector2(-70 * player.direction, 0 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir(player, true);
            MoveToPoint(point, 12.5f);

            GenDust(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState != 1)
                {
                    if (mainTimer % 120 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
                    {
                        extraAI[2] = Main.rand.Next(20, 45);
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
                Projectile.frame = 0;
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Playing();
            }
        }
    }
}


