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
 * Hydra <-- done
 * 
 * overkill <--done
 * 
 * 
 * tower dives
 * 
 * 
 * ult only close
 * 
 * */


namespace KhazixSharp
{{
    internal class KhazixSharp
    {{

        public const string CharName = Khazix";

        public static Menu Config;

        public static HpBarIndicator hpi = new HpBarIndicator();

        public KhazixSharp()
        {{
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {{

            Game.PrintChat("Khazix - Sharp by DeTuKs");

            try
            {{

                Config = new Menu("【超神汉化】螳螂", Khazix", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("走砍", Orbwalker"));
                Khazix.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                var SimpleTSMenu = new Menu("目标选择", Target Selector");
                SimpleTS.AddToMenu(SimpleTSMenu);
                Config.AddSubMenu(SimpleTSMenu);
                //Combo
                Config.AddSubMenu(new Menu("连招", combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", Use Items")).SetValue(true);

                //LastHit
                Config.AddSubMenu(new Menu("补兵", lHit"));
               
                //LaneClear
                Config.AddSubMenu(new Menu("清线", lClear"));
               
                //Harass
                Config.AddSubMenu(new Menu("骚扰", harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("harassBtn", Harass Target")).SetValue(new KeyBind('A', KeyBindType.Press, false));

                //Extra
                Config.AddSubMenu(new Menu("其他", extra"));
                

                //Debug
                Config.AddSubMenu(new Menu("调试", debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", 修正目标")).SetValue(new KeyBind('T', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                GameObject.OnPropertyChange += OnPropertyChange;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

                Game.OnGameSendPacket += OnGameSendPacket;
                Game.OnGameProcessPacket += OnGameProcessPacket;

                Khazix.setSkillshots();
            }
            catch
            {{
                Game.PrintChat("Oops. Something went wrong with KhazixSharp");
            }

        }

        private static void OnGameUpdate(EventArgs args)
        {{
            try
            {{

                if (Khazix.orbwalker.ActiveMode.ToString() == Combo")
                {{
                    Obj_AI_Hero target = SimpleTS.GetTarget(Khazix.getBestRange(), SimpleTS.DamageType.Physical);

                    Khazix.checkUpdatedSpells();


                    Khazix.doCombo(target);
                    //Console.WriteLine(target.NetworkId);
                }
                if (Config.Item("harassBtn").GetValue<KeyBind>().Active)
                {{
                    Obj_AI_Hero target = SimpleTS.GetTarget(Khazix.getBestRange(), SimpleTS.DamageType.Physical);


                    Khazix.doHarass(target);
                }



            }
            catch (Exception ex)
            {{
                Console.WriteLine(ex);
            }
        }


        private static void onDraw(EventArgs args)
        {{
            foreach (
                            var enemy in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
            {{
                hpi.unit = enemy;
                hpi.drawDmg(Khazix.fullComboDmgOn(enemy), Color.Yellow);
            }
            Drawing.DrawCircle(Khazix.Player.Position, Khazix.Q.Range, Color.Pink);
            Drawing.DrawCircle(Khazix.Player.Position, Khazix.W.Range, Color.Pink);
            Drawing.DrawCircle(Khazix.Player.Position, Khazix.E.Range, Color.Pink);
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {{
            

        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {{

        }



        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base sender, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {{
           
        }

        public static void OnPropertyChange(LeagueSharp.GameObject obj, LeagueSharp.GameObjectPropertyChangeEventArgs prop)
        {{

        }



        public static void OnGameProcessPacket(GamePacketEventArgs args)
        {{

        }

        public static void OnGameSendPacket(GamePacketEventArgs args)
        {{
            if (args != null && (args.PacketData[0] == 175))
            {{
                //Console.WriteLine("aa  + args.PacketData[0]);
               // args.Process = false;
            }
        }




    }
}
