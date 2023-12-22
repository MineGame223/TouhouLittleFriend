using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets
{
    public class TouhouPetPlayer : ModPlayer
    {
        public bool yukariLicenseLeft;
        public bool yukariLicenseRight;
        private void CommonResetUpdate()
        {
            yukariLicenseLeft = false;
            yukariLicenseRight = false;
        }
        public override void ResetEffects()
        {
            CommonResetUpdate();
        }
        public override void UpdateDead()
        {
            CommonResetUpdate();
        }
        public override void PreUpdateBuffs()
        {          
            if (Player.miscEquips[0].type == ItemType<YukariTicketLeft>())
            {
                yukariLicenseLeft = true;
            }
            if (Player.miscEquips[1].type == ItemType<YukariTicketRight>())
            {
                yukariLicenseRight = true;
            }
            
        }
        public override void PostUpdateBuffs()
        {
            Player.buffImmune[BuffType<LightYukariBuff>()] = true;
            if (!yukariLicenseLeft || !yukariLicenseRight)
            {
                Player.buffImmune[BuffType<YukariBuff>()] = true;
            }
        }
    }
}
