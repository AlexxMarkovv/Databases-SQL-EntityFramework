using P02_FootballBetting.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models;

public class Bet
{
    [Key]
    public int BetId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public Prediction Prediction { get; set; }

    public DateTime DateTime { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;
}
