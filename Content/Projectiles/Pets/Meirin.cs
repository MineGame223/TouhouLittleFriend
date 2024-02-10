using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Meirin : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 24;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawMeirin(auraFrame, Color.White, 1, default, null, true);
            Projectile.DrawStateNormalizeForPet();
            DrawMeirin(hairFrame, lightColor, 1, hairPosOffset);
            DrawMeirin(Projectile.frame, lightColor);
            if (PetState == 1 || PetState == 4)
                DrawMeirin(blinkFrame, lightColor, 1);
            DrawMeirin(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Meirin_Cloth"), true);
            DrawMeirin(clothFrame, lightColor, 1, clothPosOffset, null, true);
            if (PetState == 3 || PetState == 4)
            {
                DrawUmbrella(lightColor);
            }
            return false;
        }
        private void DrawMeirin(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
        {
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
        private void DrawUmbrella(Color lightColor)
        {
            int type = ItemID.Umbrella;
            Main.instance.LoadItem(type);
            Texture2D tex = AltVanillaFunction.ItemTexture(type);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(17 * Projectile.spriteDirection, -20) + new Vector2(0, 7f * Main.essScale);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = tex.Size() / 2;
            Main.EntitySpriteDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
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
                if (PetState == 4)
                {
                    PetState = 3;
                }
                else
                {
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        int auraFrame, auraFrameCounter;
        Vector2 clothPosOffset;
        Vector2 hairPosOffset;
        private void Kongfu()
        {
            if (Projectile.frame < 6)
            {
                Projectile.frame = 6;
            }

            var count = Projectile.frame switch
            {
                9 => 120,
                13 => 120,
                17 => 120,
                19 => 15,
                20 => 20,
                21 => 20,
                22 => 180,
                _ => 10,
            };
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.rand.NextBool(25) && extraAI[1] <= 0)
                    {
                        SetChat(myColor, ModUtils.GetChatText("Meirin", "5"), 5);
                        extraAI[1]++;
                    }
                }
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Projectile.frame > 22)
                    {
                        Projectile.frame = 0;
                        extraAI[0] = 1200;
                        extraAI[1] = 0;
                        PetState = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        private void Serve()
        {
            Projectile.frame = 23;
            if (Main.player[Projectile.owner].ownedProjectileCounts[ProjectileType<Flandre>()] <= 0
                || !Remilia.HateSunlight(Projectile))
            {
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        private void Sleep()
        {
            Player player = Main.player[Projectile.owner];
            if (extraAI[0] == 0)
            {
                Projectile.velocity *= 0.5f;
                Projectile.frame = 1;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (++extraAI[1] >= extraAI[2])
                    {
                        extraAI[0]++;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0.1f;
                Projectile.frame = 2;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (player.velocity.Length() > 6f)
                    {
                        PetState = 0;
                        extraAI[0] = 1200;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        private void GetHurt()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            var count = Projectile.frame switch
            {
                3 => 60,
                _ => 10,
            };
            if (++Projectile.frameCounter >= count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                extraAI[1]++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 4;
            }
            if (extraAI[1] > 10)
            {
                extraAI[1] = 0;
                extraAI[0] = 1200;
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 6)
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
            if (++hairFrameCounter > 8)
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
            if (++auraFrameCounter > 7)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 18)
            {
                auraFrame = 11;
            }
        }
        Color myColor = new Color(255, 81, 81);
        public override string GetChatText(out string[] text)
        {
            Player player = Main.player[Projectile.owner];
            text = new string[21];
            text[1] = ModUtils.GetChatText("Meirin", "1");
            text[2] = ModUtils.GetChatText("Meirin", "2");
            text[3] = ModUtils.GetChatText("Meirin", "3");
            text[4] = ModUtils.GetChatText("Meirin", "4");
            if (player.ownedProjectileCounts[ProjectileType<Sakuya>()] > 0)
            {
                text[10] = ModUtils.GetChatText("Meirin", "10");
                text[13] = ModUtils.GetChatText("Meirin", "13");
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
            Player player = Main.player[Projectile.owner];
            bool chance = Main.rand.NextBool(player.HasBuff<ScarletBuff>() ? 30 : 5);

            int sakuya = ProjectileType<Sakuya>();
            int flandre = ProjectileType<Flandre>();

            if (FindChatIndex(out Projectile _, sakuya, 4, default, 0) && PetState <= 4
                || FindChatIndex(out Projectile _, flandre, 10, default, 0))
            {
                ChatCD = 1;
            }
            if (FindChatIndex(out Projectile p, sakuya, 4))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "9"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, sakuya, 6, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "11"), myColor, 11);
            }
            else if (FindChatIndex(out p, sakuya, 7, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "12"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, sakuya, 8, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "14"), myColor, 14);
            }
            else if (FindChatIndex(out p, sakuya, 9, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "15"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, flandre, 9, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "6"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (FindChatIndex(out p, flandre, 10))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "7"), myColor, 7);
            }
            else if (FindChatIndex(out p, flandre, 11, default, 1, true))
            {
                SetChatWithOtherOne(p, ModUtils.GetChatText("Meirin", "8"), myColor, 0);
                p.localAI[2] = 0;
            }
            else if (mainTimer % 960 == 0 && chance && PetState <= 1)
            {
                SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        private void SetMeirinLight()
        {
            float r = Main.DiscoR / 255f;
            float g = Main.DiscoG / 255f;
            float b = Main.DiscoB / 255f;
            float strength = 2f;
            r = (strength + r) / 1.5f;
            g = (strength + g) / 1.5f;
            b = (strength + b) / 1.5f;
            Lighting.AddLight(Projectile.Center, r, g, b);
            Lighting.AddLight(Projectile.Center, 0.40f, 0.31f, 0.48f);
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.003f;

            ChangeDir(player, false, 120);

            Vector2 point = new Vector2((player.HasBuff<ScarletBuff>() ? -100 : 50) * player.direction, -30 + player.gfxOffY);
            Vector2 center = default;
            float speed = 15f;
            if (FindPet(out Projectile master, ProjectileType<Flandre>()))
            {
                Projectile.spriteDirection = master.spriteDirection;
                if (PetState == 3 || PetState == 4)
                {
                    point = new Vector2((-25 - 60) * master.spriteDirection, player.gfxOffY - 20);
                    speed = 19f;
                }
            }
            MoveToPoint(point, speed, center);
        }
        public override void AI()
        {
            SetMeirinLight();
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MeirinBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (Remilia.HateSunlight(Projectile) && PetState != 3 && PetState != 4
                    && player.ownedProjectileCounts[ProjectileType<Flandre>()] > 0)
                {
                    PetState = 3;
                    Projectile.netUpdate = true;
                }
                if (PetState == 3)
                {
                    if (mainTimer % 270 == 0)
                    {
                        PetState = 4;
                        Projectile.netUpdate = true;
                    }
                }
                if (PetState <= 0)
                {
                    if (mainTimer % 270 == 0 && PetState <= 0)
                    {
                        PetState = 1;
                        Projectile.netUpdate = true;
                    }
                    if (mainTimer >= 10 && mainTimer < 3600)
                    {
                        if (mainTimer % 480 == 0 && Main.rand.NextBool(9) && extraAI[0] <= 0 && player.velocity.Length() <= 5f)
                        {
                            bool chance = !player.HasBuff<ScarletBuff>();
                            extraAI[1] = 0;
                            if (chance)
                                extraAI[2] = Main.rand.Next(120, 540);
                            PetState = chance ? 5 : 2;
                            Projectile.netUpdate = true;
                        }
                    }
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
                Blink();
                Projectile.frame = 0;
            }
            else if (PetState == 2)
            {
                Kongfu();
            }
            else if (PetState == 3 || PetState == 4)
            {
                if (PetState == 4)
                    Blink();
                Serve();
            }
            else if (PetState == 5)
            {
                Sleep();
            }
            else if (PetState == 6)
            {
                GetHurt();
            }
            clothPosOffset = Projectile.frame switch
            {
                7 => new Vector2(2, 0),
                8 => new Vector2(2, 0),
                9 => new Vector2(2, -2),
                10 => new Vector2(2, 0),
                11 => new Vector2(2, 0),
                13 => new Vector2(0, 2),
                14 => new Vector2(0, 2),
                15 => new Vector2(0, 2),
                16 => new Vector2(2, 2),
                17 => new Vector2(4, 2),
                18 => new Vector2(2, 2),
                19 => new Vector2(0, 2),
                21 => new Vector2(0, -2),
                _ => Vector2.Zero,
            };
            clothPosOffset.X *= -Projectile.spriteDirection;
            hairPosOffset = Projectile.frame switch
            {
                2 => new Vector2(0, 2),
                3 => new Vector2(0, 4),
                7 => new Vector2(2, 0),
                8 => new Vector2(2, 0),
                9 => new Vector2(2, -2),
                10 => new Vector2(2, 0),
                11 => new Vector2(2, 0),
                13 => new Vector2(0, 2),
                14 => new Vector2(0, 2),
                15 => new Vector2(0, 2),
                16 => new Vector2(2, 2),
                17 => new Vector2(4, 2),
                18 => new Vector2(2, 2),
                19 => new Vector2(0, 2),
                21 => new Vector2(0, -2),
                _ => Vector2.Zero,
            };
            hairPosOffset.X *= -Projectile.spriteDirection;
        }
    }
}


