using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Alice : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = false;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Alice_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            DrawAura();
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig);

            Projectile.DrawPet(clothFrame, lightColor, config, 1);
            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
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
        private void Blink()
        {
            if (blinkFrame < 10)
            {
                blinkFrame = 10;
            }
            if (++blinkFrameCounter > 3)
            {
                blinkFrameCounter = 0;
                blinkFrame++;
            }
            if (blinkFrame > 12)
            {
                blinkFrame = 10;
                PetState = 0;
                Projectile.netUpdate = true;
            }
        }
        int auraFrame, auraFrameCounter;
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        private void Puppetia()
        {
            Projectile.velocity *= 0.8f;
            if (++Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.frame == 2 && Projectile.frameCounter % 4 == 0 && Projectile.frameCounter <= 7)
                {
                    float posX = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    float posY = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                            , new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), ProjectileType<AliceDoll_Proj>(), 0, 0
                            , Main.myPlayer, Projectile.whoAmI, posX, posY);
                    }
                }
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 3;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        extraAI[2] = Main.rand.Next(400, 600);
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (extraAI[0] == 1)
            {
                if (Projectile.frame == 6 && Projectile.frameCounter % 4 == 0 && Projectile.frameCounter <= 7)
                {
                    float posX = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    float posY = Main.rand.NextFloat(Main.rand.NextFloat(-210f, -90f), Main.rand.NextFloat(90f, 210f));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                            , new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f)), ProjectileType<AliceDoll_Proj>(), 0, 0
                            , Main.myPlayer, Projectile.whoAmI, posX, posY);
                    }
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 7;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    extraAI[1]++;
                    if (extraAI[1] > extraAI[2])
                    {
                        extraAI[1] = 0;
                        extraAI[0]++;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (Projectile.frame > 9)
                {
                    Projectile.frame = 0;
                    extraAI[0] = 240;
                    extraAI[2] = 0;
                    PetState = 0;
                }
            }
        }
        private void UpdateAuraFrame()
        {
            int count = 3;
            if (auraFrame < 4)
            {
                auraFrame = 4;
            }
            if (++auraFrameCounter > count)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 7)
            {
                auraFrame = 4;
            }
        }
        private void UpdateClothFrame()
        {
            int count = 6;
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
        public override Color ChatTextColor => new Color(185, 228, 255);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Alice";
            indexRange = new Vector2(1, 10);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 1200;
            chance = 4;
            whenShouldStop = false;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                if (FindPet(ProjectileType<Marisa>(), 0, 1, true))
                {
                    chat.Add(ChatDictionary[4]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateAuraFrame();
            UpdateClothFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(4, 7))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Marisa>();
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
            Projectile alice = chatRoom.initiator;
            Projectile marisa = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //爱丽丝：我说，你上次偷走的我的蘑菇什么时候能还我？
                marisa.CloseCurrentDialog();

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //魔理沙：哎呀，人家的事情那能叫偷嘛？那叫借啦！
                marisa.SetChat(ChatSettingConfig, 12, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //爱丽丝：别在这里耍嘴皮子了！给我个期限啊。
                alice.SetChat(ChatSettingConfig, 5, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 2)
            {
                //魔理沙：放心，死了以后保证还给你！
                marisa.SetChat(ChatSettingConfig, 13, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 3)
            {
                //爱丽丝：...你还是别还了...下次不许再偷了！
                alice.SetChat(ChatSettingConfig, 6, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 4)
            {
                //魔理沙：下次一定！
                marisa.SetChat(ChatSettingConfig, 14, 20);

                if (marisa.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 5)
            {
                //爱丽丝：你！......
                alice.SetChat(ChatSettingConfig, 7, 20);

                if (alice.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<AliceBuff>());
            UpdateTalking();
            Vector2 point = new Vector2(-50 * player.direction, -40 + player.gfxOffY);
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.006f;

            ChangeDir(true);
            MoveToPoint(point, 12f);
            if (OwnerIsMyPlayer)
            {
                if (mainTimer % 270 == 0 && PetState == 0)
                {
                    PetState = 1;
                    Projectile.netUpdate = true;
                }
                if (mainTimer >= 600 && mainTimer < 3600 && PetState <= 1 && extraAI[0] == 0)
                {
                    if (mainTimer % 600 == 0 && Main.rand.NextBool(12) && Projectile.velocity.Length() <= 3f)
                    {
                        PetState = 2;
                        extraAI[2] = Main.rand.Next(400, 600);
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
            else if (PetState == 2)
            {
                Puppetia();
            }
        }
    }
}


