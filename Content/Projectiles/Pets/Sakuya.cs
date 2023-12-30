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
            Main.projFrames[Type] = 17;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawSakuya(Projectile.frame, lightColor);
            if (PetState == 1)
                DrawSakuya(blinkFrame, lightColor);
            DrawSakuya(Projectile.frame, lightColor, AltVanillaFunction.GetExtraTexture("Sakuya_Cloth"), true);
            DrawSakuya(clothFrame, lightColor, null, true);
            Projectile.DrawStateNormalizeForPet();
            if (PetState == 3)
            {
                DrawUmbrella(lightColor);
            }
            return false;
        }
        private void DrawSakuya(int frame, Color lightColor, Texture2D tex = null, bool entitySpriteDraw = false)
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
        private void DrawUmbrella(Color lightColor)
        {
            int type = ItemID.TragicUmbrella;
            Main.instance.LoadItem(type);
            Texture2D tex = AltVanillaFunction.ItemTexture(type);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(11 * Projectile.spriteDirection, -18) + new Vector2(0, 7f * Main.essScale);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = tex.Size() / 2;
            SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, effect, 0);
        }
        private void Blink()
        {
            if (blinkFrame < 9)
            {
                blinkFrame = 9;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 11)
            {
                blinkFrame = 9;
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void Serve(bool alt = false)
        {
            if (alt)
            {
                Projectile.frame = 8;
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
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 1;
            }
            if (!FindPetState(out _, ProjectileType<Remilia>(), 2))
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
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
        Color myColor = new Color(114, 106, 255);
        public override string GetChatText(out string[] text)
        {
            //Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Sakuya", "1");
            text[2] = ModUtils.GetChatText("Sakuya", "2");
            text[3] = ModUtils.GetChatText("Sakuya", "3");
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != null)
                    {
                        int weight = 1;
                        if (PetState == 2 && (i > 4 || i < 3))
                        {
                            weight = 0;
                        }
                        chat.Add(text[i], weight);
                    }
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (mainTimer % 960 == 0 && Main.rand.NextBool(4) && mainTimer > 0 && PetState == 2)
            {
                SetChat(myColor);
            }
            else if (mainTimer % 720 == 0 && Main.rand.NextBool(12) && mainTimer > 0)
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

            ChangeDir(player, true);

            Vector2 point = new Vector2((player.HasBuff<ScarletBuff>() ? 90 : -50) * player.direction, -30 + player.gfxOffY);
            Vector2 center = default;
            if (FindPet(out Projectile master, ProjectileType<Remilia>()))
            {
                Projectile.spriteDirection = master.spriteDirection;
                if (PetState == 3)
                {
                    center = master.Center;
                    point = new Vector2(-20 * master.spriteDirection, player.gfxOffY);                    
                }
                else if (PetState == 2)
                {
                    center = master.Center;
                    point = new Vector2(-40 * master.spriteDirection, player.gfxOffY);
                }
            }

            MoveToPoint(point, 19f, center);
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
                if (FindPet(out Projectile master, ProjectileType<Remilia>()))
                {
                    if (Remilia.HateSunlight(Projectile) && PetState != 3)
                    {
                        Teleport(master.Center + new Vector2(-20 * master.spriteDirection, player.gfxOffY));
                        PetState = 3;
                        Projectile.netUpdate = true;
                    }
                    else if (master.ai[1] == 2 && PetState != 2)
                    {
                        Teleport(master.Center + new Vector2(-40 * master.spriteDirection, player.gfxOffY));
                        PetState = 2;
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
        }
    }
}


