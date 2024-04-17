using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class KeineBuff : BasicPetBuff
    {
        public override int PetType => ProjectileType<Keine>();
        public override bool LightPet => false;
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip = Language.GetTextValue("Mods.TouhouPets.Buffs.KeineBuff.Description_2");
            if (Keine.UseAlternateForm)
            {
                tip = Language.GetTextValue("Mods.TouhouPets.Buffs.KeineBuff.Description");
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            drawParams.SourceRectangle = drawParams.Texture.Frame(verticalFrames: 2, frameY: Keine.UseAlternateForm ? 1 : 0);
            return true;
        }
    }
}
