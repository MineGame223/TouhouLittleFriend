using Terraria;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class YukariBuff : BasicPetBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Yukari>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Ran>());
            player.SpawnPetAndSetBuffTime(buffIndex, ProjectileType<Chen>());
        }
    }
    public class LightYukariBuff : BasicPetBuff
    {
        public override string Texture => "TouhouPets/Content/Buffs/PetBuffs/YukariBuff";
        public override bool LightPet => true;
    }
}
