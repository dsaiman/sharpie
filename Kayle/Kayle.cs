using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

namespace Kayle
{
    internal class Kayle
    {
        public const string CharName = "Kayle";

        public static Obj_AI_Hero Target;

        public Kayle()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            try{
            Game.PrintChat("<font color=\"#00BFFF\">Kayle# -</font> Loaded!");
            KMenu.AddMenu();

            K.GetSmiteSlot();

            Drawing.OnDraw += OnDraw;
            Game.OnGameUpdate += OnGameUpdate;

            Obj_AI_Base.OnProcessSpellCast += MinionSpellCast;
            Obj_AI_Base.OnProcessSpellCast += HeroSpellCast;
            Obj_AI_Base.OnProcessSpellCast += TowerSpellCast;
            Obj_AI_Base.OnProcessSpellCast += IncomingDamage.ChargeOnTowerSpellCast;

            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;

        }

    catch
    {
        Game.PrintChat("Oops. Something went wrong with Kayle");
    }
    }

        private static void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen || ObjectManager.Player.IsChannelingImportantSpell() ||
                ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (KMenu.Config.Item("useWSustain").GetValue<bool>() && K.Orbwalker.ActiveMode.ToString() != "Combo")
            {
                if (K.W.IsReady())
                    Use.UseWSustain(ObjectManager.Player);
            }

            if (KMenu.Config.Item("manR").GetValue<bool>())
            {
                Use.UseRManual(ObjectManager.Player);
            }

            if (K.Orbwalker.ActiveMode.ToString() == "Combo")
            {
                Target = TargetSelector.GetTarget(K.Q.Range, TargetSelector.DamageType.Magical);
                
                if (Target != null && Target.IsValidTarget())
                {
                    K.Combo(Target);
                }
            }

            if (K.Orbwalker.ActiveMode.ToString() == "Mixed")
            {
                Target = TargetSelector.GetTarget(K.Q.Range, TargetSelector.DamageType.Magical);
                if (Target.IsValidTarget() && Target != null)
                    K.Mixed(Target);
                else
                    K.Mixed();
            }

            if (K.Orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                K.LaneClear();
            }

        }

        private static void OnDraw(EventArgs args)
        {
            if (KMenu.Config.Item("drawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(K.Player.Position, K.Q.Range, Color.Firebrick);
            }
            if (KMenu.Config.Item("drawW").GetValue<bool>())
            {
                Drawing.DrawCircle(K.Player.Position, K.W.Range, Color.DeepSkyBlue);
            }
            if (KMenu.Config.Item("drawE").GetValue<bool>())
            {
                Drawing.DrawCircle(K.Player.Position, K.E.Range, Color.DeepPink);
            }
            if (KMenu.Config.Item("drawR").GetValue<bool>())
            {
                Drawing.DrawCircle(K.Player.Position, K.R.Range, Color.GreenYellow);
            }


        }

        private static void MinionSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.InFountain())
            {
                return;
            }
            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Minion)
            {
                if (args.Target.IsMe)
                {
                    if (IncomingDamage.MinionIsLethal(sender, (Obj_AI_Base) args.Target, args))
                    {
                        if (KMenu.Config.Item("autoW").GetValue<bool>() && K.W.IsReady())
                        {
                            K.W.Cast((Obj_AI_Base) args.Target);
                        }
                        if (KMenu.Config.Item("autoR").GetValue<bool>() && K.R.IsReady())
                        {
                            K.R.Cast((Obj_AI_Base)args.Target);
                        }
                    }
                }
  //              else
  //                  foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly && !ally.IsDead && ally.CountEnemysInRange(900f) > 0 ))
  //                  {
  ////todo                      
  //                  }
            }
        }

        private static void HeroSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.InFountain())
            {
                return;
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Hero)
            {
                if (args.Target == null)
                {
                    //if (IncomingDamage.SkillshotHeroIsLethal(sender, args))
                    //{
                    //    if (KMenu.Config.Item("autoW").GetValue<bool>() && K.W.IsReady())
                    //    {
                    //        K.W.Cast(ObjectManager.Player);
                    //    }
                    //    if (KMenu.Config.Item("autoR").GetValue<bool>() && K.R.IsReady())
                    //    {
                    //        K.R.Cast(ObjectManager.Player);
                    //    }
                    //}
                }
                else if (args.Target.IsMe)
                {

                    if (IncomingDamage.TargetedHeroIsLethal(sender, (Obj_AI_Base) args.Target, args))
                    {
                        if (KMenu.Config.Item("autoW").GetValue<bool>() && K.W.IsReady())
                        {
                            K.W.Cast((Obj_AI_Base)args.Target);
                        }
                        if (KMenu.Config.Item("autoR").GetValue<bool>() && K.R.IsReady())
                        {
                            K.R.Cast((Obj_AI_Base)args.Target);
                        }
                    }
                }
            }
        }

        private static void TowerSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.InFountain())
            {
                return;
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Turret && sender.Distance(ObjectManager.Player) < 2000f)
            {
                if (args.Target.IsMe)
                {
                    if (IncomingDamage.TowerIsLethal(sender, (Obj_AI_Base) args.Target, args))
                    {
                        if (KMenu.Config.Item("autoW").GetValue<bool>() && K.W.IsReady())
                        {
                            K.W.Cast((Obj_AI_Base)args.Target);
                        }
                        if (KMenu.Config.Item("autoR").GetValue<bool>() && K.R.IsReady())
                        {
                            K.R.Cast((Obj_AI_Base)args.Target);
                        }
                    }
                }
            }
        }

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (KMenu.Config.Item("QWgap").GetValue<bool>() && gapcloser.Sender.IsValidTarget() &&
                ObjectManager.Player.Distance(gapcloser.Sender.Position) <= K.Q.Range)
            {
                if (K.Q.IsReady())
                {
                    K.Q.Cast(gapcloser.Sender);
                }
                if (K.W.IsReady() &&
                    ObjectManager.Player.MaxHealth - ObjectManager.Player.Health < K.W.GetDamage(ObjectManager.Player))
                {
                    K.W.Cast(ObjectManager.Player);
                }
            }

        }

    }

}