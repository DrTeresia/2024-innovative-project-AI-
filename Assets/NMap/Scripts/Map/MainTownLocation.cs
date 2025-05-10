using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Record the coordinates of the main town.
 * Coordinates use the graph coordinate system (start from left buttom, length 600, width 400).
 * 
 */
namespace Assets.Map
{
    public class MainTownLocation
    {
        public static Dictionary<string, Vector2> weiMainTown = new Dictionary<string, Vector2>
        {
            {"YouZhou",  new Vector2(347,  351) }, 
            {"XiLiang", new Vector2(215, 329) },
            {"BingZhou", new Vector2(320, 325) },
            {"JiZhou", new Vector2(366, 323) },
            {"QingZhou", new Vector2(390, 313) },
            {"Ye", new Vector2(343, 298) },
            {"YanZhou", new Vector2(361, 389) },
            {"LuoYang", new Vector2(310, 278) }, 
            {"XuZhou", new Vector2(392, 276) },
            {"ChangAn", new Vector2(262, 267) },
            {"XuChang", new Vector2(348, 267) },
            {"ShouChun", new Vector2(391, 258) },
            {"RuNan", new Vector2(352, 254) },
            {"HeFei", new Vector2(389, 237) },
            {"HanZhong", new Vector2(253, 234) },
            {"XiangYang", new Vector2(299, 212) }
        };

        public static Dictionary<string, Vector2> shuMainTown = new Dictionary<string, Vector2>
        {
            {"Chengdu", new Vector2(212, 197) },
            {"JinZhou", new Vector2(305, 195) },
            {"Wulin", new Vector2(294, 180) }
        };

        public static Dictionary<string, Vector2> wuMainTown = new Dictionary<string, Vector2>
        {
            {"JianYe", new Vector2(394, 194) },
            {"QinSang", new Vector2(374, 194) },
            {"ChangSha", new Vector2(332, 182) },
            {"LingLin", new Vector2(290, 148) },
            {"GuiYang", new Vector2(326, 148) },
            {"JiaoZhou", new Vector2(303, 97) }
        };
    }
}

