using Terraria.Localization;

namespace TouhouPets
{
    public struct CommentInfo(int type, LocalizedText commentText)
    {
        public int ObjectType = type;
        public LocalizedText CommentText = commentText;
    }
}
