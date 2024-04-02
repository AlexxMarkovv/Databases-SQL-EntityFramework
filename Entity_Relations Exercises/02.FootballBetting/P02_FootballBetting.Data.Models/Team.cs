namespace P02_FootballBetting.Data.Models;

using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Team
{
    public Team()
    {
        HomeGames = new HashSet<Game>();
        AwayGames = new HashSet<Game>();
        Players = new HashSet<Player>();
    }

    [Key] // Sets PK of the table -> UNIQUE, NOT NULL
    public int TeamId {  get; set; }

    [Required] // NOT NULL Constraint in SQL
    [MaxLength(ValidationConstants.TeamNameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.TeamLogoUrlMaxLength)]
    public string LogoUrl { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.TeamInitialsMaxLength)]
    public string Initials { get; set; } = null!;

    public decimal Budget { get; set; }

    [ForeignKey(nameof(PrimaryKitColor))]
    public int PrimaryKitColorId { get; set; }

    public virtual Color PrimaryKitColor { get; set; } = null!;

    [ForeignKey(nameof (SecondaryKitColor))]
    public int SecondaryKitColorId { get; set;}

    public virtual Color SecondaryKitColor { get; set; } = null!;

    [ForeignKey(nameof(Town))]
    public int TownId { get; set; }

    public virtual Town Town { get; set; } = null!;

    [InverseProperty(nameof(Game.HomeTeam))]
    public virtual ICollection<Game> HomeGames { get; set; }

    [InverseProperty(nameof(Game.AwayTeam))]
    public virtual ICollection<Game> AwayGames { get; set; }

    public virtual ICollection<Player> Players { get; set; }
}