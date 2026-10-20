using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class LstArticle
{
    public int Codearticle { get; set; }

    public int Codef { get; set; }

    public int? Qte { get; set; }

    public decimal? Prix { get; set; }

    public virtual Article CodearticleNavigation { get; set; }

    public virtual Fournisseur CodefNavigation { get; set; }
}
