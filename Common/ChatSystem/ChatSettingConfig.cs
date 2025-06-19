using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;

namespace TouhouPets
{
    public struct ChatSettingConfig()
    {
        /// <summary>
        /// 每个字符的剩余时间值，默认为20；文本持续时间为该变量 * 字符数，上限为420刻
        /// </summary>
        public int TimeLeftPerWord = 20;
        /// <summary>
        /// 是否应当自动处理剩余时间计算，是则当字符串长度超过10时、TimeLeftPerWord最大为10
        /// </summary>
        public bool AutoHandleTimeLeft = true;
        /// <summary>
        /// 打字机模式打印文本所需总时长；默认为字符数 * 5、上限150刻
        /// </summary>
        public int TyperModeUseTime = -1;
        /// <summary>
        /// 文本颜色，默认为白色
        /// </summary>
        public Color TextColor = Color.White;
        /// <summary>
        /// 文本边框颜色，默认为黑色
        /// </summary>
        public Color TextBoardColor = Color.Black;
    }
}
