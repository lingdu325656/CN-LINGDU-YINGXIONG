using System;
using System.Drawing;
using System.Linq;
using Assemblies.Champions;
using LeagueSharp;
using LeagueSharp.Common;

namespace Assemblies.Utilitys {{
    internal class AssassinManager {{
        public AssassinManager() {{
            Load();
        }

        private static void Load() {{
            Champion.SimpleTSMenu.AddSubMenu(new Menu("击杀助手", MenuAssassin"));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddItem(
                new MenuItem("AssassinActive", 打开").SetValue(true));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("Ax", "));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddItem(
                new MenuItem("AssassinSelectOption", 设置").SetValue(
                    new StringList(new[] {{"单目标", 多目标"})));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("Ax", "));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddItem(
                new MenuItem("AssassinSetClick", 右键添加/删除").SetValue(true));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddItem(
                new MenuItem("AssassinReset", 重置列表").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddSubMenu(new Menu("显示", Draw"));

            Champion.SimpleTSMenu.SubMenu("MenuAssassin").SubMenu("Draw").AddItem(
                new MenuItem("DrawSearch", 搜索范围").SetValue(new Circle(true, Color.Gray)));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").SubMenu("Draw").AddItem(
                new MenuItem("DrawActive", 活跃的敌人").SetValue(new Circle(true, Color.GreenYellow)));
            Champion.SimpleTSMenu.SubMenu("MenuAssassin").SubMenu("Draw").AddItem(
                new MenuItem("DrawNearest", 最近的敌人").SetValue(new Circle(true, Color.DarkSeaGreen)));


            Champion.SimpleTSMenu.SubMenu("MenuAssassin").AddSubMenu(new Menu("列表", AssassinMode"));
            foreach (
                Obj_AI_Hero enemy in
                    ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != ObjectManager.Player.Team)) {{
                Champion.SimpleTSMenu.SubMenu("MenuAssassin")
                    .SubMenu("AssassinMode")
                    .AddItem(
                        new MenuItem("Assassin" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(
                            SimpleTS.GetPriority(enemy) > 3));
            }
            Champion.SimpleTSMenu.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSearchRange", 搜索范围")).SetValue(new Slider(1000, 2000));

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void ClearAssassinList() {{
            foreach (
                Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy)) {{
                Champion.SimpleTSMenu.Item("Assassin" + enemy.BaseSkinName).SetValue(false);
            }
        }

        private static void OnGameUpdate(EventArgs args) {{}

        private static void Game_OnWndProc(WndEventArgs args) {{
            if (Champion.SimpleTSMenu.Item("AssassinReset").GetValue<KeyBind>().Active && args.Msg == 257) {{
                ClearAssassinList();
                Game.PrintChat(
                    <font color='#FFFFFF'>Reset Assassin List is Complete! Click on the enemy for Add/Remove.</font>");
            }

            if (args.Msg != 0x201) {{
                return;
            }

            if (Champion.SimpleTSMenu.Item("AssassinSetClick").GetValue<bool>()) {{
                foreach (Obj_AI_Hero objAiHero in from hero in ObjectManager.Get<Obj_AI_Hero>()
                    where hero.IsValidTarget()
                    select hero
                    into h
                    orderby h.Distance(Game.CursorPos) descending
                    select h
                    into enemy
                    where enemy.Distance(Game.CursorPos) < 100f
                    select enemy) {{
                    if (objAiHero != null && objAiHero.IsVisible && !objAiHero.IsDead) {{
                        int xSelect =
                            Champion.SimpleTSMenu.Item("AssassinSelectOption").GetValue<StringList>()
                                .SelectedIndex;

                        switch (xSelect) {{
                            case 0:
                                ClearAssassinList();
                                Champion.SimpleTSMenu.Item("Assassin" + objAiHero.BaseSkinName).SetValue(true);
                                Game.PrintChat(
                                    string.Format(
                                        <font color='FFFFFF'>Added to Assassin List</font> <font color='#09F000'>{{0} ({{1})</font>",
                                        objAiHero.Name, objAiHero.BaseSkinName));
                                break;
                            case 1:
                                var menuStatus =
                                    Champion.SimpleTSMenu.Item("Assassin" + objAiHero.BaseSkinName).GetValue<bool>
                                        ();
                                Champion.SimpleTSMenu.Item("Assassin" + objAiHero.BaseSkinName).SetValue(
                                    !menuStatus);
                                Game.PrintChat(
                                    string.Format("<font color='{{0}'>{{1}</font> <font color='#09F000'>{{2} ({{3})</font>",
                                        !menuStatus ? #FFFFFF" : #FF8877",
                                        !menuStatus ? Added to Assassin List:" : Removed from Assassin List:",
                                        objAiHero.Name, objAiHero.BaseSkinName));
                                break;
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args) {{
            if (!Champion.SimpleTSMenu.Item("AssassinActive").GetValue<bool>())
                return;

            var drawSearch = Champion.SimpleTSMenu.Item("DrawSearch").GetValue<Circle>();
            var drawActive = Champion.SimpleTSMenu.Item("DrawActive").GetValue<Circle>();
            var drawNearest = Champion.SimpleTSMenu.Item("DrawNearest").GetValue<Circle>();

            int drawSearchRange = Champion.SimpleTSMenu.Item("AssassinSearchRange").GetValue<Slider>().Value;
            if (drawSearch.Active) {{
                Utility.DrawCircle(ObjectManager.Player.Position, drawSearchRange, drawSearch.Color);
            }

            foreach (
                Obj_AI_Hero enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                        .Where(
                            enemy =>
                                enemy.IsVisible &&
                                Champion.SimpleTSMenu.Item("Assassin" + enemy.BaseSkinName) != null &&
                                !enemy.IsDead)
                        .Where(
                            enemy => Champion.SimpleTSMenu.Item("Assassin" + enemy.BaseSkinName).GetValue<bool>())
                ) {{
                if (ObjectManager.Player.Distance(enemy) < drawSearchRange) {{
                    if (drawActive.Active)
                        Utility.DrawCircle(enemy.Position, 85f, drawActive.Color);
                }
                else if (ObjectManager.Player.Distance(enemy) > drawSearchRange &&
                         ObjectManager.Player.Distance(enemy) < drawSearchRange + 400) {{
                    if (drawNearest.Active)
                        Utility.DrawCircle(enemy.Position, 85f, drawNearest.Color);
                }
            }
        }
    }
}