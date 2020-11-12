﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using ReconNess.Core;
using ReconNess.Data.Npgsql.Seeding;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Data.Npgsql
{
    /// <summary>
    /// This class implement the interface <see cref="IDbContext"/>
    /// </summary>
    public class ReconNessContext : DbContext, IDbContext
    {
        public DbSet<Agent> Agents { get; set; }
        public DbSet<AgentRun> AgentRuns { get; set; }
        public DbSet<AgentTrigger> AgentTriggers { get; set; }
        public DbSet<AgentHistory> AgentHistories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<RootDomain> RootDomains { get; set; }
        public DbSet<Subdomain> Subdomains { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// A transaction Object
        /// </summary>
        private IDbContextTransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReconNessContext" /> class
        /// </summary>
        /// <param name="options">DB Context options</param>
        public ReconNessContext(DbContextOptions options)
        : base(options)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agent>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgentRun>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgentRun>()
                .HasOne(t => t.Agent)
                .WithMany(r => r.AgentRuns)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgentHistory>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgentHistory>()
                .HasOne(t => t.Agent)
                .WithMany(r => r.AgentHistories)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgentTrigger>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgentTrigger>()
                .HasOne(t => t.Agent)
                .WithOne(r => r.AgentTrigger)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reference>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Target>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RootDomain>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RootDomain>()
                .HasOne(t => t.Target)
                .WithMany(r => r.RootDomains)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subdomain>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Subdomain>()
                .HasOne(t => t.RootDomain)
                .WithMany(r => r.Subdomains)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subdomain>()
                .HasOne(t => t.Notes)
                .WithOne(r => r.Subdomain)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RootDomain>()
                .HasOne(t => t.Notes)
                .WithOne(r => r.RootDomain)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Directory>()
                .HasOne(t => t.Subdomain)
                .WithMany(r => r.Directories)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Service>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Service>()
                .HasOne(t => t.Subdomain)
                .WithMany(r => r.Services)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Directory>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Label>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Note>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Notification>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgentCategory>()
                .HasKey(t => new { t.AgentId, t.CategoryId });

            modelBuilder.Entity<AgentCategory>()
                .HasOne(pt => pt.Agent)
                .WithMany(p => p.AgentCategories)
                .HasForeignKey(pt => pt.AgentId);

            modelBuilder.Entity<AgentCategory>()
                .HasOne(pt => pt.Category)
                .WithMany(t => t.AgentCategories)
                .HasForeignKey(pt => pt.CategoryId);

            modelBuilder.Entity<SubdomainLabel>()
                .HasKey(t => new { t.SubdomainId, t.LabelId });

            modelBuilder.Entity<SubdomainLabel>()
                .HasOne(pt => pt.Subdomain)
                .WithMany(p => p.Labels)
                .HasForeignKey(pt => pt.SubdomainId);

            modelBuilder.Entity<SubdomainLabel>()
                .HasOne(pt => pt.Label)
                .WithMany(t => t.Subdomains)
                .HasForeignKey(pt => pt.LabelId);

            modelBuilder.Ignore<BaseEntity>();

            this.RunSeed(modelBuilder);
        }

        /// <summary>
        /// Run seeding
        /// </summary>
        private void RunSeed(ModelBuilder modelBuilder)
        {
            LabelSeeding.Run(modelBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changes = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var item in changes)
            {
                item.Property(p => p.UpdatedAt).CurrentValue = DateTime.Now;

                if (item.State == EntityState.Added)
                {
                    item.Property(p => p.CreatedAt).CurrentValue = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        public override int SaveChanges()
        {
            var changes = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var item in changes)
            {
                item.Property(p => p.UpdatedAt).CurrentValue = DateTime.Now;
                if (item.State == EntityState.Added)
                {
                    item.Property(p => p.CreatedAt).CurrentValue = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

        #region IDbContext

        /// <summary>
        /// <see cref="IDbContext.BeginTransaction(CancellationToken)"/>
        /// </summary>
        public void BeginTransaction(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (transaction != null)
            {
                var dbTransaction = transaction.GetDbTransaction();
                try
                {
                    if (dbTransaction != null && dbTransaction?.Connection != null && dbTransaction?.Connection?.State == System.Data.ConnectionState.Open)
                    {
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }

            this.transaction = this.Database.BeginTransaction();
        }

        /// <summary>
        /// <see cref="IDbContext.Commit(CancellationToken)"></see>
        /// </summary>
        public int Commit(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.BeginTransaction(cancellationToken);
                var saveChanges = this.SaveChanges();
                this.EndTransaction(cancellationToken);

                return saveChanges;
            }
            catch (Exception)
            {
                this.Rollback(cancellationToken);
                throw;
            }
            finally
            {
                // base.Dispose();
            }
        }

        /// <summary>
        /// <see cref="IDbContext.CommitAsync(CancellationToken)"></see>
        /// </summary>
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.BeginTransaction(cancellationToken);
                var saveChangesAsync = await this.SaveChangesAsync();
                this.EndTransaction(cancellationToken);

                return saveChangesAsync;
            }
            catch (Exception)
            {
                this.Rollback(cancellationToken);
                throw;
            }
            finally
            {
                // base.Dispose();
            }
        }

        /// <summary>
        /// <see cref="IDbContext.Rollback(CancellationToken)"></see>
        /// </summary>
        public void Rollback(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (this.transaction != null && this.transaction.GetDbTransaction().Connection != null)
            {
                this.transaction.Rollback();
            }
        }

        /// <summary>
        /// Finish the transaction
        /// </summary>
        private void EndTransaction(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.transaction.Commit();
        }

        /// <summary>
        /// <see cref="IDbContext.SetAsAdded{TEntity}(TEntity, CancellationToken)"></see>
        /// </summary>
        public void SetAsAdded<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateEntityState<TEntity>(entity, EntityState.Added, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.SetAsAdded{TEntity}(List{TEntity}, CancellationToken)"></see>
        /// </summary>
        public void SetAsAdded<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.SetAsAdded<TEntity>(entities, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.SetAsModified{TEntity}(TEntity, CancellationToken)"></see>
        /// </summary>
        public void SetAsModified<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateEntityState<TEntity>(entity, EntityState.Modified, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.SetAsModified{TEntity}(List{TEntity}, CancellationToken)"></see>
        /// </summary>
        public void SetAsModified<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.SetAsModified<TEntity>(entities, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.SetAsDeleted{TEntity}(TEntity, CancellationToken)"></see>
        /// </summary>
        public void SetAsDeleted<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateEntityState<TEntity>(entity, EntityState.Deleted, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.SetAsDeleted{TEntity}(List{TEntity}, CancellationToken)"></see>
        /// </summary>
        public void SetAsDeleted<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.SetAsDeleted<TEntity>(entities, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.FindAsync{TEntity}(Guid, CancellationToken)"></see>
        /// </summary>
        public Task<TEntity> FindAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().FindAsync(id).AsTask();
        }

        /// <summary>
        /// <see cref="IDbContext.FindByCriteriaAsync{TEntity}(Expression{Func{TEntity, bool}}, CancellationToken)"></see>
        /// </summary>
        public Task<TEntity> FindByCriteriaAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().Local.AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken) ?? this.FirstOrDefaultAsync<TEntity>(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.FirstOrDefaultAsync{TEntity}(Expression{Func{TEntity, bool}}, CancellationToken)"></see>
        /// </summary>
        public Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.ToListAsync{TEntity}(CancellationToken)"></see>
        /// </summary>
        public Task<List<TEntity>> ToListAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.ToListByCriteriaAsync{TEntity}(Expression{Func{TEntity, bool}}, CancellationToken)"></see>
        /// </summary>
        public Task<List<TEntity>> ToListByCriteriaAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().Where(predicate).ToListAsync<TEntity>(cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.AnyAsync{TEntity}(Expression{Func{TEntity, bool}}, CancellationToken)"></see>
        /// </summary>
        public Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IDbContext.ToQueryable{TEntity}(CancellationToken)"></see>
        /// </summary>
        public IQueryable<TEntity> ToQueryable<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().AsQueryable();
        }

        /// <summary>
        /// <see cref="IDbContext.ToQueryableByCriteria{TEntity}(Expression{Func{TEntity, bool}}, CancellationToken)"></see>
        /// </summary>
        public IQueryable<TEntity> ToQueryableByCriteria<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().Where(predicate).AsQueryable();
        }

        /// <summary>
        /// Update entity state
        /// </summary>
        private void UpdateEntityState<TEntity>(TEntity entity, EntityState entityState, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entityEntry = this.GetDbEntityEntrySafely<TEntity>(entity, cancellationToken);
            if (entityEntry.State == EntityState.Unchanged)
            {
                entityEntry.State = entityState;
            }
        }

        /// <summary>
        /// Attach entity
        /// </summary>
        private EntityEntry GetDbEntityEntrySafely<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entityEntry = Entry<TEntity>(entity);
            if (entityEntry.State == EntityState.Detached)
            {
                this.Set<TEntity>().Attach(entity);
            }

            return entityEntry;
        }
        #endregion IDbContext
    }
}
