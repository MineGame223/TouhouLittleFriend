using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Iku : BasicTouhouPetNeo
    {
        private enum States
        {
            Idle,
            Blink,
            Spinning,
            Discharging,
            AfterDischarging,
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
        private bool InLunarInvasion => NPC.AnyNPCs(NPCID.LunarTowerStardust) || NPC.AnyNPCs(NPCID.LunarTowerSolar) ||
                    NPC.AnyNPCs(NPCID.LunarTowerNebula) || NPC.AnyNPCs(NPCID.LunarTowerVortex);

        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Iku_Cloth");
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = new Vector2(0, 3f * Main.essScale),
                    ShouldUseEntitySpriteDraw = true,
                }, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        public override Color ChatTextColor => new Color(79, 215, 239);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Iku";
            indexRange = new Vector2(1, 19);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 600;
            chance = 3;
            whenShouldStop = !IsIdleState;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Main.bloodMoon)
                {
                    chat.Add(ChatDictionary[1]);
                }
                if (Main.eclipse)
                {
                    chat.Add(ChatDictionary[2]);
                }
                if (!Main.eclipse && !Main.bloodMoon)
                {
                    if (Sandstorm.Happening && Owner.ZoneDesert && Owner.ZoneOverworldHeight)
                    {
                        chat.Add(ChatDictionary[3]);
                    }
                    else if (Main.slimeRain)
                    {
                        chat.Add(ChatDictionary[4]);
                    }
                    else if (!Owner.AnyBosses())
                    {
                        chat.Add(ChatDictionary[5]);
                        if (BirthdayParty.PartyIsUp)
                        {
                            chat.Add(ChatDictionary[6]);
                        }
                        if (LanternNight.LanternsUp)
                        {
                            chat.Add(ChatDictionary[7]);
                        }
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) <= 0.1f)
                    {
                        chat.Add(ChatDictionary[8]);
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) > 0.1f
                       && Math.Abs(Main.windSpeedTarget) < 0.25f)
                    {
                        chat.Add(ChatDictionary[9]);
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) > .25f
                       && Math.Abs(Main.windSpeedTarget) < 0.4f)
                    {
                        chat.Add(ChatDictionary[10]);
                    }
                    if (Main.cloudAlpha == 0 && Math.Abs(Main.windSpeedTarget) >= .4f)
                    {
                        chat.Add(ChatDictionary[11]);
                    }
                    if (Main.cloudAlpha > 0f && Main.cloudAlpha < 0.3f)
                    {
                        chat.Add(ChatDictionary[12]);
                    }
                    if (Main.cloudAlpha >= 0.4f && Math.Abs(Main.windSpeedTarget) >= 0.3f
                        && Main.cloudAlpha < 0.5f && Math.Abs(Main.windSpeedTarget) < 0.4f)
                    {
                        chat.Add(ChatDictionary[13]);
                    }
                    if (Main.cloudAlpha >= 0.5f && Math.Abs(Main.windSpeedTarget) >= 0.4f
                        && Main.cloudAlpha < 0.8f && Math.Abs(Main.windSpeedTarget) < 0.65f)
                    {
                        chat.Add(ChatDictionary[14]);
                    }
                    if (Main.cloudAlpha >= 0.8f && Math.Abs(Main.windSpeedTarget) >= 0.65f)
                    {
                        chat.Add(ChatDictionary[15]);
                    }
                }
                if (Main.pumpkinMoon)
                {
                    chat.Add(ChatDictionary[17]);
                }
                if (Main.snowMoon)
                {
                    chat.Add(ChatDictionary[18]);
                }
                if (InLunarInvasion)
                {
                    chat.Add(ChatDictionary[19]);
                }
            }
            return chat;
        }
        private void UpdateTalking()
        {
        }
        public override void VisualEffectForPreview()
        {
            if (IsIdleState)
            {
                IdleAnimation();
            }
            UpdateClothFrame();
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<IkuBuff>());

            UpdateTalking();

            ControlMovement();

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.Spinning:
                    shouldNotTalking = true;
                    Spinning();
                    break;

                case States.Discharging:
                    shouldNotTalking = true;
                    Discharging();
                    break;

                case States.AfterDischarging:
                    shouldNotTalking = true;
                    AfterDischarging();
                    break;

                default:
                    Idle();
                    break;
            }

            if (IsIdleState && ActionCD > 0)
            {
                ActionCD--;
            }

            Lighting.AddLight(Projectile.Center, 0.79f, 2.15f, 2.39f);
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();

            Vector2 point = new Vector2(45 * Owner.direction, -45 + Owner.gfxOffY);
            if (FindPet(ProjectileType<Tenshi>(), false))
            {
                point = new Vector2(-45 * Owner.direction, -45 + Owner.gfxOffY);
            }
            MoveToPoint(point, 15f);
        }
        private void Idle()
        {
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (mainTimer > 0 && mainTimer % 610 == 0 && currentChatRoom == null && ActionCD <= 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        RandomCount = Main.rand.Next(7, 14);
                        CurrentState = States.Spinning;
                    }
                }
            }
        }
        private void Blink()
        {
            if (blinkFrame < 12)
            {
                blinkFrame = 12;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 14)
            {
                blinkFrame = 12;
                CurrentState = States.Idle;
            }
        }
        private void Spinning()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 6;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                RandomCount = Main.rand.Next(30, 50);
                CurrentState = States.Discharging;
            }
        }
        private void Discharging()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 10)
            {
                Projectile.frame = 9;
                Timer++;
            }
            if (OwnerIsMyPlayer && Timer > RandomCount)
            {
                Timer = 0;
                CurrentState = States.AfterDischarging;
            }
            if (Projectile.frame >= 9)
            {
                Dust.NewDustDirect(Projectile.Center + new Vector2(-16 * Projectile.spriteDirection, -10)
                    , 1, 1, Main.rand.NextBool(2) ? MyDustId.TrailingCyan : MyDustId.ElectricCyan,
                    Main.rand.Next(-4, 4), Main.rand.Next(-6, -1)
                    , 100, Color.White, Main.rand.NextFloat(0.3f, 1.2f)).noGravity = false;
            }
        }
        private void AfterDischarging()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 11)
            {
                Projectile.frame = 0;
                if (OwnerIsMyPlayer)
                {
                    ActionCD = 1000;
                    CurrentState = States.Idle;
                }
            }
        }
        private void IdleAnimation()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 0)
            {
                clothFrame = 0;
            }
            if (++clothFrameCounter > 6)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 0)
            {
                clothFrame = 0;
            }
        }
    }
}


