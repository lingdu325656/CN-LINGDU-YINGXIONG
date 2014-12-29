﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{{
    class Fiora : Champion
    {{
        public Fiora()
        {{
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {{
            Q = new Spell(SpellSlot.Q, 600);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E);

            R = new Spell(SpellSlot.R, 400);
        }

        private void LoadMenu()
        {{
            //key
            var key = new Menu("热键", Key");
            {{
                key.AddItem(new MenuItem("ComboActive", 连招").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", 骚扰").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LaneClearActive", 清线").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LastHitKey", 补兵").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("Combo_Switch", 模式切换热键").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            var spellMenu = new Menu("技能", SpellMenu");
            {{
                var qMenu = new Menu("Q", QMenu");
                {{
                    qMenu.AddItem(new MenuItem("Q_Min_Distance", Q最小范围").SetValue(new Slider(300, 0, 600)));
                    qMenu.AddItem(new MenuItem("Q_Gap_Close", Q小兵突进").SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }
                var wMenu = new Menu("W", WMenu");
                {{
                    wMenu.AddItem(new MenuItem("W_Incoming", W格挡伤害").SetValue(true));
                    wMenu.AddItem(new MenuItem("W_minion", W格挡小兵").SetValue(false));
                    spellMenu.AddSubMenu(wMenu);
                }
                var eMenu = new Menu("E", EMenu");
                {{
                    eMenu.AddItem(new MenuItem("E_Reset", E重置攻击").SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("R", RMenu");
                {{
                    rMenu.AddItem(new MenuItem("R_If_HP", 血量<X使用R").SetValue(new Slider(20)));

                    //evading spells
                    var dangerous = new Menu("躲避危险技能", Dodge Dangerous");
                    {{
                        foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                        {{
                            dangerous.AddSubMenu(new Menu(hero.ChampionName, hero.ChampionName));
                            dangerous.SubMenu(hero.ChampionName).AddItem(new MenuItem(hero.Spellbook.GetSpell(SpellSlot.Q).Name + R_Dodge", hero.Spellbook.GetSpell(SpellSlot.Q).Name).SetValue(false));
                            dangerous.SubMenu(hero.ChampionName).AddItem(new MenuItem(hero.Spellbook.GetSpell(SpellSlot.W).Name + R_Dodge", hero.Spellbook.GetSpell(SpellSlot.W).Name).SetValue(false));
                            dangerous.SubMenu(hero.ChampionName).AddItem(new MenuItem(hero.Spellbook.GetSpell(SpellSlot.E).Name + R_Dodge", hero.Spellbook.GetSpell(SpellSlot.E).Name).SetValue(false));
                            dangerous.SubMenu(hero.ChampionName).AddItem(new MenuItem(hero.Spellbook.GetSpell(SpellSlot.R).Name + R_Dodge", hero.Spellbook.GetSpell(SpellSlot.R).Name).SetValue(false));
                        }
                        rMenu.AddSubMenu(dangerous);
                    }

                    spellMenu.AddSubMenu(rMenu);
                }

                menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("连招", Combo");
            {{
                combo.AddItem(new MenuItem("selected", 锁定目标").SetValue(true));
                combo.AddItem(new MenuItem("Combo_mode", 连招模式").SetValue(new StringList(new[] {{ 正常", Q-AA-Q-AA-Ult" })));
                combo.AddItem(new MenuItem("UseQCombo", 使用Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", 使用W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", 使用E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", 使用R").SetValue(true));
                combo.AddItem(new MenuItem("Ignite", 使用点燃").SetValue(true));
                combo.AddItem(new MenuItem("Botrk", 使用破败/弯刀").SetValue(true));
                //add to menu
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("骚扰", Harass");
            {{
                harass.AddItem(new MenuItem("UseQHarass", 使用Q").SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", 使用W").SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", 使用E").SetValue(true));
                AddManaManagertoMenu(harass, Harass", 30);
                //add to menu
                menu.AddSubMenu(harass);
            }

            var lastHit = new Menu("补兵", Lasthit");
            {{
                lastHit.AddItem(new MenuItem("UseQLastHit", 使用Q").SetValue(true));
                AddManaManagertoMenu(lastHit, Lasthit", 30);
                //add to menu
                menu.AddSubMenu(lastHit);
            }

            var farm = new Menu("清线", LaneClear");
            {{
                farm.AddItem(new MenuItem("UseQFarm", 使用Q").SetValue(true));
                farm.AddItem(new MenuItem("UseQFarm_Tower", 塔下不Q").SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", 使用E").SetValue(true));
                AddManaManagertoMenu(farm, LaneClear", 30);
                //add to menu
                menu.AddSubMenu(farm);
            }
            var misc = new Menu("杂项", Misc");
            {{
                misc.AddItem(new MenuItem("smartKS", 智能抢人头").SetValue(true));
                //add to menu
                menu.AddSubMenu(misc);
            }
            var drawMenu = new Menu("显示", Drawing");
            {{
                drawMenu.AddItem(new MenuItem("Draw_Disabled", 禁用").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", Q范围").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", E范围").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", R范围").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Killable", 显示R可杀标记").SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", 显示伤害").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", 显示连招伤害").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {{
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {{
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                //add to menu
                menu.AddSubMenu(drawMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {{
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q)*2;

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R) / countEnemiesNearPosition(target.ServerPosition, R.Range);

            if (Items.CanUseItem(Bilge.Id))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(Botrk.Id))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Botrk);

            if (Items.CanUseItem(3077))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Tiamat);

            if (Items.CanUseItem(3074))
                comboDamage += Player.GetItemDamage(target, Damage.DamageItems.Hydra);

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                comboDamage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 3);
        }

        private void Combo()
        {{
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), Combo");
        }

        private void Harass()
        {{
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false, Harass");
        }

        private void UseSpells(bool useQ, bool useE, bool useR, string source)
        {{
            if (source == Harass" && !HasMana("Harass"))
                return;

            if (useQ)
                Cast_Q();

            if (source == Combo")
            {{
                var qTarget = SimpleTS.GetTarget(Q.Range, SimpleTS.DamageType.Physical);
                if (qTarget != null)
                {{
                    if (GetComboDamage(qTarget) >= qTarget.Health && Ignite_Ready() && menu.Item("Ignite").GetValue<bool>() && Player.Distance(qTarget) < 300)
                        Use_Ignite(qTarget);

                    if (menu.Item("Botrk").GetValue<bool>())
                    {{
                        if (GetComboDamage(qTarget) > qTarget.Health && !qTarget.HasBuffOfType(BuffType.Slow))
                        {{
                            Use_Bilge(qTarget);
                            Use_Botrk(qTarget);
                        }
                    }
                }
            }

            if (useE && !menu.Item("E_Reset").GetValue<bool>())
                E.Cast(packets());

            if (useR)
                Cast_R();

        }

        private void Lasthit()
        {{
            if (menu.Item("UseQLastHit").GetValue<bool>() && HasMana("Lasthit"))
                Cast_Q_Last_Hit();
        }

        private void Farm()
        {{
            if (!HasMana("LaneClear"))
                return;

            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(Player.ServerPosition, Player.AttackRange + Player.BoundingRadius,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useE = menu.Item("UseEFarm").GetValue<bool>();

            if (useQ)
                Cast_Q_Last_Hit();

            if (useE && allMinionsE.Count > 0 && E.IsReady())
                E.Cast();

        }

        private void SmartKs()
        {{
            if (!menu.Item("smartKS").GetValue<bool>())
                return;
            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.HasBuffOfType(BuffType.Invulnerability)))
            {{
                //Q *2
                if (Player.GetSpellDamage(target, SpellSlot.Q)*2 > target.Health && Player.Distance(target) < Q.Range && Q.IsReady())
                {{
                    Q.CastOnUnit(target, packets());
                    return;
                }
                //Q
                if (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health && Player.Distance(target) < Q.Range && Q.IsReady())
                {{
                    Q.CastOnUnit(target, packets());
                    return;
                }
            }
        }

        private void Cast_Q()
        {{
            var target = SimpleTS.GetTarget(Q.Range * 2, SimpleTS.DamageType.Physical);

            if (GetTargetFocus(Q.Range) != null)
                target = GetTargetFocus(Q.Range);

            int mode = menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
            if (mode == 0)
            {{
                if (Q.IsReady() && target != null)
                {{
                    if (Q.IsKillable(target))
                        Q.CastOnUnit(target, packets());

                    if (Player.GetSpellDamage(target, SpellSlot.Q)*2 > target.Health)
                        Q.CastOnUnit(target, packets());

                    if (Environment.TickCount - Q.LastCastAttemptT > 3800)
                        Q.CastOnUnit(target, packets());

                    var minDistance = menu.Item("Q_Min_Distance").GetValue<Slider>().Value;

                    if (Player.Distance(target) > Q.Range && menu.Item("Q_Gap_Close").GetValue<bool>())
                    {{
                        var allMinionQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                            MinionTeam.NotAlly);

                        Obj_AI_Base bestMinion = allMinionQ[0];

                        foreach (var minion in allMinionQ)
                        {{
                            if (target.Distance(minion) < Q.Range && Player.Distance(minion) < Q.Range &&
                                target.Distance(minion) < target.Distance(Player))
                                if (target.Distance(minion) < target.Distance(bestMinion))
                                    bestMinion = minion;
                        }
                    }

                    if (Player.Distance(target) > minDistance &&
                        Player.Distance(target) < Q.Range + target.BoundingRadius)
                    {{
                        Q.CastOnUnit(target, packets());
                    }
                }
            }
            else if (mode == 1)//Ham mode
            {{
                if (target == null)
                    return;

                if (Q.IsReady() && Environment.TickCount - Q.LastCastAttemptT > 4000 && Player.Distance(target) < Q.Range && Player.Distance(target) > Player.AttackRange)
                    Q.CastOnUnit(target, packets());
            }
        }

        private void Cast_Q_Last_Hit()
        {{
            var allMinionQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Player.BoundingRadius, MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionQ.Count > 0 && Q.IsReady())
            {{

                foreach (var minion in allMinionQ)
                {{
                    double dmg = Player.GetSpellDamage(minion, SpellSlot.Q);

                    if (dmg > minion.Health + 35)
                    {{
                        if (menu.Item("UseQFarm_Tower").GetValue<bool>())
                        {{
                            if (!Utility.UnderTurret(minion, true))
                            {{
                                Q.Cast(minion, packets());
                                return;
                            }
                        }
                        else
                            Q.Cast(minion, packets());
                    }
                }
            }
        }

        private void Cast_R()
        {{
            var target = SimpleTS.GetTarget(R.Range, SimpleTS.DamageType.Physical);

            var range = R.Range;
            if (GetTargetFocus(range) != null)
                target = GetTargetFocus(range);

            if (target != null && R.IsReady())
            {{
                if (Player.GetSpellDamage(target, SpellSlot.R)/
                    countEnemiesNearPosition(target.ServerPosition, R.Range) >
                    target.Health - Player.GetAutoAttackDamage(target)*2)
                    R.CastOnUnit(target, packets());

                var rHpValue = menu.Item("R_If_HP").GetValue<Slider>().Value;
                if (GetHealthPercent() <= rHpValue)
                    R.CastOnUnit(target, packets());
            }
            
        }

        private int _lasttick;
        private void ModeSwitch()
        {{
            int mode = menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
            int lasttime = Environment.TickCount - _lasttick;

            if (menu.Item("Combo_Switch").GetValue<KeyBind>().Active && lasttime > Game.Ping)
            {{
                if (mode == 0)
                {{
                    menu.Item("Combo_mode").SetValue(new StringList(new[] {{ Normal", Q-AA-Q-AA-Ult" }, 1));
                    _lasttick = Environment.TickCount + 300;
                }
                else if (mode == 1)
                {{
                    menu.Item("Combo_mode").SetValue(new StringList(new[] {{ Normal", Q-AA-Q-AA-Ult" }));
                    _lasttick = Environment.TickCount + 300;
                }
            }
        }

        private Obj_AI_Base dodgeHero;

        public override void Game_OnGameUpdate(EventArgs args)
        {{
            //check if player is dead
            if (Player.IsDead) return;

            SmartKs();

            ModeSwitch();

            if(Environment.TickCount - R.LastCastAttemptT < 750)
                R.Cast(dodgeHero, packets());

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {{
                Combo();
            }
            else
            {{
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("LastHitKey").GetValue<KeyBind>().Active)
                    Lasthit();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();
            }
        }

        public override void AfterAttack(AttackableUnit unit, AttackableUnit mytarget)
        {{
            var target = (Obj_AI_Base) mytarget;
            if (unit.IsMe)
            {{
                if ((menu.Item("ComboActive").GetValue<KeyBind>().Active || menu.Item("HarassActive").GetValue<KeyBind>().Active )
                    && (target is Obj_AI_Hero))
                {{
                    if (menu.Item("E_Reset").GetValue<bool>() && E.IsReady())
                        E.Cast();

                    if(Items.CanUseItem(3077))
                        Items.UseItem(3077);
                    if (Items.CanUseItem(3074))
                        Items.UseItem(3074);

                    int mode = menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
                    if (mode == 1)
                    {{
                        Q.CastOnUnit(target, packets());

                        if(qSpell.State == SpellState.Cooldown && R.IsReady())
                            R.CastOnUnit(target, packets());
                    }
                }
            }
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {{
            SpellSlot castedSlot = Player.GetSpellSlot(args.SData.Name, false);

            if (castedSlot == SpellSlot.Q)
            {{
                Q.LastCastAttemptT = Environment.TickCount;
            }

            if (unit.IsMe)
                return;

            if (xSLxOrbwalker.IsAutoAttack(args.SData.Name) && args.Target.IsMe && Player.Distance(args.End) < 450)
            {{
                if (menu.Item("W_Incoming").GetValue<bool>() ||
                    (menu.Item("ComboActive").GetValue<KeyBind>().Active && E.IsReady() && menu.Item("UseWCombo").GetValue<bool>()) ||
                    (menu.Item("HarassActive").GetValue<KeyBind>().Active && menu.Item("UseWHarass").GetValue<bool>()))
                {{
                    if (!menu.Item("W_minion").GetValue<bool>() && !(unit is Obj_AI_Hero))
                        return;

                        W.Cast(packets());
                }
            }

            if (unit.IsEnemy && (unit is Obj_AI_Hero))
            {{
                if (Player.Distance(args.End) > R.Range || !R.IsReady())
                    return;

                if (menu.Item(args.SData.Name + R_Dodge").GetValue<bool>() && args.SData.Name == SyndraR")
                {{
                    Utility.DelayAction.Add(150, () => R.CastOnUnit(unit, packets()));
                    return;
                }

                if (menu.Item(args.SData.Name + R_Dodge").GetValue<bool>())
                {{
                    //Game.PrintChat("RAWR");
                    R.Cast(unit, packets());
                    dodgeHero = unit;
                    R.LastCastAttemptT = Environment.TickCount;
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {{
            if (menu.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (menu.Item("Draw_R_Killable").GetValue<bool>() && R.IsReady())
            {{
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(5000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {{
                    Vector2 wts = Drawing.WorldToScreen(target.Position);
                    if (Player.GetSpellDamage(target, SpellSlot.R) / countEnemiesNearPosition(target.ServerPosition, R.Range) > target.Health)
                    {{
                        Drawing.DrawText(wts[0] - 20, wts[1], Color.White, KILL!!!");

                    }
                }
            }

            Vector2 wts2 = Drawing.WorldToScreen(Player.Position);
            int mode = menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
            if (mode == 0)
                Drawing.DrawText(wts2[0] - 20, wts2[1], Color.White, Normal");
            else if (mode == 1)
                Drawing.DrawText(wts2[0] - 20, wts2[1], Color.White, Q-AA-Q-AA-Ult");
        }
    }
}
