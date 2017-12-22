using System;
using System.Collections.Generic;
using Mud.Data.Character;
using Mud.Data.Configuration;
using Mud.DataAccess.Adapters;

namespace Mud.DataAccess.Repositories
{
    public class RaceRepo
    {
        private MartenAdapter Adapter;

        public RaceRepo(AppSettings appSettings)
        {
            Adapter = new MartenAdapter(appSettings.PostgresSettings);
        }

        public List<Race> GetAllRaces()
        {
            return Adapter.QueryList<Race>(race => !string.IsNullOrEmpty(race.Name));
        }

        public Race GetRaceByName(string name)
        {
            return Adapter.QueryFirstOrDefault<Race>(race => string.Equals(race.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
