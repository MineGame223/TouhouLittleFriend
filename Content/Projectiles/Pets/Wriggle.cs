using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Wriggle : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Wriggle_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 extraPos = new Vector2(extraAdjX, extraAdjY);
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };
            DrawPetConfig config2 = drawConfig with
            {
                PositionOffset = extraPos,
            };

            Projectile.DrawPet(wingFrame, lightColor, config2, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (Projectile.frame != 4)
            {
                if (PetState == 1)
                    Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
                Projectile.DrawPet(antennaeFrame, lightColor, config2, 1);
            }
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
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
        private static int FireflyType(Player player)
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
        bool AntennaeActive
        {
            get => Projectile.ai[2] == 0;
            set => Projectile.ai[2] = value ? 0 : 1;
        }
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

                if (extraAI[1] == 0)
                    AltVanillaFunction.PlaySound(SoundID.Pixie, Projectile.Center);
                extraAI[1]++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] % 2 == 0 &&
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
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 2400;
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
            if (++antennaeFrameCounter > count && AntennaeActive)
            {
                antennaeFrameCounter = 0;
                antennaeFrame++;
            }
            if (antennaeFrame > 3)
            {
                antennaeFrame = 0;
                AntennaeActive = false;
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
                NPCID.Stinkbug
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
        public override Color ChatTextColor => new Color(107, 252, 75);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Wriggle";
            indexRange = new Vector2(1, 10);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 910;
            chance = 6;
            whenShouldStop = PetState > 1;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (PetState == 3)
                {
                    chat.Add(ChatDictionary[6]);
                }
                else
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        if (j == 6)
                            continue;
                        chat.Add(ChatDictionary[j]);
                    }
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateAntennaeFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(1))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Mystia>();
            if (FindPet(out Projectile member, type))
            {
                chatRoom.member[0] = member;
                member.ToPetClass().currentChatRoom = chatRoom;
            }
            else
            {
                chatRoom.CloseChatRoom();
                return;
            }
            Projectile wriggle = chatRoom.initiator;
            Projectile mystia = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //莉格露：一闪一闪亮晶晶~满天都是小蜻蜓~
                mystia.CloseCurrentDialog();

                if (wriggle.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //米斯蒂娅：挂在天空放光明~好似无数...欸蜻蜓不会发光啊！
                mystia.SetChat(ChatSettingConfig, 9, 20);

                if (mystia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
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

            ChangeDir();
            MoveToPoint(point, 14f);
            if (mainTimer % (PetState == 2 ? 15 : 30) == 0 && CanGenFireFly(player))
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40))
                            , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), ProjectileType<WriggleFirefly>(), 0, 0, Main.myPlayer
                            , FireflyType(player), Main.rand.Next(0, 2));
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    if (Main.rand.NextBool(4))
                        AntennaeActive = true;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState <= 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(1) && CanGenFireFly(player))
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(600, 1200);
                        Projectile.netUpdate = true;
                    }
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


