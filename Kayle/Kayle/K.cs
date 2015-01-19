using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Kayle
{
    class K
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Orbwalking.Orbwalker Orbwalker;

        public static SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");
        public static SpellSlot smiteSlot = SpellSlot.Unknown;

        //Credits to Kurisu for Smite Stuff :^)
        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static Spellbook SBook = Player.Spellbook;
        public static SpellDataInst Qdata = SBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = SBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = SBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = SBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 650f);
        public static Spell W = new Spell(SpellSlot.W, 900f);
        public static Spell E = new Spell(SpellSlot.E, 625f);
        public static Spell R = new Spell(SpellSlot.R, 900f);
        public static Spell Smite;

        public static void Combo(Obj_AI_Hero target)
        {

            if (KMenu.Config.Item("useIgniteCombo").GetValue<bool>())
                Use.UseIgnite(target);
            if (KMenu.Config.Item("useSmiteCombo").GetValue<bool>())
                Use.UseSmiteOnChamp(target);
            if (KMenu.Config.Item("comboItems").GetValue<bool>())
                Use.UseComboItems(target);
            if (Q.IsReady() && target.Distance(ObjectManager.Player) <= Q.Range)
            {
                Use.UseQCombo(target);
            }
            if (W.IsReady())
            {
                Use.UseWCombo(target);
            }
            if (E.IsReady() && target.Distance(ObjectManager.Player) <= E.Range)
            {
                E.Cast();
            }
        }

        public static void Mixed()
        {
            if (E.IsReady())
                E.Cast();
        }

        public static void Mixed(Obj_AI_Hero target)
        {
            Mixed();
            Use.UseEMixed(target);
        }

        public static void LaneClear()
        {
            if (E.IsReady())
                E.Cast();
            Use.UseELaneClear();
        }

        public static string GetSmiteType()
        {
            if (SmiteBlue.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(id => Items.HasItem(id)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        public static void GetSmiteSlot()
        {
            foreach (var spell in ObjectManager.Player.Spellbook.Spells.Where(spell => String.Equals(spell.Name, GetSmiteType(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                Smite = new Spell(smiteSlot, 700);
                return;
            }
        }


    }
}
