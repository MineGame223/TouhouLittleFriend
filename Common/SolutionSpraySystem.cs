using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.Content.Projectiles.Pets.Yuka;

namespace TouhouPets
{
    public class SolutionSpraySystem : ModSystem
    {
        private static int _sprayMode;
        private static Projectile yuka;
        public static Item _solution;
        public static Item Sprayer => new(ItemID.Clentaminator2);
        public static bool InSprayMode => PetState >= Phase_Spray_Mode1 && PetState <= Phase_Spray_Mode2;
        private static float PetState
        {
            get
            {
                return yuka.ai[1];
            }
            set
            {
                yuka.ai[1] = value;
            }
        }
        public override void PostUpdateProjectiles()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (_sprayMode > 1 || _sprayMode < 0)
            {
                _sprayMode = 0;
            }
            yuka = null;
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active)
                {
                    if (p.owner == Main.myPlayer && p.type == ProjectileType<Yuka>())
                    {
                        yuka = p;
                    }
                }
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int EmoteIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Emote Bubbles"));
            if (EmoteIndex != -1)
            {
                layers.Insert(EmoteIndex, new LegacyGameInterfaceLayer(
                    "TouhouPets : YukaRequest",
                    delegate
                    {
                        SetSpray();
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }
        private static void SetSpray()
        {
            if (yuka == null)
                return;

            if (yuka.isAPreviewDummy || PetState == Phase_StopSpray)
                return;

            if (!InSprayMode)
                _solution = new Item();

            bool request = false;
            Player player = Main.player[yuka.owner];
            if (!PlayerInput.IgnoreMouseInterface)
            {
                Vector2 yukaPos = yuka.position - Main.screenPosition;
                Rectangle yukaRect = new((int)yukaPos.X, (int)yukaPos.Y, yuka.width, yuka.height);
                if (yukaRect.Contains(new Point(Main.mouseX, Main.mouseY)))
                {
                    player.mouseInterface = true;
                    request = true;
                    _solution = player.ChooseAmmo(Sprayer);
                    if (yukaRect.Contains(new Point(Main.mouseX, Main.mouseY)) && _solution != null && !_solution.IsAir)
                    {
                        if (Main.mouseRight && Main.mouseRightRelease)
                        {
                            int targetMode = _sprayMode == 0 ? Phase_Spray_Mode1 : Phase_Spray_Mode2;
                            if (PetState != targetMode)
                            {
                                PetState = targetMode;
                                AltVanillaFunction.PlaySound(SoundID.MenuOpen, yuka.position);
                            }
                            else
                            {
                                PetState = Phase_StopSpray;
                                AltVanillaFunction.PlaySound(SoundID.MenuClose, yuka.position);
                            }
                        }
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            if (!InSprayMode)
                            {
                                AltVanillaFunction.PlaySound(SoundID.MenuTick, yuka.position);
                                _sprayMode++;
                            }
                        }
                    }
                }
            }
            DrawLeftSolution(request);
        }
        private static void DrawLeftSolution(bool drawRequestText)
        {
            if (_solution == null || _solution.IsAir)
            {
                return;
            }
            Main.instance.LoadItem(_solution.type);
            Texture2D t = AltVanillaFunction.ItemTexture(_solution.type);
            Vector2 pos = yuka.Center - Main.screenPosition + new Vector2(0, 7f * Main.essScale)
                + new Vector2(-20, -50);
            Rectangle rect = new(0, 0, t.Width, t.Height);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = SpriteEffects.None;
            if (_solution.ammo != AmmoID.None)
            {
                Main.spriteBatch.TeaNPCDraw(t, pos, rect, yuka.GetAlpha(Color.White), 0, orig, 1.3f, effect, 0f);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                                , ": " + _solution.stack.ToString(), pos.X + 20
                                , pos.Y - 4
                                , Color.White, Color.Black
                                , orig, 1f);
            }
            string modeText = InSprayMode ? "Stop" : "Request";
            if (drawRequestText)
            {
                if (!InSprayMode && PetState != Phase_StopSpray)
                    DrawSprayModeSign(_sprayMode == 0 ? 9 : 8);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value
                                , Language.GetTextValue($"Mods.TouhouPets.Yuka{modeText}"), Main.MouseScreen.X + TextureAssets.Cursors[2].Width()
                                , Main.MouseScreen.Y + TextureAssets.Cursors[2].Height()
                                , Color.White, Color.Black
                                , orig, 1f);
            }
        }
        private static void DrawSprayModeSign(int frame)
        {
            Texture2D t = AltVanillaFunction.ProjectileTexture(yuka.type);
            int width = t.Width / 2;
            int height = t.Height / Main.projFrames[yuka.type];
            Vector2 pos = yuka.Center + new Vector2(-50, -50)
                - Main.screenPosition + new Vector2(0, 7f * Main.essScale);
            Rectangle rect = new(width, frame * height, 32, 32);
            Vector2 orig = rect.Size() / 2;
            SpriteEffects effect = SpriteEffects.None;
            Main.spriteBatch.TeaNPCDraw(t, pos, rect, yuka.GetAlpha(Color.White), 0, orig, yuka.scale, effect, 0f);
        }
    }
}
