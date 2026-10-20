using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Fournisseur
{
    public int Codef { get; set; }

    public string Nomf { get; set; }

    public string Adresse { get; set; }

    public string Email { get; set; }

    public string Tel { get; set; }

    public virtual ICollection<LstArticle> LstArticles { get; set; } = new List<LstArticle>();
}
