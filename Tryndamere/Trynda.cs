using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Tryhardamere
{
    internal class Trynda
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
        public static Spell Q = new Spell(SpellSlot.Q, 0);
        public static Spell W = new Spell(SpellSlot.W, 400);
        public static Spell E = new Spell(SpellSlot.E, 660);
        public static Spell R = new Spell(SpellSlot.R, 0);
        public static Spell Smite;



        public static void Combo(Obj_AI_Hero target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Tryhardamere.Config.Item("useSmiteCombo").GetValue<bool>())
            {
                Use.UseSmiteOnChamp(target);
            }
            if (Tryhardamere.Config.Item("useIgniteCombo").GetValue<bool>())
            {
                Use.UseIgnite(target);
            }
            if (Tryhardamere.Config.Item("useQCombo").GetValue<bool>())
            {
                Use.UseQSmart();
            }
            if (Tryhardamere.Config.Item("useWCombo").GetValue<bool>())
            {
                Use.UseWSmart(target);
            }
            if (Tryhardamere.Config.Item("useECombo").GetValue<bool>())
            {
                Use.UseESmart(target);
            }
            if (Tryhardamere.Config.Item("comboItems").GetValue<bool>())
            {
                Use.UseComboItems(target);
            }
        }

        public static void Mixed(Obj_AI_Hero target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Tryhardamere.Config.Item("useW").GetValue<bool>())
            {
                Use.UseWTrade(target);
            }
            if (Tryhardamere.Config.Item("useHydraMix").GetValue<bool>())
            {
                Use.UseHydra(target);
            }
        }

        public static void LaneClear(Obj_AI_Hero target)
        {
            if (Tryhardamere.Config.Item("useHydraLC").GetValue<bool>())
            {
                Use.UseHydraLc();
            }
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Tryhardamere.Config.Item("useW").GetValue<bool>())
            {
                Use.UseWTrade(target);
            }
        }

        public static void SetSkillShots()
        {
            W.SetSkillshot(-0.5f, 0, 500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 160, 700, false, SkillshotType.SkillshotLine);
        }

        public static int MyHpPerc()
        {
            return (int) ((Player.Health / Player.MaxHealth) * 100);
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
