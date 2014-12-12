using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

//using Color = System.Drawing.Color;

namespace SephElise
{
    internal class Program
    {
        private const string ChampionName = "Elise";
        private static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R, QS, WS, ES;

        private static Menu Config;
        public static Items.Item DFG;
        private static Items.Item HDR;
        private static Items.Item BKR;
        private static Items.Item BWC;
        private static SpellSlot IgniteSlot;

        private static Items.Item YOU;

        private static Obj_AI_Hero Player;
        private static bool HumanForm;
        private static bool SpiderForm;

        public static Menu SEPH;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1075f);
            QS = new Spell(SpellSlot.Q, 475f);
            WS = new Spell(SpellSlot.W, 0);
            ES = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 0);
            W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);

            HDR = new Items.Item(3074, 175f);
            BKR = new Items.Item(3153, 450f);
            BWC = new Items.Item(3144, 450f);
            YOU = new Items.Item(3142, 185f);
            DFG = new Items.Item(3128, 750f);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");


            Config = new Menu("【超神汉化】蜘蛛", "Elise", true);


            //TargetSelector
            var targetSelectorMenu = new Menu("目标选择", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalker
            Config.AddSubMenu(new Menu("走砍", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo
            Config.AddSubMenu(new Menu("连招", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQHuman", "使用Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWHuman", "使用W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseEHuman", "使用E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "使用R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQSpider", "使用Q(蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWSpider", "使用W(蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseESpider", "使用E(蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "使用物品")).SetValue(true);
            Config.SubMenu("Combo")
                .AddItem(new MenuItem("ActiveCombo", "热键").SetValue(new KeyBind(32, KeyBindType.Press)));


            //Harass
            Config.AddSubMenu(new Menu("骚扰", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "使用Q")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "使用W")).SetValue(true);
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("ActiveHarass", "热键").SetValue(new KeyBind("X".ToCharArray()[0],
                        KeyBindType.Press)));

            //Farm
            Config.AddSubMenu(new Menu("补兵/清线", "Farm"));
            Config.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "使用Q (蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Farm").AddItem(new MenuItem("UseSpiderEFarm", "使用E (蜘蛛形态)")).SetValue(false);
            Config.SubMenu("Farm").AddItem(new MenuItem("UseWFarm", "使用W (蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("ActiveFarm", "热键").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Kill Steal
            Config.AddSubMenu(new Menu("抢人头", "Ks"));
            Config.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "抢人头")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseQKs", "使用Q")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseQKsSpider", "使用Q(蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseWKsSpider", "使用W(蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseEKsSpider", "使用E(蜘蛛形态)")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "使用点燃")).SetValue(true);


            //Drawings
            Config.AddSubMenu(new Menu("显示", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Q范围")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "W范围")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "E范围")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "无延迟线圈").SetValue(true));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleQuality", "质量").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("CircleThickness", "密度").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            Game.PrintChat("<font color='#1d87f2'>SephElise has been Loaded.</font>");
        }


        private static void OnGameUpdate(EventArgs args)
        {
            Player = ObjectManager.Player;
            QS = new Spell(SpellSlot.Q, QS.Range);
            Orbwalker.SetAttack(true);

            CheckForm();

            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (Config.Item("ActiveHarass").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            if (Config.Item("ActiveFarm").GetValue<KeyBind>().Active)
            {
                Farm();
            }
            if (Config.Item("ActiveKs").GetValue<bool>())
            {
                KillSteal();
            }
        }

        private static void Harass()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target != null)
            {
                if (HumanForm && Player.Distance(target) <= Q.Range && Config.Item("UseQHarass").GetValue<bool>() &&
                    Q.IsReady())
                {
                    Q.Cast(target);
                }

                if (HumanForm && Player.Distance(target) <= W.Range && Config.Item("UseWHarass").GetValue<bool>() &&
                    W.IsReady())
                {
                    W.Cast(target);
                }
            }
        }


        private static void JungleFarm()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(QS.Range, SimpleTs.DamageType.Magical);
            List<Obj_AI_Base> mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);

            if (Config.Item("UseQFarm").GetValue<bool>())
            {
                foreach (Obj_AI_Base minion in mobs)
                    if (HumanForm)
                    {
                        if (QS.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= Q.Range)
                        {
                            Q.Cast(minion);
                        }
                        if (W.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= W.Range)
                        {
                            W.Cast();
                        }
                        R.Cast();
                    }
                foreach (Obj_AI_Base minion in mobs)
                {
                    if (QS.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= QS.Range)
                    {
                        QS.Cast(minion);
                    }
                    if (WS.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= 125)
                    {
                        WS.Cast();
                    }
                    if (ES.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= ES.Range &&
                        Config.Item("UseSpiderEFarm").GetValue<bool>())
                    {
                        ES.Cast(minion);
                    }
                }
            }
        }

        private static void Farm()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(QS.Range, SimpleTs.DamageType.Magical);
            List<Obj_AI_Base> allminions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.Enemy, MinionOrderTypes.Health);

            if (Config.Item("UseQFarm").GetValue<bool>())
            {
                foreach (Obj_AI_Base minion in allminions)
                    if (HumanForm)
                    {
                        if (QS.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= Q.Range)
                        {
                            Q.Cast(minion);
                        }
                        if (W.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= W.Range)
                        {
                            W.Cast();
                        }
                        R.Cast();
                    }
                foreach (Obj_AI_Base minion in allminions)
                {
                    if (QS.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= QS.Range)
                    {
                        QS.Cast(minion);
                    }
                    if (WS.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= 125)
                    {
                        WS.Cast();
                    }
                    if (ES.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= ES.Range &&
                        Config.Item("UseSpiderEFarm").GetValue<bool>())
                    {
                        ES.Cast(minion);
                    }
                }
            }
        }


        private static void KillSteal()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            double igniteDmg = Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            double QHDmg = Player.GetSpellDamage(target, SpellSlot.Q);
            double WDmg = Player.GetSpellDamage(target, SpellSlot.W);

            if (target != null && Config.Item("UseIgnite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }

            if (Q.IsReady() && Player.Distance(target) <= Q.Range && target != null &&
                Config.Item("UseQKs").GetValue<bool>())
            {
                if (target.Health <= QHDmg)
                {
                    Q.Cast(target);
                }
            }
            if (QS.IsReady() && Player.Distance(target) <= QS.Range && target != null &&
                Config.Item("UseQKs").GetValue<bool>())
            {
                if (target.Health <= QHDmg)
                {
                    Q.Cast(target);
                }
            }
            if (W.IsReady() && Player.Distance(target) <= W.Range && target != null &&
                Config.Item("UseWKs").GetValue<bool>() && HumanForm)
            {
                if (target.Health <= WDmg)
                {
                    W.Cast(target);
                }
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
        }

        private static void CheckForm()
        {
            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE")
            {
                float Qcd, Wcd, Ecd, QScd, WScd, EScd;
                Qcd = (Player.Spellbook.GetSpell(SpellSlot.Q).Cooldown);
                Wcd = (Player.Spellbook.GetSpell(SpellSlot.W).Cooldown);
                Ecd = (Player.Spellbook.GetSpell(SpellSlot.E).Cooldown);


                HumanForm = true;
                SpiderForm = false;

                // Game.PrintChat("We are in Human form.");
            }

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial")
            {
                //Game.PrintChat("We are in Spider form.");
                float QScd, WScd, EScd;
                QScd = (Player.Spellbook.GetSpell(SpellSlot.Q).Cooldown);
                WScd = (Player.Spellbook.GetSpell(SpellSlot.W).Cooldown);
                EScd = (Player.Spellbook.GetSpell(SpellSlot.E).Cooldown);

                HumanForm = false;
                SpiderForm = true;
            }
        }


        private static void Combo()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            Orbwalker.SetAttack((!Q.IsReady() || E.IsReady() || W.IsReady()));

            if (target != null)
            {
                if (HumanForm)
                {
                    // Human Rotation
                    if (Player.Distance(target) <= Q.Range && Config.Item("UseQHuman").GetValue<bool>() && Q.IsReady())
                    {
                        Q.Cast(target);
                    }
                    if (Player.Distance(target) <= W.Range && Config.Item("UseWHuman").GetValue<bool>() && W.IsReady())
                    {
                        W.Cast(target);
                    }
                    if (Player.Distance(target) <= E.Range && Config.Item("UseEHuman").GetValue<bool>() && E.IsReady())
                    {
                        E.Cast(target);
                    }
                    if (!Q.IsReady() && !W.IsReady() && !E.IsReady() && Player.Distance(target) <= 750 &&
                        Config.Item("UseR").GetValue<bool>())
                    {
                        R.Cast();
                    }
                    if (!Q.IsReady() && !W.IsReady() && Player.Distance(target) <= 750 &&
                        Config.Item("UseQHuman").GetValue<bool>())
                    {
                        R.Cast();
                    }
                }
                // Spider Rotation
                if (SpiderForm)
                {
                    if (Player.Distance(target) <= QS.Range && Config.Item("UseQSpider").GetValue<bool>() &&
                        QS.IsReady())
                    {
                        QS.Cast(target);
                    }
                    if (Player.Distance(target) <= 140 && Config.Item("UseWSpider").GetValue<bool>() && WS.IsReady())
                    {
                        WS.Cast();
                    }
                    if (Player.Distance(target) <= ES.Range && Player.Distance(target) > QS.Range &&
                        Config.Item("UseESpider").GetValue<bool>() && ES.IsReady())
                    {
                        ES.Cast(target);
                    }
                    if (Player.Distance(target) > QS.Range && !ES.IsReady() && R.IsReady() &&
                        Player.Distance(target) <= 1075 && Config.Item("UseR").GetValue<bool>())
                    {
                        R.Cast();
                    }
                    if (!QS.IsReady() && Player.Distance(target) >= 125 && !ES.IsReady() && R.IsReady() &&
                        Player.Distance(target) <= 1075 && Config.Item("UseR").GetValue<bool>())
                    {
                        R.Cast();
                    }
                    if (ES.IsReady() && Player.Distance(target) > QS.Range && Config.Item("UseESpider").GetValue<bool>())
                    {
                        ES.Cast(target);
                    }
                }
            }
        }
    }
}