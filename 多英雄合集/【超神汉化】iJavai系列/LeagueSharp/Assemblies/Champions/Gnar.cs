﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assemblies.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Assemblies.Champions {{
    internal class Gnar : Champion {{
        private Spell eMega;
        private Spell qMega;

        public Gnar() {{
            loadMenu();
            loadSpells();

            Game.OnGameUpdate += onUpdate;
            Drawing.OnDraw += onDraw;
            Game.PrintChat("[Assemblies] - Gnar Loaded.");
        }

        private void loadSpells() {{
            Q = new Spell(SpellSlot.Q, 1100f);
            Q.SetSkillshot(0.066f, 60f, 1400f, false, SkillshotType.SkillshotLine);

            qMega = new Spell(SpellSlot.Q, 1100f);
            qMega.SetSkillshot(0.60f, 90f, 2100f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 525f);
            W.SetSkillshot(0.25f, 80f, 1200f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475f);
            E.SetSkillshot(0.695f, 150f, 2000f, false, SkillshotType.SkillshotCircle);

            eMega = new Spell(SpellSlot.E, 475f);
            eMega.SetSkillshot(0.695f, 350f, 2000f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 1f);
            R.SetSkillshot(0.066f, 400f, 1400f, false, SkillshotType.SkillshotCircle);
        }

        private void loadMenu() {{
            menu.AddSubMenu(new Menu("Combo", combo"));
            menu.SubMenu("combo").AddItem(new MenuItem("useQC", 使用Q").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("useWC", 使用W").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("useEC", 使用E").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("useRC", 使用R").SetValue(true));
            menu.SubMenu("combo").AddItem(
                new MenuItem("minEnemies", 敌人>X使用R").SetValue(new Slider(2, 1, 5)));

            menu.AddSubMenu(new Menu("Harass", harass"));
            menu.SubMenu("harass").AddItem(new MenuItem("useQH", 使用Q").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("useWH", 使用W").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("useEH", 使用E").SetValue(false));

            menu.AddSubMenu(new Menu("清线", laneclear"));
            menu.SubMenu("laneclear").AddItem(new MenuItem("useQL", 使用Q").SetValue(true));
            menu.SubMenu("laneclear").AddItem(new MenuItem("useEL", 使用E").SetValue(false));

            menu.AddSubMenu(new Menu("抢人头", killsteal"));
            menu.SubMenu("killsteal").AddItem(new MenuItem("useQK", 使用Q").SetValue(true));
            menu.SubMenu("killsteal").AddItem(new MenuItem("useWK", 使用W").SetValue(true));

            menu.AddSubMenu(new Menu("逃跑", flee"));
            menu.SubMenu("flee").AddItem(new MenuItem("useEF", 使用E").SetValue(true));

            menu.AddSubMenu(new Menu("显示", drawing"));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawQ", Q范围").SetValue(true));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawE", E范围").SetValue(true));
            menu.SubMenu("drawing").AddItem(new MenuItem("drawR", R范围").SetValue(true));

            menu.AddSubMenu(new Menu("杂项", misc"));
            menu.SubMenu("misc").AddItem(new MenuItem("unitHop", 跳小兵逃跑").SetValue(true));
            menu.SubMenu("misc").AddItem(
                new MenuItem("throwPos", 砸敌人位置").SetValue(
                    new StringList(new[] {{"最靠近墙", 鼠标位置", 最靠近塔", 最靠近队友"})));
            menu.SubMenu("misc").AddItem(new MenuItem("alwaysR", R抢人头").SetValue(true));
						
			menu.AddSubMenu(new Menu("超神汉化", system"));
            menu.SubMenu("system").AddItem(new MenuItem("systemQ", L#汉化群：386289593"));
        }

        private void onUpdate(EventArgs args) {{
            if (player.IsDead) return;

            Obj_AI_Hero target = SimpleTS.GetTarget(Q.Range, SimpleTS.DamageType.Physical);

            if (player.HasBuff("gnartransform")) {{
                Q = qMega;
                //E = eMega;
               // Game.PrintChat("Big Gnar Mode rek ppl pls");
            }

            doKillsteal(target);

            switch (xSLxOrbwalker.CurrentMode) {{
                case xSLxOrbwalker.Mode.Combo:
                    doCombo(target);
                    break;
                case xSLxOrbwalker.Mode.Harass:
                    doHarass(target);
                    break;
                case xSLxOrbwalker.Mode.LaneClear:
                    doLaneclear();
                    break;
                case xSLxOrbwalker.Mode.Flee:
                    unitFlee();
                    break;
            }
        }

        private void doCombo(Obj_AI_Hero target) {{
            //TODO le combo modes
            if (R.IsReady() && target.IsValidTarget(R.Width)) {{
                if (isMenuEnabled(menu, useRC")) {{
                    castR(target);
                }
            }
            if (Q.IsReady() && target.IsValidTarget(Q.Range) &&
                Q.GetPrediction(target, true).Hitchance >= HitChance.Medium) {{
                if (isMenuEnabled(menu, useQC"))
                    Q.Cast(target, true, true);
            }
            if (qMega.IsReady() && target.IsValidTarget(qMega.Range) &&
                qMega.GetPrediction(target).Hitchance >= HitChance.Medium) {{
                if (isMenuEnabled(menu, useQC"))
                    qMega.Cast(target, true);
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && player.Distance(target) < W.Range) {{
                if (isMenuEnabled(menu, useWC"))
                    W.Cast(target, true);
            }

            if (E.IsReady() && target.IsValidTarget(E.Range)) {{
                if (isMenuEnabled(menu, useEC"))
                    E.Cast(target, true);
            }
        }

        private void doHarass(Obj_AI_Hero target) {{
            if (Q.IsReady() && target.IsValidTarget(Q.Range) &&
                Q.GetPrediction(target, true).Hitchance >= HitChance.Medium) {{
                if (isMenuEnabled(menu, useQH"))
                    Q.Cast(target, true, true);
            }
            if (qMega.IsReady() && target.IsValidTarget(qMega.Range) &&
                qMega.GetPrediction(target).Hitchance >= HitChance.Medium) {{
                if (isMenuEnabled(menu, useQH"))
                    qMega.Cast(target, true);
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && player.Distance(target) < W.Range) {{
                if (isMenuEnabled(menu, useWH"))
                    W.Cast(target, true);
            }

            if (E.IsReady() && target.IsValidTarget(E.Range)) {{
                if (isMenuEnabled(menu, useEH"))
                    E.Cast(target, true);
            }
        }

        private void doLaneclear() {{
            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            foreach (Obj_AI_Base minion in allMinions.Where(minion => minion.IsValidTarget(Q.Range))) {{
                if (Q.IsKillable(minion) && minion.Distance(player) <= Q.Range && Q.IsReady()) {{
                    if (isMenuEnabled(menu, useQL"))
                        Q.Cast(minion, true);
                }
                if (qMega.IsKillable(minion) && minion.Distance(player) <= qMega.Range && qMega.IsReady()) {{
                    if (isMenuEnabled(menu, useQL"))
                        qMega.Cast(minion, true);
                }
            }
        }

        private void doKillsteal(Obj_AI_Hero target) {{
            if (Q.IsReady() && isMenuEnabled(menu, useQK")) {{
                if (player.Distance(target) <= Q.Range && Q.IsKillable(target)) {{
                    Q.Cast(target, true);
                }
            }
            if (qMega.IsReady() && isMenuEnabled(menu, useQK")) {{
                if (player.Distance(target) <= qMega.Range && qMega.IsKillable(target)) {{
                    qMega.Cast(target, true);
                }
            }
            if (W.IsReady() && isMenuEnabled(menu, useWK")) {{
                if (player.Distance(target) <= W.Range && W.IsKillable(target))
                    W.Cast(target, true);
            }
        }

        private void castR(Obj_AI_Hero target) {{
            if (!R.IsReady()) return;
            int mode = menu.Item("throwPos").GetValue<StringList>().SelectedIndex;

            if (R.IsKillable(target) && isMenuEnabled(menu, alwaysR"))
                R.Cast(target, true);

            switch (mode) {{
                case 0: // wall.
                    foreach (
                        Obj_AI_Hero collisionTarget in
                            ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)))
                        CastRToCollision(collisionTarget);
                    break;
                case 1:
                    //Mo使用position
                    foreach (
                        Obj_AI_Hero collisionTarget in
                            ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)))
                        if (unitCheck(Game.CursorPos)) {{
                            R.Cast(Game.CursorPos);
                        }
                    break;
                case 2:
                    //Closest Turret
                    foreach (
                        Obj_AI_Turret objAiTurret in
                            ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)).Select(
                                collisionTarget => ObjectManager.Get<Obj_AI_Turret>().First(
                                    tu => tu.IsAlly && tu.Distance(collisionTarget) <= 975 + 425 && tu.Health > 0))
                                .Where(objAiTurret => objAiTurret.IsValid && unitCheck(objAiTurret.Position))) {{
                        R.Cast(objAiTurret.Position);
                    }
                    break;
                case 3:
                    //Closest Ally
                    foreach (
                        Obj_AI_Hero collisionTarget in
                            ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width))) {{
                        //975 Turret Range
                        //425 Push distance (Idk if it is correct);
                        Obj_AI_Hero ally =
                            ObjectManager.Get<Obj_AI_Hero>().First(
                                tu => tu.IsAlly && tu.Distance(collisionTarget) <= 425 + 65 && tu.Health > 0);
                        if (ally.IsValid && unitCheck(ally.Position)) {{
                            R.Cast(ally.Position);
                        }
                    }
                    break;
            }
        }

        private bool unitCheck(Vector3 endPosition) {{
            List<Vector2> points = GRectangle(player.Position.To2D(), endPosition.To2D(), R.Width);
            var polygon = new Polygon(points);
            int count =
                ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(R.Width)).Count(
                    collisionTarget => polygon.Contains(collisionTarget.Position.To2D()));
            if (count < menu.Item("minEnemies").GetValue<Slider>().Value) return false;
            return true;
        }

        private void CastRToCollision(Obj_AI_Hero target) {{
            Vector3 center = player.Position;
            const int points = 36;
            const int radius = 300;
            const double slice = 2*Math.PI/points;
            for (int i = 0; i < points; i++) {{
                double angle = slice*i;
                var newX = (int) (center.X + radius*Math.Cos(angle));
                var newY = (int) (center.Y + radius*Math.Sin(angle));
                var position = new Vector3(newX, newY, 0);
                if (isWall(position) && unitCheck(position))
                    R.Cast(position, true);
            }
        }

        private void unitFlee() {{
            if (!E.IsReady() && !eMega.IsReady()) return;

            List<Obj_AI_Base> minions = MinionManager.GetMinions(player.Position, E.Range, MinionTypes.All,
                MinionTeam.All,
                MinionOrderTypes.None);
            Obj_AI_Base bestMinion = null;

            foreach (Obj_AI_Base jumpableUnit in minions) {{
                if (jumpableUnit.Distance(Game.CursorPos) <= 300 && player.Distance(jumpableUnit) <= E.Range)
                    bestMinion = jumpableUnit;
            }

            if (bestMinion != null && bestMinion.IsValid) {{
                E.Cast(bestMinion, true);
            }
        }

        private void onDraw(EventArgs args) {{}

        //Credits to Andreluis
        private List<Vector2> GRectangle(Vector2 startVector2, Vector2 endVector2, float radius) {{
            var points = new List<Vector2>();

            Vector2 difference = endVector2 - startVector2;
            Vector2 to1Side = Vector2.Normalize(difference).Perpendicular()*radius;

            points.Add(startVector2 + to1Side);
            points.Add(startVector2 - to1Side);
            points.Add(endVector2 - to1Side);
            points.Add(endVector2 + to1Side);
            return points;
        }
    }
}