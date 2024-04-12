using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Patchouli : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Patchouli_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                AltTexture = clothTex,
            };
            DrawAura();

            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(clothFrame, lightColor, drawConfig, 1);
            Projectile.DrawPet(clothFrame, lightColor, config);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1 || PetState == 3)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    ShouldUseEntitySpriteDraw = true,
                });
            return false;
        }
        private void DrawAura()
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };
            Main.spriteBatch.QuickToggleAdditiveMode(true, Projectile.isAPreviewDummy);
            for (int i = 0; i < 8; i++)
            {
                Vector2 spinningpoint = new Vector2(0f, -1f);
                Projectile.DrawPet(auraFrame, Projectile.GetAlpha(Color.White) * 0.4f,
                    config with
                    {
                        PositionOffset = spinningpoint.RotatedBy(MathHelper.TwoPi * Main.GlobalTimeWrappedHourly
                        + MathHelper.TwoPi / 8 * i * 0.6f)
                    }
                    , 1);
            }
            Main.spriteBatch.QuickToggleAdditiveMode(false, Projectile.isAPreviewDummy);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(auraFrame, Projectile.GetAlpha(Color.White) * 0.4f, config, 1);
        }
        private void Reading()
        {
            if (Projectile.frame < 1)
            {
                Projectile.frame = 1;
            }
            if (++Projectile.frameCounter > 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] < 999)
            {
                if (Projectile.frame >= 3 && extraAI[1] < extraAI[0])
                {
                    extraAI[1]++;
                    Projectile.frame = 3;
                }
                if (Projectile.frame > 6)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        extraAI[0] = Main.rand.Next(360, 540);
                        extraAI[1] = 0;
                        Projectile.netUpdate = true;
                    }
                    Projectile.frame = 3;
                }
            }
            if (Projectile.velocity.Length() > 4.5f)
            {
                extraAI[0] = 999;
            }
            if (extraAI[0] >= 999)
            {
                if (Projectile.frame < 7)
                {
                    Projectile.frame = 7;
                }
                if (Projectile.frame > 8)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 180;
                    extraAI[1] = 0;
                    PetState = 0;
                }
            }
        }
        private void Blink(bool alt = false)
        {
            if (++blinkFrameCounter > 6)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (alt)
            {
                if (blinkFrame < 10)
                {
                    blinkFrame = 10;
                }
                if (blinkFrame > 11)
                {
                    blinkFrame = 10;
                    PetState = 2;
                }
            }
            else
            {
                if (blinkFrame < 9)
                {
                    blinkFrame = 9;
                }
                if (blinkFrame > 11)
                {
                    blinkFrame = 9;
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int auraFrame, auraFrameCounter;
        private void UpdateClothFrame()
        {
            if (clothFrame < 12)
            {
                clothFrame = 12;
            }
            if (++clothFrameCounter > 10)
            {
                clothFrameCounter = 0;
                clothFrame++;
            }
            if (clothFrame > 15)
            {
                clothFrame = 12;
            }
        }
        private void UpdateAuraFrame()
        {
            if (++auraFrameCounter > 3)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 4)
            {
                auraFrame = 0;
            }
        }
        public override Color ChatTextColor => new Color(252, 197, 238);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Patchouli";
            indexRange = new Vector2(1, 35);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 720;
            chance = 12;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if (Projectile.velocity.Length() >= 4f)
                {
                    chat.Add(ChatDictionary[1]);
                }
                if (PetState > 1)
                {
                    chat.Add(ChatDictionary[2]);
                    chat.Add(ChatDictionary[3]);
                    chat.Add(ChatDictionary[4]);
                    chat.Add(ChatDictionary[5]);
                    if (FindPetState(ProjectileType<Remilia>(), 0, 1) && !Main.dayTime)
                    {
                        chat.Add(ChatDictionary[8]);
                    }
                }
                else
                {
                    chat.Add(ChatDictionary[7]);
                }
                chat.Add(ChatDictionary[6]);
                if (FindPetState(ProjectileType<Alice>()))
                {
                    chat.Add(ChatDictionary[12]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateClothFrame();
            UpdateAuraFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(8, 11))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
            if (FindChatIndex(12, 15))
            {
                Chatting2(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Remilia>();
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
            Projectile patchouli = chatRoom.initiator;
            Projectile remilia = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                remilia.CloseCurrentDialog();

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                remilia.SetChat(ChatSettingConfig, 10, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                patchouli.SetChat(ChatSettingConfig, 9, 20);

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                remilia.SetChat(ChatSettingConfig, 11, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                patchouli.SetChat(ChatSettingConfig, 10, 20);

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                remilia.SetChat(ChatSettingConfig, 12, 20);

                if (remilia.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 5)
            {
                patchouli.SetChat(ChatSettingConfig, 11, 20);

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void Chatting2(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Alice>();
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
            Projectile patchouli = chatRoom.initiator;
            Projectile alice = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                alice.CloseCurrentDialog();

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                alice.SetChat(ChatSettingConfig, 8, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                patchouli.SetChat(ChatSettingConfig, 13, 20);

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                alice.SetChat(ChatSettingConfig, 9, 20);
                patchouli.SetChat(ChatSettingConfig, 14, 20);

                if (alice.CurrentDialogFinished())
                {
                    chatRoom.chatTurn++;
                }
            }
            else if (turn == 3)
            {
                patchouli.CloseCurrentDialog();
                alice.SetChat(ChatSettingConfig, 10, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                patchouli.SetChat(ChatSettingConfig, 15, 20);

                if (patchouli.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.012f;

            ChangeDir();

            Vector2 point = new Vector2(50 * player.direction, -20 + player.gfxOffY);
            if (player.HasBuff<ScarletBuff>())
            {
                point = new Vector2(0, -120 + player.gfxOffY);
            }

            MoveToPoint(point, 4.5f);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 2.52f, 1.97f, 2.38f);
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<PatchouliBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (mainTimer % 270 == 0)
                {
                    if (PetState <= 1)
                    {
                        PetState = 1;
                    }
                    else
                    {
                        PetState = 3;
                    }
                    Projectile.netUpdate = true;
                }
                if (PetState == 0)
                {
                    if (mainTimer % 180 == 0 && Main.rand.NextBool(4) && extraAI[0] <= 0 && Projectile.velocity.Length() < 2f)
                    {
                        extraAI[0] = Main.rand.Next(360, 540);
                        PetState = 2;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (PetState == 0)
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
            else
            {
                Reading();
                if (PetState == 1)
                {
                    PetState = 3;
                }
                if (PetState == 3)
                {
                    Blink(true);
                }
            }
        }
    }
}


