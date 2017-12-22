using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mud.Data;
using Mud.Data.Configuration;
using Mud.DataAccess.Adapters;

namespace Mud.DataAccess.Repositories
{
    public class SplashScreenRepo
    {
        private MartenAdapter Adapter;

        public SplashScreenRepo(AppSettings appSettings)
        {
            Adapter = new MartenAdapter(appSettings.PostgresSettings);
        }

        public List<SplashScreen> GetAllSplashScreens()
        {
            return Adapter.QueryList<SplashScreen>(ss => ss.Screen != null);
        }

        public SplashScreen GetSplashScreenById(string id)
        {
            return Adapter.GetById<SplashScreen>(id);
        }

        public SplashScreen GetRandomSplashScreen()
        {
            var screens = GetAllSplashScreens();
            if (!screens.Any())
            {
                return new SplashScreen
                {
                    Screen = new string[] {"You've connected to the mud!"}
                };
            }
            var rand = new Random(DateTime.Now.Millisecond);
            return screens[rand.Next(screens.Count) - 1];
        }

        public SplashScreen SaveSplashScreen(SplashScreen screen)
        {
            return Adapter.Upsert(screen);
        }
    }
}
