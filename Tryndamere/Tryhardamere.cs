using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

namespace Tryhardamere
{
    internal class Tryhardamere
    {
        public const string CharName = "Tryndamere";

        public static Menu Config;

        public static Obj_AI_Hero Target;

        public Tryhardamere()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            Game.PrintChat("<font color=\"#00BFFF\">Tryhardamere# -</font> Loaded!");

            try
            {
                Config = new Menu("Tryndamere", "Tryndamere", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Trynda.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

                //TS
                var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(targetSelectorMenu);
                Config.AddSubMenu(targetSelectorMenu);

                //Combo
                Config.AddSubMenu(new Menu("Combo", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("useQCombo", "Use Q during combo")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useWCombo", "Use W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useECombo", "Use E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useIgniteCombo", "Use Ignite")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useSmiteCombo", "Use Smite on champ")).SetValue(true);

                //Mixed
                Config.AddSubMenu(new Menu("Mixed", "mix"));
                Config.SubMenu("mix").AddItem(new MenuItem("useHydraMix", "Use Hydra")).SetValue(true);
                Config.SubMenu("mix").AddItem(new MenuItem("harassTower", "Harass Under Tower")).SetValue(true);


                //LaneClear
                Config.AddSubMenu(new Menu("LaneClear", "lClear"));
                Config.SubMenu("lClear").AddItem(new MenuItem("useHydraLC", "Use Hydra")).SetValue(true);


                //Utilities
                Config.AddSubMenu(new Menu("Q and R settings", "utils"));
                Config.SubMenu("utils").AddItem(new MenuItem("useW", "Use W to trade (no slow check)")).SetValue(false);
                Config.SubMenu("utils").AddItem(new MenuItem("autoQ", "Auto Q")).SetValue(true);
                Config.SubMenu("utils").AddItem(new MenuItem("manQ", "Manual Q (set %)")).SetValue(false);
                Config.SubMenu("utils").AddItem(new MenuItem("QonHp", "Use Q on % hp")).SetValue(new Slider(25, 1, 99));
                Config.SubMenu("utils").AddItem(new MenuItem("autoR", "Auto R")).SetValue(true);
                Config.SubMenu("utils").AddItem(new MenuItem("manR", "Manual R (Set %)")).SetValue(false);
                Config.SubMenu("utils").AddItem(new MenuItem("RonHp", "Use R on % hp")).SetValue(new Slider(25, 1, 99));

                //Drawings
                Config.AddSubMenu(new Menu("Drawings", "drawings"));
                Config.SubMenu("drawings").AddItem(new MenuItem("drawW", "Draw W range")).SetValue(true);
                Config.SubMenu("drawings").AddItem(new MenuItem("drawE", "Draw E range")).SetValue(true);
                Config.SubMenu("drawings").AddItem(new MenuItem("drawAAtoKill", "Draw AA to kill")).SetValue(true);


                Config.AddToMainMenu();
                Trynda.GetSmiteSlot();

                Drawing.OnDraw += OnDraw;
                Game.OnGameUpdate += OnGameUpdate;

                Obj_AI_Base.OnProcessSpellCast += MinionSpellCast;
                Obj_AI_Base.OnProcessSpellCast += HeroSpellCast;
                Obj_AI_Base.OnProcessSpellCast += TowerSpellCast;
                Obj_AI_Base.OnProcessSpellCast += IncomingDamage.ChargeOnTowerSpellCast;

            }
            catch
            {
                Game.PrintChat("Oops. Something went wrong with Tryndamere");
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Trynda.Player.IsDead || MenuGUI.IsChatOpen || Trynda.Player.IsChannelingImportantSpell() ||
                Trynda.Player.IsRecalling())
            {
                return;
            }

            if (Config.Item("manQ").GetValue<bool>() && Trynda.Orbwalker.ActiveMode.ToString() != "Combo")
            {
                Use.UseQSmart();
            }

            if (Config.Item("manR").GetValue<bool>())
            {
                Use.UseRSmart();
            }

            if (Trynda.Orbwalker.ActiveMode.ToString() == "Combo")
            {
                if (Trynda.E.IsReady())
                {
                    Target = TargetSelector.GetTarget(950, TargetSelector.DamageType.Physical);
                }
                else if (Trynda.W.IsReady())
                {
                    Target = TargetSelector.GetTarget(450, TargetSelector.DamageType.Physical);
                }
                else
                {
                    Target = TargetSelector.GetTarget(250, TargetSelector.DamageType.Physical);
                }
                if (Target != null)
                {
                    Trynda.Combo(Target);
                }
            }

            if (Trynda.Orbwalker.ActiveMode.ToString() == "Mixed")
            {
                Target = TargetSelector.GetTarget(250, TargetSelector.DamageType.Physical);
                Trynda.Mixed(Target);
            }

            if (Trynda.Orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                Target = TargetSelector.GetTarget(250, TargetSelector.DamageType.Physical);
                Trynda.LaneClear(Target);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("drawE").GetValue<bool>())
            {
                Drawing.DrawCircle(Trynda.Player.Position, Trynda.E.Range, Color.RoyalBlue);
            }
            if (Config.Item("drawW").GetValue<bool>())
            {
                Drawing.DrawCircle(Trynda.Player.Position, 820f, Color.Firebrick);
            }

            if (Config.Item("drawAAtoKill").GetValue<bool>())
            {
                Target = TargetSelector.GetTarget(1050f, TargetSelector.DamageType.Physical);
                if (Target != null && Target.IsValid)
                {
                    var targetpos = Drawing.WorldToScreen(Target.Position);

                    Drawing.DrawText(
    targetpos[0] - 80, targetpos[1] + 80, Color.White, " " + OutgoingDamage.TimeToReach(Target));


                    Drawing.DrawText(
                        targetpos[0] - 40, targetpos[1] + 40, Color.White, "Autos To Kill: " + OutgoingDamage.AutosToLethal(Target));

                }
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
                    if (IncomingDamage.MinionIsLethal(sender, args))
                    {
                        if (Config.Item("autoR").GetValue<bool>() && Trynda.R.IsReady())
                        {
                            Trynda.R.Cast();
                        }
                        if (Config.Item("autoQ").GetValue<bool>() && Trynda.Q.IsReady() &&
                            !Trynda.Player.HasBuff("Undying Rage") && !Trynda.R.IsReady())
                        {
                            Trynda.Q.Cast();
                        }
                    }
                }
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
                    if (IncomingDamage.SkillshotHeroIsLethal(sender, args))
                    {
                        if (Config.Item("autoR").GetValue<bool>() && Trynda.R.IsReady())
                        {
                            Trynda.R.Cast();
                        }
                        if (Config.Item("autoQ").GetValue<bool>() && Trynda.Q.IsReady() &&
                            !Trynda.Player.HasBuff("Undying Rage") && !Trynda.R.IsReady())
                        {
                            Trynda.Q.Cast();
                        }
                    }
                }
                else if (args.Target.IsMe)
                {

                    if (IncomingDamage.TargetedHeroIsLethal(sender, args))
                    {
                        if (Config.Item("autoR").GetValue<bool>() && Trynda.R.IsReady())
                        {
                            Trynda.R.Cast();
                        }
                        if (Config.Item("autoQ").GetValue<bool>() && Trynda.Q.IsReady() &&
                            !Trynda.Player.HasBuff("Undying Rage") && !Trynda.R.IsReady())
                        {
                            Trynda.Q.Cast();
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
                    if (Trynda.E.IsReady() && Config.Item("harassTower").GetValue<bool>() &&
                        (Trynda.Orbwalker.ActiveMode.ToString() == "Mixed" ||
                         Trynda.Orbwalker.ActiveMode.ToString() == "LaneClear"))
                    {
                        Trynda.E.Cast(Trynda.Player.Position.To2D().Extend(sender.Position.To2D(), -Trynda.E.Range));
                    }


                    if (IncomingDamage.TowerIsLethal(sender, args))
                    {
                        if (Config.Item("autoR").GetValue<bool>() && Trynda.R.IsReady())
                        {
                            Trynda.R.Cast();
                        }
                        if (Config.Item("autoQ").GetValue<bool>() && Trynda.Q.IsReady() &&
                            !Trynda.Player.HasBuff("Undying Rage") && !Trynda.R.IsReady())
                        {
                            Trynda.Q.Cast();
                        }
                    }
                }

            }
        }

    }

}
