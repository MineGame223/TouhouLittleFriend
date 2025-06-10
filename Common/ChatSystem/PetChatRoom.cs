using Terraria;

namespace TouhouPets
{
    /// <summary>
    /// 聊天室类，最多存在50个；每个聊天室最多包括6名参与者+1名发起者
    /// </summary>
    public class PetChatRoom
    {
        public const int MaxChatMember = 6;
        /// <summary>
        /// 聊天成员数组
        /// </summary>
        public Projectile[] member = new Projectile[MaxChatMember];
        /// <summary>
        /// 聊天发起者
        /// </summary>
        public Projectile initiator;
        /// <summary>
        /// 聊天室是否应当存在
        /// </summary>
        public bool active = false;
        /// <summary>
        /// 当前对话轮数
        /// </summary>
        public int chatTurn;
        /// <summary>
        /// 聊天室类的构造函数，最多存在50个；每个聊天室最多包括6名参与者+1名发起者
        /// </summary>
        public PetChatRoom()
        {
        }
    }
}
