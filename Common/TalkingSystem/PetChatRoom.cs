using Terraria;

namespace TouhouPets
{
    public class PetChatRoom
    {
        public const int MaxChatMember = 5;
        public Projectile[] member = new Projectile[MaxChatMember];
        public Projectile initiator;
        public bool active = false;
        public int chatTurn;
        public PetChatRoom()
        {
        }
    }
}
