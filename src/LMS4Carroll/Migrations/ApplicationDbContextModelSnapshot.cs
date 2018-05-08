using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LMS4Carroll.Data;

namespace LMS4Carroll.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LMS4Carroll.Models.Animal", b =>
                {
                    b.Property<int>("AnimalID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CAT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("DOB");

                    b.Property<DateTime>("DOR");

					b.Property<int>("CageID");

                    b.Property<string>("Gender")
                        .HasAnnotation("MaxLength", 50);

					b.Property<string>("Weight")
						.HasAnnotation("MaxLength", 50);

                    b.Property<string>("LOT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("LocationID");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("NormalizedLocation")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("OrderID");

                    b.Property<string>("Species")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("AnimalID");

                    b.HasIndex("LocationID");

                    b.HasIndex("OrderID");

					b.HasIndex("CageID");

                    b.ToTable("Animal");
                });

            modelBuilder.Entity("LMS4Carroll.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("IPAddress");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("LMS4Carroll.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("CarrollYear");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("RoleName");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("LMS4Carroll.Models.BioEquipment", b =>
                {
                    b.Property<int>("BioEquipmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CAT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("EquipmentModel")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("EquipmentName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("InspectionDate");

                    b.Property<DateTime>("InstalledDate");

                    b.Property<string>("LOT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("LocationID");

                    b.Property<int?>("OrderID");

                    b.Property<string>("SerialNumber")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Type")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Comments")
                        .HasAnnotation("MaxLength", 150);

                    b.HasKey("BioEquipmentID");

                    b.HasIndex("LocationID");

                    b.HasIndex("OrderID");

                    b.ToTable("BioEquipments");
                });

            modelBuilder.Entity("LMS4Carroll.Models.PhyEquipment", b =>
            {
                b.Property<int>("PhyEquipmentID")
                    .ValueGeneratedOnAdd();

                b.Property<string>("CAT")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("EquipmentModel")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("EquipmentName")
                    .HasAnnotation("MaxLength", 50);

                b.Property<DateTime>("InspectionDate");

                b.Property<DateTime>("InstalledDate");

                b.Property<string>("LOT")
                    .HasAnnotation("MaxLength", 50);

                b.Property<int?>("LocationID");

                b.Property<int?>("OrderID");

                b.Property<string>("SerialNumber")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Type")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Comments")
                    .HasAnnotation("MaxLength", 150);

                b.HasKey("PhyEquipmentID");

                b.HasIndex("LocationID");

                b.HasIndex("OrderID");

                b.ToTable("PhyEquipments");
            });

            modelBuilder.Entity("LMS4Carroll.Models.Cage", b =>
            {
              b.Property<int>("CageID")
                .ValueGeneratedOnAdd();

              b.Property<string>("CageDesignation")
                .HasAnnotation("MaxLength", 50);

              b.Property<int?>("LocationID");

              b.Property<string>("Species")
                .HasAnnotation("MaxLength", 50);

              b.Property<string>("NormalizedLocation")
                .HasAnnotation("MaxLength", 50);

              b.HasKey("CageID");

              b.ToTable("Cage");
            });

            modelBuilder.Entity("LMS4Carroll.Models.PhyArchive", b =>
            {
                b.Property<int>("PhyArchiveID")
                    .ValueGeneratedOnAdd();

                b.Property<string>("EquipmentModel")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("EquipmentName")
                    .HasAnnotation("MaxLength", 50);

                b.Property<DateTime>("ArchiveDate");

                b.Property<DateTime>("InstalledDate");

                b.Property<int?>("OrderID");

                b.Property<string>("SerialNumber")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Type")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Comments")
                    .HasAnnotation("MaxLength", 150);

                b.HasKey("PhyArchiveID");

                b.HasIndex("OrderID");

                b.ToTable("PhyArchives");
            });

            modelBuilder.Entity("LMS4Carroll.Models.BioArchive", b =>
            {
                b.Property<int>("BioArchiveID")
                    .ValueGeneratedOnAdd();

                b.Property<string>("EquipmentModel")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("EquipmentName")
                    .HasAnnotation("MaxLength", 50);

                b.Property<DateTime>("ArchiveDate");

                b.Property<DateTime>("InstalledDate");

                b.Property<int?>("OrderID");

                b.Property<string>("SerialNumber")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Type")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Comments")
                    .HasAnnotation("MaxLength", 150);

                b.HasKey("BioArchiveID");

                b.HasIndex("OrderID");

                b.ToTable("BioArchives");
            });

            modelBuilder.Entity("LMS4Carroll.Models.ChemArchive", b =>
            {
                b.Property<int>("ChemArchiveID")
                    .ValueGeneratedOnAdd();

                b.Property<string>("EquipmentModel")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("EquipmentName")
                    .HasAnnotation("MaxLength", 50);

                b.Property<DateTime>("ArchiveDate");

                b.Property<DateTime>("InstalledDate");

                b.Property<int?>("OrderID");

                b.Property<string>("SerialNumber")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Type")
                    .HasAnnotation("MaxLength", 50);

                b.Property<string>("Comments")
                    .HasAnnotation("MaxLength", 150);

                b.HasKey("ChemArchiveID");

                b.HasIndex("OrderID");

                b.ToTable("ChemArchives");
            });

            modelBuilder.Entity("LMS4Carroll.Models.CageLog", b =>
            {
                b.Property<int>("CageLogId")
                    .ValueGeneratedOnAdd();

                b.Property<int>("CageID");

                b.Property<bool>("Clean");

                b.Property<DateTime>("DatetimeCreated")
                    .ValueGeneratedOnAddOrUpdate();

                b.Property<bool>("Food");

                b.Property<string>("FoodComments")
                    .HasAnnotation("MaxLength", 150);

                b.Property<bool>("Social");

                b.Property<string>("SocialComments")
                    .HasAnnotation("MaxLength", 150);

                b.Property<string>("WashComments")
                    .HasAnnotation("MaxLength", 150);

                b.Property<bool>("Washed");

                b.HasKey("CageLogId");

                b.HasIndex("CageID");

                b.ToTable("CageLog");
            });

            modelBuilder.Entity("LMS4Carroll.Models.ChemEquipment", b =>
                {
                    b.Property<int>("ChemEquipmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CAT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("EquipmentModel")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("EquipmentName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("InspectionDate");

                    b.Property<DateTime>("InstalledDate");

                    b.Property<string>("LOT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("LocationID");

                    b.Property<int?>("OrderID");

                    b.Property<string>("SerialNumber")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Type")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Comments")
                        .HasAnnotation("MaxLength", 150);

                    b.HasKey("ChemEquipmentID");

                    b.HasIndex("LocationID");

                    b.HasIndex("OrderID");

                    b.ToTable("ChemicalEquipments");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Chemical", b =>
                {
                    b.Property<int?>("ChemID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CAS")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("CAT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Formula")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("FormulaName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("FormulaWeight")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Hazard")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("SDS");

                    b.Property<string>("State")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("ChemID");

                    b.ToTable("Chemical");
                });

            modelBuilder.Entity("LMS4Carroll.Models.ChemInventory", b =>
                {
                    b.Property<int?>("ChemInventoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CAT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("ChemID");

                    b.Property<string>("Department")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("ExpiryDate");

                    b.Property<string>("LOT")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("LocationID");

                    b.Property<string>("NormalizedLocation")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("OrderID");

                    b.Property<float>("QtyLeft");

                    b.Property<string>("Units")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("ChemInventoryId");

                    b.HasIndex("ChemID");

                    b.HasIndex("LocationID");

                    b.HasIndex("OrderID");

                    b.ToTable("ChemInventory");
                });

            modelBuilder.Entity("LMS4Carroll.Models.ChemLog", b =>
                {
                    b.Property<int>("ChemLogId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChemInventoryId");

                    b.Property<int>("CourseID");

                    b.Property<DateTime>("DatetimeCreated")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<float>("QtyUsed");

                    b.HasKey("ChemLogId");

                    b.HasIndex("ChemInventoryId");

                    b.HasIndex("CourseID");

                    b.ToTable("ChemLog");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Course", b =>
                {
                    b.Property<int>("CourseID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Handler")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("LocationID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("NormalizedLocation")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("NormalizedStr")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("CourseID");

                    b.HasIndex("LocationID");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("LMS4Carroll.Models.FileDetail", b =>
                {
                    b.Property<int>("FileDetailID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<DateTime>("DatetimeCreated")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<byte[]>("File")
                        .IsRequired();

                    b.Property<string>("FileName")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<string>("FileType")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<int?>("OrderID");

                    b.HasKey("FileDetailID");

                    b.HasIndex("OrderID");

                    b.ToTable("FileDetails");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Location", b =>
                {
                    b.Property<int?>("LocationID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("NormalizedStr")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Room")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("StorageCode")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("LocationID");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Order", b =>
                {
                    b.Property<int?>("OrderID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Invoice")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("Orderdate");

                    b.Property<string>("PO")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("Recievedate");

                    b.Property<string>("Status")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("VendorID");

                    b.HasKey("OrderID");

                    b.HasIndex("VendorID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Vendor", b =>
                {
                    b.Property<int>("VendorID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("VendorID");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Animal", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Location", "Location")
                        .WithMany("Animals")
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS4Carroll.Models.Order", "Order")
                        .WithMany("Animals")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS4Carroll.Models.BioEquipment", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Location", "Location")
                        .WithMany("BioEquipments")
                        .HasForeignKey("LocationID");

                    b.HasOne("LMS4Carroll.Models.Order", "Order")
                        .WithMany("BioEquipments")
                        .HasForeignKey("OrderID");
                });

            modelBuilder.Entity("LMS4Carroll.Models.CageLog", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Animal", "Animal")
                        .WithMany("CageLogs")
                        .HasForeignKey("AnimalID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS4Carroll.Models.ChemEquipment", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Location", "Location")
                        .WithMany("ChemEquipments")
                        .HasForeignKey("LocationID");

                    b.HasOne("LMS4Carroll.Models.Order", "Order")
                        .WithMany("ChemEquipments")
                        .HasForeignKey("OrderID");
                });

            modelBuilder.Entity("LMS4Carroll.Models.ChemInventory", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Chemical", "Chemical")
                        .WithMany("ChemInventories")
                        .HasForeignKey("ChemID");

                    b.HasOne("LMS4Carroll.Models.Location", "Location")
                        .WithMany("ChemInventories")
                        .HasForeignKey("LocationID");

                    b.HasOne("LMS4Carroll.Models.Order", "Order")
                        .WithMany("ChemInventorys")
                        .HasForeignKey("OrderID");
                });

            modelBuilder.Entity("LMS4Carroll.Models.ChemLog", b =>
                {
                    b.HasOne("LMS4Carroll.Models.ChemInventory", "ChemInventory")
                        .WithMany("ChemLogs")
                        .HasForeignKey("ChemInventoryId");

                    b.HasOne("LMS4Carroll.Models.Course", "Course")
                        .WithMany("ChemLogs")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS4Carroll.Models.Course", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Location", "Location")
                        .WithMany("Courses")
                        .HasForeignKey("LocationID");
                });

            modelBuilder.Entity("LMS4Carroll.Models.FileDetail", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Order", "Order")
                        .WithMany("FileDetails")
                        .HasForeignKey("OrderID");
                });

            modelBuilder.Entity("LMS4Carroll.Models.Order", b =>
                {
                    b.HasOne("LMS4Carroll.Models.Vendor", "Vendor")
                        .WithMany("Order")
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("LMS4Carroll.Models.ApplicationRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("LMS4Carroll.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("LMS4Carroll.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("LMS4Carroll.Models.ApplicationRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS4Carroll.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
