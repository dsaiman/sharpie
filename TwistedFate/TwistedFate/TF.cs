using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace TwistedFate
{
    internal class TF
    {
        public static Orbwalking.Orbwalker Orbwalker;
        public static SpellSlot IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
        public static SpellSlot smiteSlot = SpellSlot.Unknown;

        //Credits to Kurisu for Smite Stuff :^)
        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static Spellbook SBook = ObjectManager.Player.Spellbook;
        public static SpellDataInst Qdata = SBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = SBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = SBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = SBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 1450);
        public static float QAngle = 28 * (float) Math.PI / 180;
        public static Spell W = new Spell(SpellSlot.W, 600);
        public static Spell E = new Spell(SpellSlot.E, 0);
        public static Spell R = new Spell(SpellSlot.R, 5500);
        public static Spell Smite;

        public static void SetSkillShots()
        {
            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);
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
                smiteSlot = spell.Slot;
                Smite = new Spell(smiteSlot, 700);
                return;
            }
        }

        public static void Combo(Obj_AI_Hero target)
        {
            Use.UseItems(target);
            if (target.Distance(ObjectManager.Player) < W.Range + 200f)
            {
                Use.UseWCombo(target);
            }
            if (Q.IsReady() && target.Distance(ObjectManager.Player) < Q.Range)
            {
                Use.UseQCombo(target);
            }
            Use.UseIgnite(target);
        }

        public static void Harass()
        {
            Use.UseWHarass();
            Use.UseQHarass();
        }

        public static void Harass(Obj_AI_Hero target)
        {
            Use.UseWHarass(target);
            Use.UseQHarass(target);
        }

        public static void LaneClear()
        {
            Use.UseWLaneClear();
            Use.UseQLaneClear();
        }

        //Teach me esk0r senpai
        private static int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Q.Range * (position - startPoint).Normalized();
            var originalEndPoint = startPoint + originalDirection;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                for (var k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0)
                    {
                        endPoint = originalEndPoint;
                    }
                    if (k == 1)
                    {
                        endPoint = startPoint + originalDirection.Rotated(QAngle);
                    }
                    if (k == 2)
                    {
                        endPoint = startPoint + originalDirection.Rotated(-QAngle);
                    }

                    if (point.Distance(startPoint, endPoint, true, true) <
                        (Q.Width + hitBoxes[i]) * (Q.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }

            return result;
        }

        public static Vector3 GetBestQPosition(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            var startPoint = ObjectManager.Player.ServerPosition.To2D();
            var originalDirection = Q.Range * (unitPosition - startPoint).Normalized();

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = Q.GetPrediction(enemy);
                    if (pos.Hitchance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int) enemy.BoundingRadius);
                    }
                }
            }


            var posiblePositions = new List<Vector2>();

            for (var i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    posiblePositions.Add(unitPosition + originalDirection.Rotated(0));
                }
                if (i == 1)
                {
                    posiblePositions.Add(startPoint + originalDirection.Rotated(QAngle));
                }
                if (i == 2)
                {
                    posiblePositions.Add(startPoint + originalDirection.Rotated(-QAngle));
                }
            }


            if (startPoint.Distance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).Normalized().Perpendicular();
                    var k = (2 / 3 * (unit.BoundingRadius + Q.Width));
                    posiblePositions.Add(startPoint - k * direction);
                    posiblePositions.Add(startPoint + k * direction);
                }
            }

            var bestPosition = new Vector2();
            var bestHit = -1;

            foreach (var position in posiblePositions)
            {
                var hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            //if (bestHit + 1 <= minTargets)
            //    return;

            return bestPosition.To3D();
        }
    }
}