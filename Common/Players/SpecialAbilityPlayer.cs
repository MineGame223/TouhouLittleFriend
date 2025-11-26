using Terraria;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets
{
    public class SpecialAbilityPlayer : ModPlayer
    {
        public bool treasureShine;
        private void CommonResetUpdate()
        {
            treasureShine = false;
        }
        public override void ResetEffects()
        {
            CommonResetUpdate();
        }
        public override void UpdateDead()
        {
            CommonResetUpdate();
        }
        public override void ModifyLuck(ref float luck)
        {
            if (Player.HasBuff(BuffType<TenshiBuff>()) && SpecialAbility_Tenshin)
            {
                luck += 0.5f;
            }

            if (Player.HasBuff(BuffType<HinaBuff>()) && SpecialAbility_Hina)
            {
                if (luck < 0)
                {
                    luck = 0;
                }
            }

            if ((Player.HasBuff(BuffType<TewiBuff>()) || Player.HasBuff<EienteiBuff>()) && SpecialAbility_Tewi)
            {
                luck += 0.3f;
            }
        }
        public override void PostUpdateBuffs()
        {
            if (treasureShine && SpecialAbility_Star)
            {
                Player.spelunkerTimer++;
                if (Player.spelunkerTimer >= 10)
                {
                    Player.spelunkerTimer = 0;
                    Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(Player.Center);
                }
            }
        }
        public override void UpdateEquips()
        {
            if (Player.HasBuff<MurasaBuff>() && Main.zenithWorld && SpecialAbility_Murasa)
            {
                Player.waterWalk = false;

                if (Player.breathCD > 0 && Player.breath > 0)
                    Player.breath = 0;
            }
        }
    }
}
