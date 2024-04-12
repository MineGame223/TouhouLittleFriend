using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Hina : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 17;
            Main.projPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(1);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Hina_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);
            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);
            Projectile.DrawPet(Projectile.frame, lightColor,
                drawConfig with
                {
                    AltTexture = clothTex,
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        private void Blink()
        {
            if (blinkFrame < 14)
            {
                blinkFrame = 14;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 16)
            {
                blinkFrame = 14;
                PetState = 0;
            }
        }
        private void Idle()
        {
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }
        }
        int blinkFrame, blinkFrameCounter;
        private void Turning()
        {
            if (Projectile.frame < 6)
            {
                Projectile.frame = 6;
            }
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame > 13)
                {
                    Projectile.frame = 6;
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
                if (Projectile.frame > 13)
                {
                    extraAI[2] = 0;
                    Projectile.frame = 0;
                    extraAI[0] = 300;
                    PetState = 0;
                }
            }
        }
        public override Color ChatTextColor => new Color(70, 226, 164);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Hina";
            indexRange = new Vector2(1, 8);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 6;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                if (FindPetState(ProjectileType<Nitori>(), 0, 1))
                {
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[7]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            if (PetState != 2)
                Idle();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(4, 8))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
        }
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Nitori>();
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
            Projectile hina = chatRoom.initiator;
            Projectile nitori = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index >= 4 && index <= 6)
            {
                if (turn == -1)
                {
                    nitori.CloseCurrentDialog();

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    nitori.SetChat(ChatSettingConfig, 4, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    hina.SetChat(ChatSettingConfig, 5, 20);

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 2)
                {
                    nitori.SetChat(ChatSettingConfig, 5, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 3)
                {
                    hina.SetChat(ChatSettingConfig, 6, 20);

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 4)
                {
                    nitori.SetChat(ChatSettingConfig, 6, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
            else if (index == 7 || index == 8)
            {
                if (turn == -1)
                {
                    nitori.CloseCurrentDialog();

                    if (hina.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    nitori.SetChat(ChatSettingConfig, 7, 20);

                    if (nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    nitori.SetChat(ChatSettingConfig, 8, 20);
                    hina.SetChat(ChatSettingConfig, 8, 20);

                    if (hina.CurrentDialogFinished() || nitori.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<HinaBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-40 * player.direction, -30 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.032f;
            if (PetState != 2)
            {
                ChangeDir(true);
            }

            MoveToPoint(point, 13f);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0 && PetState != 2)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 1200 && mainTimer < 3600 && PetState == 0)
                {
                    if (mainTimer % 200 == 0 && Main.rand.NextBool(2) && extraAI[0] <= 0)
                    {
                        PetState = 2;
                        Projectile.netUpdate = true;
                        if (Main.rand.NextBool(3) && currentChatRoom == null && chatTimeLeft <= 0)
                            Projectile.SetChat(ChatSettingConfig, 3, 20);
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
                Turning();
            }
        }
    }
}


