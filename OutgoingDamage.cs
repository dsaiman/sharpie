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
            return (int) Math.Ceiling(target.Health / Trynda.Player.GetAutoAttackDamage(target));
        }

        public static int TimeToKill(Obj_AI_Hero target)
        {
            var aspd = Trynda.Player.AttackSpeedMod * 0.67;
            return (int) Math.Round(AutosToLethal(target) / aspd);
        }

        public static float TimeToReach(Obj_AI_Hero target)
        {
            var dist = Trynda.Player.Distance(target);
            var trueAARange = Trynda.Player.AttackRange + target.BoundingRadius;
            Vector2 movePos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                movePos = tpos + (path * 100);
            }
            var targMs = (target.IsMoving && Trynda.Player.Distance(movePos) > dist) ? target.MoveSpeed : 0;
            var msDif = Math.Abs((Trynda.Player.MoveSpeed - targMs)) < Single.Epsilon ? 0.0001f : (Trynda.Player.MoveSpeed - targMs);
            return (dist - trueAARange) / msDif;
        }

    }
}
