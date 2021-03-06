﻿using System.Data.Entity;
using System.Web.Mvc;
using Antix.Data.Keywords.EF;

namespace Example.MvcApplication.Data
{
    public class DataContext : DbContext
    {
        private readonly EFKeywordsManager _keywordsManager;

        public DataContext(EFKeywordsManager keywordsManager)
        {
            _keywordsManager = keywordsManager
                               ?? DependencyResolver.Current
                                   .GetService<EFKeywordsManager>();
        }

        // Required by migrations
        public DataContext() : this(null)
        {
        }

        public IDbSet<BlogEntryData> BlogEntries
        {
            get { return Set<BlogEntryData>(); }
        }

        public IDbSet<BlogTagData> BlogTags
        {
            get { return Set<BlogTagData>(); }
        }

        public IDbSet<UserData> Users
        {
            get { return Set<UserData>(); }
        }

        public IDbSet<UserSessionData> UserSessions
        {
            get { return Set<UserSessionData>(); }
        }

        public override int SaveChanges()
        {
            _keywordsManager.UpdateKeywordsAsync(this).Wait();

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogEntryData>()
                .HasMany(e => e.Tags)
                .WithMany();

            modelBuilder.Entity<BlogEntryData>()
                .HasMany(e => e.Redirects)
                .WithRequired();

            modelBuilder.Entity<BlogEntryRedirectData>()
                .HasKey(e => new
                {
                    e.Identifier,
                    e.BlogEntryId
                });

            modelBuilder.Entity<UserData>()
                .HasKey(e => e.Email);

            modelBuilder.Entity<UserSessionData>()
                .HasRequired(e => e.User)
                .WithMany();
        }
    }
}