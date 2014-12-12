using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace SFSeries
{
    class Singed
    {
        public static Menu Menu;
        public static Spell Q, W, E;
        public static Orbwalking.Orbwalker Orbwalker;
        public static bool delayed = false;
        public Singed()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 125);
            W.SetSkillshot(0.5f,350f,700f,false,SkillshotType.SkillshotCircle);
            Menu = new Menu("【初见汉化】无毒炼金", "menu", true);

            var orbwalkerMenu = new Menu("走砍", "orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            var comboMenu = new Menu("连招", "combo");
            comboMenu.AddItem(new MenuItem("useW", "使用 W").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "使用 E").SetValue(true));
            Menu.AddSubMenu(comboMenu);
            Menu.AddItem(
                new MenuItem("enabled", "隐形毒药").SetValue(new KeyBind("T".ToCharArray()[0],
                    KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("spam", "无限大笑").SetValue(true));
            Menu.AddToMainMenu();
Menu.AddSubMenu(new Menu("初见汉化", "by chujian"));

Menu.SubMenu("by chujian").AddItem(new MenuItem("qunhao", "汉化群：386289593"));
Menu.SubMenu("by chujian").AddItem(new MenuItem("qunhao2", "交流群群：333399"));

            Program.PrintMessage("鍔犺浇鎴愬姛!鍒濊姹夊寲QQ 5011477");
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
            QExploit();
        }

        private static void Combo()
        {
            var useW = Menu.Item("useW").GetValue<bool>();
            var useE = Menu.Item("useE").GetValue<bool>();
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            if (W.IsReady() && ObjectManager.Player.Distance(target) < W.Range + target.BoundingRadius && useW)
                W.Cast(target);
            if (E.IsReady() && ObjectManager.Player.Distance(target) < E.Range && useE)
                E.Cast(target, true);
        }

        static void QExploit()
        {
            if (!Menu.Item("enabled").GetValue<KeyBind>().Active) return;
            if (Q.IsReady())
                Q.Cast(ObjectManager.Player, true);
            if (!Menu.Item("spam").GetValue<bool>()) return;
            Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(2)).Send();
            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
        }
        static void danceSpam()
        {
            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
            if (delayed) return;
            Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct(2)).Send();
            delayed = true;
            Utility.DelayAction.Add((int) 75, () => delayed = false);
        }
        
    }
}
