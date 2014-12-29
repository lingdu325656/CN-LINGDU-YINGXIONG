﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace xSaliceReligionAIO.Champions
{{
    class Teemo : Champion
    {{
        public Teemo()
        {{
            SetUpSpells();
            LoadMenu();
        }

        public void SetUpSpells()
        {{
            Q = new Spell(SpellSlot.Q, 580);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E);

            R = new Spell(SpellSlot.R);
        }

        public void LoadMenu()
        {{
            var key = new Menu("热键", Key");
            {{
                key.AddItem(new MenuItem("ComboActive", 连招").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", 骚扰").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", 骚扰 (锁定)").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                menu.AddSubMenu(key);
            }

            var combo = new Menu("连招", Combo");
            {{
                combo.AddItem(new MenuItem("UseQCombo", 使用Q").SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", 使用W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", 使用E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", 使用R").SetValue(true));
                combo.AddItem(new MenuItem("Ignite", 使用点燃").SetValue(true));
                menu.AddSubMenu(combo);
            }

            var harass = new Menu("骚扰", Harass");
            {{
                harass.AddItem(new MenuItem("UseQHarass", 使用Q").SetValue(true));
                AddManaManagertoMenu(harass, Harass", 30);
                //add to menu
                menu.AddSubMenu(harass);
            }

            var miscMenu = new Menu("杂项", Misc");
            {{
                miscMenu.AddItem(new MenuItem("Get_Cord", 获取坐标").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("显示", Drawing");
            {{
                drawMenu.AddItem(new MenuItem("Draw_Disabled", 禁用").SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", Q范围").SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", W范围").SetValue(true));
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

                menu.AddSubMenu(drawMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {{
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E) * 2;

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                comboDamage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 2);
        }

        private void Combo()
        {{
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                false, menu.Item("UseRCombo").GetValue<bool>(), Combo");
        }

        private void Harass()
        {{
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), false,
                false, false, Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {{
            var target = SimpleTS.GetTarget(Q.Range, SimpleTS.DamageType.Magical);

            if (target != null)
            {{
                if(useQ && Q.IsReady())
                    Q.CastOnUnit(target, packets());

                if(useW && W.IsReady())
                    W.Cast(packets());
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {{
            if (menu.Item("Get_Cord").GetValue<KeyBind>().Active)
            {{
                Game.PrintChat("X:  + Player.ServerPosition.X +  Y:  + Player.ServerPosition.Y +  Z:  + Player.ServerPosition.Z);
            }
            

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {{
                Combo();
            }
            else
            {{
                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();
            }
        }
    }
}
