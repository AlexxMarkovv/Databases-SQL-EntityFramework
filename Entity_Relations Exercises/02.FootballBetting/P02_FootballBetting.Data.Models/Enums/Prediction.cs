namespace P02_FootballBetting.Data.Models.Enums;

// Enumerations are not entities int the DB
// Enumerations are string representation of int values
// In the DB -> int
public enum Prediction
{
    Win = 1,
    Lose = 2,
    Draw = 0
}
