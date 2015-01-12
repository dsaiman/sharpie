using System;
using LeagueSharp;
using LeagueSharp.Common;


namespace Tryhardamere
{
    internal class OutgoingDamage
    {
        public static int AutosToLethal(Obj_AI_Hero target)
        {
            return (int) Math.Round(target.Health / Trynda.Player.GetAutoAttackDamage(target));
        }

        public static float TimeToMeleeKill(Obj_AI_Hero target)
        {
            var aspd = Trynda.Player.AttackSpeedMod * 0.67f;
            return (float) Math.Round(AutosToLethal(target) / aspd);
        }

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
                return ObjectManager.Player.Distance(target) / ObjectManager.Player.MoveSpeed;
            }
            
            return ObjectManager.Player.Distance(target) / moveSpeedDiff;
        }

        public static bool IsMovingToMe(Obj_AI_Hero target)
        {
            if (target.IsMoving && target.Path[0].IsValid())
            {
                var targetPos = target.Position.To2D();
                var targetPath = target.Path[0].To2D();
                targetPath.Normalize();
                targetPath = targetPath * 100f;
                targetPath += targetPos;
                if (ObjectManager.Player.Distance(target) > ObjectManager.Player.Distance(targetPath))
                {
                    return true;
                }
            }
            return false;
        }

    }
}