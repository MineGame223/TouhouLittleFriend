using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Youmu : BasicTouhouPet
    {
        private enum States
        {
            Idle,
            Blink,
            FindEnemy,
            FindEnemyBlink,
            Afraid,
        }
        private States CurrentState
        {
            get => (States)PetState;
            set => PetState = (int)value;
        }
        private bool IsAfraid => CurrentState == States.Afraid;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override TouhouPetID UniqueID => TouhouPetID.Youmu;

        private int handFrame;
        private int blinkFrame, blinkFrameCounter;
        private int clothFrame, clothFrameCounter;
        private Vector2 shake;

        private DrawPetConfig drawConfig = new(2);
        private readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Youmu_Cloth");
        public override bool DrawPetSelf(ref Color lightColor)
        {
            drawConfig = drawConfig with
            {
                PositionOffset = shake,
            };
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            DrawPetConfig config2 = config with
            {
                AltTexture = clothTex,
            };

            for (int i = 0; i < 4; i++)
            {
                Color clr = Color.White * 0.8f * mouseOpacity;
                Projectile.DrawPet(8, clr,
                    drawConfig with
                    {
                        PositionOffset = new Vector2(Main.rand.Next(-10, 11) * 0.15f, Main.rand.Next(-10, 11) * 0.15f),
                        ShouldUseEntitySpriteDraw = true,
                    }, 1);
            }
            Projectile.ResetDrawStateForPet();

            if (Projectile.frame != 4)
                Projectile.DrawPet(10, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (CurrentState == States.Blink || CurrentState == States.FindEnemyBlink)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor, config2);
            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.DrawPet(clothFrame + 4, lightColor, config, 1);
            Projectile.ResetDrawStateForPet();

            if (Projectile.frame >= 1 && Projectile.frame <= 4)
            {
                Projectile.DrawPet(handFrame, lightColor, drawConfig);
                Projectile.DrawPet(handFrame, lightColor, config2);
            }
            return false;
        }
        public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
        {
            TextColor = new Color(188, 248, 248),
        };
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Youmu";
            indexRange = new Vector2(1, 11);
        }
        public override void PostRegisterChat()
        {
            Projectile.RegisterComment();
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = IsAfraid ? 500 : 900;
            chance = IsAfraid ? 3 : 12;
            whenShouldStop = false;
        }
        public override WeightedRandom<string> RegularDialogText()
        {
            WeightedRandom<string> chat = new ();
            {
                if (IsAfraid)
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                }
            }
            return chat;
        }
        public override void OnFindBoss(NPC boss)
        {
            if (Owner.HasBuff<YuyukoBuff>())
            {
                Projectile.SetChat(11);
            }
            else
            {
                if (!Projectile.GiveCertainBossComment(boss))
                {
                    Projectile.SetChat(3);
                }
            }
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
        }
        public override void SetPetLight(ref Vector2 position, ref Vector3 rgb, ref bool inactive)
        {
            rgb = new Vector3(1.83f, 2.02f, 1.99f) * 0.7f;
        }
        public override void AI()
        {
            Projectile.SetPetActive(Owner, BuffType<YoumuBuff>());

            ControlMovement();

            if (YoumuFindBoss())
            {
                CurrentState = States.FindEnemy;
            }
            shake = Vector2.Zero;

            switch (CurrentState)
            {
                case States.Blink:
                    Blink();
                    break;

                case States.FindEnemy:
                    shouldNotTalking = true;
                    FindEnemy();
                    break;

                case States.FindEnemyBlink:
                    shouldNotTalking = true;
                    FindEnemyBlink();
                    break;

                case States.Afraid:
                    shouldNotTalking = true;
                    Afraid();
                    break;

                default:
                    Idle();
                    break;
            }
        }
        private void ControlMovement()
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.001f;

            ChangeDir();

            Vector2 point = new Vector2(54 * Owner.direction, -34 + Owner.gfxOffY);
            if (FindPet(ProjectileType<Yuyuko>(), false))
            {
                point = new Vector2(-50 * Owner.direction, -50 + Owner.gfxOffY);
            }
            if (IsAfraid)
            {
                point = new Vector2(-30 * Owner.direction, -20 + Owner.gfxOffY);
            }
            MoveToPoint(point, 15f * (PetState == 2 ? 0.5f : 1));
        }
        private void Idle()
        {
            Projectile.frame = 0;
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    CurrentState = States.Blink;
                }
                if (Owner.ZoneGraveyard)
                {
                    CurrentState = States.Afraid;
                }
            }
        }
        private bool YoumuFindBoss()
        {
            bool hasBoss = false;
            if (Owner.active && !Owner.dead)
            {
                foreach (NPC b in Main.ActiveNPCs)
                {
                    if (b.boss && (b.target == Owner.whoAmI || Vector2.Distance(b.Center, Owner.Center) <= 1280))
                    {
                        hasBoss = true;
                        Projectile.spriteDirection = (b.position.X > Projectile.position.X) ? 1 : -1;
                    }
                }
            }
            return hasBoss;
        }
        private void Blink()
        {
            Projectile.frame = 0;
            int startFrame = 5;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = startFrame;
                CurrentState = States.Idle;
            }
        }
        private void FindEnemy()
        {
            Projectile.frame = 4;
            handFrame = 9;
            if (!YoumuFindBoss())
            {
                if (OwnerIsMyPlayer)
                {
                    CurrentState = States.Idle;
                }
            }
        }
        private void FindEnemyBlink()
        {
            int startFrame = 6;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 7)
            {
                blinkFrame = startFrame;
                CurrentState = States.FindEnemy;
            }
        }
        private void Afraid()
        {
            handFrame = 8;
            shake = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0);
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 180)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 1;
            }
            if (OwnerIsMyPlayer && !Owner.ZoneGraveyard)
            {
                CurrentState = States.Idle;
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
    }
}


