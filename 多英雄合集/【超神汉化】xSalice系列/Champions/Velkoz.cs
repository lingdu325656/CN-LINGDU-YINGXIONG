﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace xSaliceReligionAIO.Champions
{{
    class Velkoz : Champion
    {{
        private Spell _qSplit;
        private Spell _qDummy;
        private Obj_SpellMissile _qMissle;

        public Velkoz()
        {{
            qMana = new[] {{40, 40, 45, 50, 55, 60};
            wMana = new[] {{50, 50, 55, 60, 65, 70};
            eMana = new[] {{50, 50, 55, 60, 65, 70};
            rMana = new[] {{100, 100, 100, 100};

            LoadSpell();
            LoadMenu();
        }

        private void LoadSpell()
        {{
            //intalize spell
            Q = new Spell(SpellSlot.Q, 1000);
            _qSplit = new Spell(SpellSlot.Q, 800);
            _qDummy = new Spell(SpellSlot.Q, (float)Math.Sqrt(Math.Pow(Q.Range, 2) + Math.Pow(_qSplit.Range, 2)));
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 850);
            R = new Spell(SpellSlot.R, 1500);

            Q.SetSkillshot(0.25f, 60f, 1300f, true, SkillshotType.SkillshotLine);
            _qDummy.SetSkillshot(0.25f, 65f, float.MaxValue, false, SkillshotType.SkillshotLine);
            _qSplit.SetSkillshot(0.25f, 55f, 2100, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 10f, 1700f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 80f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.3f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }

        private void LoadMenu()
        {{
            //key
            var key = new Menu("热键", Key");
            {{
                key.AddItem(new MenuItem("ComboActive", 连招").SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", 骚扰").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", 骚扰 (锁定)").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", 清线").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("R_Mouse", R鼠标位置").SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }

            //Spell Menu
            var spell = new Menu("技能", Spell");
            {{
                //Q Menu
                var qMenu = new Menu("Q", QSpell"); {{
                    qMenu.AddItem(new MenuItem("qSplit", 自动分叉Q").SetValue(true));
                    qMenu.AddItem(new MenuItem("qAngle", 在角落Q").SetValue(true));
                    spell.AddSubMenu(qMenu);
                }   
                //R
                var rMenu = new Menu("R", RSpell");
                {{
                    rMenu.AddItem(new MenuItem("rAimer", R瞄准").SetValue(new StringList(new[] {{ 自动", 鼠标位置" })));
                    rMenu.AddSubMenu(new Menu("不R", DontUlt"));
                    foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                        rMenu.SubMenu("DontUlt").AddItem(new MenuItem("DontUlt" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
                    spell.AddSubMenu(rMenu);
                }
                menu.AddSubMenu(spell);
            }

            //Combo menu:
            var combo = new Menu("连招", Combo");
            {{
                combo.AddItem(new MenuItem("selected", 锁定目标").SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", 使用Q").SetValue(true));
                combo.AddItem(new MenuItem("qHit", Q击中").SetValue(new Slider(3, 1, 4)));
                combo.AddItem(new MenuItem("UseWCombo", 使用W").SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", 使用E").SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", 使用R").SetValue(true));
                combo.AddItem(new MenuItem("ignite", 使用点燃").SetValue(true));
                combo.AddItem(new MenuItem("igniteMode", 点燃模式").SetValue(new StringList(new[] {{"连招", 抢人头"})));
                menu.AddSubMenu(combo);
            }

            //Harass menu:
            var harass = new Menu("骚扰", Harass");
            {{
                harass.AddItem(new MenuItem("UseQHarass", 使用Q").SetValue(true));
                harass.AddItem(new MenuItem("qHit2", Q击中").SetValue(new Slider(3, 1, 4)));
                harass.AddItem(new MenuItem("UseWHarass", 使用W").SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", 使用E").SetValue(true));
                menu.AddSubMenu(harass);
            }

            //Farming menu:
            var farm = new Menu("清线", Farm");
            {{
                farm.AddItem(new MenuItem("UseQFarm", 使用Q").SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", 使用W").SetValue(false));
                farm.AddItem(new MenuItem("UseEFarm", 使用E").SetValue(false));
                menu.AddSubMenu(farm);
            }
            //Misc Menu:
            var misc = new Menu("杂项", Misc");
            {{
                misc.AddItem(new MenuItem("UseInt", 使用E打断").SetValue(true));
                misc.AddItem(new MenuItem("UseGap", 使用E防突").SetValue(true));
                misc.AddItem(new MenuItem("smartKS", 智能抢人头").SetValue(true));
                misc.AddItem(new MenuItem("mana", 点燃前检查蓝量").SetValue(true));
                menu.AddSubMenu(misc);
            }

            //Drawings menu:
            var draw = new Menu("显示", Drawings"); {{ 
                draw.AddItem(new MenuItem("QRange", Q范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("WRange", W范围").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("ERange", E范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("RRange", R范围").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("drawUlt", R可杀").SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", 显示伤害").SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", 显示连招伤害").SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                draw.AddItem(drawComboDamageMenu);
                draw.AddItem(drawFill);
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

                menu.AddSubMenu(draw);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {{
            double damage = 0d;

            int collisionCount = Q.GetPrediction(enemy).CollisionObjects.Count;
            if (Q.IsReady() && collisionCount < 1)
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                damage += W.Instance.Ammo * Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            if (R.IsReady())
                damage += GetUltDmg((Obj_AI_Hero)enemy);

            damage += GetPassiveDmg();

            return (float)damage;
        }

        private float GetPassiveDmg()
        {{
            double stack = 0;
            double dmg = 25 + (10 * Player.Level);

            if (Q.IsReady())
                stack++;

            if (W.IsReady())
                stack += 2;

            if (E.IsReady())
                stack++;

            stack = stack / 3;

            stack = Math.Floor(stack);

            dmg = dmg * stack;

            //Game.PrintChat("Stacks:  + stack);

            return (float)dmg;
        }
        private float GetUltDmg(Obj_AI_Hero target)
        {{
            double dmg = 0;

            float dist = (Player.ServerPosition.To2D().Distance(target.ServerPosition.To2D()) - 600) / 100;
            double div = Math.Ceiling(10 - dist);

            //Game.PrintChat("ult dmg" + target.BaseSkinName +   + div);

            if (Player.Distance(target) < 600)
                div = 10;

            if (Player.Distance(target) < 1550)
                if (R.IsReady())
                {{
                    double ultDmg = Player.GetSpellDamage(target, SpellSlot.R) / 10;

                    dmg += ultDmg * div;
                }

            if (div >= 3)
                dmg += 25 + (10 * Player.Level);

            if (menu.Item("drawUlt").GetValue<bool>())
            {{
                if (R.IsReady() && dmg > target.Health + 20)
                {{
                    Vector2 wts = Drawing.WorldToScreen(target.Position);
                    Drawing.DrawText(wts[0], wts[1], Color.White, Killable with Ult");
                }
                else
                {{
                    Vector2 wts = Drawing.WorldToScreen(target.Position);
                    Drawing.DrawText(wts[0], wts[1], Color.Red, No Ult Kill");
                }
            }

            return (float)dmg;
        }


        private void Combo()
        {{
            UseSpells(menu.Item("UseQCombo").GetValue<bool>(), menu.Item("UseWCombo").GetValue<bool>(),
                menu.Item("UseECombo").GetValue<bool>(), menu.Item("UseRCombo").GetValue<bool>(), Combo");
        }

        private void Harass()
        {{
            UseSpells(menu.Item("UseQHarass").GetValue<bool>(), menu.Item("UseWHarass").GetValue<bool>(),
                menu.Item("UseEHarass").GetValue<bool>(), false, Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {{
            var range = R.IsReady() ? R.Range : Q.Range;
            var focusSelected = menu.Item("selected").GetValue<bool>();

            Obj_AI_Hero target = SimpleTS.GetTarget(range, SimpleTS.DamageType.Magical);
            if (SimpleTS.GetSelectedTarget() != null)
                if (focusSelected && SimpleTS.GetSelectedTarget().Distance(Player.ServerPosition) < range)
                    target = SimpleTS.GetSelectedTarget();

            Obj_AI_Hero qDummyTarget = SimpleTS.GetTarget(_qDummy.Range, SimpleTS.DamageType.Magical);

            if (target == null)
                return;

            bool hasmana = manaCheck();
            float dmg = GetComboDamage(target);

            int igniteMode = menu.Item("igniteMode").GetValue<StringList>().SelectedIndex;

            useR = (menu.Item("DontUlt" + target.BaseSkinName) != null && menu.Item("DontUlt" + target.BaseSkinName).GetValue<bool>() == false) && useR;

            if (useW && W.IsReady() && Player.Distance(target) <= W.Range &&
                W.GetPrediction(target).Hitchance >= HitChance.High)
            {{
                W.Cast(target);
                return;
            }

            if (useE && E.IsReady() && Player.Distance(target) < E.Range &&
                E.GetPrediction(target).Hitchance >= HitChance.High)
            {{
                E.Cast(target, packets());
            }

            //Ignite
            if (menu.Item("ignite").GetValue<bool>() && Ignite_Ready() && hasmana)
            {{
                if (igniteMode == 0 && dmg > target.Health)
                {{
                    Player.Spellbook.CastSpell(IgniteSlot, target);
                }
            }

            if (useR && R.IsReady() && Player.Distance(target) < R.Range)
            {{
                if (GetUltDmg(target) >= target.Health)
                {{
                    R.Cast(target.ServerPosition);
                    return;
                }

            }

            if (useQ && Q.IsReady())
            {{
                CastQ(target, qDummyTarget, source);
            }
        }

        private void Farm()
        {{
            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width, MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                MinionTypes.Ranged, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = menu.Item("UseQFarm").GetValue<bool>();
            var useW = menu.Item("UseWFarm").GetValue<bool>();
            var useE = menu.Item("UseEFarm").GetValue<bool>();

            if (useW && W.IsReady() && allMinionsW.Count > 0)
            {{
                MinionManager.FarmLocation wPos = W.GetLineFarmLocation(allMinionsW);

                if (wPos.MinionsHit > 2)
                    W.Cast(wPos.Position, packets());
            }

            if (useE && allMinionsE.Count > 0 && E.IsReady())
            {{
                MinionManager.FarmLocation ePos = E.GetCircularFarmLocation(allMinionsE);

                if (ePos.MinionsHit > 2)
                    E.Cast(ePos.Position, packets());
            }

            if (useQ && Q.IsReady() && allMinionsQ.Count > 0)
            {{
                MinionManager.FarmLocation qPos = Q.GetLineFarmLocation(allMinionsQ);

                Q.Cast(qPos.Position, packets());
            }
        }

        private void CastQ(Obj_AI_Hero target, Obj_AI_Hero targetExtend, string source)
        {{
            PredictionOutput pred = Q.GetPrediction(target);
            int collision = pred.CollisionObjects.Count;

            //cast Q with no collision
            if (Player.Distance(target) < 1050 && Q.Instance.Name == VelkozQ")
            {{
                if (collision == 0)
                {{
                    if (pred.Hitchance >= GetHitchance(source))
                    {{
                        Q.Cast(pred.CastPosition, packets());
                    }

                    return;
                }
            }

            if (!menu.Item("qAngle").GetValue<bool>())
                return;

            if (Q.Instance.Name == VelkozQ" && targetExtend != null)
            {{
                _qDummy.Delay = Q.Delay + Q.Range / Q.Speed * 1000 + _qSplit.Range / _qSplit.Speed * 1000;
                pred = _qDummy.GetPrediction(targetExtend);

                if (pred.Hitchance >= GetHitchance(source))
                {{
                    //math by esk0r <3
                    for (int i = -1; i < 1; i = i + 2)
                    {{
                        const float alpha = 28 * (float)Math.PI / 180;
                        Vector2 cp = Player.ServerPosition.To2D() +
                                     (pred.CastPosition.To2D() - Player.ServerPosition.To2D()).Rotated(i * alpha);

                        //Utility.DrawCircle(cp.To3D(), 100, Color.Blue, 1, 1);

                        if (Q.GetCollision(Player.ServerPosition.To2D(), new List<Vector2> {{ cp }).Count == 0 &&
                            _qSplit.GetCollision(cp, new List<Vector2> {{ pred.CastPosition.To2D() }).Count == 0)
                        {{
                            if (Player.Distance(cp) <= R.Range)
                            {{
                                Q.Cast(cp, packets());
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void SplitMissle()
        {{
            //Game.PrintChat("bleh");

            var range = R.IsReady() ? R.Range : Q.Range;
            var focusSelected = menu.Item("selected").GetValue<bool>();

            Obj_AI_Hero target = SimpleTS.GetTarget(range, SimpleTS.DamageType.Magical);
            if (SimpleTS.GetSelectedTarget() != null)
                if (focusSelected && SimpleTS.GetSelectedTarget().Distance(Player.ServerPosition) < range)
                    target = SimpleTS.GetSelectedTarget();

            _qSplit.From = _qMissle.Position;
            PredictionOutput pred = _qSplit.GetPrediction(target);

            Vector2 perpendicular = (_qMissle.EndPosition - _qMissle.StartPosition).To2D().Normalized().Perpendicular();

            Vector2 lineSegment1End = _qMissle.Position.To2D() + perpendicular * _qSplit.Range;
            Vector2 lineSegment2End = _qMissle.Position.To2D() - perpendicular * _qSplit.Range;

            float d1 = pred.UnitPosition.To2D().Distance(_qMissle.Position.To2D(), lineSegment1End, true);
            float d2 = pred.UnitPosition.To2D().Distance(_qMissle.Position.To2D(), lineSegment2End, true);

            //cast split
            if (pred.CollisionObjects.Count == 0 && (d1 < _qSplit.Width || d2 < _qSplit.Width))
            {{
                Q.Cast();
                _qMissle = null;
                Game.PrintChat("splitted");
            }
        }

        private void SmartKs()
        {{
            if (!menu.Item("smartKS").GetValue<bool>())
                return;

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.IsValidTarget(Q.Range) && x.IsEnemy && !x.IsDead).OrderByDescending(GetComboDamage))
            {{
                //Q
                if (Player.Distance(target.ServerPosition) <= Q.Range &&
                    (Player.GetSpellDamage(target, SpellSlot.Q)) > target.Health + 30)
                {{
                    if (Q.IsReady())
                    {{
                        CastQ(target, target, Combo");
                        return;
                    }
                }

                //EW
                if (Player.Distance(target.ServerPosition) <= E.Range && (Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.W)) > target.Health + 30)
                {{
                    if (W.IsReady() && E.IsReady())
                    {{
                        E.Cast(target);
                        W.Cast(target, packets());
                        return;
                    }
                }

                //E
                if (Player.Distance(target.ServerPosition) <= E.Range &&
                    (Player.GetSpellDamage(target, SpellSlot.E)) > target.Health + 30)
                {{
                    if (E.IsReady())
                    {{
                        E.CastOnUnit(target, packets());
                        return;
                    }
                }

                //W
                if (Player.Distance(target.ServerPosition) <= W.Range &&
                    (Player.GetSpellDamage(target, SpellSlot.W)) > target.Health + 50)
                {{
                    if (W.IsReady())
                    {{
                        W.Cast(target, packets());
                        return;
                    }
                }

                //ignite
                if (menu.Item("ignite").GetValue<bool>() && Ignite_Ready())
                {{
                    int igniteMode = menu.Item("igniteMode").GetValue<StringList>().SelectedIndex;
                    if (igniteMode == 1 && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health + 20)
                    {{
                        Player.Spellbook.CastSpell(IgniteSlot, target);
                    }
                }
            }
        }

        public override void Game_OnGameUpdate(EventArgs args)
        {{
            //check if player is dead
            if (Player.IsDead) return;

            if (Player.IsChannelingImportantSpell())
            {{
                var range = R.IsReady() ? R.Range : Q.Range;
                var focusSelected = menu.Item("selected").GetValue<bool>();

                Obj_AI_Hero target = SimpleTS.GetTarget(range, SimpleTS.DamageType.Magical);
                if (SimpleTS.GetSelectedTarget() != null)
                    if (focusSelected && SimpleTS.GetSelectedTarget().Distance(Player.ServerPosition) < range)
                        target = SimpleTS.GetSelectedTarget();

                int aimMode = menu.Item("rAimer").GetValue<StringList>().SelectedIndex;

                if (target != null && aimMode == 0)
                    Packet.C2S.ChargedCast.Encoded(new Packet.C2S.ChargedCast.Struct(SpellSlot.R,
                        target.ServerPosition.X, target.ServerPosition.Z, target.ServerPosition.Y)).Send();
                else
                    Packet.C2S.ChargedCast.Encoded(new Packet.C2S.ChargedCast.Struct(SpellSlot.R, Game.CursorPos.X,
                        Game.CursorPos.Z, Game.CursorPos.Y)).Send();

                return;
            }

            if (_qMissle != null && _qMissle.IsValid && menu.Item("qSplit").GetValue<bool>())
                SplitMissle();

            SmartKs();

            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {{
                Combo();
            }
            else
            {{
                if (menu.Item("LaneClearActive").GetValue<KeyBind>().Active)
                    Farm();

                if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Harass();
            }

            if (menu.Item("R_Mouse").GetValue<KeyBind>().Active)
                R.Cast(Game.CursorPos);
        }

        public override void Drawing_OnDraw(EventArgs args)
        {{
            foreach (Spell spell in SpellList)
            {{
                var menuItem = menu.Item(spell.Slot + Range").GetValue<Circle>();
                if (menuItem.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }
        }


        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {{
            if (!menu.Item("UseGap").GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender, packets());
        }

        public override void Interrupter_OnPosibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {{
            if (!menu.Item("UseInt").GetValue<bool>()) return;

            if (Player.Distance(unit) < E.Range && unit != null && E.IsReady())
            {{
                E.Cast(unit, packets());
            }
        }

        public override void Game_OnSendPacket(GamePacketEventArgs args)
        {{
            //Disable action on Ult
            if (args.PacketData[0] == Packet.C2S.ChargedCast.Header)
            {{
                var decodedPacket = Packet.C2S.ChargedCast.Decoded(args.PacketData);

                if (decodedPacket.SourceNetworkId == Player.NetworkId)
                {{
                    args.Process = !(menu.Item("ComboActive").GetValue<KeyBind>().Active && menu.Item("UseRCombo").GetValue<bool>() && menu.Item("smartKS").GetValue<bool>()
                        && menu.Item("HarassActive").GetValue<KeyBind>().Active && menu.Item("HarassActiveT").GetValue<KeyBind>().Active && menu.Item("R_Mouse").GetValue<KeyBind>().Active);
                }
            }
        }

        public override void GameObject_OnCreate(GameObject obj, EventArgs args)
        {{
            // return if its not a missle
            if (!(obj is Obj_SpellMissile))
                return;

            var spell = (Obj_SpellMissile)obj;

            if (Player.Distance(obj.Position) < 1500)
            {{
                //Q
                if (spell.IsValid && spell.SData.Name == VelkozQMissile")
                {{
                    //Game.PrintChat("Woot");
                    _qMissle = spell;
                }
            }
        }
    }
}
