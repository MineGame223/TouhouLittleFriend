using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sakuya : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sakuya_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame,lightColor,drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor, config);
            Projectile.DrawStateNormalizeForPet();

            if (PetState == 3)
            {
                DrawUmbrella(lightColor);
            }
            return false;
        }
        private void DrawUmbrella(Color lightColor)
        {
            int type = ItemID.TragicUmbrella;
            Main.instance.LoadItem(type);
            Texture2D tex = AltVanillaFunction.ItemTexture(type);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(13 * Projectile.spriteDirection, -20) + new Vector2(0, 7f * Main.essScale);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = tex.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, effect, 0);
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
        int clothFrame, clothFrameCounter;
        private void Serve(bool alt = false)
        {
            if (alt)
            {
                Projectile.frame = 9;
                if (Main.player[Projectile.owner].ownedProjectileCounts[ProjectileType<Remilia>()] <= 0
                    || !Remilia.HateSunlight(Projectile))
                {
                    Projectile.frame = 0;
                    PetState = 0;
                }
                return;
            }
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.rand.NextBool(15) && extraAI[1] <= 0)
                    {
                        SetChat(myColor, ModUtils.GetChatText("Sakuya", "5"), 5);
                        extraAI[1]++;
                    }
                }
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 1;
            }
            if (!FindPetState(out _, ProjectileType<Remilia>(), 2))
            {
                extraAI[1] = 0;
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        private void WakeUp()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (FindPet(out Projectile meirin, ProjectileType<Meirin>()))
            {
                Projectile.spriteDirection = Projectile.position.X > meirin.position.X ? -1 : 1;
                if (Projectile.frameCounter == 2 && Projectile.frame == 6)
                {
                    Vector2 vel = Vector2.Normalize(meirin.Center - Projectile.Center) * 4f;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<SakuyaKnife>()
                        , 0, 0, Projectile.owner);
                }
            }
            if (Projectile.frame > 8)
            {
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        private void Teleport(Vector2 targetPos)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.BlueMagic, 0, 0
                    , 100, default, Main.rand.NextFloat(1.5f, 2.2f)).noGravity = true;
            }
            Projectile.Center = targetPos;
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, MyDustId.BlueMagic, 0, 0
                    , 100, default, Main.rand.NextFloat(1.5f, 2.2f)).noGravity = true;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 13)
            {
                clothFrame = 13;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 16)
            {
                clothFrame = 13;
            }
        }
        Color myColor = new Color(114, 106, 255);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Sakuya", "1");
            text[2] = ModUtils.GetChatText("Sakuya", "2");
            text[3] = ModUtils.GetChatText("Sakuya", "3");
            if (player.ownedProjectileCounts[ProjectileType<Meirin>()] > 0)
            {
                text[4] = ModUtils.GetChatText("Sakuya", "4");
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
            int remilia = ProjectileType<Remilia>();
            int meirin = ProjectileType<Meirin>();

            Player player = Main.player[Projectile.owner];
            bool chance = Main.rand.NextBool(player.HasBuff<ScarletBuff>() ? 30 : 12);

            if (FindChatIndex(out Projectile _, meirin, 13, default, 0)
                || FindChatIndex(out Projectile _, meirin, 10, default, 0))
            {
                ChatCD = 1;
            }

            if (FindChatIndex(out Projectile p, remilia, 14, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Sakuya", "10"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, remilia, 14, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Sakuya", "10"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, meirin, 10))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Sakuya", "6"), myColor, 6);
            }
            else if (FindChatIndex(out p, meirin, 11, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Sakuya", "7"), myColor, 7);
            }
            else if (FindChatIndex(out p, meirin, 13))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Sakuya", "8"), myColor, 8);
            }
            else if (FindChatIndex(out p, meirin, 14, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Sakuya", "9"), myColor, 9);
            }
            else if (mainTimer % 960 == 0 && chance && PetState <= 1 && mainTimer > 0)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            if (PetState <= 1)
                Projectile.rotation = Projectile.velocity.X * 0.01f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.001f;

            ChangeDir(player, true, 120);

            Vector2 point = new Vector2((player.HasBuff<ScarletBuff>() ? 100 : -50) * player.direction, -30 + player.gfxOffY);
            Vector2 center = default;
            float speed = 16f;
            if (FindPet(out Projectile master, ProjectileType<Remilia>()))
            {
                Projectile.spriteDirection = master.spriteDirection;
                if (PetState == 3 || PetState == 2)
                {
                    point = new Vector2(((PetState == 2 ? -40 : -20) + 60) * master.spriteDirection, player.gfxOffY - 20);
                    speed = 19f;
                }
            }
            MoveToPoint(point, speed, center);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SakuyaBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (FindPet(out Projectile master, ProjectileType<Remilia>()) && PetState != 3)
                {
                    if (Remilia.HateSunlight(Projectile))
                    {
                        Teleport(master.Center + new Vector2(-20 * master.spriteDirection, player.gfxOffY));
                        PetState = 3;
                        Projectile.netUpdate = true;
                    }
                    else if (master.ai[1] == 2 && PetState != 2)
                    {
                        extraAI[1] = 0;
                        Teleport(master.Center + new Vector2(-40 * master.spriteDirection, player.gfxOffY));
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
                else if (FindPet(out Projectile meirin, ProjectileType<Meirin>()) && PetState <= 1)
                {
                    if (meirin.frame == 2 && mainTimer % 480 == 0 && Main.rand.NextBool(3))
                    {
                        PetState = 4;
                        Projectile.netUpdate = true;
                    }
                }
                if (mainTimer % 270 == 0 && PetState <= 0)
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
            else if (PetState == 2)
            {
                Serve();
            }
            else if (PetState == 3)
            {
                Serve(true);
            }
            else if (PetState == 4)
            {
                WakeUp();
            }
        }
    }
}


