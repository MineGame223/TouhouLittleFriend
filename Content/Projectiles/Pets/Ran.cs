using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Ran : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Ran_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            Projectile.DrawPet(tailFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, 
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            return false;
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
        int tailFrame, tailFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void UpdateTailFrame()
        {
            int count = 6;
            if (++tailFrameCounter > count)
            {
                tailFrameCounter = 0;
                tailFrame++;
            }
            if (tailFrame > 3)
            {
                tailFrame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 4)
            {
                clothFrame = 4;
            }
            int count = 5;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 7)
            {
                clothFrame = 4;
            }
        }
        Color myColor = new Color(254, 216, 82);
        public override string GetChatText(out string[] text)
        {
            text = new string[5];
            text[1] = ModUtils.GetChatText("Ran", "1");
            text[2] = ModUtils.GetChatText("Ran", "2");
            text[3] = ModUtils.GetChatText("Ran", "3");
            text[4] = ModUtils.GetChatText("Ran", "4");
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
            int type1 = ProjectileType<Yukari>();
            if (FindChatIndex(out Projectile _, type1, 2, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, type1, 2))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Ran", "5"), myColor, 5, 600);
            }
            else if (FindChatIndex(out Projectile p1, type1, 6, default, 1, true))
            {
                SetChatWithOtherOne(p1, ModUtils.GetChatText("Ran", "6"), myColor, 0, 360);
                p1.localAI[2] = 0;
            }
            else if (FindChatIndex(out Projectile p2, type1, 5, default, 1, true))
            {
                SetChatWithOtherOne(p2, ModUtils.GetChatText("Ran", "7"), myColor, 0, 600);
                p2.localAI[2] = 0;
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(8) && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateTailFrame();
            UpdateClothFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<YukariBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-60 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.015f;

            ChangeDir(player, true);
            MoveToPoint(point, 18f);
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
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
        }
    }
}


