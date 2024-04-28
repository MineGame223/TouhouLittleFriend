using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Danmaku;
using static TouhouPets.DanmakuFightHelper;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Moku : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            BeforeBattle,
            Battling,
            Win,
            Lose,
            Burning,
            AfterBurning,
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
        private int wingFrame, wingFrameCounter;
        private int hairFrame, hairFrameCounter;
        private float extraX, extraY;

        private float floatingX, floatingY;
        private float ringAlpha, flameAlhpa;
        private int[] abilityCD;
        private int health;

        private const int MaxHealth = 360;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Moku_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (IsBattling)
            {
                DrawDanmakuRing();
            }

            for (int i = 0; i < 7; i++)
            {
                Projectile.DrawPet(wingFrame, Color.White * 0.3f,
                    drawConfig with
                    {
                        ShouldUseEntitySpriteDraw = true,
                        PositionOffset = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f))
                        + new Vector2(extraX, extraY),
                        Scale = 1 + flameAlhpa * 0.3f,
                    }, 1);
            }
            Projectile.DrawStateNormalizeForPet();
            if (flameAlhpa > 0f)
            {
                int max = 5;
                for (int i = 0; i < max; i++)
                {
                    Color clr = Color.Goldenrod * 0.4f;
                    clr.A *= 0;
                    DrawMokuBody(clr,
                        drawConfig with
                        {
                            PositionOffset = new Vector2(0, -2)
                            .RotatedBy(MathHelper.TwoPi / max * i + Main.GlobalTimeWrappedHourly * 3),
                            Scale = 1.2f,
                        });
                }
            }
            DrawMokuBody(lightColor, drawConfig);

            if (OwnerIsMyPlayer && IsBattleState)
            {
                DrawFightState();
            }
            return false;
        }
        private void DrawMokuBody(Color lightColor, DrawPetConfig config)
        {
            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
                ShouldUseEntitySpriteDraw = true,
            };
            Projectile.DrawPet(hairFrame, lightColor,
                config with
                {
                    PositionOffset = new Vector2(extraX, extraY),
                }, 1);
            Projectile.DrawPet(hairFrame, lightColor,
               config2 with
               {
                   PositionOffset = new Vector2(extraX, extraY),
               }, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, config);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, config, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            Projectile.DrawStateNormalizeForPet();
        }
        private void DrawFightState()
        {
            if (IsBattling && health < MaxHealth)
            {
                Main.instance.DrawHealthBar(Projectile.Center.X, Projectile.position.Y + Projectile.height + 16
                    , health, MaxHealth, 0.8f);
            }
            if (CurrentState == States.Win || CurrentState == States.Lose)
            {
                Projectile.DrawIndividualSource(PlayerB_Source);
            }
        }
        private void DrawDanmakuRing()
        {
            Main.spriteBatch.QuickToggleAdditiveMode(true);
            Main.instance.LoadFlameRing();
            Texture2D t = TextureAssets.FlameRing.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new Rectangle(0, 0, t.Width, t.Height / 3);
            Vector2 orig = rect.Size() / 2;
            Color clr = Projectile.GetAlpha(Color.White) * ringAlpha;
            float scale = Projectile.scale * DanmakuRingScale;
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(t, pos + new Vector2(Main.rand.NextFloat(-1.3f, 1.3f)), rect, clr * 0.34f, Main.GlobalTimeWrappedHourly, orig, scale * 0.65f, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(t, pos + new Vector2(Main.rand.NextFloat(-1.3f, 1.3f)), rect, clr * 0.3f, -Main.GlobalTimeWrappedHourly, orig, scale * 0.5f, SpriteEffects.FlipHorizontally, 0f);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false);
        }
        public override Color ChatTextColor => new Color(200, 200, 200);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Moku";
            indexRange = new Vector2(1, 15);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 940;
            chance = 5;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                if (FindPet(ProjectileType<Keine>(), false, 2, 3))
                {
                    chat.Add(ChatDictionary[9]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(9))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Keine>();
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
            Projectile moku = chatRoom.initiator;
            Projectile keine = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //妹红：每到满月你都会这样，怪吓人的！
                keine.CloseCurrentDialog();

                if (moku.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //慧音：这是天性，也是使命。
                keine.SetChat(ChatSettingConfig, 9, 20);

                if (keine.CurrentDialogFinished())
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
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(2.15f, 1.84f, 0.87f);
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            abilityCD = new int[2];
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<MokuBuff>());

            UpdateTalking();

            ControlMovement();

            GenDust();

            bool noKaguya = !FindPet(ProjectileType<Kaguya>(), false) || !Owner.HasBuff<KaguyaBuff>();
            if (IsBattleState && (Owner.afkCounter <= 0 || noKaguya))
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

                case States.Burning:
                    shouldNotTalking = true;
                    Burning();
                    break;

                case States.AfterBurning:
                    shouldNotTalking = true;
                    AfterBurning();
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
                Vector2 point = new Vector2(70 * Owner.direction, -30 + Owner.gfxOffY);
                ChangeDir();
                MoveToPoint(point, 15f);
            }
        }
        private void GenDust()
        {
            if (Main.rand.NextBool(7))
                Dust.NewDustPerfect(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), MyDustId.Fire
                    , new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 100, default
                    , Main.rand.NextFloat(1f, 2f)).noGravity = true;
        }
        private void UpdateMiscData()
        {
            extraX = 0;
            extraY = 0;
            if (Projectile.frame >= 7 && Projectile.frame <= 8)
            {
                extraX = 2 * Projectile.spriteDirection;
            }
            if (Projectile.frame == 11)
            {
                extraY = -2;
            }
            if (Projectile.frame == 13 || Projectile.frame == 8)
            {
                extraY = 2;
            }
            if (CurrentState == States.Burning)
            {
                flameAlhpa += 0.1f;
            }
            else
            {
                flameAlhpa -= 0.03f;
            }
            ringAlpha = MathHelper.Clamp(ringAlpha += 0.05f * (IsBattling ? 1 : -1), 0, 1);
            flameAlhpa = MathHelper.Clamp(flameAlhpa, 0, 1);

            if (!OwnerIsMyPlayer)
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
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (Owner.afkCounter >= 600)
                {
                    bool ableToFight = mainTimer % 60 == 0 && Main.rand.NextBool(2) 
                        && FindPet(ProjectileType<Kaguya>(), false, 0, 1);
                    if (ableToFight || FindPet(ProjectileType<Kaguya>(), false, (int)States.BeforeBattle))
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
                if (mainTimer > 0 && mainTimer % 990 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(120, 240);
                        CurrentState = States.Burning;
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
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

            Projectile.frame = 0;

            if (Timer == 0)
            {
                Round++;
            }
            Timer++;
            if (OwnerIsMyPlayer && Timer > 375)
            {
                Timer = 0;
                health = MaxHealth;
                CurrentState = States.Battling;
            }

            Projectile.spriteDirection = 1;
            Vector2 point = new Vector2(-200, -200);
            MoveToPoint2(point, 15f);
        }
        private void Battling()
        {
            Projectile.velocity *= 0.5f;
            hairFrameCounter += 2;
            if (Projectile.frame < 7)
            {
                Projectile.frame = 7;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame >= 8)
            {
                Projectile.frame = 8;
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
                    if (Main.rand.NextBool(30) && abilityCD[0] <= 0
                        && Owner.ownedProjectileCounts[ProjectileType<MokuFireball>()] < 1)
                    {
                        abilityCD[0] = 180;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                            new Vector2(Main.rand.Next(7, 9), 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-3, 3)))
                            , ProjectileType<MokuFireball>(), Main.rand.Next(12, 20), 0, Projectile.owner);
                    }
                    else if (Main.rand.NextBool(25) && abilityCD[1] <= 0)
                    {
                        abilityCD[1] = 180;
                        for (int i = -3; i <= 3; i++)
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                                new Vector2(5, 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(5, 6) * i))
                                , ProjectileType<MokuBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner);
                        }
                    }
                    else
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Main.rand.Next(-50, -50)).RotatedByRandom(MathHelper.ToRadians(360)),
                            new Vector2(Main.rand.Next(4, 8), 0).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-9, 9)))
                            , ProjectileType<MokuBullet>(), Main.rand.Next(6, 13), 0, Projectile.owner);
                    }
                }
            }
            Projectile.HandleHurt(ref health, false);
            if (OwnerIsMyPlayer)
            {
                if (FindPet(ProjectileType<Kaguya>(), false, (int)States.Lose))
                {
                    CombatText.NewText(Projectile.getRect(), Color.Yellow, "WIN!", true, false);

                    PlayerB_Source++;
                    Timer = 0;
                    CurrentState = States.Win;
                }
                else if (health <= 0)
                {
                    Projectile.FailEffect();
                    CombatText.NewText(Projectile.getRect(), Color.Gray, "lose...", true, false);

                    Timer = 0;
                    CurrentState = States.Lose;
                }
            }

            Projectile.spriteDirection = 1;
            Vector2 point = new Vector2(-200 + floatingX, -200 + floatingY);
            MoveToPoint2(point, 4f);
        }
        private void Win()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = 1;
            if (Projectile.frame < 10)
            {
                Projectile.frame = 10;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 10;
            }
            if (OwnerIsMyPlayer)
            {
                if (Timer == 0)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            Projectile.SetChat(ChatSettingConfig, 10);
                            break;
                        case 2:
                            Projectile.SetChat(ChatSettingConfig, 11);
                            break;
                        default:
                            Projectile.SetChat(ChatSettingConfig, 12);
                            break;
                    }
                }
                if (++Timer > 480 || FindPet(ProjectileType<Kaguya>(), false, (int)States.BeforeBattle))
                {
                    Timer = 0;
                    CurrentState = States.BeforeBattle;
                }
            }
        }
        private void Lose()
        {
            Projectile.velocity *= 0;
            Projectile.spriteDirection = 1;
            if (Projectile.frame < 12)
            {
                Projectile.frame = 12;
            }
            if (++Projectile.frameCounter > 30)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 13)
            {
                Projectile.frame = 12;
            }
            if (OwnerIsMyPlayer)
            {
                if (Timer == 0)
                {
                    int chance = Main.rand.Next(3);
                    switch (chance)
                    {
                        case 1:
                            Projectile.SetChat(ChatSettingConfig, 13);
                            break;
                        case 2:
                            Projectile.SetChat(ChatSettingConfig, 14);
                            break;
                        default:
                            Projectile.SetChat(ChatSettingConfig, 15);
                            break;
                    }
                }
                if (++Timer > 480 || FindPet(ProjectileType<Kaguya>(), false, (int)States.BeforeBattle))
                {
                    Timer = 0;
                    CurrentState = States.BeforeBattle;
                }
            }
        }
        private void Burning()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 2;
                if (OwnerIsMyPlayer)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis()
                            , Projectile.Center + new Vector2(0, Main.rand.Next(20, 90)).RotatedByRandom(MathHelper.TwoPi)
                                , Vector2.Zero, ProjectileType<MokuFlame>(), 0, 0);
                    }
                    Timer++;
                }
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterBurning;
            }
        }
        private void AfterBurning()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 900;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateMiscFrame()
        {
            if (wingFrame < 9)
            {
                wingFrame = 9;
            }
            if (++wingFrameCounter > 5)
            {
                wingFrameCounter = 0;
                wingFrame++;
            }
            if (wingFrame > 14)
            {
                wingFrame = 9;
            }

            if (hairFrame < 3)
            {
                hairFrame = 3;
            }
            if (++hairFrameCounter > 6)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 8)
            {
                hairFrame = 3;
            }
        }
    }
}


