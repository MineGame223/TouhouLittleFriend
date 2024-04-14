using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kogasa : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 20;
            Main.projPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Kogasa_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            int eyeFramePlus = Projectile.spriteDirection == -1 ? 0 : 3;

            Projectile.DrawPet(umbrellaFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(umbrellaFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1 || PetState == 4)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
            if (PetState == 0 || PetState == 3)
                Projectile.DrawPet(14 + eyeFramePlus, lightColor, drawConfig);
            if (Projectile.frame == 1 || Projectile.frame == 4)
                Projectile.DrawPet(15 + eyeFramePlus, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config);
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(handFrame, lightColor, drawConfig);
            Projectile.DrawPet(handFrame, lightColor, config);
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                if (PetState == 4)
                    PetState = 3;
                else
                    PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int handFrame, handFrameCounter;
        int clothFrame, clothFrameCounter;
        int umbrellaFrame, umbrellaFrameCounter;
        private void UpdateUmbrellaFrame()
        {
            if (PetState >= 5)
            {
                umbrellaFrame = 14;
                return;
            }
            if (++umbrellaFrameCounter > 5)
            {
                umbrellaFrameCounter = 0;
                umbrellaFrame++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (PetState == 3 || PetState == 4)
                    {
                        extraAI[1]++;
                    }
                }
            }
            if (PetState <= 2)
            {
                if (umbrellaFrame > 6)
                {
                    umbrellaFrame = 0;
                }
            }
            else
            {
                if (umbrellaFrame < 7)
                {
                    umbrellaFrame = 7;
                }
                if (umbrellaFrame > 13)
                {
                    umbrellaFrame = 7;
                }
            }
        }
        private void UpdateHandFrame()
        {
            if (PetState >= 5)
            {
                handFrame = 13;
                return;
            }
            if (PetState <= 1)
            {
                handFrame = 10;
            }
            else if (PetState == 3 || PetState == 4)
            {
                if (handFrame < 11)
                {
                    handFrame = 11;
                }
                if (++handFrameCounter > 5)
                {
                    handFrameCounter = 0;
                    handFrame++;
                }
                if (handFrame > 12)
                {
                    handFrame = 11;
                }
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 6)
            {
                clothFrame = 6;
            }
            if (++clothFrameCounter > 5)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 9)
            {
                clothFrame = 6;
            }
        }
        private void Scare()
        {
            if (Projectile.frame < 2)
            {
                Projectile.frame = 2;
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.owner == Main.myPlayer)
                    extraAI[1]++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 2;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (extraAI[0] == 1)
            {
                if (Projectile.frame > 4)
                {
                    extraAI[1] = 0;
                    extraAI[2] = 0;
                    extraAI[0] = 1200;
                    Projectile.frame = 0;
                    PetState = 0;
                }
            }
        }
        private void SpingUmbrella()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (extraAI[1] > extraAI[2])
                {
                    if (umbrellaFrame == 7)
                    {
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        extraAI[0] = 1200;
                        PetState = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        public override Color ChatTextColor => new Color(172, 69, 191);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Kogasa";
            indexRange = new Vector2(1, 2);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 888;
            chance = 8;
            whenShouldStop = PetState == 5;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateUmbrellaFrame();
            UpdateHandFrame();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<KogasaBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();
            MoveToPoint(point, 12.5f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState == 0)
                        PetState = 1;
                    else if (PetState == 3)
                        PetState = 4;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState < 1)
                {
                    if (mainTimer % 120 == 0 && Main.rand.NextBool(2) && extraAI[0] <= 0)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            PetState = 3;
                            extraAI[2] = Main.rand.Next(30, 40);
                        }
                        else
                        {
                            PetState = 2;
                            extraAI[2] = Main.rand.Next(14, 20);
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (player.AnyBosses())
            {
                PetState = 5;
            }
            if (PetState == 5)
            {
                Projectile.frame = 5;
                if (!player.AnyBosses())
                    PetState = 0;
                return;
            }
            if (PetState == 0)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
                Projectile.frame = 0;
            }
            else if (PetState == 1 || PetState == 4)
            {
                Projectile.frame = 0;
                Blink();
            }
            else if (PetState == 2)
            {
                Scare();
            }
            else if (PetState == 3 || PetState == 4)
            {
                Projectile.frame = 0;
                SpingUmbrella();
            }
        }
    }
}


