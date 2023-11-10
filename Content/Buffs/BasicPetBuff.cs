using Terraria;

namespace TouhouPets.Content.Buffs
{
    public abstract class BasicPetBuff : ModBuff
    {
        public virtual int PetType => -1;
        public virtual bool LightPet => false;
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = !LightPet;
            Main.lightPet[Type] = LightPet;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, PetType);
            OnSummonPet(player);
        }
        public virtual void OnSummonPet(Player player)
        {
        }
    }
}
