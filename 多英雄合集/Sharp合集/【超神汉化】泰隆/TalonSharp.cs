﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
/*
 * ToDo:
 * 
 * */
using SharpDX;
using Color = System.Drawing.Color;


namespace TalonSharp
{{
    internal class TalonSharp
    {{

        public const string CharName = Talon";

        public static Menu Config;

        public static bool useQ;

        public static HpBarIndicator hpi = new HpBarIndicator();

        public TalonSharp()
        {{
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {{

            Game.PrintChat("Talon - Sharp by DeTuKs");

            try
            {{

                Config = new Menu("【超神汉化】泰隆", Talon", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("走砍", Orbwalker"));
                Talon.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                var SimpleTSMenu = new Menu("目标选择", Target Selector");
                SimpleTS.AddToMenu(SimpleTSMenu);
                Config.AddSubMenu(SimpleTSMenu);
                //Combo
                Config.AddSubMenu(new Menu("连招", combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", 使用物品")).SetValue(true);

                //LastHit
                Config.AddSubMenu(new Menu("补兵", lHit"));
               
                //LaneClear
                Config.AddSubMenu(new Menu("清线", lClear"));
               
                //Harass
                Config.AddSubMenu(new Menu("骚扰", harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("harHard", 大骚扰")).SetValue(new KeyBind('T', KeyBindType.Press, false));
                Config.SubMenu("harass").AddItem(new MenuItem("smaHard", 小骚扰")).SetValue(new KeyBind('A', KeyBindType.Press, false));

                //Extra
                Config.AddSubMenu(new Menu("其他", extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("drawHp", 显示血量")).SetValue(true);
                

                //Debug
                Config.AddSubMenu(new Menu("调试", debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", 修正目标")).SetValue(new KeyBind('N', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Drawing.OnEndScene += OnEndScene;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                Game.OnGameProcessPacket += OnGameProcessPacket;
                Talon.setSkillshots();

            }
            catch
            {{
                Game.PrintChat("Oops. Something went wrong with Yasuo- Sharpino");
            }

        }



        private static void OnGameUpdate(EventArgs args)
        {{

            if (useQ && Talon.Q.IsReady())
            {{
                Talon.Q.Cast();
            }
            if (!Talon.Q.IsReady())
            {{
                useQ = false;
            }

            Obj_AI_Hero target = SimpleTS.GetTarget(750, SimpleTS.DamageType.Physical);
            if (Talon.orbwalker.ActiveMode.ToString() == Combo")
            {{
                Talon.doCombo(target);   
            }

            if (Talon.orbwalker.ActiveMode.ToString() == Mixed")
            {{
               //Console.WriteLine("Skill name: +Talon.Rdata.Name);
               // foreach (var buf in target.Buffs)
               // {{
               //     Console.WriteLine("buf: +buf.Name);
               // }
            }

            if (Talon.orbwalker.ActiveMode.ToString() == LaneClear")
            {{
               // Console.WriteLine("Qbleed dmg  + Talon.getTargetBleedDmg(target));
            }

            if (Config.Item("harHard").GetValue<KeyBind>().Active)
            {{
                Talon.doHarassHard(target);
            }

            if (Config.Item("smaHard").GetValue<KeyBind>().Active)
            {{
                Talon.doHarassSmall(target);
            }

            if (Config.Item("harassOn").GetValue<bool>() && Talon.orbwalker.ActiveMode.ToString() == None")
            {{
              
            }
        }

        private static void OnEndScene(EventArgs args)
        {{
            if (Config.Item("drawHp").GetValue<bool>())
            {{
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {{
                    hpi.unit = enemy;
                    hpi.drawDmg(Talon.getFullComboDmg(enemy), Color.Yellow);
                }
            }
        }

        private static void onDraw(EventArgs args)
        {{
             
           // if (Config.Item("drawFull").GetValue<bool>())
              

            Utility.DrawCircle(Talon.Player.Position, 700, Color.Red);
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {{
          

        }



        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {{
          
        }

        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base obj, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {{


           
        }
        public static void OnGameProcessPacket(GamePacketEventArgs args)
        {{
            try
            {{
                if (Talon.orbwalker.ActiveMode.ToString() == Combo" || Config.Item("harHard").GetValue<KeyBind>().Active)
                {{
                    if (args.PacketData[0] == 101 && Talon.Q.IsReady())
                    {{
                        Packet.S2C.Damage.Struct dmg = Packet.S2C.Damage.Decoded(args.PacketData);
                        // LogPacket(args);
                        GamePacket gp = new GamePacket(args.PacketData);
                        gp.Position = 1;

                        int targetID = gp.ReadInteger();
                        int dType = (int)gp.ReadByte();
                        int Unknown = gp.ReadShort();
                        float DamageAmount = gp.ReadFloat();
                        int TargetNetworkIdCopy = gp.ReadInteger();
                        int SourceNetworkId = gp.ReadInteger();
                        if (Talon.Player.NetworkId != dmg.SourceNetworkId)
                            return;
                        Obj_AI_Hero targ = ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(targetID);
                        if (targ != null)
                        {{
                            Talon.sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                            Talon.Q.Cast();
                            useQ = true;
                            Orbwalking.ResetAutoAttackTimer();
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {{
                Console.WriteLine(ex.ToString());
            }
        }




    }
}
