using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Sizuha : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Sizuha_Cloth");
        readonly Texture2D glowTex = AltVanillaFunction.GetGlowTexture("Sizuha_Glow");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, Color.White * 0.7f,
                drawConfig with
                {
                    AltTexture = glowTex,
                });
            return false;
        }
        private void Blink()
        {
            int startFrame = Owner.ZoneSnow ? 9 : 8;
            if (blinkFrame < startFrame)
            {
                blinkFrame = startFrame;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 10)
            {
                blinkFrame = startFrame;
                PetState = 0;
            }
        }
        private void UpdateClothFrame()
        {
            if (clothFrame < 4)
            {
                clothFrame = 4;
            }
            if (++clothFrameCounter > 6)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 7)
            {
                clothFrame = 4;
            }
        }
        private void Idle()
        {
            if (Owner.ZoneSnow)
            {
                Projectile.frame = 11;
                return;
            }
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        public override Color ChatTextColor => new Color(244, 150, 91);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Sizuha";
            indexRange = new Vector2(1, 9);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 8;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Owner.ZoneSnow)
                {
                    chat.Add(ChatDictionary[9]);
                }
                else
                {
                    chat.Add(ChatDictionary[1]);
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    chat.Add(ChatDictionary[6]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            Idle();
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(6, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Minoriko>();
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
            Projectile sizuha = chatRoom.initiator;
            Projectile minoriko = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                minoriko.CloseCurrentDialog();

                if (sizuha.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                minoriko.SetChat(ChatSettingConfig, 6, 20);

                if (minoriko.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                sizuha.SetChat(ChatSettingConfig, 7, 20);

                if (sizuha.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                minoriko.SetChat(ChatSettingConfig, 7, 20);

                if (minoriko.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                sizuha.SetChat(ChatSettingConfig, 8, 20);

                if (sizuha.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.61f, 0.98f, 0.58f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<SizuhaBuff>());

            UpdateTalking();

            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.02f;
            ChangeDir();

            MoveToPoint(point, 10f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.rand.NextBool(16) && Projectile.velocity.Length() > 3f)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(0, 50))
                                , new Vector2(0, Main.rand.NextFloat(0.3f, 0.4f)), ProjectileType<SizuhaLeaf>(), 0, 0, Main.myPlayer, Main.rand.Next(0, 3));
                }

                if (mainTimer % 270 == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
            }
            if (PetState == 1)
            {
                Blink();
            }
            if (Owner.ZoneSnow)
            {
                Projectile.frame = 0;
                if (blinkFrame < 9)
                {
                    blinkFrame = 9;
                }
            }
        }
    }
}


