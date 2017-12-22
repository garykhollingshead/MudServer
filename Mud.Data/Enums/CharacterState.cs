namespace Mud.Data
{
    public enum CharacterState
    {
        AddName, ConfirmName,
        AddPassword, ConfirmPassword,
        AddGender,
        AddRace, ConfirmRace,
        AddWeight,
        AddHeight,
        ChooseStats, AdjustStats, ConfirmStats,
        ConfirmCharacterCreation,
        Changed, Saved
    }
}