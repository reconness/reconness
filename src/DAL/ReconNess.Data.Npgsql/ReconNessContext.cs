using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
    public class ReconNessContext : IdentityDbContext<User, Role, Guid>, IDbContext
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

            modelBuilder.Entity<AgentHistory>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AgentTrigger>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

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

            modelBuilder.Entity<Subdomain>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Service>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

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

            modelBuilder.Entity<AgentRun>()
                .HasOne(t => t.Agent)
                .WithMany(r => r.AgentRuns)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgentHistory>()
               .HasOne(t => t.Agent)
               .WithMany(r => r.AgentHistories)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AgentTrigger>()
               .HasOne(t => t.Agent)
               .WithOne(r => r.AgentTrigger)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RootDomain>()
                .HasOne(t => t.Target)
                .WithMany(r => r.RootDomains)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

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
               .HasOne(t => t.Subdomain)
               .WithMany(r => r.Services)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Ignore<BaseEntity>();

            RunSeed(modelBuilder);
        }

        /// <summary>
        /// Run seeding
        /// </summary>
        private static void RunSeed(ModelBuilder modelBuilder)
        {
            LabelSeeding.Run(modelBuilder);
            IdentitySeeding.Run(modelBuilder);
        }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changes = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var item in changes)
            {
                item.Property(p => p.UpdatedAt).CurrentValue = DateTime.UtcNow;

                if (item.State == EntityState.Added)
                {
                    item.Property(p => p.CreatedAt).CurrentValue = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override int SaveChanges()
        {
            var changes = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var item in changes)
            {
                item.Property(p => p.UpdatedAt).CurrentValue = DateTime.UtcNow;
                if (item.State == EntityState.Added)
                {
                    item.Property(p => p.CreatedAt).CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }

        #region IDbContext

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.BeginTransaction(cancellationToken);
                var saveChangesAsync = await this.SaveChangesAsync(cancellationToken);
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

        /// <inheritdoc/>
        public void Rollback(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (this.transaction != null && this.transaction.GetDbTransaction().Connection != null)
            {
                this.transaction.Rollback();
            }
        }

        /// <inheritdoc/>
        private void EndTransaction(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.transaction.Commit();
        }

        /// <inheritdoc/>
        public void SetAsAdded<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateEntityState<TEntity>(entity, EntityState.Added, cancellationToken);
        }

        /// <inheritdoc/>
        public void SetAsAdded<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            entities.ForEach(entity => this.SetAsAdded<TEntity>(entity, cancellationToken));
        }

        /// <inheritdoc/>
        public void SetAsModified<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateEntityState<TEntity>(entity, EntityState.Modified, cancellationToken);
        }

        /// <inheritdoc/>
        public void SetAsModified<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            entities.ForEach(entity => this.SetAsModified<TEntity>(entity, cancellationToken));
        }

        /// <inheritdoc/>
        public void SetAsDeleted<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateEntityState<TEntity>(entity, EntityState.Deleted, cancellationToken);
        }

        /// <inheritdoc/>
        public void SetAsDeleted<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            entities.ForEach(entity => this.SetAsDeleted<TEntity>(entity, cancellationToken));
        }

        /// <inheritdoc/>
        public Task<TEntity> FindAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().FindAsync(id, cancellationToken).AsTask();
        }

        /// <inheritdoc/>
        public Task<TEntity> FindByCriteriaAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().Local.AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken) ?? this.FirstOrDefaultAsync<TEntity>(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> ToListAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> ToListByCriteriaAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().Where(predicate).ToListAsync<TEntity>(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> ToQueryable<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>().AsQueryable();
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> ToQueryableByCriteria<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
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
