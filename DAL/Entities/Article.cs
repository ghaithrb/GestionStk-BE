using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Article
{
    public int Codearticle { get; set; }

    public string Libelle { get; set; }

    public int? Quantite { get; set; }

    public decimal? Prix { get; set; }

    public virtual ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();

    public virtual ICollection<LstArticle> LstArticles { get; set; } = new List<LstArticle>();
}
