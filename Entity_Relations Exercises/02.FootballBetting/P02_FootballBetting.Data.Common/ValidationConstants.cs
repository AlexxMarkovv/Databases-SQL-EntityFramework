namespace P02_FootballBetting.Data.Common;

public static class ValidationConstants
{
    // Team
    public const int TeamNameMaxLength = 50;

    public const int TeamLogoUrlMaxLength = 2048;

    public const int TeamInitialsMaxLength = 4;

    //Color
    public const int ColorNameMaxLength = 50;

    //Town
    public const int TownNameMaxLength = 70;

    // Country
    public const int CountryNameMaxLength = 70;

    // Player
    public const int PlayerNameMaxLength = 100;

    // Position
    public const int PositionNameMaxLength = 50;

    // Game 99 : 99
    public const int GameResultMaxLength = 10;

    // User
    public const int UserUsernameMaxLength = 56;
    public const int UserPasswordMaxLength = 255;
    public const int UserEmailMaxLength = 255;
    public const int UserNameMaxLength = 100;
}
