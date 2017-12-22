using System;
using System.Collections.Generic;
using System.Linq;
using Mud.Data;
using Mud.Data.Commands;
using Mud.Data.Configuration;
using Mud.Data.Enums;
using Mud.DataAccess.Repositories;

namespace Mud.Server
{
    public class CharacterCreationProccessor
    {
        public RaceRepo RaceRepository;

        public CharacterCreationProccessor(AppSettings appSettings)
        {
            RaceRepository = new RaceRepo(appSettings);
        }

        public void ProcessCharacter(User user, List<string> inputData)
        {
            switch (user.Character.CurrentState)
            {
                case CharacterState.AddName:
                    var name = inputData[0];
                    name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

                    if (!CommandProccessor.CheckUserLogin(user, name))
                    {
                        return;
                    }

                    user.Connection.SendData($"{name}. Is that the name by which you wish to be known?");
                    user.Character.Name = name;
                    break;
                case CharacterState.ConfirmName:
                    var reply = inputData[0];
                    if (StringConstants.AffirmativeReply.Contains(reply.ToLower()))
                    {
                        user.Connection.SendData("Ok, what name do you wish to be known by?");
                        user.Character.CurrentState = CharacterState.AddName;
                        return;
                    }
                    user.Character.CurrentState = CharacterState.AddPassword;
                    user.Connection.SendData($"Welcome {user.Character.Name}. Please enter a password: ");
                    break;
                case CharacterState.AddPassword:
                    var password = CommandProccessor.HashPassword(inputData[0]);
                    user.Character.Password = password;
                    user.Character.CurrentState = CharacterState.ConfirmPassword;
                    user.Connection.SendData("Please confirm your password: ");
                    break;
                case CharacterState.ConfirmPassword:
                    var confirmPassword = CommandProccessor.HashPassword(inputData[0]);
                    if (confirmPassword == user.Character.Password)
                    {
                        user.Character.CurrentState = CharacterState.AddGender;
                        user.Connection.SendData("Is this avatar (m)ale or (f)emale?");
                        return;
                    }
                    user.Character.CurrentState = CharacterState.AddPassword;
                    user.Connection.SendData("The passwords did not match! Please enter a password: ");
                    break;
                case CharacterState.AddGender:
                    var genderReply = inputData[0].ToLower();
                    user.Character.Gender = (genderReply == "male" || genderReply == "m")
                        ? Gender.Male
                        : Gender.Female;
                    user.Character.CurrentState = CharacterState.AddRace;
                    var races = RaceRepository.GetAllRaces();
                    user.Connection.SendData("Please choose your race: ");
                    races.ForEach(race => user.Connection.SendData($"\t{race.Name}"));
                    user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                    break;
                case CharacterState.AddRace:
                    races = RaceRepository.GetAllRaces();
                    if (inputData[0].ToLower() == "help")
                    {
                        if (inputData.Count == 1)
                        {
                            user.Connection.SendData("You are in the middle of selecting your avatar's race. Please choose a race: ");
                            races.ForEach(race => user.Connection.SendData(race.Name));
                            user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                            return;
                        }
                        if (races.Any(race =>
                            string.Equals(inputData[1], race.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            var raceDescription = races.First(race =>
                                    string.Equals(inputData[1], race.Name, StringComparison.CurrentCultureIgnoreCase))
                                    .Description;
                            user.Connection.SendData(raceDescription);

                            user.Connection.SendData("Please choose your race: ");
                            races.ForEach(race => user.Connection.SendData($"\t{race.Name}"));
                            user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                            return;
                        }
                    }
                    if (races.Any(race =>
                        string.Equals(inputData[0], race.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        user.Character.Race = races.First(race => string.Equals(inputData[0], race.Name,
                            StringComparison.CurrentCultureIgnoreCase));
                        user.Character.CurrentState = CharacterState.ConfirmRace;
                        user.Connection.SendData($"{user.Character.Race.Name}. Is that the correct race?");
                        return;
                    }
                    user.Connection.SendData($"{inputData[0]} is not a valid race! Please choose your race: ");
                    races.ForEach(race => user.Connection.SendData($"\t{race.Name}"));
                    user.Connection.SendData("(You can use 'help raceName' to get a description of particular race)");
                    break;
                case CharacterState.ConfirmRace:
                    break;
                case CharacterState.AddHeight:
                    break;
                case CharacterState.AddWeight:
                    break;
                case CharacterState.ChooseStats:
                    break;
                case CharacterState.AdjustStats:
                    break;
                case CharacterState.ConfirmStats:
                    break;
                case CharacterState.ConfirmCharacterCreation:
                    break;
                default:
                    user.Character.CommandsAvailiable.Remove(Commands.CreateCharacter);
                    break;
            }
        }
    }
}
