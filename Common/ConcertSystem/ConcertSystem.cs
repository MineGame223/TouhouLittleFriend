using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
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
                Mod.Logger.Info($"由于未开启设置或正处于多人模式，已跳过音乐文件加载环节。");
            }
        }
        public override void ModifyScreenPosition()
        {
            GuaranteedUpdate();
        }
        public override void PreSaveAndQuit()
        {
            Stop();
            PostStop();
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
        private static void ButtonAction_0(Mod mod, int buttonState, bool buttonDisabled, ConcertPlayer bp)
        {
            if (buttonDisabled)
            {
                return;
            }
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (bp.CustomModeOn)
                {
                    if (PlayMode == PlayModeID.SingleLoop)
                    {
                        PlayMode = PlayModeID.RandomLoop;
                    }
                    else if (PlayMode == PlayModeID.RandomLoop)
                    {
                        PlayMode = PlayModeID.ListLoop;
                    }
                    else if (PlayMode == PlayModeID.ListLoop)
                    {
                        PlayMode = PlayModeID.SingleLoop;
                    }
                }
            }
            string stateText = buttonState switch
            {
                1 => mod.GetLocalization("RandomLoop").Value,
                2 => mod.GetLocalization("ListLoop").Value,
                _ => mod.GetLocalization("SingleLoop").Value,
            };
            Main.instance.MouseText(mod.GetLocalization("LoopDisplay").Format(stateText));
        }
        private static void ButtonAction_1(Mod mod, int buttonState, bool buttonDisabled, ConcertPlayer bp)
        {
            if (buttonDisabled)
            {
                return;
            }
            bool customEnable = GetInstance<MiscConfig>().EnableCustomMusicMode && Main.netMode == NetmodeID.SinglePlayer;
            if (Main.mouseRight && Main.mouseRightRelease && customEnable)
            {
                if (Directory.Exists(FullPath))
                {
                    Process.Start("explorer.exe", FullPath);
                }
            }
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                bool canClick = customEnable;
                if (NoCustomMusic)
                {
                    Main.NewText(mod.GetLocalization("NoCustomMusicNotice"), Color.Yellow);
                    canClick = false;
                }
                if (canClick)
                {
                    bp.CustomModeOn = !bp.CustomModeOn;
                    bp.MusicRerolled = false;
                }
            }
            string stateText = buttonState switch
            {
                1 => mod.GetLocalization("TurnOn").Value,
                _ => mod.GetLocalization("TurnOff").Value,
            };

            string finalText = mod.GetLocalization("CustomDisplay").Format(stateText);
            if (customEnable)
            {
                finalText += "\n" + mod.GetLocalization("OpenFolder");
            }
            if (!GetInstance<MiscConfig>().EnableCustomMusicMode)
            {
                finalText = mod.GetLocalization("CustomMusicDisabledNotice").Value;
            }
            Main.instance.MouseText(finalText);
        }
        private static void ButtonAction_2(Mod mod, int buttonState, bool buttonDisabled, ConcertPlayer bp)
        {
            if (buttonDisabled)
            {
                return;
            }
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (bp.CustomModeOn)
                {
                    EnableBackgroundAudio = !EnableBackgroundAudio;
                }
            }
            string stateText = buttonState switch
            {
                1 => mod.GetLocalization("TurnOn").Value,
                _ => mod.GetLocalization("TurnOff").Value,
            };
            Main.instance.MouseText(mod.GetLocalization("BackAudioDisplay").Format(stateText));
        }
        private void DrawConcertUI()
        {
            Player player = Main.LocalPlayer;
            ConcertPlayer bp = player.GetModPlayer<ConcertPlayer>();

            if (bp.ConcertStart && player.HeldItem.type == ItemType<SupportStick>()
                && Main.netMode == NetmodeID.SinglePlayer)
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

            Texture2D uiImage = AltVanillaFunction.GetExtraTexture("ConcertUI");
            float yPos = -50 + player.gfxOffY;
            Vector2 buttonSize = new Vector2(uiImage.Width / 3, uiImage.Height / 3);

            int[] buttonState = new int[3];
            bool[] buttonDisable = new bool[3];
            Vector2[] buttonPos = new Vector2[3];
            Rectangle[] buttonRect = new Rectangle[3];

            if (!bp.CustomModeOn)
            {
                buttonDisable[0] = true;
                buttonDisable[2] = true;
            }
            buttonState[0] = (int)PlayMode;
            buttonState[1] = bp.CustomModeOn ? 1 : 0;
            buttonState[2] = EnableBackgroundAudio ? 1 : 0;
            if (!bp.CustomModeOn)
            {
                buttonState[0] = 0;
                buttonState[2] = 0;
            }

            for (int i = 0; i < 3; i++)
            {
                float xPos = i switch
                {
                    0 => -50,
                    2 => 50,
                    _ => 0
                };

                buttonPos[i] = player.Center + new Vector2(xPos, yPos) - Main.screenPosition - buttonSize / 2;
                buttonRect[i] = new((int)buttonPos[i].X, (int)buttonPos[i].Y, (int)buttonSize.X, (int)buttonSize.Y);

                if (!PlayerInput.IgnoreMouseInterface)
                {
                    if (buttonRect[i].Contains(new Point(Main.mouseX, Main.mouseY)))
                    {
                        player.mouseInterface = true;
                        switch (i)
                        {
                            case 1:
                                ButtonAction_1(Mod, buttonState[i], buttonDisable[i], bp);
                                break;

                            case 2:
                                ButtonAction_2(Mod, buttonState[i], buttonDisable[i], bp);
                                break;

                            default:
                                ButtonAction_0(Mod, buttonState[i], buttonDisable[i], bp);
                                break;
                        }
                    }
                }

                if (!buttonDisable[i])
                {
                    Color buttonColor = buttonDisable[i] ? Color.DarkSlateGray : Color.White;
                    Main.spriteBatch.MyDraw(uiImage, buttonPos[i]
                        , new Rectangle(buttonState[i] * (int)buttonSize.X, (int)buttonSize.Y * i, (int)buttonSize.X, (int)buttonSize.Y)
                        , buttonColor * buttonOpacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }
    }
}
