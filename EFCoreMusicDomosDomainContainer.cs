using Grammophone.DataAccess;
using Grammophone.Domos.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;
using Grammophone.Domos.Tests.Music.Domain;
using Microsoft.EntityFrameworkCore;

namespace Grammophone.Domos.Tests.Music.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// EF Core music Domos test container.
	/// </summary>
	public class EFCoreMusicDomosDomainContainer : EFCoreWorkflowUsersDomainContainer<MusicUser, AlbumStateTransition>
	{
		public EFCoreMusicDomosDomainContainer(DbContextOptions options)
			: base(options)
		{
		}

		public EFCoreMusicDomosDomainContainer(DbContextOptions options, TransactionMode transactionMode)
			: base(options, transactionMode)
		{
		}

		public DbSet<RecordLabel> RecordLabels { get; set; }

		public DbSet<RecordLabelAdministrator> RecordLabelAdministrators { get; set; }

		public DbSet<RecordLabelContributor> RecordLabelContributors { get; set; }

		public DbSet<Artist> Artists { get; set; }

		public DbSet<Album> Albums { get; set; }

		public DbSet<Track> Tracks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// The music tests do not require EF Core lazy-loading or change-tracking proxies.

			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseChangeTrackingProxies();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Disposition uses UserTrackingEntityWithID<User, long> so its navigation
			// properties (OwningUser, CreatorUser, LastModifierUser) are typed as User.
			// The base class (EFCoreUsersDomainContainer<U>.OnModelCreating) already maps
			// these via the string-based API using typeof(U) — but omits OnDelete for
			// CreatorUser and LastModifierUser.  EF Core defaults required FKs (long) to
			// Cascade, and SQL Server rejects three cascade paths to Users on Dispositions.
			// Override CreatorUser and LastModifierUser to NoAction to match EF6 behavior.
			modelBuilder.Entity<Disposition>()
				.HasOne(typeof(MusicUser), nameof(Disposition.CreatorUser))
				.WithMany()
				.HasForeignKey(nameof(Disposition.CreatorUserID))
				.OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Disposition>()
				.HasOne(typeof(MusicUser), nameof(Disposition.LastModifierUser))
				.WithMany()
				.HasForeignKey(nameof(Disposition.LastModifierUserID))
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<StateGroup>().HasOne(sg => sg.WorkflowGraph).WithMany(wg => wg.StateGroups).HasForeignKey(sg => sg.WorkflowGraphID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<State>().HasOne(s => s.Group).WithMany(sg => sg.States).HasForeignKey(s => s.GroupID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<StatePath>().HasOne(sp => sp.WorkflowGraph).WithMany().HasForeignKey(sp => sp.WorkflowGraphID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<StatePath>().HasOne(sp => sp.PreviousState).WithMany().HasForeignKey(sp => sp.PreviousStateID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<StatePath>().HasOne(sp => sp.NextState).WithMany().HasForeignKey(sp => sp.NextStateID).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<RecordLabel>().Property(l => l.Name).IsRequired().HasMaxLength(200);
			modelBuilder.Entity<RecordLabel>().HasIndex(l => l.Name).IsUnique();
			modelBuilder.Entity<RecordLabel>().HasOne(l => l.CreatorUser).WithMany().HasForeignKey(l => l.CreatorUserID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<RecordLabel>().HasOne(l => l.LastModifierUser).WithMany().HasForeignKey(l => l.LastModifierUserID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<RecordLabel>().HasOne(l => l.OwningUser).WithMany().HasForeignKey(l => l.OwningUserID).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<RecordLabelDisposition>()
				.HasOne(d => d.RecordLabel)
				.WithMany()
				.HasForeignKey(d => d.RecordLabelID)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Artist>().Property(a => a.Name).IsRequired().HasMaxLength(200);
			modelBuilder.Entity<Artist>().HasOne(a => a.RecordLabel).WithMany().HasForeignKey(a => a.RecordLabelID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Artist>().HasIndex(a => new { a.RecordLabelID, a.Name }).IsUnique();

			modelBuilder.Entity<Album>().Property(a => a.Title).IsRequired().HasMaxLength(200);
			modelBuilder.Entity<Album>().HasOne(a => a.RecordLabel).WithMany().HasForeignKey(a => a.RecordLabelID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Album>().HasOne(a => a.Artist).WithMany().HasForeignKey(a => a.ArtistID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Album>().HasOne(a => a.Owner).WithMany().HasForeignKey(a => a.OwnerID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Album>().HasOne(a => a.State).WithMany().HasForeignKey(a => a.StateID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Album>().HasOne(a => a.CreatorUser).WithMany().HasForeignKey(a => a.CreatorUserID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Album>().HasOne(a => a.LastModifierUser).WithMany().HasForeignKey(a => a.LastModifierUserID).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Track>().Property(t => t.Title).IsRequired().HasMaxLength(200);
			modelBuilder.Entity<Track>().HasOne(t => t.RecordLabel).WithMany().HasForeignKey(t => t.RecordLabelID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Track>().HasOne(t => t.Album).WithMany(a => a.Tracks).HasForeignKey(t => t.AlbumID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Track>().HasOne(t => t.Owner).WithMany().HasForeignKey(t => t.OwnerID).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<AlbumStateTransition>().HasOne(st => st.Album).WithMany(a => a.StateTransitions).HasForeignKey(st => st.AlbumID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<AlbumStateTransition>().HasOne(st => st.CreatorUser).WithMany().HasForeignKey(st => st.CreatorUserID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<AlbumStateTransition>().HasOne(st => st.LastModifierUser).WithMany().HasForeignKey(st => st.LastModifierUserID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<AlbumStateTransition>().HasOne(st => st.Path).WithMany().HasForeignKey(st => st.PathID).OnDelete(DeleteBehavior.NoAction);
		}
	}
}
