using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;


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
            Vector2 movePos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                movePos = tpos + (path * 100);
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

            float msDif;
            if (Math.Abs((Trynda.Player.MoveSpeed - targMs)) < 0.01f)
            {
                msDif = 0f;
            }
            else
            {
                msDif = Trynda.Player.MoveSpeed - targMs;
            }

            return dist / msDif;
        }

        public static bool IsReachable(Obj_AI_Hero target)
        {
            return !(TimeToReach(target) < 0);
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