using Assemblies.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

namespace Assemblies.Champions {{
    internal class Champion : ChampionUtils {{
        public Obj_AI_Hero player = ObjectManager.Player;
        private readonly WardJumper wardJumper;
        public AntiRengar antiRengar;
        protected Spell E;
        protected Spell Q;
        protected Spell R;
        protected Spell W;
        protected Menu menu;
        public static Menu SimpleTSMenu;
        //public MenuWrapper menuWrapper;
        //protected Orbwalking.Orbwalker orbwalker;

        public Champion() {{
            addBasicMenu();
            wardJumper = new WardJumper();
            antiRengar = new AntiRengar();
        }

        private void addBasicMenu() {{
            //menuWrapper = new MenuWrapper("Assemblies -  + player.ChampionName);
            menu = new Menu("【超神汉化】" + player.ChampionName, Assemblies -  + player.ChampionName,
                true);

            SimpleTSMenu = new Menu("目标选择", Target Selector");
            SimpleTS.AddToMenu(SimpleTSMenu);
            menu.AddSubMenu(SimpleTSMenu);

            //Orbwalker submenu
            var orbwalkerMenu = new Menu("xSLx走砍", orbwalker");
            xSLxOrbwalker.AddToMenu(orbwalkerMenu);
            menu.AddSubMenu(orbwalkerMenu);

            menu.AddToMainMenu();
        }
    }
}