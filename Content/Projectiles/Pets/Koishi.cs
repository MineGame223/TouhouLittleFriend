using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Koishi : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            Calling,
            CallingFadeIn,
            CallingFadeOut,
            BeforeKill,
            KILL,
            AfterKill,
            Annoying,
            AfterAnnoying,
            Fading,
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
        private bool IsKillingState => CurrentState >= States.Calling && CurrentState <= States.AfterKill;

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private int annoyingFrame, annoyingFrameCounter;
        private int killCD;
        private Vector2 eyePosition, eyePositionOffset;
        private bool whiteDye;
        private bool yellowBlackDye;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Koishi_Cloth");
        private readonly Texture2D whiteTex = AltVanillaFunction.GetExtraTexture("Koishi_White");
        private readonly Texture2D newTex = AltVanillaFunction.GetExtraTexture("Koishi_New");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 18;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        public override bool DrawPetSelf(ref Color lightColor)
        {
            bool hasDye = whiteDye || yellowBlackDye;

            Texture2D tex = null;
            Texture2D cloth = clothTex;
            if (whiteDye)
            {
                tex = whiteTex;
            }
            else if (yellowBlackDye)
            {
                tex = newTex;
            }

            DrawPetConfig config = drawConfig with
            {
                AltTexture = tex,
            };
            DrawPetConfig config2 = config with
            {
                ShouldUseEntitySpriteDraw = !hasDye,
            };

            if (eyePositionOffset.Y <= 0)
                DrawEye(tex, eyePosition - Main.screenPosition, lightColor);

            if (yellowBlackDye)
                Projectile.DrawPet(10, lightColor,
                    config with
                    {
                        PositionOffset = new Vector2(-2 * Projectile.spriteDirection, 3f * Main.essScale)
                    }, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, config);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, config, 1);

            if (!hasDye)
            {
                Projectile.DrawPet(Projectile.frame, lightColor,
                config2 with
                {
                    AltTexture = cloth,
                });
            }
            Projectile.DrawPet(clothFrame, lightColor, config2, 1);
            Projectile.ResetDrawStateForPet();

            if (CurrentState == States.Annoying)
                Projectile.DrawPet(annoyingFrame, lightColor, config, 1);

            if (eyePositionOffset.Y > 0)
                DrawEye(tex, eyePosition - Main.screenPosition, lightColor);
            return false;
        }
        private void DrawEye(Texture2D tex, Vector2 eyePos, Color lightColor)
        {
            Texture2D t = tex ?? AltVanillaFunction.ProjectileTexture(Type);
            int height = t.Height / Main.projFrames[Type];
            Rectangle rect = new Rectangle(t.Width / 2, 7 * height, t.Width / 2, height);
            Vector2 orig = rect.Size() / 2;
            Main.spriteBatch.TeaNPCDraw(t, eyePos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
        }
        public override Color ChatTextColor => new Color(145, 255, 183);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Koishi";
            indexRange = new Vector2(1, 11);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 721;
            chance = 6;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                chat.Add(ChatDictionary[5]);
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            if (Projectile.isAPreviewDummy)
            {
                UpdateEyePosition();
            }
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(5))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Satori>();
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
            Projectile koishi = chatRoom.initiator;
            Projectile satori = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //恋恋：就算是姐姐，也不知道恋在想什么哦。
                satori.CloseCurrentDialog();

                if (koishi.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //觉：姐姐现在就在看着你呢...
                satori.SetChat(ChatSettingConfig, 5, 20);

                if (satori.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            if (!SetKoishiActive(Owner))
                return;

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Calling:
                    shouldNotTalking = true;
                    Calling();
                    break;

                case States.CallingFadeIn:
                    shouldNotTalking = true;
                    CallingFadeIn();
                    break;

                case States.CallingFadeOut:
                    shouldNotTalking = true;
                    CallingFadeOut();
                    break;

                case States.BeforeKill:
                    shouldNotTalking = true;
                    BeforeKill();
                    break;

                case States.KILL:
                    shouldNotTalking = true;
                    KILL();
                    break;

                case States.AfterKill:
                    shouldNotTalking = true;
                    AfterKill();
                    break;

                case States.Fading:
                    shouldNotTalking = true;
                    Fading();
                    break;

                case States.Annoying:
                    shouldNotTalking = true;
                    Annoying();
                    break;

                case States.AfterAnnoying:
                    shouldNotTalking = true;
                    AfterAnnoying();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }
            if (killCD > 0)
            {
                killCD--;
            }
            UpdateEyePosition();

            whiteDye =
                Owner.miscDyes[0].type == ItemID.SilverDye
                || Owner.miscDyes[0].type == ItemID.BrightSilverDye;

            yellowBlackDye = Owner.miscDyes[0].type == ItemID.YellowandBlackDye;
        }
        private void UpdateEyePosition()
        {
            float time = Main.GlobalTimeWrappedHourly * 2f;
            eyePositionOffset = new Vector2(1.2f * (float)Math.Cos(time), 0.35f * (float)Math.Sin(time)) * 26f;
            eyePosition = Projectile.Center + eyePositionOffset + new Vector2(0, 8);
        }
        private bool ShouldKillPlayer()
        {
            bool lowHealth = !Owner.dead && Owner.statLife < Owner.statLifeMax2 / 10;
            bool noSatori = !Owner.HasBuff<SatoriBuff>() && !Owner.HasBuff<KomeijiBuff>() && !Owner.HasBuff<KokoroBuff>();
            if (lowHealth && noSatori)
            {
                if (mainTimer > 0 && mainTimer % 120 == 0 && Main.rand.NextBool(3) && killCD <= 0)
                {
                    Timer = 0;
                    return true;
                }
            }
            return false;
        }
        private bool SetKoishiActive(Player player)
        {
            Projectile.timeLeft = 2;

            bool noActiveBuff = !player.HasBuff(BuffType<KoishiBuff>()) && !player.HasBuff(BuffType<KomeijiBuff>());
            bool shouldInactiveNormally = noActiveBuff && !IsKillingState;

            if (shouldInactiveNormally || player.dead)
            {
                Projectile.velocity *= 0;
                Projectile.frame = CurrentState == States.AfterKill ? 13 : 0;
                Projectile.Opacity -= 0.009f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (CurrentState != States.Fading && !IsKillingState)
                {
                    if (Projectile.Opacity < 1)
                        Projectile.Opacity += 0.009f;
                }
                return true;
            }
            return false;
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.01f;

            ChangeDir();

            Vector2 point = new Vector2(-54 * Owner.direction, -34 + Owner.gfxOffY);
            if (Owner.HasBuff<KomeijiBuff>())
                point = new Vector2(-44 * Owner.direction, -80 + Owner.gfxOffY);
            if (!Owner.dead)
                MoveToPoint(point, 13f);
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (ShouldKillPlayer())
                {
                    CurrentState = States.Calling;
                    return;
                }
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 360 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        if (Main.rand.NextBool(2)
                            && !FindPet(ProjectileType<Satori>(), false)
                            && !FindPet(ProjectileType<Kokoro>(), false))
                        {
                            RandomCount = Main.rand.Next(1800, 3600);
                            CurrentState = States.Fading;
                        }
                        else if (Projectile.CurrentDialogFinished())
                        {
                            RandomCount = Main.rand.Next(5, 14);
                            CurrentState = States.Annoying;
                        }
                    }
                }
            }
        }
        private void Blink()
        {
            Projectile.frame = 0;
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
        private void Fading()
        {
            Projectile.frame = 0;
            Timer++;
            if (Timer > RandomCount - 255 / 2)
            {
                if (Projectile.Opacity < 1)
                    Projectile.Opacity += 0.01f;
            }
            else
            {
                if (Projectile.Opacity > 0)
                    Projectile.Opacity -= 0.005f;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                ActionCD = 3600;
                CurrentState = States.Idle;
            }
        }
        private void Annoying()
        {
            if (++annoyingFrameCounter > 4)
            {
                annoyingFrameCounter = 0;
                annoyingFrame++;
            }
            if (annoyingFrame < 8)
            {
                annoyingFrame = 8;
            }
            if (annoyingFrame > 9)
            {
                annoyingFrame = 8;
            }

            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame < 14)
            {
                Projectile.frame = 14;
            }
            if (Projectile.frame > 16)
            {
                Projectile.frame = 15;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterAnnoying;
            }
        }
        private void AfterAnnoying()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 17)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1800;
                    CurrentState = States.Idle;
                }
            }
        }
        private void UpdateClothFrame()
        {
            int count = 4;
            if (++clothFrameCounter > count)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 3)
            {
                clothFrame = 0;
            }
        }
        private void Calling()
        {
            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 5;
            }

            if (Projectile.frame >= 5)
            {
                if (OwnerIsMyPlayer)
                {
                    ChatSettingConfig chatConfig = ChatSettingConfig with
                    {
                        TimeLeftPerWord = 60,
                    };
                    if (Timer == 0)
                    {
                        Projectile.SetChat(chatConfig, 6, 60);
                    }
                    if (Timer == 360)
                    {
                        Projectile.SetChat(chatConfig, 7, 60);
                    }
                    if (Timer >= 360 + 540)
                    {
                        Timer = 0;
                        CurrentState = States.CallingFadeIn;
                        Projectile.CloseCurrentDialog();
                        return;
                    }
                }
                Timer++;
            }
        }
        private void CallingFadeIn()
        {
            Projectile.frame = 6;
            Projectile.Opacity -= 0.01f;
            if (Projectile.Opacity <= 0f)
            {
                Projectile.Opacity = 0;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.CallingFadeOut;
                }
            }
        }
        private void CallingFadeOut()
        {
            if (!Owner.dead)
            {
                Projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, Owner.gfxOffY);
            }
            Projectile.frame = 7;
            Projectile.Opacity += 0.01f;
            if (Projectile.Opacity >= 1f)
            {
                Projectile.Opacity = 1;
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.BeforeKill;
                }
            }
        }
        private void BeforeKill()
        {
            if (!Owner.dead)
            {
                Projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, Owner.gfxOffY);
            }

            textShaking = true;

            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 9;
            }
            if (Projectile.frame > 8)
            {
                Timer++;
            }
            if (OwnerIsMyPlayer)
            {
                if (Projectile.frame == 8 && Projectile.frameCounter == 1)
                {
                    Projectile.SetChat(ChatSettingConfig with
                    {
                        TimeLeftPerWord = 45,
                        TyperModeUseTime = 300,
                    }, 8, 0, Color.Red);
                }
                if (Projectile.frame >= 9 && Timer > 540)
                {
                    Timer = 0;
                    CurrentState = States.KILL;
                }
            }
        }
        private void KILL()
        {
            if (!Owner.dead)
            {
                Projectile.Center = Owner.Center + new Vector2(-30 * Owner.direction, Owner.gfxOffY);
            }
            textShaking = true;

            if (++Projectile.frameCounter > 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            if (Projectile.frame > 12)
            {
                Projectile.frame = 13;
                if (OwnerIsMyPlayer)
                {
                    Owner.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.TouhouPets.DeathReason.KilledByKoishi", Owner.name)), 0, 0, false);
                    killCD = 3600;
                    CurrentState = States.AfterKill;
                    Projectile.CloseCurrentDialog();
                }
            }
        }
        private void AfterKill()
        {
            Projectile.frame = 13;
            if (!Owner.dead && OwnerIsMyPlayer)
            {
                CurrentState = States.Idle;
            }
        }
    }
}


