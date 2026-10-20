using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class LigneCommande
{
    public int Numc { get; set; }

    public int Codearticle { get; set; }

    public int? Qte { get; set; }

    public decimal? Prix { get; set; }

    public virtual Article CodearticleNavigation { get; set; }

    public virtual Commande NumcNavigation { get; set; }
}
