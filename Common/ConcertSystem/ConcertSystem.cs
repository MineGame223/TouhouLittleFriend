using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using TouhouPets.Content.Items;
using static TouhouPets.CustomMusicManager;

namespace TouhouPets
{
    public class ConcertSystem : ModSystem
    {
        private static float buttonOpacity = 0f;
        public override void OnModLoad()
        {
            if (GetInstance<MiscConfig>().EnableCustomMusicMode && Main.netMode == NetmodeID.SinglePlayer)
            {
                EnsureMusicFolder();
                Initialize(FullPath);
            }
            else
            {
                Console.WriteLine("由于未开启设置或正处于多人模式，已跳过音乐文件加载环节。");
            }
        }
        public override void ModifyScreenPosition()
        {
            GuaranteedUpdate();
        }
        public override void PreSaveAndQuit()
        {
            Stop();
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int concertIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Emote Bubbles"));
            if (concertIndex != -1)
            {
                layers.Insert(concertIndex, new LegacyGameInterfaceLayer(
                    "TouhouPets : ConcertUI",
                    delegate
                    {
                        DrawConcertUI();
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }
        private static void DrawConcertUI()
        {
            bool[] buttonDisable = new bool[3];
            Player player = Main.LocalPlayer;
            ConcertPlayer bp = player.GetModPlayer<ConcertPlayer>();

            if (bp.IsConcertStarted && player.HeldItem.type == ItemType<SupportStick>())
            {
                buttonOpacity = Math.Clamp(buttonOpacity += 0.1f, 0, 1);
            }
            else
            {
                buttonOpacity = Math.Clamp(buttonOpacity -= 0.1f, 0, 1);
            }

            if (buttonOpacity <= 0f)
            {
                return;
            }
            int button1State, button2State, button3State;
            Texture2D uiImage = AltVanillaFunction.GetExtraTexture("ConcertUI");
            float yPos = -50 + player.gfxOffY;
            Vector2 buttonSize = new Vector2(uiImage.Width / 3, uiImage.Height / 3);
            Vector2 buttonPos1 = player.Center + new Vector2(0, yPos) - Main.screenPosition - buttonSize / 2;
            Rectangle button1 = new Rectangle((int)buttonPos1.X, (int)buttonPos1.Y, (int)buttonSize.X, (int)buttonSize.Y);
            Vector2 buttonPos2 = player.Center + new Vector2(-50, yPos) - Main.screenPosition - buttonSize / 2;
            Rectangle button2 = new Rectangle((int)buttonPos2.X, (int)buttonPos2.Y, (int)buttonSize.X, (int)buttonSize.Y);
            Vector2 buttonPos3 = player.Center + new Vector2(50, yPos) - Main.screenPosition - buttonSize / 2;
            Rectangle button3 = new Rectangle((int)buttonPos3.X, (int)buttonPos3.Y, (int)buttonSize.X, (int)buttonSize.Y);

            if (!PlayerInput.IgnoreMouseInterface)
            {
                if (button1.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    player.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (bp.customMode)
                        {
                            if (PlayMode == PlayModeID.SingleLoop)
                            {
                                PlayMode = PlayModeID.RandomLoop;
                            }
                            else if (PlayMode == PlayModeID.RandomLoop)
                            {
                                PlayMode = PlayModeID.ListLoop;
                                if (bp.ShouldBandPlaying)
                                {
                                    CurrentMusicID++;
                                }
                            }
                            else if (PlayMode == PlayModeID.ListLoop)
                            {
                                PlayMode = PlayModeID.SingleLoop;
                            }
                        }
                    }
                    string stateText = (int)PlayMode switch
                    {
                        1 => Language.GetTextValue("Mods.TouhouPets.RandomLoop"),
                        2 => Language.GetTextValue("Mods.TouhouPets.ListLoop"),
                        _ => Language.GetTextValue("Mods.TouhouPets.SingleLoop"),
                    };
                    Main.instance.MouseText(Language.GetTextValue("Mods.TouhouPets.LoopDisplay", stateText));
                }
                if (button2.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    player.mouseInterface = true;

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        bp.customMode = !bp.customMode;
                        bp.musicRerolled = false;
                    }
                    string stateText;
                    if (bp.customMode)
                    {
                        stateText = Language.GetTextValue("Mods.TouhouPets.TurnOn");
                    }
                    else
                    {
                        stateText = Language.GetTextValue("Mods.TouhouPets.TurnOff");
                    }
                    Main.instance.MouseText(Language.GetTextValue("Mods.TouhouPets.CustomDisplay", stateText));
                }
                if (button3.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    player.mouseInterface = true;

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (bp.customMode)
                        {
                            EnableBackgroundAudio = !EnableBackgroundAudio;
                        }
                    }
                    string stateText;
                    if (EnableBackgroundAudio)
                    {
                        stateText = Language.GetTextValue("Mods.TouhouPets.TurnOn");
                    }
                    else
                    {
                        stateText = Language.GetTextValue("Mods.TouhouPets.TurnOff");
                    }
                    Main.instance.MouseText(Language.GetTextValue("Mods.TouhouPets.BackAudioDisplay", stateText));
                }
            }
            if (!bp.customMode)
            {
                buttonDisable[0] = true;
                buttonDisable[2] = true;
            }
            button1State = (int)PlayMode;
            button2State = bp.customMode ? 1 : 0;
            button3State = EnableBackgroundAudio ? 1 : 0;
            if (!bp.customMode)
            {
                button1State = 0;
                button3State = 0;
            }

            Color buttonColor = buttonDisable[0] ? Color.DarkSlateGray : Color.White;
            Main.spriteBatch.TeaNPCDraw(uiImage, buttonPos1
                , new Rectangle(button1State * (int)buttonSize.X, 0, (int)buttonSize.X, (int)buttonSize.Y)
                , buttonColor * buttonOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            buttonColor = buttonDisable[1] ? Color.DarkSlateGray : Color.White;
            Main.spriteBatch.TeaNPCDraw(uiImage, buttonPos2
                , new Rectangle(button2State * (int)buttonSize.X, (int)buttonSize.Y, (int)buttonSize.X, (int)buttonSize.Y)
                , buttonColor * buttonOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            buttonColor = buttonDisable[2] ? Color.DarkSlateGray : Color.White;
            Main.spriteBatch.TeaNPCDraw(uiImage, buttonPos3
                , new Rectangle(button3State * (int)buttonSize.X, (int)buttonSize.Y * 2, (int)buttonSize.X, (int)buttonSize.Y)
                , buttonColor * buttonOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
