using Terraria.Localization;
using Terraria.Utilities;

namespace TouhouPets
{
    public struct CommentInfo(int type, WeightedRandom<LocalizedText> commentText)
    {
        public int ObjectType = type;
        public WeightedRandom<LocalizedText> CommentText = commentText;
    }
}
