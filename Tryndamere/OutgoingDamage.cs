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
            var dist = Trynda.Player.Distance(target);
            float msDif, time;
            var movePos = target.Position.To2D();
            if (target.IsMoving)
            {
                var path = Prediction.GetPrediction(target, 0.5f).UnitPosition.To2D();
                path.Normalize();
                movePos += (path * 100f);
            }
            
            float targMs;
            if (target.IsMoving && Trynda.Player.Distance(movePos) > dist)
            {
                targMs = target.MoveSpeed;
            }
            else
            {
                targMs = 0f;
            }

            if (Math.Abs((Trynda.Player.MoveSpeed - targMs)) < 0.1f)
            {
                time = -1f;
            }
            else
            {
                msDif = Trynda.Player.MoveSpeed - targMs;
                time = dist / msDif;
            }


            return time;
        }

        public static bool IsReachable(Obj_AI_Hero target)
        {
            return !(TimeToReach(target) < 0f);
        }

        public static float TimeToKill(Obj_AI_Hero target)
        {
            if (IsReachable(target))
            {
                return TimeToReach(target) + TimeToMeleeKill(target);
            }
            return Math.Abs(TimeToReach(target) + TimeToMeleeKill(target));
        }
    }
}