using Terraria;

namespace TouhouPets
{
    /// <summary>
    /// 聊天室类，最多存在50个；每个聊天室最多包括6名参与者+1名发起者
    /// </summary>
    public class PetChatRoom
    {
        public const int MaxChatMember = 6;
        public Projectile[] member = new Projectile[MaxChatMember];
        public Projectile initiator;
        public bool active = false;
        public int chatTurn;
        public PetChatRoom()
        {
        }
    }
}
