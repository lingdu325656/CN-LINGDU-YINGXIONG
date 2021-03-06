﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace BlackWarwick
{{
    class Program
    {{
        // Generic
        public static readonly string champName = Warwick";
        private static readonly Obj_AI_Hero player = ObjectManager.Player;

        // Spells
        private static readonly List<Spell> spellList = new List<Spell>();
        private static Spell Q, W, E, R;
        private static SpellSlot IgniteSlot;

        // Menu
        public static Menu menu;

        private static Orbwalking.Orbwalker OW;

        public static void Main(string[] args)
        {{
            // Register events
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {{
            //Champ validation
            if (player.ChampionName != champName) return;

            //Define spells
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1500);
            R = new Spell(SpellSlot.R, 700);
            spellList.AddRange(new[] {{ Q, W, E, R });

            IgniteSlot = player.GetSpellSlot("SummonerDot");

            // Finetune spells
            R.SetTargetted(0.5f, float.MaxValue);

            // Create menu
            createMenu();

            // Register events
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            // Print
            Game.PrintChat(String.Format("<font color='#08F5F8'>blacky -</font> <font color='#FFFFFF'>{{0} Loaded!</font>", champName));
        }

        private static void Drawing_OnDraw(EventArgs args)
        {{
            // Spell ranges
            foreach (var spell in spellList)
            {{
                // Regular spell ranges
                var circleEntry = menu.Item("drawRange" + spell.Slot).GetValue<Circle>();
                if (circleEntry.Active)
                    Utility.DrawCircle(player.Position, spell.Range, circleEntry.Color);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {{
            Obj_AI_Hero target = SimpleTS.GetTarget(R.Range, SimpleTS.DamageType.Magical);

            // Combo
            if (menu.SubMenu("combo").Item("comboActive").GetValue<KeyBind>().Active)
                OnCombo(target);

            // Harass
            if (menu.SubMenu("harass").Item("harassActive").GetValue<KeyBind>().Active &&
               (player.Mana / player.MaxMana * 100) >
                menu.Item("harassMana").GetValue<Slider>().Value)
                OnHarass(target);

            // WaveClear
            if (menu.SubMenu("waveclear").Item("wcActive").GetValue<KeyBind>().Active &&
               (player.Mana / player.MaxMana * 100) >
                menu.Item("wcMana").GetValue<Slider>().Value)
                waveclear();

            // Killsteal
            Killsteal(target);

        }

        private static void OnCombo(Obj_AI_Hero target)
        {{
            Menu comboMenu = menu.SubMenu("combo");
            bool useQ = comboMenu.Item("comboUseQ").GetValue<bool>() && Q.IsReady();
            bool useW = comboMenu.Item("comboUseW").GetValue<bool>() && W.IsReady();
            bool useR = comboMenu.Item("comboUseR").GetValue<bool>() && R.IsReady();

            if (target.HasBuffOfType(BuffType.Invulnerability)) return;

            if (useW && player.Distance(target) < R.Range)
            {{
                W.Cast(player, packets());
            }

            if (useR && player.Distance(target) < R.Range)
            {{
                if (target != null && menu.Item("DontUlt" + target.BaseSkinName) != null && menu.Item("DontUlt" + target.BaseSkinName).GetValue<bool>() == false)
                    R.Cast(target, packets());
            }

            if (useQ && player.Distance(target) < Q.Range)
            {{
                if (target != null)
                    Q.Cast(target, packets());
            }

            if (target != null && menu.Item("miscIgnite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
            player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {{
                if (GetComboDamage(target) > target.Health)
                {{
                    player.Spellbook.CastSpell(IgniteSlot, target);
                }
            }
        }

        private static void OnHarass(Obj_AI_Hero target)
        {{
            Menu harassMenu = menu.SubMenu("harass");
            bool useQ = harassMenu.Item("harassUseQ").GetValue<bool>() && Q.IsReady();

            if (target.HasBuffOfType(BuffType.Invulnerability)) return;

            if (useQ && player.Distance(target) < Q.Range)
            {{
                if (target != null)
                    Q.Cast(target, packets());
            }
        }

        private static void Killsteal(Obj_AI_Hero target)
        {{
            Menu killstealMenu = menu.SubMenu("killsteal");
            bool useQ = killstealMenu.Item("killstealUseQ").GetValue<bool>() && Q.IsReady();
            bool useR = killstealMenu.Item("killstealUseR").GetValue<bool>() && R.IsReady();

            if (target.HasBuffOfType(BuffType.Invulnerability)) return;

            if (useQ && target.Distance(player) < Q.Range)
            {{
                if (Q.IsKillable(target))
                {{
                    Q.Cast(target, packets());
                }
            }

            if (useR && target.Distance(player) < R.Range)
            {{
                if (R.IsKillable(target))
                {{
                    R.Cast(target, packets());
                }
            }
        }

        private static void waveclear()
        {{
            Menu waveclearMenu = menu.SubMenu("waveclear");
            bool useQ = waveclearMenu.Item("wcUseQ").GetValue<bool>() && Q.IsReady();
            bool useW = waveclearMenu.Item("wcUseW").GetValue<bool>() && W.IsReady();

            var allMinionsQ = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);

            if (useQ)
            {{
                foreach (var minion in allMinionsQ)
                {{
                    if (minion.IsValidTarget() &&
                    Q.IsKillable(minion))
                    {{
                        Q.CastOnUnit(minion, packets());
                        return;
                    }
                }
            }

            if (useW && allMinionsQ.Count > 1)
            {{
                W.Cast(player, packets());
            }

            var jcreeps = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All,
            MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (jcreeps.Count > 0)
            {{
                var jcreep = jcreeps[0];

                if (useQ)
                {{
                    Q.Cast(jcreep, packets());
                }

                if (useW)
                {{
                    W.Cast(player, packets());
                }
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {{
            var damage = 0d;
            if (R.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.R);

            if (Q.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);

            if (IgniteSlot != SpellSlot.Unknown && player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                damage += player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            return (float)damage;
        }

        private static bool packets()
        {{
            return menu.Item("miscPacket").GetValue<bool>();
        }

        private static void createMenu()
        {{
            menu = new Menu("【超神汉化】" + champName, black" + champName, true);

            // Target selector
            Menu ts = new Menu("目标选择", ts");
            menu.AddSubMenu(ts);
            SimpleTS.AddToMenu(ts);

            // Orbwalker
            Menu orbwalk = new Menu("走砍", orbwalk");
            menu.AddSubMenu(orbwalk);
            OW = new Orbwalking.Orbwalker(orbwalk);

            // Combo
            Menu combo = new Menu("连招", combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("comboUseQ", 使用Q").SetValue(true));
            combo.AddItem(new MenuItem("comboUseW", 使用W").SetValue(true));
            combo.AddItem(new MenuItem("comboUseR", 使用R").SetValue(true));
            combo.AddItem(new MenuItem("comboActive", 热键").SetValue(new KeyBind(32, KeyBindType.Press)));

            // Harass
            Menu harass = new Menu("骚扰", harass");
            menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("harassUseQ", 使用Q").SetValue(true));
            harass.AddItem(new MenuItem("harassMana", 蓝量控制").SetValue(new Slider(40, 100, 0)));
            harass.AddItem(new MenuItem("harassActive", 热键").SetValue(new KeyBind('C', KeyBindType.Press)));

            // WaveClear
            Menu waveclear = new Menu("清线", waveclear");
            menu.AddSubMenu(waveclear);
            waveclear.AddItem(new MenuItem("wcUseQ", 使用Q").SetValue(true));
            waveclear.AddItem(new MenuItem("wcUseW", 使用W").SetValue(true));
            waveclear.AddItem(new MenuItem("wcMana", 蓝量控制").SetValue(new Slider(40, 100, 0)));
            waveclear.AddItem(new MenuItem("wcActive", 热键").SetValue(new KeyBind('V', KeyBindType.Press)));

            // Killsteal
            Menu killsteal = new Menu("抢头", killsteal");
            menu.AddSubMenu(killsteal);
            killsteal.AddItem(new MenuItem("killstealUseQ", 使用Q").SetValue(true));
            killsteal.AddItem(new MenuItem("killstealUseR", 使用R").SetValue(false));

            // Misc
            Menu misc = new Menu("杂项", misc");
            menu.AddSubMenu(misc);
            misc.AddItem(new MenuItem("miscPacket", 封包").SetValue(true));
            misc.AddItem(new MenuItem("miscIgnite", 使用点燃").SetValue(true));
            misc.AddItem(new MenuItem("DontUlt", 不R"));
            misc.AddItem(new MenuItem("sep0", ========="));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != player.Team))
                misc.AddItem(new MenuItem("DontUlt" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
            misc.AddItem(new MenuItem("sep1", ========="));


            //Damage after combo:
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", 显示伤害").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {{
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            // Drawings
            Menu drawings = new Menu("显示", drawings");
            menu.AddSubMenu(drawings);
            drawings.AddItem(new MenuItem("drawRangeQ", Q范围").SetValue(new Circle(true, Color.Aquamarine)));
            drawings.AddItem(new MenuItem("drawRangeW", W范围").SetValue(new Circle(false, Color.Aquamarine)));
            drawings.AddItem(new MenuItem("drawRangeE", E范围").SetValue(new Circle(false, Color.Aquamarine)));
            drawings.AddItem(new MenuItem("drawRangeR", R范围").SetValue(new Circle(false, Color.Aquamarine)));
            drawings.AddItem(dmgAfterComboItem);

			
            Menu System = new Menu("超神汉化", System");
            menu.AddSubMenu(System);
            System.AddItem(new MenuItem("SystemQ", L#汉化群：386289593"));
            // Finalizing
            menu.AddToMainMenu();
        }
    }
}

