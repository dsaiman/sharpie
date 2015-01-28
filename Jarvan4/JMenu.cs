using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jarvan4
{
    class JMenu
    {
        public static Menu Config;
        public static Menu MenuExtras;

        public static void AddMenu()
        {
            Config = new Menu("Jarvan", "Jarvan", true);

            //Orbwalker
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            J.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            //TS
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Combo
            Config.AddSubMenu(new Menu("Combo", "combo"));
            Config.SubMenu("combo").AddItem(new MenuItem("EQR", "EQR Reaching Combo").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("combo").AddItem(new MenuItem("EQFlash", "EQ Flash Reaching Combo").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("useIgniteCombo", "Use Ignite")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("useSmiteCombo", "Use Smite on champ")).SetValue(true);

            //Mixed
            Config.AddSubMenu(new Menu("Mixed", "mix"));
            Config.SubMenu("mix").AddItem(new MenuItem("useQMix", "Use Q to harass")).SetValue(true);
            Config.SubMenu("mix").AddItem(new MenuItem("useHydraMix", "Use Hydra to harass")).SetValue(true);

            //LaneClear
            Config.AddSubMenu(new Menu("LaneClear and Jungle", "lClear"));
            Config.SubMenu("lClear").AddItem(new MenuItem("useQLC", "Use Q to LaneClear")).SetValue(true);
            Config.SubMenu("lClear").AddItem(new MenuItem("JungleMode", "Activate Jungle Mode")).SetValue(true);
            Config.SubMenu("lClear").AddItem(new MenuItem("EQJungle", "Use EQ to Jungle Clear")).SetValue(true);
            Config.SubMenu("lClear").AddItem(new MenuItem("WJungle", "Use W in Jungle")).SetValue(true);
            Config.SubMenu("lClear").AddItem(new MenuItem("useHydraLC", "Use Hydra")).SetValue(true);

            // Utilities
            Config.AddSubMenu(new Menu("Utilities", "utils"));
            Config.SubMenu("utils").AddItem(new MenuItem("WIncoming", "Use W on Incoming Damage (with slow check)")).SetValue(false);
            Config.SubMenu("utils").AddItem(new MenuItem("NoRTower", "Never R under enemy tower")).SetValue(false);
            Config.SubMenu("utils").AddItem(new MenuItem("TowerTrap", "Under tower R outplay :^)")).SetValue(false);
            Config.SubMenu("utils").AddSubMenu(new Menu("Always R", "alwR"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
            {
                Config.SubMenu("utils")
                    .SubMenu("alwR")
                    .AddItem(new MenuItem("R" + enemy.ChampionName, enemy.ChampionName).SetValue(false));
            }
            Config.SubMenu("utils").AddSubMenu(new Menu("Wait for Gapcloser to R", "waitGC"));
            foreach (var gc in from gc in AntiGapcloser.Spells
                               from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy)
                               where gc.ChampionName == enemy.ChampionName
                               select gc)
            {
                Config.SubMenu("utils").SubMenu("waitGC").AddItem(new MenuItem("waitGap" + gc.Slot + gc.ChampionName, gc.ChampionName + " " + gc.Slot.ToString())).SetValue(true);
            }
            Config.SubMenu("utils").AddSubMenu(new Menu("Anti Gapcloser", "GC"));
            Config.SubMenu("utils").SubMenu("GC").AddItem(new MenuItem("WGap", "Use W on gapcloser")).SetValue(true);
            Config.SubMenu("utils").AddSubMenu(new Menu("Interrupter", "Inte"));
            foreach (var interr in from interr in Interrupter.Spells
                                   from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy)
                                   where interr.ChampionName == enemy.ChampionName
                                   select interr)
            {
                Config.SubMenu("utils").SubMenu("Inte").AddItem(new MenuItem("Interr" + interr.Slot + interr.ChampionName, interr.ChampionName + " " + interr.Slot.ToString())).SetValue(true);
            }
            Config.SubMenu("utils").SubMenu("Inte").AddItem(new MenuItem("EQInterrupt", "Use E to interrupt important spells")).SetValue(true);


            //Drawings
            Config.AddSubMenu(new Menu("Drawings", "drawings"));
            Config.SubMenu("drawings").AddItem(new MenuItem("drawQ", "Draw Q range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawE", "Draw E range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawR", "Draw R range")).SetValue(false);
            Config.SubMenu("drawings").AddItem(new MenuItem("drawEQR", "Draw EQR range")).SetValue(false);


            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);
            Config.SubMenu("drawings").AddItem(dmgAfterComboItem);
            Utility.HpBarDamageIndicator.DamageToUnit = OutgoingDamage.ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            { Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>(); };

            //new PotionManager();
            //Config.AddSubMenu(MenuExtras);
            Config.AddToMainMenu();

        }
    }
}
