using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Marisa : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Marisa_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(broomFrame, lightColor, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(lightFrame, Color.White, drawConfig, 1);
            return false;
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
                PetState = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        int broomFrame, broomFrameCounter;
        int lightFrame, lightFrameCounter;
        private void Happy()
        {
            if (Projectile.frame < 4)
            {
                Projectile.frame = 4;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 11)
                {
                    Gore.NewGoreDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -27), new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)) * 0.1f, Main.rand.Next(570, 573), Main.rand.NextFloat(0.9f, 1.1f));
                    Projectile.frame = 4;
                    extraAI[1]++;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    if (extraAI[1] > Main.rand.Next(10, 20))
                    {
                        extraAI[1] = 0;
                        extraAI[0] = 1;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 11)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 3600;
                    PetState = 0;
                }
            }
        }
        private void Idel()
        {
            if (++Projectile.frameCounter > 6)
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
            if (broomFrame < 4)
            {
                broomFrame = 4;
            }
            if (++broomFrameCounter > 6)
            {
                broomFrameCounter = 0;
                broomFrame++;
            }
            if (broomFrame > 7)
            {
                broomFrame = 4;
            }

            if (++lightFrameCounter > 10)
            {
                lightFrameCounter = 0;
                lightFrame++;
            }
            if (lightFrame > 3)
            {
                lightFrame = 0;
            }
        }
        public override Color ChatTextColor => new Color(255, 249, 137);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Marisa";
            indexRange = new Vector2(1, 14);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 5;
            whenShouldStop = PetState == 2;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (Owner.ZoneForest && Main.cloudAlpha == 0 && !Main.bloodMoon)
                {
                    if (Main.dayTime)
                        chat.Add(ChatDictionary[4]);
                    else
                        chat.Add(ChatDictionary[5]);
                }
                if (Main.bloodMoon || Main.eclipse)
                {
                    chat.Add(ChatDictionary[6]);
                }
                if (FindPetState(ProjectileType<Reimu>(), 0, 1)
                    || FindPetState(ProjectileType<Reimu>(), 3, 4))
                {
                    chat.Add(ChatDictionary[7]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
            if (PetState != 2)
            {
                Idel();
            }
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(7, 11))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Reimu>();
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
            Projectile marisa = chatRoom.initiator;
            Projectile reimu = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                reimu.CloseCurrentDialog();

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                reimu.SetChat(ChatSettingConfig, 5, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                marisa.SetChat(ChatSettingConfig, 8, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                reimu.SetChat(ChatSettingConfig, 6, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                marisa.SetChat(ChatSettingConfig, 9, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                reimu.SetChat(ChatSettingConfig, 7, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 5)
            {
                marisa.SetChat(ChatSettingConfig, 10, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 6)
            {
                reimu.SetChat(ChatSettingConfig, 8, 20);

                if (reimu.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 7)
            {
                marisa.SetChat(ChatSettingConfig, 11, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.95f, 1.90f, 1.03f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MarisaBuff>());

            UpdateTalking();
            Vector2 point = new Vector2(50 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();
            MoveToPoint(point, 12.5f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState == 0 && Projectile.frame == 3)
                {
                    if (mainTimer % 120 == 0 && Main.rand.NextBool(6) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 0)
            {
                if (extraAI[0] >= 1)
                {
                    extraAI[0]--;
                }
            }
            else if (PetState == 1)
            {
                Blink();
            }
            else if (PetState == 2)
            {
                Happy();
            }
        }
    }
}


