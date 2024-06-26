﻿using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models;

public class User
{
    public User()
    {
        Bets = new HashSet<Bet>();
    }

    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(ValidationConstants.UserUsernameMaxLength)]
    public string Username { get; set; } = null!;

    // Password is saved like hashSet
    [Required]
    [MaxLength(ValidationConstants.UserPasswordMaxLength)]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.UserEmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.UserNameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Balance { get; set; }

    public virtual ICollection<Bet> Bets { get; set; }
}
