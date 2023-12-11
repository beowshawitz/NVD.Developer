using Microsoft.EntityFrameworkCore;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Core.Data
{
    public class NvdDeveloperContext : DbContext
    {
        public NvdDeveloperContext(DbContextOptions<NvdDeveloperContext> options) : base(options)
        {
			this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Application> Applications { get; set; } = null!;
		public DbSet<ApplicationVersion> ApplicationVersions { get; set; } = null!;
		public DbSet<PackageManager> PackageManagers { get; set; } = null!;
		public DbSet<AppToPackManager> AppsToPackageManagers { get; set; } = null!;
		public DbSet<ApplicationListItem> ApplicationLists { get; set; } = null!;
		public DbSet<ApplicationRequestStatus> ApplicationRequestStatus { get; set; } = null!;
		public DbSet<ApplicationRequest> ApplicationRequests { get; set; } = null!;
		public DbSet<ApplicationReportStatus> ApplicationReportStatus { get; set; } = null!;
		public DbSet<ApplicationReport> ApplicationReports { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasDefaultSchema("nvddev");

			modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");  
			
		}
	}
}
