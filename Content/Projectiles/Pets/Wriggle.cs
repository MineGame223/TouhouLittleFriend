using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Wriggle : BasicTouhouPet
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawWriggle(wingFrame, lightColor, 1, new Vector2(extraAdjX, extraAdjY));
            DrawWriggle(Projectile.frame, lightColor);
            if (Projectile.frame != 4)
            {
                if (PetState == 1)
                    DrawWriggle(blinkFrame, lightColor);
                DrawWriggle(antennaeFrame, lightColor, 1, new Vector2(extraAdjX, extraAdjY));
            }
            DrawWriggle(Projectile.frame, lightColor, 0, default, AltVanillaFunction.GetExtraTexture("Wriggle_Cloth"), true);
            return false;
        }
        private void DrawWriggle(int frame, Color lightColor, int columns = 0, Vector2 extraPos = default, Texture2D tex = null, bool entitySpriteDraw = false)
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
                Main.EntitySpriteDraw(t, pos + shake, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
            else
                Main.spriteBatch.TeaNPCDraw(t, pos + shake, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, effect, 0f);
        }
        private static bool CheckEmptyPlace(Vector2 position)
        {
            return !(WorldGen.SolidTile2(position.ToTileCoordinates().X, position.ToTileCoordinates().Y) || WorldGen.SolidTile(position.ToTileCoordinates().X + 1, position.ToTileCoordinates().Y) || WorldGen.SolidTile(position.ToTileCoordinates().X, position.ToTileCoordinates().Y - 1) || WorldGen.SolidTile(position.ToTileCoordinates().X + 1, position.ToTileCoordinates().Y - 1) || WorldGen.SolidTile(position.ToTileCoordinates().X, position.ToTileCoordinates().Y - 2) || WorldGen.SolidTile(position.ToTileCoordinates().X + 1, position.ToTileCoordinates().Y - 2));
        }
        private bool CanGenFireFly(Player player)
        {
            return PetState != 3 &&
                (!Main.dayTime
                || player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight);
        }
        private int FireflyType(Player player)
        {
            if (player.ZoneHallow)
            {
                return 2;
            }
            else if (player.ZoneUnderworldHeight)
            {
                return 3;
            }
            else
            {
                return Main.rand.Next(0, 2);
            }
        }
        private void Blink()
        {
            if (blinkFrame < 5)
            {
                blinkFrame = 5;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = 5;
                PetState = 0;
            }
        }
        int wingFrame, wingFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int antennaeFrame, antennaeFrameCounter;
        int extraAdjX, extraAdjY;
        Vector2 shake;
        bool antennaeActive;
        private void BugSwarm()
        {
            Player player = Main.player[Projectile.owner];
            if (!CanGenFireFly(player) || player.ZoneUnderworldHeight)
            {
                Projectile.frame = 0;
                extraAI[0] = 0;
                extraAI[1] = 0;
                extraAI[2] = 0;
                PetState = 0;
                return;
            }
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 2)
                    Projectile.frame = 2;
                extraAI[1]++;
                if (extraAI[1] % 3 == 0 &&
                   player.ownedProjectileCounts[ProjectileType<WriggleFirefly>()] < 100)
                {
                    Vector2 point = Projectile.Center + new Vector2(Main.rand.Next(-600, 600), Main.rand.Next(-600, 600));
                    if (CheckEmptyPlace(point))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), point
                            , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), ProjectileType<WriggleFirefly>(), 0, 0, Main.myPlayer
                            , FireflyType(player), Main.rand.Next(0, 2));
                    }
                }
                if (extraAI[1] > extraAI[2])
                {
                    extraAI[1] = 0;
                    extraAI[0] = 1;
                }
            }
            else
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    extraAI[2] = 0;
                    PetState = 0;
                }
            }
        }
        private void UpdateWingFrame()
        {
            int count = 5;
            if (wingFrame < 4)
            {
                wingFrame = 4;
            }
            if (++wingFrameCounter > count)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 7)
            {
                wingFrame = 4;
            }
        }
        private void UpdateAntennaeFrame()
        {
            int count = 5;
            if (++antennaeFrameCounter > count)
            {
                antennaeFrameCounter = 0;
                antennaeFrame++;
            }
            if (antennaeFrame > 3)
            {
                antennaeFrame = 0;
                antennaeActive = false;
            }
        }
        private static List<int> IsFlyInsect()
        {
            return new List<int>
            {
                NPCID.Butterfly,
                NPCID.EmpressButterfly,
                NPCID.GoldButterfly,
                NPCID.HellButterfly,
                NPCID.Firefly,
                NPCID.Lavafly,
                NPCID.LightningBug,
                NPCID.LadyBug,
                NPCID.GoldLadyBug,
                NPCID.BlackDragonfly,
                NPCID.BlueDragonfly,
                NPCID.GoldDragonfly,
                NPCID.GreenDragonfly,
                NPCID.OrangeDragonfly,
                NPCID.RedDragonfly,
                NPCID.YellowDragonfly,
            };
        }
        private void AttractInsect()
        {
            foreach (NPC bug in Main.npc)
            {
                if (IsFlyInsect().Contains(bug.type))
                    if (bug.Distance(Projectile.Center) <= 500 && bug.Distance(Projectile.Center) >= Main.rand.Next(60, 120))
                    {
                        bug.velocity = Vector2.Normalize(Projectile.Center - bug.Center) * 1.2f
                            * (bug.Distance(Projectile.Center) / 120);
                    }
            }
        }
        Color myColor = new Color(107, 252, 75);
        public override string GetChatText(out string[] text)
        {
            text = new string[21];
            text[1] = ModUtils.GetChatText("Wriggle", "1");
            text[2] = ModUtils.GetChatText("Wriggle", "2");
            text[3] = ModUtils.GetChatText("Wriggle", "3");
            text[4] = ModUtils.GetChatText("Wriggle", "4");
            text[5] = ModUtils.GetChatText("Wriggle", "5");
            text[7] = ModUtils.GetChatText("Wriggle", "7");
            text[8] = ModUtils.GetChatText("Wriggle", "8");
            text[9] = ModUtils.GetChatText("Wriggle", "9");
            text[10] = ModUtils.GetChatText("Wriggle", "10");
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
            if (mainTimer % 900 == 0 && Main.rand.NextBool(6) && mainTimer > 0)
            {
                if (PetState == 3)
                    SetChat(myColor, ModUtils.GetChatText("Wriggle", "6"), 6);
                else if (PetState <= 1)
                    SetChat(myColor);
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            if (antennaeActive)
                UpdateAntennaeFrame();
        }
        public override void AI()
        {
            if (PetState != 3)
                Lighting.AddLight(Projectile.Center, 1.48f * Main.essScale, 1.44f * Main.essScale, 0.44f * Main.essScale);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<WriggleBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(50 * player.direction, -60 + player.gfxOffY);
            Projectile.tileCollide = false;
            if (PetState != 2)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir(player, true);
            MoveToPoint(point, 14f);
            if (mainTimer % 30 == 0 && CanGenFireFly(player))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40))
                            , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), ProjectileType<WriggleFirefly>(), 0, 0, Main.myPlayer
                            , FireflyType(player), Main.rand.Next(0, 2));
            }
            if (mainTimer % 270 == 0 && PetState == 0)
            {
                PetState = 1;
                if (Main.rand.NextBool(4))
                    antennaeActive = true;
            }
            if (mainTimer >= 1200 && mainTimer < 3600 && PetState <= 1 && extraAI[0] == 0)
            {
                if (mainTimer % 600 == 0 && Main.rand.NextBool(1) && CanGenFireFly(player))
                {
                    PetState = 2;
                    extraAI[2] = Main.rand.Next(600, 1200);
                }
            }
            shake = Vector2.Zero;
            if (player.ZoneSnow)
            {
                PetState = 3;
            }
            else if (PetState == 3)
            {
                PetState = 0;
            }
            if (PetState == 3)
            {
                Projectile.frame = 4;
                shake = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0);
            }
            else if (PetState == 0)
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
                BugSwarm();
            }
            extraAdjX = 0;
            extraAdjY = 0;
            if (Projectile.frame >= 1 && Projectile.frame <= 3)
            {
                extraAdjY = -2;
                if (Projectile.frame == 3)
                {
                    extraAdjX = -2 * Projectile.spriteDirection;
                }
            }
            AttractInsect();
        }
    }
}


