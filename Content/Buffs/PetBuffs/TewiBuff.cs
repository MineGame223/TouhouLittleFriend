﻿using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Buffs.PetBuffs
{
    public class TewiBuff : BasicPetBuff
    {      
        public override int PetType => ProjectileType<Tewi>();
        public override bool LightPet => false;
    }
}
