using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;
using Grammophone.Domos.Tests.Music.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Grammophone.Domos.Tests.Music.DataAccess.EntityFrameworkCore
{
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
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseChangeTrackingProxies();

			optionsBuilder.UseFlexibleChangeTracking();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<StateGroup>().HasOne(sg => sg.WorkflowGraph).WithMany(wg => wg.StateGroups).HasForeignKey(sg => sg.WorkflowGraphID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<State>().HasOne(s => s.Group).WithMany(sg => sg.States).HasForeignKey(s => s.GroupID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<StatePath>().HasOne(sp => sp.WorkflowGraph).WithMany().HasForeignKey(sp => sp.WorkflowGraphID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<StatePath>().HasOne(sp => sp.PreviousState).WithMany().HasForeignKey(sp => sp.PreviousStateID).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<StatePath>().HasOne(sp => sp.NextState).WithMany().HasForeignKey(sp => sp.NextStateID).OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<RecordLabel>().Property(l => l.Name).IsRequired().HasMaxLength(200);
			modelBuilder.Entity<RecordLabel>().HasIndex(l => l.Name).IsUnique();
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
			modelBuilder.Entity<AlbumStateTransition>().HasOne(st => st.Path).WithMany().HasForeignKey(st => st.PathID).OnDelete(DeleteBehavior.NoAction);

			ConfigureAllTrackingEntitiesNavigations<MusicUser>(modelBuilder);
		}
	}
}
