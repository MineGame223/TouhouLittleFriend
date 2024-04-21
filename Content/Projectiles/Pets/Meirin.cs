using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Projectiles.Pets
{
    public class Meirin : BasicTouhouPetNeo
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 24;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        DrawPetConfig drawConfig = new(2);
        readonly Texture2D clothTex = AltVanillaFunction.GetExtraTexture("Meirin_Cloth");
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPetConfig config = drawConfig with
            {
                ShouldUseEntitySpriteDraw = true,
            };

            Projectile.DrawPet(auraFrame, Color.White, config, 1);
            Projectile.DrawStateNormalizeForPet();

            Projectile.DrawPet(hairFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = hairPosOffset,
                }, 1);

            Projectile.DrawPet(Projectile.frame, lightColor, drawConfig);

            if (PetState == 1 || PetState == 4)
                Projectile.DrawPet(blinkFrame, lightColor, drawConfig, 1);

            Projectile.DrawPet(Projectile.frame, lightColor,
                config with
                {
                    AltTexture = clothTex,
                });
            Projectile.DrawPet(clothFrame, lightColor,
                drawConfig with
                {
                    PositionOffset = clothPosOffset,
                }, 1);

            if (Projectile.frame == 23)
            {
                DrawUmbrella(lightColor);
            }
            return false;
        }
        private void DrawUmbrella(Color lightColor)
        {
            int type = ItemID.Umbrella;
            Main.instance.LoadItem(type);
            Texture2D tex = AltVanillaFunction.ItemTexture(type);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(17 * Projectile.spriteDirection, -20) + new Vector2(0, 7f * Main.essScale);
            Color clr = Projectile.GetAlpha(lightColor);
            Vector2 orig = tex.Size() / 2;
            Main.EntitySpriteDraw(tex, pos, null, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
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
                if (PetState == 4)
                {
                    PetState = 3;
                }
                else
                {
                    PetState = 0;
                }
            }
        }
        int blinkFrame, blinkFrameCounter;
        int clothFrame, clothFrameCounter;
        int hairFrame, hairFrameCounter;
        int auraFrame, auraFrameCounter;
        Vector2 clothPosOffset;
        Vector2 hairPosOffset;
        private void Kongfu()
        {
            if (Projectile.frame < 6)
            {
                Projectile.frame = 6;
            }

            var count = Projectile.frame switch
            {
                9 => 120,
                13 => 120,
                17 => 120,
                19 => 15,
                20 => 20,
                21 => 20,
                22 => 180,
                _ => 10,
            };
            if (++Projectile.frameCounter > count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.rand.NextBool(25) && extraAI[1] <= 0)
                    {
                        Projectile.SetChat(ChatSettingConfig, 5);
                        extraAI[1]++;
                    }
                }
            }
            if (extraAI[0] == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Projectile.frame > 22)
                    {
                        Projectile.frame = 0;
                        extraAI[0] = 1200;
                        extraAI[1] = 0;
                        PetState = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        private void Serve()
        {
            Projectile.frame = 23;
            if (!FindPet(ProjectileType<Flandre>(), false) || !Remilia.HateSunlight(Projectile))
            {
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        private void Sleep()
        {
            Player player = Main.player[Projectile.owner];
            if (extraAI[0] == 0)
            {
                Projectile.velocity *= 0.5f;
                Projectile.frame = 1;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (++extraAI[1] >= extraAI[2])
                    {
                        extraAI[0]++;
                        extraAI[1] = 0;
                        extraAI[2] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0.1f;
                Projectile.frame = 2;
                if (Projectile.owner == Main.myPlayer)
                {
                    if (player.velocity.Length() > 6f)
                    {
                        PetState = 0;
                        extraAI[0] = 1200;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        private void GetHurt()
        {
            if (Projectile.frame < 3)
            {
                Projectile.frame = 3;
            }
            var count = Projectile.frame switch
            {
                3 => 60,
                _ => 10,
            };
            if (++Projectile.frameCounter >= count)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                extraAI[1]++;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 4;
            }
            if (extraAI[1] > 10)
            {
                extraAI[1] = 0;
                extraAI[0] = 1200;
                Projectile.frame = 0;
                PetState = 0;
            }
        }
        private void UpdateMiscFrame()
        {
            if (clothFrame < 3)
            {
                clothFrame = 3;
            }
            if (++clothFrameCounter > 6)
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
            if (++hairFrameCounter > 8)
            {
                hairFrameCounter = 0;
                hairFrame++;
            }
            if (hairFrame > 10)
            {
                hairFrame = 7;
            }

            if (auraFrame < 11)
            {
                auraFrame = 11;
            }
            if (++auraFrameCounter > 7)
            {
                auraFrameCounter = 0;
                auraFrame++;
            }
            if (auraFrame > 18)
            {
                auraFrame = 11;
            }
        }
        public override Color ChatTextColor => new Color(255, 81, 81);
        public override void RegisterChat(ref string name, ref Vector2 indexRange)
        {
            name = "Meirin";
            indexRange = new Vector2(1, 15);
        }
        public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
        {
            timePerDialog = 960;
            chance = Owner.HasBuff<ScarletBuff>() ? 30 : 5;
            whenShouldStop = PetState > 1;
        }
        public override string GetRegularDialogText()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                chat.Add(ChatDictionary[1]);
                chat.Add(ChatDictionary[2]);
                chat.Add(ChatDictionary[3]);
                chat.Add(ChatDictionary[4]);
                if (FindPet(ProjectileType<Sakuya>()))
                {
                    chat.Add(ChatDictionary[10]);
                    chat.Add(ChatDictionary[13]);
                }
            }
            return chat;
        }
        public override void VisualEffectForPreview()
        {
            UpdateMiscFrame();
        }
        private void UpdateTalking()
        {
            if (FindChatIndex(10, 15))
            {
                Chatting1(currentChatRoom ?? Projectile.CreateChatRoomDirect(), chatIndex);
            }
            if (FindChatIndex(5, 6))
            {
                Chatting2(currentChatRoom ?? Projectile.CreateChatRoomDirect());
            }
        }
        private void Chatting1(PetChatRoom chatRoom, int index)
        {
            int type = ProjectileType<Sakuya>();
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
            Projectile meirin = chatRoom.initiator;
            Projectile sakuya = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (index >= 10 && index <= 12)
            {
                if (turn == -1)
                {
                    //美铃：咲夜小姐每天那么忙，有过休假的时候吗？
                    sakuya.CloseCurrentDialog();

                    if (meirin.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //咲夜：和大小姐在一起的每一天都是休假，你不也没有什么“假期”么？
                    sakuya.SetChat(ChatSettingConfig, 6, 20);

                    if (sakuya.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //美铃：这个工作和休假没啥区别啊...
                    meirin.SetChat(ChatSettingConfig, 11, 20);

                    if (meirin.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 2)
                {
                    //咲夜：什么？
                    sakuya.SetChat(ChatSettingConfig, 7, 20);

                    if (sakuya.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 3)
                {
                    //美铃：没！没什么...
                    meirin.SetChat(ChatSettingConfig, 12, 20);

                    if (meirin.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
            else if (index == 13 || index == 15)
            {
                if (turn == -1)
                {
                    //美铃：咲夜小姐，我最近发现了一本讲保安和女仆谈恋爱的漫画欸！
                    sakuya.CloseCurrentDialog();

                    if (meirin.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 0)
                {
                    //咲夜：站岗期间你看漫画？
                    sakuya.SetChat(ChatSettingConfig, 8, 20);

                    if (sakuya.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 1)
                {
                    //美铃：啊，糟了！偷懒的事暴露了...
                    meirin.SetChat(ChatSettingConfig, 14, 20);

                    if (meirin.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 2)
                {
                    //咲夜：看我晚上不好好收拾你！...书记得给我看看...
                    sakuya.SetChat(ChatSettingConfig, 9, 20);

                    if (sakuya.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else if (turn == 3)
                {
                    //美铃：呜呜呜...欸？
                    meirin.SetChat(ChatSettingConfig, 15, 20);

                    if (meirin.CurrentDialogFinished())
                        chatRoom.chatTurn++;
                }
                else
                {
                    chatRoom.CloseChatRoom();
                }
            }
        }
        private void Chatting2(PetChatRoom chatRoom)
        {
            int type = ProjectileType<Flandre>();
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
            Projectile meirin = chatRoom.initiator;
            Projectile flandre = chatRoom.member[0];
            int turn = chatRoom.chatTurn;
            if (turn == -1)
            {
                //美铃：易有太极，是生两仪...
                flandre.CloseCurrentDialog();

                if (meirin.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 0)
            {
                //芙兰：美铃在说什么？
                flandre.SetChat(ChatSettingConfig, 9, 20);

                if (flandre.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else if (turn == 1)
            {
                //美铃：是我的家乡话哦，二小姐。
                meirin.SetChat(ChatSettingConfig, 6, 20);

                if (meirin.CurrentDialogFinished())
                    chatRoom.chatTurn++;
            }
            else
            {
                chatRoom.CloseChatRoom();
            }
        }
        private void SetMeirinLight()
        {
            float r = Main.DiscoR / 255f;
            float g = Main.DiscoG / 255f;
            float b = Main.DiscoB / 255f;
            float strength = 2f;
            r = (strength + r) / 1.5f;
            g = (strength + g) / 1.5f;
            b = (strength + b) / 1.5f;
            Lighting.AddLight(Projectile.Center, r, g, b);
            Lighting.AddLight(Projectile.Center, 0.40f, 0.31f, 0.48f);
        }
        private void ControlMovement(Player player)
        {
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.X * 0.003f;

            ChangeDir(120);

            Vector2 point = new Vector2((player.HasBuff<ScarletBuff>() ? -100 : 50) * player.direction, -30 + player.gfxOffY);
            Vector2 center = default;
            float speed = 15f;
            if (FindPet(out Projectile master, ProjectileType<Flandre>()))
            {
                Projectile.spriteDirection = master.spriteDirection;
                if (PetState == 3 || PetState == 4)
                {
                    point = new Vector2((-25 - 60) * master.spriteDirection, player.gfxOffY - 20);
                    speed = 19f;
                }
            }
            MoveToPoint(point, speed, center);
        }
        public override void AI()
        {
            SetMeirinLight();
            Player player = Main.player[Projectile.owner];
            Projectile.SetPetActive(player, BuffType<MeirinBuff>());
            Projectile.SetPetActive(player, BuffType<ScarletBuff>());

            UpdateTalking();
            ControlMovement(player);

            if (Projectile.owner == Main.myPlayer)
            {
                if (Remilia.HateSunlight(Projectile) && PetState != 3 && PetState != 4
                    && FindPet(ProjectileType<Flandre>(), false))
                {
                    PetState = 3;
                    Projectile.netUpdate = true;
                }
                if (PetState == 3)
                {
                    if (mainTimer % 270 == 0)
                    {
                        PetState = 4;
                        Projectile.netUpdate = true;
                    }
                }
                if (PetState <= 0)
                {
                    if (mainTimer % 270 == 0)
                    {
                        PetState = 1;
                        Projectile.netUpdate = true;
                    }
                    if (mainTimer >= 10 && mainTimer < 3600)
                    {
                        if (mainTimer % 480 == 0 && Main.rand.NextBool(9) && extraAI[0] <= 0 && player.velocity.Length() <= 5f)
                        {
                            bool chance = !player.HasBuff<ScarletBuff>();
                            extraAI[1] = 0;
                            if (chance)
                            {
                                extraAI[2] = Main.rand.Next(120, 540);
                                PetState = 5;
                            }
                            else
                            {
                                PetState = 2;
                            }
                            Projectile.netUpdate = true;
                        }
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
                Blink();
                Projectile.frame = 0;
            }
            else if (PetState == 2)
            {
                Kongfu();
            }
            else if (PetState == 3 || PetState == 4)
            {
                if (PetState == 4)
                    Blink();
                Serve();
            }
            else if (PetState == 5)
            {
                Sleep();
            }
            else if (PetState == 6)
            {
                GetHurt();
            }
            clothPosOffset = Projectile.frame switch
            {
                7 => new Vector2(2, 0),
                8 => new Vector2(2, 0),
                9 => new Vector2(2, -2),
                10 => new Vector2(2, 0),
                11 => new Vector2(2, 0),
                13 => new Vector2(0, 2),
                14 => new Vector2(0, 2),
                15 => new Vector2(0, 2),
                16 => new Vector2(2, 2),
                17 => new Vector2(4, 2),
                18 => new Vector2(2, 2),
                19 => new Vector2(0, 2),
                21 => new Vector2(0, -2),
                _ => Vector2.Zero,
            };
            clothPosOffset.X *= -Projectile.spriteDirection;
            hairPosOffset = Projectile.frame switch
            {
                2 => new Vector2(0, 2),
                3 => new Vector2(0, 4),
                7 => new Vector2(2, 0),
                8 => new Vector2(2, 0),
                9 => new Vector2(2, -2),
                10 => new Vector2(2, 0),
                11 => new Vector2(2, 0),
                13 => new Vector2(0, 2),
                14 => new Vector2(0, 2),
                15 => new Vector2(0, 2),
                16 => new Vector2(2, 2),
                17 => new Vector2(4, 2),
                18 => new Vector2(2, 2),
                19 => new Vector2(0, 2),
                21 => new Vector2(0, -2),
                _ => Vector2.Zero,
            };
            hairPosOffset.X *= -Projectile.spriteDirection;
        }
    }
}


