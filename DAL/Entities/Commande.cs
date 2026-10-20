using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Commande
{
    public int Numc { get; set; }

    public DateOnly Datecommande { get; set; }

    public decimal? Prixht { get; set; }

    public decimal? Tva { get; set; }

    public decimal? Ttc { get; set; }

    public int Codeclt { get; set; }

    public virtual Client CodecltNavigation { get; set; }

    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public virtual ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();
}
