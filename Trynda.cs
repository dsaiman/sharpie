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
    class Trynda
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spellbook SBook = Player.Spellbook;
        
        public static Orbwalking.Orbwalker Orbwalker;

        public static SpellDataInst Qdata = SBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = SBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = SBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = SBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 0);
        public static Spell W = new Spell(SpellSlot.W, 400);
        public static Spell E = new Spell(SpellSlot.E, 660);
        public static Spell R = new Spell(SpellSlot.R, 0);


        public static void Combo(Obj_AI_Hero target)
        {
            if (!target.IsValidTarget())
                return;
            if (Tryhardamere.Config.Item("useQCombo").GetValue<bool>())
                Use.UseQSmart();
            if (Tryhardamere.Config.Item("useWCombo").GetValue<bool>())
                Use.UseWSmart(target);
            if (Tryhardamere.Config.Item("useECombo").GetValue<bool>())
                Use.UseESmart(target);
            if (Tryhardamere.Config.Item("comboItems").GetValue<bool>())
                Use.UseComboItems(target);
        }

        public static void Mixed(Obj_AI_Hero target)
        {
            if (!target.IsValidTarget())
                return;
            if (Tryhardamere.Config.Item("useW").GetValue<bool>())
                Use.UseWTrade(target);
            if (Tryhardamere.Config.Item("useHydraMix").GetValue<bool>())
                Use.UseHydra(target);
        }

        public static void LaneClear(Obj_AI_Hero target)
        {
            if (Tryhardamere.Config.Item("useHydraLC").GetValue<bool>())
                Use.UseHydraLC();
            if (!target.IsValidTarget())
                return;
            if (Tryhardamere.Config.Item("useW").GetValue<bool>())
                Use.UseWTrade(target);
        }

        public static void SetSkillShots()
        {
            W.SetSkillshot(-0.5f, 0, 500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 160, 700, false, SkillshotType.SkillshotLine);
        }

        public static int MyHpPerc()
        {
            return (int)((Player.Health / Player.MaxHealth) * 100);
        }
 
    }
}
