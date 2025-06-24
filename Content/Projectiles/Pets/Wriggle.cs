using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Wriggle : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Swarming,
            AfterSwarming,
            Cold,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private int ActionCD
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        private int Timer
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        private int RandomCount
        {
            get => (int)Projectile.localAI[2];
            set => Projectile.localAI[2] = value;
        }
        private bool IsIdleState => CurrentState <= States.Blink;
        private bool FeelCold => Owner.ZoneSnow || Owner.GetModPlayer<TouhouPetPlayer>().lettyCold;
        private bool AntennaeActive
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        private bool CanGenFireFly
        {
            get
            {
                return CurrentState != States.Cold &&
                    (!Main.dayTime
                    || Owner.ZoneDirtLayerHeight || Owner.ZoneRockLayerHeight || Owner.ZoneUnderworldHeight);
            }
        }

        private int wingFrame, wingFrameCounter;
        private int blinkFrame, blinkFrameCounter;
        private int antennaeFrame, antennaeFrameCounter;
        private int extraAdjX, extraAdjY;
        private Vector2 shake;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Wriggle_Cloth");
        public override void PetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Wriggle;
        public override bool DrawPetSelf(ref Color lightColor)
        {
            drawConfig = drawConfig with
            {
                PositionOffset = shake,
            };
            Vector2 extraPos = new Vector2(extraAdjX, extraAdjY);
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
                AltTexture = clothTex,
            };
            DrawPetConfig config2 = drawConfig with
            {
                PositionOffset = extraPos + shake,
            };

            Projectile.DrawPet(wingFrame, lightColor, config2, 1);
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (Projectile.frame != 4)
            {
                if (CurrentState == States.Blink)
                    Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
                Projectile.DrawPet(antennaeFrame, lightColor, config2, 1);
            }
            Projectile.DrawPet(Projectile.frame, lightColor, config);
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(107, 252, 75),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Wriggle";
            indexRange = new Vector2(1, 10);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 910;//910
            chance = 6;//6
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<LocalizedText> RegularDialogText()
        {
            WeightedRandom<LocalizedText> chat = new ();
            {
                if (CurrentState == States.Cold)
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
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
            };
        }
        private List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID wriggle = TouhouPetID.Wriggle;
            TouhouPetID mystia = TouhouPetID.Mystia;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(wriggle, ChatDictionary[1], -1), //莉格露：一闪一闪亮晶晶~满天都是小蜻蜓~
                new ChatRoomInfo(mystia, GetChatText("Mystia",9), 0),//米斯蒂娅：挂在天空放光明~好似无数...欸蜻蜓不会发光啊！
            ];

            return list;
        }
        public override void VisualEffectForPreview()
        {
            UpdateWingFrame();
            UpdateAntennaeFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.48f * Main.essScale, 1.44f * Main.essScale, 0.44f * Main.essScale);
            inactive = CurrentState == States.Cold;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<WriggleBuff>());

            ControlMovement();

            if (FeelCold)
            {
                CurrentState = States.Cold;
            }
            else if (ShouldExtraVFXActive)
            {
                SpawnFirefly();
            }
            shake = Vector2.Zero;

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Swarming:
                    shouldNotTalking = true;
                    Swarming();
                    break;

                case States.AfterSwarming:
                    shouldNotTalking = true;
                    AfterSwarming();
                    break;

                case States.Cold:
                    shouldNotTalking = true;
                    Cold();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            UpdateMiscData();
            AttractInsect();
        }
        private void SpawnFirefly()
        {
            if (mainTimer % (CurrentState == States.Swarming ? 10 : 30) == 0 && CanGenFireFly)
            {
                if (OwnerIsMyPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40))
                            , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), ProjectileType<WriggleFirefly>(), 0, 0, Main.myPlayer
                            , FireflyType(Owner), Main.rand.Next(0, 2));
                }
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            if (CurrentState != States.Swarming)
                Projectile.rotation = Projectile.velocity.X * 0.035f;
            else
                Projectile.rotation = Projectile.velocity.X * 0.005f;

            ChangeDir();

            Vector2 point = new Vector2(50 * Owner.direction, -60 + Owner.gfxOffY);
            MoveToPoint(point, 14f);
        }
        private void UpdateMiscData()
        {
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
        }
        private static bool CheckEmptyPlace(Vector2 position)
        {
            return !(WorldGen.SolidTile2(position.ToTileCoordinates().X, position.ToTileCoordinates().Y)
                || WorldGen.SolidTile(position.ToTileCoordinates().X + 1, position.ToTileCoordinates().Y)
                || WorldGen.SolidTile(position.ToTileCoordinates().X, position.ToTileCoordinates().Y - 1)
                || WorldGen.SolidTile(position.ToTileCoordinates().X + 1, position.ToTileCoordinates().Y - 1)
                || WorldGen.SolidTile(position.ToTileCoordinates().X, position.ToTileCoordinates().Y - 2)
                || WorldGen.SolidTile(position.ToTileCoordinates().X + 1, position.ToTileCoordinates().Y - 2));
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
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        AntennaeActive = true;
                        Projectile.netUpdate = true;
                    }

                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 600 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (CanGenFireFly && !Owner.ZoneUnderworldHeight)
                    {
                        RandomCount = Main.rand.Next(900, 1200);
                        CurrentState = States.Swarming;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
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
                CurrentState = States.Idle;
            }
        }
        private void Swarming()
        {
            if (!CanGenFireFly)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    CurrentState = States.Idle;
                }
                return;
            }
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 2)
                Projectile.frame = 2;

            if (Timer == 0)
            {
                AltVanillaFunction.PlaySound(SoundID.Pixie, Projectile.Center);
            }
            Timer++;
            if (OwnerIsMyPlayer)
            {
                if (Timer % 2 == 0 && Owner.ownedProjectileCounts[ProjectileType<WriggleFirefly>()] < 100)
                {
                    Vector2 point = Projectile.Center + new Vector2(Main.rand.Next(-600, 600), Main.rand.Next(-600, 600));
                    if (CheckEmptyPlace(point) && GetInstance<PetAbilitiesConfig>().SpecialAbility_Wriggle)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), point
                            , new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), ProjectileType<WriggleFirefly>(), 0, 0, Main.myPlayer
                            , FireflyType(Owner), Main.rand.Next(0, 2));
                    }
                }
                if (Timer > RandomCount)
                {
                    Timer = 0;
                    CurrentState = States.AfterSwarming;
                }
            }
        }
        private void AfterSwarming()
        {
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 2400;
                    CurrentState = States.Idle;
                }
            }
        }
        private void Cold()
        {
            Projectile.frame = 4;
            shake = new Vector2(Main.rand.NextFloat(-1f, 1f), 0);
            if (OwnerIsMyPlayer && !FeelCold)
            {
                CurrentState = States.Idle;
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
        private void AttractInsect()
        {
            if (!GetInstance<PetAbilitiesConfig>().SpecialAbility_Wriggle)
                return;

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
        private static List<int> IsFlyInsect()
        {
            return
            [
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
            ];
        }
    }
}


