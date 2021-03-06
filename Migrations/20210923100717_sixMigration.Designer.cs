// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrueKeyServer.DB;

namespace TrueKeyServer.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20210923100717_sixMigration")]
    partial class sixMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("TrueKeyServer.DB.Models.Comment", b =>
                {
                    b.Property<Guid>("InnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CommentId")
                        .HasColumnType("text");

                    b.Property<string>("DateCreate")
                        .HasColumnType("text");

                    b.Property<string>("ImageSource")
                        .HasColumnType("text");

                    b.Property<long>("LastModified")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TaskId")
                        .HasColumnType("text");

                    b.Property<string>("UserUUID")
                        .HasColumnType("text");

                    b.Property<string>("WhoModified")
                        .HasColumnType("text");

                    b.HasKey("InnerId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("TrueKeyServer.DB.Models.FRData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AdressPlaceOfS")
                        .HasColumnType("text");

                    b.Property<string>("AmountFnRereg")
                        .HasColumnType("text");

                    b.Property<string>("CheckResource")
                        .HasColumnType("text");

                    b.Property<string>("DateFirstUnDoc")
                        .HasColumnType("text");

                    b.Property<string>("EndDataFN")
                        .HasColumnType("text");

                    b.Property<string>("FFD")
                        .HasColumnType("text");

                    b.Property<string>("FirmwareID")
                        .HasColumnType("text");

                    b.Property<string>("FnExpireDays")
                        .HasColumnType("text");

                    b.Property<string>("Inn")
                        .HasColumnType("text");

                    b.Property<string>("LastModifiedDate")
                        .HasColumnType("text");

                    b.Property<string>("LastRegDateFN")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("NDS")
                        .HasColumnType("text");

                    b.Property<string>("NomberFN")
                        .HasColumnType("text");

                    b.Property<string>("NumFirstUnDoc")
                        .HasColumnType("text");

                    b.Property<string>("OperatorFD")
                        .HasColumnType("text");

                    b.Property<string>("Owner")
                        .HasColumnType("text");

                    b.Property<string>("QueueDocOFD")
                        .HasColumnType("text");

                    b.Property<string>("RegFnCount")
                        .HasColumnType("text");

                    b.Property<string>("RegisterNumber")
                        .HasColumnType("text");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("text");

                    b.Property<string>("StateInfoEx")
                        .HasColumnType("text");

                    b.Property<string>("VersBoot")
                        .HasColumnType("text");

                    b.Property<string>("VersConfigur")
                        .HasColumnType("text");

                    b.Property<string>("VersionFn")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FRData");
                });

            modelBuilder.Entity("TrueKeyServer.DB.Models.Key", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("login")
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Key");
                });

            modelBuilder.Entity("TrueKeyServer.DB.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("dateCreate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("imageSource")
                        .HasColumnType("text");

                    b.Property<string>("linkToObj")
                        .HasColumnType("text");

                    b.Property<string>("msg")
                        .HasColumnType("text");

                    b.Property<string>("timeChange")
                        .HasColumnType("text");

                    b.Property<string>("title")
                        .HasColumnType("text");

                    b.Property<string>("type")
                        .HasColumnType("text");

                    b.Property<string>("usersUuids")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("TrueKeyServer.DB.Models.Organisation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("OrgId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("TrueKeyServer.DB.Models.Task", b =>
                {
                    b.Property<Guid>("InnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("DateCreate")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ImageSource")
                        .HasColumnType("text");

                    b.Property<long>("LastModified")
                        .HasColumnType("bigint");

                    b.Property<string>("Number")
                        .HasColumnType("text");

                    b.Property<string>("OrgUUID")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("Subscribers")
                        .HasColumnType("text");

                    b.Property<string>("TaskId")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("WhoModified")
                        .HasColumnType("text");

                    b.HasKey("InnerId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("TrueKeyServer.DB.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AuthKey")
                        .HasColumnType("text");

                    b.Property<bool?>("GetEmailMessage")
                        .HasColumnType("boolean");

                    b.Property<bool?>("GetSubMessage")
                        .HasColumnType("boolean");

                    b.Property<string>("LoginMp")
                        .HasColumnType("text");

                    b.Property<string>("LoginSd")
                        .HasColumnType("text");

                    b.Property<string>("MobileIds")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PasswordMp")
                        .HasColumnType("text");

                    b.Property<string>("PasswordSd")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("UUID")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
