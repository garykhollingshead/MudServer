using System.Collections.Generic;
using Mud.Data.Character;
using Mud.Data.Configuration;
using Mud.DataAccess.Adapters;

namespace Mud.DataAccess.Repositories
{
    public class CharacterRepo
    {
        private MartenAdapter Adapter;

        public CharacterRepo(AppSettings appSettings)
        {
            Adapter = new MartenAdapter(appSettings.PostgresSettings);
        }

        public List<Character> GetAllCharacter()
        {
            return Adapter.QueryList<Character>(character => !string.IsNullOrEmpty(character.Name));
        }

        public Character GetCharacterByName(string name)
        {
            return Adapter.QuerySingle<Character>(character => character.Name == name);
        }
    }
}
