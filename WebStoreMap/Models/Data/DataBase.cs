using System.Data.Entity;

namespace WebStoreMap.Models.Data
{
    public class DataBase : DbContext
    {
        public DbSet<PageDataTransferObject> Pages { get; set; }
        public DbSet<SidebarDataTransferObject> Sidebars { get; set; }
        public DbSet<CategoryDataTransferObject> Categories { get; set; }
        public DbSet<PlaceDataTransferObject> Places { get; set; }
        public DbSet<UserDataTransferObject> Users { get; set; }
        public DbSet<RoleDataTransferObject> Roles { get; set; }
        public DbSet<UserRoleDataTransferObject> UserRoles { get; set; }
        public DbSet<RegionDataTransferObject> Regions { get; set; }
        public DbSet<CityDataTransferObject> Cities { get; set; }
        public DbSet<CountryDataTransferObject> Countries { get; set; }

        public DbSet<RegionInformationDataTransferObject> RegionsInformation { get; set; }
        public DbSet<CityInformationDataTransferObject> CitiesInformation { get; set; }
        public DbSet<CountryInformationDataTransferObject> CountriesInformation { get; set; }

        public DbSet<RegistrationUserDataTransferObject> RegistrationUsers { get; set; }
        public DbSet<LinkDataTransferObject> Links { get; set; }
        public DbSet<CompanyDataTransferObject> Companies { get; set; }
        public DbSet<DesiredDataTransferObject> Desires { get; set; }
        public DbSet<FavoriteDataTransferObject> Favorites { get; set; }
        public DbSet<CommentDataTransferObject> Comments { get; set; }
        public DbSet<ReplyDataTransferObject> Replies { get; set; }
        public DbSet<RatingDataTransferObject> Ratings { get; set; }
        public DbSet<ServiceDataTransferObject> Services { get; set; }
    }
}