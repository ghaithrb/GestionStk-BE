using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Client
{
    public int Codeclt { get; set; }

    public string Nom { get; set; }

    public string Adresse { get; set; }

    public string Email { get; set; }

    public string Tel { get; set; }

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();
}
