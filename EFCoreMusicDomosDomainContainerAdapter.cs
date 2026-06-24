using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Tests.Music.DataAccess;
using Grammophone.Domos.Tests.Music.Domain;

namespace Grammophone.Domos.Tests.Music.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// EF Core adapter for the music Domos test container.
	/// </summary>
	public class EFCoreMusicDomosDomainContainerAdapter : EFCoreWorkflowUsersDomainContainerAdapter<MusicUser, AlbumStateTransition, EFCoreMusicDomosDomainContainer>, IMusicDomosDomainContainer
	{
		private IEntitySet<RecordLabel> recordLabels;
		private IEntitySet<RecordLabelAdministrator> recordLabelAdministrators;
		private IEntitySet<RecordLabelContributor> recordLabelContributors;
		private IEntitySet<Artist> artists;
		private IEntitySet<Album> albums;
		private IEntitySet<Track> tracks;

		public EFCoreMusicDomosDomainContainerAdapter(EFCoreMusicDomosDomainContainer innerContainer)
			: base(innerContainer)
		{
		}

		public IEntitySet<RecordLabel> RecordLabels => recordLabels ??= new EFCoreSet<RecordLabel>(this.InnerDomainContainer.RecordLabels, this);

		public IEntitySet<RecordLabelAdministrator> RecordLabelAdministrators => recordLabelAdministrators ??= new EFCoreSet<RecordLabelAdministrator>(this.InnerDomainContainer.RecordLabelAdministrators, this);

		public IEntitySet<RecordLabelContributor> RecordLabelContributors => recordLabelContributors ??= new EFCoreSet<RecordLabelContributor>(this.InnerDomainContainer.RecordLabelContributors, this);

		public IEntitySet<Artist> Artists => artists ??= new EFCoreSet<Artist>(this.InnerDomainContainer.Artists, this);

		public IEntitySet<Album> Albums => albums ??= new EFCoreSet<Album>(this.InnerDomainContainer.Albums, this);

		public IEntitySet<Track> Tracks => tracks ??= new EFCoreSet<Track>(this.InnerDomainContainer.Tracks, this);
	}
}
