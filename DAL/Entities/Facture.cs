using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Facture
{
    public int NumFact { get; set; }

    public string Typef { get; set; }

    public int? Numc { get; set; }

    public virtual Commande NumcNavigation { get; set; }
}
