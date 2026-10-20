using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public partial class STKDbContext : DbContext
{
    public STKDbContext()
    {
    }

    public STKDbContext(DbContextOptions<STKDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<Fournisseur> Fournisseurs { get; set; }

    public virtual DbSet<LigneCommande> LigneCommandes { get; set; }

    public virtual DbSet<LstArticle> LstArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Codearticle).HasName("article_pkey");

            entity.ToTable("article");

            entity.Property(e => e.Codearticle).HasColumnName("codearticle");
            entity.Property(e => e.Libelle)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("libelle");
            entity.Property(e => e.Prix)
                .HasPrecision(10, 3)
                .HasColumnName("prix");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Codeclt).HasName("client_pkey");

            entity.ToTable("client");

            entity.HasIndex(e => e.Email, "client_email_key").IsUnique();

            entity.Property(e => e.Codeclt).HasColumnName("codeclt");
            entity.Property(e => e.Adresse)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("adresse");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Nom)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Tel)
                .HasMaxLength(20)
                .HasColumnName("tel");
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.Numc).HasName("commande_pkey");

            entity.ToTable("commande");

            entity.Property(e => e.Numc).HasColumnName("numc");
            entity.Property(e => e.Codeclt).HasColumnName("codeclt");
            entity.Property(e => e.Datecommande).HasColumnName("datecommande");
            entity.Property(e => e.Prixht)
                .HasPrecision(10, 2)
                .HasColumnName("prixht");
            entity.Property(e => e.Ttc)
                .HasPrecision(10, 2)
                .HasColumnName("ttc");
            entity.Property(e => e.Tva)
                .HasPrecision(5, 2)
                .HasColumnName("tva");

            entity.HasOne(d => d.CodecltNavigation).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.Codeclt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_commande_client");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.NumFact).HasName("facture_pkey");

            entity.ToTable("facture");

            entity.Property(e => e.NumFact)
                .ValueGeneratedNever()
                .HasColumnName("num_fact");
            entity.Property(e => e.Numc).HasColumnName("numc");
            entity.Property(e => e.Typef)
                .HasMaxLength(30)
                .HasColumnName("typef");

            entity.HasOne(d => d.NumcNavigation).WithMany(p => p.Factures)
                .HasForeignKey(d => d.Numc)
                .HasConstraintName("fk_facture_commande");
        });

        modelBuilder.Entity<Fournisseur>(entity =>
        {
            entity.HasKey(e => e.Codef).HasName("fournisseur_pkey");

            entity.ToTable("fournisseur");

            entity.HasIndex(e => e.Email, "fournisseur_email_key").IsUnique();

            entity.Property(e => e.Codef).HasColumnName("codef");
            entity.Property(e => e.Adresse)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("adresse");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Nomf)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("nomf");
            entity.Property(e => e.Tel)
                .HasMaxLength(20)
                .HasColumnName("tel");
        });

        modelBuilder.Entity<LigneCommande>(entity =>
        {
            entity.HasKey(e => new { e.Numc, e.Codearticle }).HasName("pk_ligne_commande");

            entity.ToTable("ligne_commande");

            entity.Property(e => e.Numc).HasColumnName("numc");
            entity.Property(e => e.Codearticle).HasColumnName("codearticle");
            entity.Property(e => e.Prix)
                .HasPrecision(10, 2)
                .HasColumnName("prix");
            entity.Property(e => e.Qte).HasColumnName("qte");

            entity.HasOne(d => d.CodearticleNavigation).WithMany(p => p.LigneCommandes)
                .HasForeignKey(d => d.Codearticle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ligne_article");

            entity.HasOne(d => d.NumcNavigation).WithMany(p => p.LigneCommandes)
                .HasForeignKey(d => d.Numc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ligne_commande");
        });

        modelBuilder.Entity<LstArticle>(entity =>
        {
            entity.HasKey(e => new { e.Codearticle, e.Codef }).HasName("pk_lot_article");

            entity.ToTable("lst_article");

            entity.Property(e => e.Codearticle).HasColumnName("codearticle");
            entity.Property(e => e.Codef).HasColumnName("codef");
            entity.Property(e => e.Prix)
                .HasPrecision(10, 2)
                .HasColumnName("prix");
            entity.Property(e => e.Qte).HasColumnName("qte");

            entity.HasOne(d => d.CodearticleNavigation).WithMany(p => p.LstArticles)
                .HasForeignKey(d => d.Codearticle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_lot_article");

            entity.HasOne(d => d.CodefNavigation).WithMany(p => p.LstArticles)
                .HasForeignKey(d => d.Codef)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_lot_fournisseur");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
