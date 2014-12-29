using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace Tryhardamere
{
    class Use
    {
        public static void UseQSmart()
        {
            if (!Trynda.Q.IsReady())
                return;
            if (Trynda.MyHpPerc() <= Tryhardamere.Config.Item("QonHp").GetValue<Slider>().Value)
                Trynda.Q.Cast();
        }

        public static void UseWSmart(Obj_AI_Hero target)
        {
            if (!Trynda.W.IsReady())
                return;
            //Console.WriteLine("use W");

            float trueAARange = Trynda.Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + Trynda.W.Range;

            float dist = Trynda.Player.Distance(target);
            Vector2 dashPos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Trynda.Player.Distance(dashPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Trynda.Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Trynda.Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            //Console.WriteLine(timeToReach);
            if (dist > trueAARange && dist < trueERange)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    Trynda.W.Cast();
                }
            }

        }

        public static void UseESmart(Obj_AI_Hero target)
        {
            if (!Trynda.E.IsReady())
                return;
            //  Console.WriteLine("use E");
            float trueAARange = Trynda.Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + Trynda.E.Range;

            float dist = Trynda.Player.Distance(target);
            Vector2 movePos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                movePos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Trynda.Player.Distance(movePos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Trynda.Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Trynda.Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            //  Console.WriteLine(timeToReach);
            if (dist > trueAARange && dist < trueERange)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    Trynda.E.Cast(Trynda.Player.Position.To2D().Extend(target.Position.To2D(), target.Distance3D(Trynda.Player) + 200));
                }
            }

        }

        public static void UseRSmart()
        {
            if (!Trynda.R.IsReady() || Trynda.Player.HasBuff("Recall") || Utility.InFountain())
                return;

            if ((Trynda.MyHpPerc() <= Tryhardamere.Config.Item("RonHp").GetValue<Slider>().Value) && (Utility.CountEnemysInRange(900) > 0))
                Trynda.R.Cast();
        }

        public static void UseWTrade(Obj_AI_Hero target)
        {
            if (target.Distance(Trynda.Player) < 250 && Trynda.W.IsReady())
                Trynda.W.Cast();
        }

        public static void UseHydra(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(3074) && target.Distance(Trynda.Player) < 420)
                Items.UseItem(3074);
            if (Items.CanUseItem(3077) && target.Distance(Trynda.Player) < 420)
                Items.UseItem(3077);
        }

        public static void UseHydraLC()
        {
            if (Items.CanUseItem(3074))
                Items.UseItem(3074);
            if (Items.CanUseItem(3077))
                Items.UseItem(3077);
        }

        public static void UseComboItems(Obj_AI_Hero target)
        {
            //BOTRK and Cutlass
            if ((!Trynda.W.IsReady() && target.Distance(Trynda.Player) > Trynda.Player.AttackRange + target.BoundingRadius) || Trynda.MyHpPerc() < 40 )
            {
                if (Items.CanUseItem(3153))
                    Items.UseItem(3153, target);
                if (Items.CanUseItem(3144))
                    Items.UseItem(3144, target);                
            }
            //Ghostblade
            if (Items.CanUseItem(3142))
                Items.UseItem(3142);
                
            //Hydra and Tiamat
            if (Items.CanUseItem(3074) && target.Distance(Trynda.Player) < 420)
                Items.UseItem(3074);
            if (Items.CanUseItem(3077) && target.Distance(Trynda.Player) < 420)
                Items.UseItem(3077);

        }
    }
}
