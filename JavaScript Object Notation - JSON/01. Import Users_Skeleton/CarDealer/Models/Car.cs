﻿using System.ComponentModel.DataAnnotations;

namespace CarDealer.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public long TraveledDistance { get; set; }

        public virtual ICollection<Sale> Sales { get; set; } = null!;

        public virtual ICollection<PartCar> PartsCars { get; set; } = new List<PartCar>();
    }
}
