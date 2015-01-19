using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Kayle
{
    internal class OutgoingDamage
    {

        public static float TimeToReach(Obj_AI_Hero target)
        {
            float moveSpeedDiff;

            if (Math.Abs(ObjectManager.Player.MoveSpeed - target.MoveSpeed) < 0.1f)
            {
                moveSpeedDiff = 0f;
            }

            else
            {
                moveSpeedDiff = ObjectManager.Player.MoveSpeed - target.MoveSpeed;
            }

            if (moveSpeedDiff <= 0f)
            {
                return -1f;
            }

            return ObjectManager.Player.Distance(target) / moveSpeedDiff;
        }

        public static bool IsMovingToMe(Obj_AI_Hero target)
        {
            if (target.IsMoving && target.Path[0].IsValid())
            {
                var targetPath = target.Path[0].To2D();
                if (ObjectManager.Player.Distance(target) >= ObjectManager.Player.Distance(targetPath))
                {
                    return true;
                }
            }
            else if (!target.IsMoving)
            {
                return true;
            }
            return false;
        }

        public static bool IsEscaping(Obj_AI_Hero target)
        {
            return !IsMovingToMe(target) &&
                   TimeToReach(target) <= 0f &&
                   target.Distance(ObjectManager.Player) > 300f;
        }

        public static bool SheenProcable()
        {
            if ((Items.HasItem(3078) || Items.HasItem(3057) || Items.HasItem(3100) || Items.HasItem(3025)) && !ObjectManager.Player.HasBuff("sheen"))
            {
                return true;
            }
            return false;
        }
    }
}