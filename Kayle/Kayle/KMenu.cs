using LeagueSharp.Common;

namespace Kayle
{
    class KMenu
    {
        public static Menu Config;

        public static void AddMenu()
        {
            Config = new Menu("Kayle", "Kayle", true);

            //Orbwalker
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            K.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            //TS
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Combo
            Config.AddSubMenu(new Menu("Combo", "combo"));
            Config.SubMenu("combo").AddItem(new MenuItem("useWCombo", "Use W")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("useIgniteCombo", "Use Ignite")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("useSmiteCombo", "Use Smite on champ")).SetValue(true);

            //Self Utils
            Config.AddSubMenu(new Menu("Self Utilities", "Sutils"));
            Config.SubMenu("Sutils").AddItem(new MenuItem("QWgap", "Use QW on Gapcloser")).SetValue(true);
            Config.SubMenu("Sutils").AddItem(new MenuItem("useWSustain", "Use W for sustain")).SetValue(true);
            Config.SubMenu("Sutils").AddItem(new MenuItem("autoW", "Auto W on lethal damage")).SetValue(true);
            Config.SubMenu("Sutils").AddItem(new MenuItem("autoR", "Auto R on lethal damage")).SetValue(true);
            Config.SubMenu("Sutils").AddItem(new MenuItem("manR", "Manual R (Set %)")).SetValue(false);
            Config.SubMenu("Sutils").AddItem(new MenuItem("RonHp", "Use R on % hp")).SetValue(new Slider(25, 1, 99));

            ////Ally Utils
            //Config.AddSubMenu(new Menu("Ally Utilities", "Autils"));
            //Config.SubMenu("Autils").AddSubMenu(new Menu("R Ally", "R"));
            //Config.SubMenu("Autils")
            //    .SubMenu("R")
            //    .AddItem(new MenuItem("autoRAlly", "Auto R on lethal damage on ally"))
            //    .SetValue(false);
            //Config.SubMenu("Autils").SubMenu("R").AddItem(new MenuItem("manRAlly", "Manual R (Set %)")).SetValue(false);
            //Config.SubMenu("Autils")
            //    .SubMenu("R")
            //    .AddItem((new MenuItem("RonHpAlly", "Use R on ally % hp")).SetValue(new Slider(25, 1, 99)));
            //Config.SubMenu("Autils").SubMenu("R").AddSubMenu(new Menu("Allies to R", "RAlly"));
            //foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly && !ally.IsMe))
            //{
            //    Config.SubMenu("Autils")
            //        .SubMenu("RAlly")
            //        .SubMenu("R")
            //        .AddItem(new MenuItem("R on " + ally.ChampionName, ally.ChampionName))
            //        .SetValue(false);
            //}
            //Config.SubMenu("Autils").AddSubMenu(new Menu("W ally", "W"));
            //Config.SubMenu("Autils")
            //    .SubMenu("W")
            //    .AddItem(new MenuItem("autoWAlly", "Auto W on lethal damage on ally"))
            //    .SetValue(false);
            //Config.SubMenu("Autils").SubMenu("W").AddItem(new MenuItem("manWAlly", "Manual W (Set %)")).SetValue(false);
            //Config.SubMenu("Autils")
            //    .SubMenu("W")
            //    .AddItem((new MenuItem("WonHpAlly", "Use W on ally % hp")).SetValue(new Slider(25, 1, 99)));
            //Config.SubMenu("Autils").SubMenu("W").AddSubMenu(new Menu("Allies to W", "WAlly"));
            //foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly && !ally.IsMe))
            //{
            //    Config.SubMenu("Autils")
            //        .SubMenu("WAlly")
            //        .SubMenu("W")
            //        .AddItem(new MenuItem("W on " + ally.ChampionName, ally.ChampionName))
            //        .SetValue(false);
            //}

            //Drawings
            Config.AddSubMenu(new Menu("Drawings", "drawings"));
            Config.SubMenu("drawings").AddItem(new MenuItem("drawQ", "Draw Q range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawW", "Draw W range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawE", "Draw E range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawR", "Draw R range")).SetValue(false);

            Config.AddToMainMenu();

        }
    }
}