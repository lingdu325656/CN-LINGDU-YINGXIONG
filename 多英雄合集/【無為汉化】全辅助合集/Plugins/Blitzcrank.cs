﻿#region LICENSE

// Copyright 2014 Support
// Blitzcrank.cs is part of Support.
// 
// Support is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Support is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Support. If not, see <http://www.gnu.org/licenses/>.
// 
// Filename: Support/Support/Blitzcrank.cs
// Created:  05/10/2014
// Date:     26/12/2014/16:23
// Author:   h3h3

#endregion

namespace Support.Plugins
{{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Support.Util;
    using ActiveGapcloser = Support.Util.ActiveGapcloser;
    using Collision = LeagueSharp.Common.Collision;

    #endregion

    public class Blitzcrank : PluginBase
    {{
        public Blitzcrank()
        {{
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, AttackRange);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
        }

        private bool BlockQ
        {{
            get
            {{
                if (!Q.IsReady())
                {{
                    return true;
                }

                if (!ConfigValue<bool>("Misc.Q.Block"))
                {{
                    return false;
                }

                if (!Target.IsValidTarget())
                {{
                    return true;
                }

                if (Target.HasBuff("BlackShield"))
                {{
                    return true;
                }

                if (Helpers.AllyInRange(1200)
                    .Any(ally => ally.Distance(Target) < ally.AttackRange + ally.BoundingRadius))
                {{
                    return true;
                }

                return Player.Distance(Target) < ConfigValue<Slider>("Misc.Q.Block.Distance").Value;
            }
        }

        public override void OnUpdate(EventArgs args)
        {{
            try
            {{
                if (ComboMode)
                {{
                    if (Q.CastCheck(Target, ComboQ") && !BlockQ)
                    {{
                        Q.Cast(Target, UsePackets);
                    }

                    if (E.CastCheck(Target))
                    {{
                        if (E.Cast())
                        {{
                            Orbwalking.ResetAutoAttackTimer();
                            Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                        }
                    }

                    if (E.IsReady() && Target.IsValidTarget() && Target.HasBuff("RocketGrab"))
                    {{
                        if (E.Cast())
                        {{
                            Orbwalking.ResetAutoAttackTimer();
                            Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                        }
                    }

                    if (W.IsReady() && ConfigValue<bool>("ComboW") && Player.CountEnemysInRange(1500) > 0)
                    {{
                        W.Cast();
                    }

                    if (R.CastCheck(Target, ComboR"))
                    {{
                        if (Helpers.EnemyInRange(ConfigValue<Slider>("ComboCountR").Value, R.Range))
                        {{
                            R.Cast();
                        }
                    }
                }

                if (HarassMode)
                {{
                    if (Q.CastCheck(Target, HarassQ") && !BlockQ)
                    {{
                        Q.Cast(Target, UsePackets);
                    }

                    if (E.CastCheck(Target))
                    {{
                        if (E.Cast())
                        {{
                            Orbwalking.ResetAutoAttackTimer();
                            Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                        }
                    }

                    if (E.IsReady() && Target.IsValidTarget() && Target.HasBuff("RocketGrab"))
                    {{
                        if (E.Cast())
                        {{
                            Orbwalking.ResetAutoAttackTimer();
                            Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                        }
                    }
                }
            }
            catch (Exception e)
            {{
                Console.WriteLine(e);
            }
        }

        public override void OnBeforeEnemyAttack(BeforeEnemyAttackEventArgs args)
        {{
            if (Q.CastCheck(args.Caster, Misc.Q.OnAttack") && (ComboMode || HarassMode) && !BlockQ &&
                args.Caster == Target && args.Type == Packet.AttackTypePacket.TargetedAA)
            {{
                var collision = Collision.GetCollision(
                    new List<Vector3> {{ args.Caster.Position },
                    new PredictionInput {{ Delay = 0.25f, Radius = 70, Speed = 1800 });

                if (collision.Count == 0)
                {{
                    Q.Cast(args.Caster.Position, UsePackets);
                }
            }
        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {{
            if (!unit.IsMe)
            {{
                return;
            }

            if (!target.IsValid<Obj_AI_Hero>() && !target.Name.ToLower().Contains("ward"))
            {{
                return;
            }

            if (!E.IsReady())
            {{
                return;
            }

            if (E.Cast())
            {{
                Orbwalking.ResetAutoAttackTimer();
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {{
            if (gapcloser.Sender.IsAlly)
            {{
                return;
            }

            if (E.CastCheck(gapcloser.Sender, GapcloserE"))
            {{
                if (E.Cast())
                {{
                    Orbwalking.ResetAutoAttackTimer();
                    Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                }
            }

            if (R.CastCheck(gapcloser.Sender, GapcloserR"))
            {{
                R.Cast();
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {{
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {{
                return;
            }

            if (E.CastCheck(unit, InterruptE"))
            {{
                if (E.Cast())
                {{
                    Orbwalking.ResetAutoAttackTimer();
                    Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                }
            }

            if (Q.CastCheck(Target, InterruptQ"))
            {{
                Q.Cast(unit, UsePackets);
            }

            if (R.CastCheck(unit, InterruptR"))
            {{
                R.Cast();
            }
        }

        public override void ComboMenu(Menu config)
        {{
            config.AddBool("ComboQ", 使用 Q", true);
            config.AddBool("ComboW", 使用 W", true);
            config.AddBool("ComboR", 使用 R", true);
            config.AddSlider("ComboCountR", 几个敌人使用大招", 2, 1, 5);
        }

        public override void MiscMenu(Menu config)
        {{
            config.AddBool("Misc.Q.OnAttack", 对目标AQ", true);
            config.AddBool("Misc.Q.Block", 使用Q限制接近目标", true);
            config.AddSlider("Misc.Q.Block.Distance", Q限制的距离ㄧ", 400, 0, 800);
        }

        public override void HarassMenu(Menu config)
        {{
            config.AddBool("HarassQ", 使用 Q", true);
        }

        public override void InterruptMenu(Menu config)
        {{
            config.AddBool("GapcloserE", 使用 E 中断突进", true);
            config.AddBool("GapcloserR", 使用 R 中断突进", true);
            config.AddBool("InterruptQ", 使用 Q 打断法术", true);
            config.AddBool("InterruptE", 使用 E 打断法术", true);
            config.AddBool("InterruptR", 使用 R 打断法术", true);
        }
    }
}