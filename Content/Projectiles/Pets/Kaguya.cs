using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Danmaku;
using static TouhouPets.DanmakuFightHelper;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Kaguya : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BeforeBattle,
            Battling,
            Win,
            Lose,
            PlayingGames,
            PlayingGames2,
            AfterPlayingGames,
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
        private bool IsBattling => CurrentState == States.Battling;
        private bool IsBattleState => CurrentState >= States.BeforeBattle && CurrentState <= States.Lose;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int hairFrame, hairFrameCounter;
        private float extraX, extraY;
        private int gameTimer, gameRandomCount;
        private bool shouldMokuTalking;

        private float floatingX, floatingY;
        private float ringAlpha;
        private int[] abilityCD;
        private int health;

        private const int MaxHealth = 360;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Kaguya_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 21;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] =
                ProjectileID.Sets.SimpleLoop(0, 4, 5)
                .WhenSelected(8, 2, 8);
        }
        public override TouhouPetID UniqueID => TouhouPetID.Kaguya;
        public override bool OnMouseHover(ref bool dontInvis)
        {
            dontInvis = IsBattleState;
            return false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            if (IsBattling)
            {
                DrawDanmakuRing();
            }
            Projectile.ResetDrawStateForPet();

            Projectile.DrawPet(hairFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = new Vector2(extraX, extraY),
                }, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });

            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            if (OwnerIsMyPlayer && IsBattleState)
            {
                DrawFightState();
            }
            return false;
        }
        private void DrawFightState()
        {
            if (IsBattling && health < MaxHealth)
            {
                Main.instance.DrawHealthBar(Projectile.Center.X, Projectile.position.Y + Projectile.height + 10
                , health, MaxHealth, 0.8f);
            }
            if (CurrentState == States.Win || CurrentState == States.Lose)
            {
                Projectile.DrawIndividualScore(PlayerA_Score);
            }
            if (CurrentState == States.BeforeBattle)
            {
                DrawBattleRound();
            }
        }
        private void DrawDanmakuRing()
        {
            Texture2D t = AltVanillaFunction.ExtraTexture(ExtrasID.CultistRitual);
            Texture2D t2 = AltVanillaFunction.ProjectileTexture(ProjectileID.CultistRitual);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.LightGoldenrodYellow * 0.7f * Main.essScale) * ringAlpha;
            float scale = Projectile.scale * DanmakuRingScale;
            SpriteEffects effect = SpriteEffects.None;
            Main.EntitySpriteDraw(t, pos, rect, clr, -Main.GlobalTimeWrappedHourly * 0.6f, orig, scale / 2, effect, 0f);
            rect = new Rectangle(0, 0, t2.Width, t2.Height);
            orig = rect.Size() / 2;
            Main.EntitySpriteDraw(t2, pos, rect, clr, Main.GlobalTimeWrappedHourly, orig, scale / 2, effect, 0f);
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(255, 165, 191),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Kaguya";
            indexRange = new Vector2(1, 21);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 730;//730
            chance = 7;//7
            whenShouldStop = !IsIdleState;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                if (FindPet(ProjectileType<Moku>()))
                {
                    chat.Add(ChatDictionary[7]);
                    chat.Add(ChatDictionary[18]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(3, 6) && shouldMokuTalking)
            {
                Chatting3(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        public override List<List<ChatRoomInfo>> RegisterChatRoom()
        {
            return new()
            {
                Chatting1(),
                Chatting2(),
            };
        }
        private static List<ChatRoomInfo> Chatting1()
        {
            TouhouPetID kaguya = TouhouPetID.Kaguya;
            TouhouPetID moku = TouhouPetID.Moku;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(kaguya, 7, -1), //辉夜：你说，咱俩斗了多久了？
                new ChatRoomInfo(moku, 3, 0),//妹红：我怎么知道，大概几千年了吧？
                new ChatRoomInfo(kaguya, 8, 1), //辉夜：今天要不要尝试点新花样？
                new ChatRoomInfo(moku, 4, 2),//妹红：...？你想干什么？
                new ChatRoomInfo(kaguya, 9, 3), //辉夜：要不就...做一点更 刺 激 的事儿？
                new ChatRoomInfo(moku, 5, 4),//妹红：想都不要想！...不过我知道哪里适合...
            ];

            return list;
        }
        private static List<ChatRoomInfo> Chatting2()
        {
            TouhouPetID kaguya = TouhouPetID.Kaguya;
            TouhouPetID moku = TouhouPetID.Moku;

            List<ChatRoomInfo> list =
            [
                new ChatRoomInfo(kaguya, 18, -1), //辉夜：我说，你保密工作做得不太行啊？
                new ChatRoomInfo(moku, 16, 0), //妹红：啊？你在胡说什么...
                new ChatRoomInfo(kaguya, 19, 1), //辉夜：我去人里玩的事情被永琳发现了，刚刚训了我一顿...
                new ChatRoomInfo(moku, 17, 2), //妹红：哈哈哈哈！你活该呗。
                new ChatRoomInfo(kaguya, 20, 3), //辉夜：你说什么？想干架是嘛？！
                new ChatRoomInfo(moku, 18, 4), //妹红：下次我可就不带你去了。
                new ChatRoomInfo(kaguya, 21, 5), //辉夜：欸欸欸别！...
            ];

            return list;
        }
        private void Chatting3(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Moku>();
            if (FindPet(out Projectile member, type))
            {
                chatRoom.member[0] = member;
                member.AsTouhouPet().currentChatRoom = chatRoom;
            }
            else
            {
                chatRoom.CloseChatRoom();
                return;
            }
            Projectile kaguya = chatRoom.initiator;
            Projectile moku = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //辉夜：菜就多练。
                moku.CloseCurrentDialog();

                if (kaguya.CurrentlyNoDialog())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //妹红：你这家伙能不能消停一会儿？
                moku.SetChatForChatRoom(6, 20);

                if (moku.CurrentlyNoDialog())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //辉夜：要你管！
                kaguya.SetChatForChatRoom(6, 20);

                if (kaguya.CurrentlyNoDialog())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            abilityCD = new int[2];
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<KaguyaBuff>());
            Projectile.SetPetActive(Owner, BuffType<EienteiBuff>());

            UpdateTalking();

            ControlMovement();

            if (ShouldExtraVFXActive)
                GenDust();

            bool noMoku = !FindPet(ProjectileType<Moku>(), false)
                || (!Owner.HasBuff<MokuBuff>());
            if (IsBattleState && (Owner.afkCounter <= 0 || noMoku))
            {
                Timer = 0;
                CurrentState = States.Idle;
            }

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.BeforeBattle:
                    shouldNotTalking = true;
                    BeforeBattle();
                    break;

                case States.Battling:
                    shouldNotTalking = true;
                    Battling();
                    break;

                case States.Win:
                    shouldNotTalking = true;
                    Win();
                    break;

                case States.Lose:
                    shouldNotTalking = true;
                    Lose();
                    break;

                case States.PlayingGames:
                    shouldNotTalking = true;
                    PlayingGames();
                    break;

                case States.PlayingGames2:
                    shouldNotTalking = true;
                    PlayingGames2();
                    break;

                case States.AfterPlayingGames:
                    shouldNotTalking = true;
                    AfterPlayingGames();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            if (IsIdleState)
            {
                IdleAnimation();
            }
            UpdateMiscData();
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            floatingX = reader.ReadSingle();
            floatingY = reader.ReadSingle();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(floatingX);
            writer.Write(floatingY);
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;
            if (!IsBattleState)
            {
                Vector2 point = new Vector2(-60 * Owner.direction, -30 + Owner.gfxOffY);
                if (Owner.HasBuff<EienteiBuff>())
                {
                    point = new Vector2(-90 * Owner.direction, -60 + Owner.gfxOffY);
                }
                ChangeDir(200);
                MoveToPoint(point, 15f);
            }
        }
        private void GenDust()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), 34), MyDustId.YellowFx
                , new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-1f, -0.2f)), 100, default
                , Main.rand.NextFloat(0.75f, 1.5f));
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(Owner.cLight, Owner);
        }
        private void UpdateMiscData()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 8 && Projectile.frame <= 11 || Projectile.frame == 15 || Projectile.frame == 18)
            {
                extraX = -2 * Projectile.spriteDirection;
            }
            if (Projectile.frame == 18)
            {
                extraY = -2;
            }
            if (Projectile.frame == 20)
            {
                extraY = 2;
            }
            ringAlpha = MathHelper.Clamp(ringAlpha += 0.05f * (IsBattling ? 1 : -1), 0, 1);

            if (Projectile.owner != Main.myPlayer)
                return;

            if (!IsBattling)
            {
                abilityCD[0] = 0;
                abilityCD[1] = 0;
            }
            else
            {
                if (abilityCD[0] > 0)
                    abilityCD[0]--;
                if (abilityCD[1] > 0)
                    abilityCD[1]--;
            }
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (Owner.afkCounter >= 600 && GetInstance<PetAbilitiesConfig>().SpecialAbility_MokuAndKaguya)
                {
                    bool ableToFight = mainTimer % 60 == 0 && Main.rand.NextBool(2)
                        && FindPet(ProjectileType<Moku>(), false, 0, 1);
                    if (ableToFight || FindPet(ProjectileType<Moku>(), false, (int)States.BeforeBattle))
                    {
                        InitializeFightData();
                        Timer = 0;
                        CurrentState = States.BeforeBattle;
                        return;
                    }
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 360 == 0 && Projectile.velocity.Length() < 2f
                    && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        RandomCount = Main.rand.Next(36, 54);
                        CurrentState = States.PlayingGames;
                    }
                }
            }
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
                CurrentState = States.Idle;
            }
        }
        private void BeforeBattle()
        {
            Projectile.CloseCurrentDialog();

            floatingX = 0;
            floatingY = 0;

            IdleAnimation();

            Timer++;
            if (OwnerIsMyPlayer)
            {
                RoundTimer = Timer;
                if (Timer > 375)
                {
                    Timer = 0;
                    health = MaxHealth;
                    CurrentState = States.Battling;
                }
            }

            Projectile.spriteDirection = -1;
            Vector2 point = new Vector2(200, -200);
            MoveToPoint2(point, 15f);
        }
        private void Battling()
        {
            Projectile.velocity *= 0.5f;
            hairFrameCounter++;
            if (Projectile.frame < 13)
            {
                Projectile.frame = 13;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 15)
            {
                Projectile.frame = 15;
            }
            if (OwnerIsMyPlayer)
            {
                Timer++;
                if (Timer >= 3600)
                {
                    Timer = 0;
                }
                if (Timer % 120 == 0)
                {
                    floatingX = Main.rand.Next(-50, 50);
                    floatingY = Main.rand.Next(-50, 50);
                    Projectile.netUpdate = true;
                }
                if (Timer % (30 * MathHelper.Clamp(health / MaxHealth, 0.5f, 1)) == 0)
                {
                    if (Main.rand.NextBool(25) && abilityCD[0] <= 0
                        && Owner.ownedProjectileCounts[ProjectileType<KaguyaWave>()] < 1)
                    {
                        abilityCD[0] = 180;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                            Vector2.Zero
                            , ProjectileType<KaguyaWave>(), Main.rand.Next(12, 20), 0, Projectile.owner);
                    }
                    else if (Main.rand.NextBool(25) && abilityCD[1] <= 0)
                    {
                        abilityCD[1] = 180;
                        for (int i = 0; i < 12; i++)
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),
                                Projectile.Center + new Vector2(0, -90).RotatedBy(MathHelper.ToRadians(360 / 12 * i)),
                                new Vector2(-4, 0)
                                , ProjectileType<KaguyaBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner, Main.rand.Next(0, 5));
                        }
                    }
                    else
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Main.rand.Next(-50, -50)).RotatedByRandom(MathHelper.ToRadians(360)),
                        new Vector2(-Main.rand.Next(4, 8), 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-9, 9)))
                        , ProjectileType<KaguyaBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner, Main.rand.Next(0, 5));
                    }
                }
            }
            Projectile.HandleHurt(ref health, true);
            if (OwnerIsMyPlayer)
            {
                if (FindPet(ProjectileType<Moku>(), false, (int)States.Lose))
                {
                    PlayerA_Score++;
                    Timer = 0;
                    CurrentState = States.Win;
                }
                else if (health <= 0)
                {
                    Timer = 0;
                    CurrentState = States.Lose;
                }
            }

            Projectile.spriteDirection = -1;
            Vector2 point = new Vector2(200 + floatingX, -200 + floatingY);
            MoveToPoint2(point, 3f);
        }
        private void Win()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = -1;
            if (Projectile.frame < 17)
            {
                Projectile.frame = 17;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 18)
            {
                Projectile.frame = 17;
            }
            if (Timer == 0)
            {
                CombatText.NewText(Projectile.getRect(), Color.Yellow, "WIN!", true, false);
            }
            Timer++;
            if (OwnerIsMyPlayer)
            {
                if (Timer == 30)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            Projectile.SetChat(10);
                            break;
                        case 2:
                            Projectile.SetChat(11);
                            break;
                        default:
                            Projectile.SetChat(12);
                            break;
                    }
                }
                if (Timer > 480 || FindPet(ProjectileType<Moku>(), false, (int)States.BeforeBattle))
                {
                    Timer = 0;
                    CurrentState = States.BeforeBattle;
                }
            }
        }
        private void Lose()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = -1;
            if (Projectile.frame < 19)
            {
                Projectile.frame = 19;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 20)
            {
                Projectile.frame = 19;
            }
            if (Timer == 0)
            {
                Projectile.FailEffect();
                CombatText.NewText(Projectile.getRect(), Color.Gray, "lose...", true, false);
            }
            Timer++;
            if (OwnerIsMyPlayer)
            {
                if (Timer == 30)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            Projectile.SetChat(13);
                            break;
                        case 2:
                            Projectile.SetChat(14);
                            break;
                        default:
                            Projectile.SetChat(15);
                            break;
                    }
                }
                if (Timer > 480 || FindPet(ProjectileType<Moku>(), false, (int)States.BeforeBattle))
                {
                    Timer = 0;
                    CurrentState = States.BeforeBattle;
                }
            }
        }
        private void PlayingGames()
        {
            Projectile.velocity *= 0.3f;
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.velocity.Length() > 7.5f)
            {
                CurrentState = States.AfterPlayingGames;
                return;
            }

            if (Projectile.frame > 9)
            {
                Projectile.frame = 8;
                Timer++;
                if (OwnerIsMyPlayer)
                {
                    if (Timer > RandomCount)
                    {
                        Timer = 0;
                        if (Main.rand.NextBool(10))
                        {
                            CurrentState = States.AfterPlayingGames;
                            return;
                        }
                        else
                        {
                            RandomCount = Main.rand.Next(36, 54);
                        }
                    }
                    else
                    {
                        if (Main.rand.NextBool(8))
                        {
                            gameRandomCount = Main.rand.Next(2, 5);
                            CurrentState = States.PlayingGames2;
                        }
                    }
                }
            }
            PlayingGamesChat();
        }
        private void PlayingGames2()
        {
            Projectile.velocity *= 0.3f;
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.velocity.Length() > 7.5f)
            {
                CurrentState = States.AfterPlayingGames;
                return;
            }

            if (Projectile.frame > 11)
            {
                Projectile.frame = 10;
                Timer++;
                gameTimer++;
                if (OwnerIsMyPlayer)
                {
                    if (gameTimer > gameRandomCount)
                    {
                        gameTimer = 0;
                        CurrentState = States.PlayingGames;
                    }
                }
            }
            PlayingGamesChat();
        }
        private void PlayingGamesChat()
        {
            if (!OwnerIsMyPlayer)
                return;

            if (Timer > 0 && Timer % 36 == 0 && Main.rand.NextBool(8))
            {
                shouldMokuTalking = false;
                int chance = Main.rand.Next(3);
                switch (chance)
                {
                    case 1:
                        Projectile.SetChat(4);
                        break;
                    case 2:
                        Projectile.SetChat(5);
                        break;
                    default:
                        Projectile.SetChat(3);
                        break;
                }
                if (Main.rand.NextBool(8))
                {
                    shouldMokuTalking = true;
                }
            }
        }
        private void AfterPlayingGames()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame < 12)
            {
                Projectile.frame = 12;
            }
            if (Projectile.frame > 12)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    Timer = 0;
                    ActionCD = 600;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 5)
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
            if (++hairFrameCounter > 5)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }
        }
    }
}


