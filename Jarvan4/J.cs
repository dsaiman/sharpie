using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jarvan4
{
    class J
    {
        #region Declarations
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Orbwalking.Orbwalker Orbwalker;

        public static SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");
        public static SpellSlot FlashSlot = Player.GetSpellSlot("SummonerFlash");
        public static SpellSlot SmiteSlot = SpellSlot.Unknown;

        //Credits to Kurisu for Smite Stuff :^)
        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        public static readonly string[] MinionList =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp"
        };


        public static Spell Smite;
        public static Spell Flash;
        
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell> {
	        {SpellSlot.Q, new Spell(SpellSlot.Q, 760)},
	        {SpellSlot.W, new Spell(SpellSlot.W, 525)},
	        {SpellSlot.E, new Spell(SpellSlot.E, 800)},
	        {SpellSlot.R, new Spell(SpellSlot.R, 610)}
        };
        #endregion

        #region Orbwalkermodes

        public static void Combo(Obj_AI_Hero target)
        {
            if (JMenu.Config.Item("useIgniteCombo").GetValue<bool>())
            {
                Use.UseIgnite(target);
            }
            if (JMenu.Config.Item("comboItems").GetValue<bool>())
            {
                Use.UseComboItems(target);
            }
            if (JMenu.Config.Item("useSmiteCombo").GetValue<bool>())
            {
                Use.UseSmiteOnChamp(target);
            }
            if (target.Distance(Player) < Spells[SpellSlot.E].Range)
                Use.UseEQCombo(target);
            else if (target.Distance(Player) < Spells[SpellSlot.E].Range + Spells[SpellSlot.W].Range && target.HealthPercentage() < 50 && Spells[SpellSlot.W].IsReady())
                Use.UseEQ(target);
            if (target.Distance(Player) < Spells[SpellSlot.Q].Range && !Spells[SpellSlot.E].IsReady())
                Use.UseQCombo(target);
            if (target.Distance(Player) < Spells[SpellSlot.W].Range)
                Use.UseWCombo(target);
            if (!Use.GapUsed(target))
            {
                return;
            }
            if (JMenu.Config.Item("NoRTower").GetValue<bool>())
            {
                var nearestTower =
                    ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsEnemy && target.Distance(turret) < 775f);
                if (!nearestTower.Any())
                    return;
            }
            if (target.Distance(Player) < Spells[SpellSlot.R].Range &&
                ((target.Distance(Player) > Player.AttackRange + Player.BoundingRadius && JMenu.Config.Item("R" + target.ChampionName).GetValue<bool>()) ||
                OutgoingDamage.ComboDamage(target) >= target.Health))
                Use.UseRCombo(target);
        }

        public static void Mixed(Obj_AI_Hero target)
        {
            if (JMenu.Config.Item("useHydraMix").GetValue<bool>())
            {
                Use.UseHydra(target);
            }
            if (JMenu.Config.Item("useQMix").GetValue<bool>())
            {
                Use.UseQMixed(target);
            }
        }

        public static void LaneClear()
        {
            if (JMenu.Config.Item("useHydraLC").GetValue<bool>())
            {
                Use.UseHydraLc();
            }
            if (JMenu.Config.Item("useQLC").GetValue<bool>())
            {
                Use.UseQLC();
            }

            var objects = ObjectManager.Get<Obj_AI_Base>().Where(obj => obj.Distance(J.Player) < 900f && !obj.IsMe);
            Console.WriteLine(objects.First().Name);


        }

        public static void JungleClear()
        {
            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            MinionList.Any(x => m.Name.StartsWith(x)) && !m.Name.StartsWith("Minion") &&
                            !m.Name.Contains("Mini") && m.Distance(Player.Position) < Spells[SpellSlot.E].Range))
            {
                if (Spells[SpellSlot.E].IsReady() && Spells[SpellSlot.Q].IsReady() &&
                    JMenu.Config.Item("EQJungle").GetValue<bool>())
                {
                    Spells[SpellSlot.E].Cast(minion.Position);
                    if (ObjectManager.Get<Obj_AI_Base>().Count(obj => obj.Name == "Beacon" && obj.Distance(J.Player) < 900f) < 1)
                    {
                        return;
                    }
                    Spells[SpellSlot.Q].Cast(ObjectManager.Get<Obj_AI_Base>().First(obj => obj.Name == "Beacon").Position);
                }
                else if (Player.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.NotLearned)
                {
                    Spells[SpellSlot.E].Cast(minion.Position);
                }
                else
                {
                    Spells[SpellSlot.Q].Cast(minion.Position);
                }
                if (JMenu.Config.Item("WJungle").GetValue<bool>() && minion.CharacterState != GameObjectCharacterState.Asleep)
                    Spells[SpellSlot.W].Cast();

                if (JMenu.Config.Item("useHydraLC").GetValue<bool>())
                {
                    if (Items.CanUseItem(3074) && minion.Distance(Player) < 420)
                    {
                        Items.UseItem(3074);
                    }
                    if (Items.CanUseItem(3077) && minion.Distance(Player) < 420)
                    {
                        Items.UseItem(3077);
                    }
                }
            }
        }

        #endregion

        #region Setskills

        public static void SetSkillShots()
        {
            Spells[SpellSlot.Q].SetSkillshot(250f, 70f, 1450f, false, SkillshotType.SkillshotLine);
            Spells[SpellSlot.E].SetSkillshot(500f, 175f, Int32.MaxValue, false, SkillshotType.SkillshotCircle);
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
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => String.Equals(spell.Name, GetSmiteType(), StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
                Smite = new Spell(SmiteSlot, 700);
                return;
            }
        }
        #endregion
    }
}
