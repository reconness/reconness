﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    [DbContext(typeof(ReconNessContext))]
    [Migration("20191116055512_UpdateAgentColumn")]
    partial class UpdateAgentColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ReconNess.Entities.Agent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Arguments")
                        .HasColumnType("text");

                    b.Property<string>("Command")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsBySubdomain")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastRun")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Script")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("ReconNess.Entities.AgentCategory", b =>
                {
                    b.Property<Guid>("AgentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("AgentId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("AgentCategory");
                });

            modelBuilder.Entity("ReconNess.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = new Guid("89217e19-681d-4c90-ac7a-612a735d9163"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Enum Subdomain Active",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("45a95787-5597-4769-a79a-083b43b38094"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Enum Subdomain Passive",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("d136e8b8-9d57-42f0-a3cb-5a2c91823256"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Enum Subdomain Brute Force",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("ReconNess.Entities.IpAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CIDR")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Ip")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("IpAddress");
                });

            modelBuilder.Entity("ReconNess.Entities.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Label");

                    b.HasData(
                        new
                        {
                            Id = new Guid("98ff4b31-b975-4138-ba66-52e339d7c30b"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Ignore",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("f4637445-1e93-4a46-914d-eaf337c8a176"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "ToCheck",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("76f06a11-ff60-431b-9888-2baa8af49a16"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Checking",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("45a518d6-78f2-47df-9837-1cde854fcde4"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Deleted = false,
                            Name = "Bounty",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("ReconNess.Entities.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.Property<Guid?>("SubdomainId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("SubdomainId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ReconNess.Entities.Subdomain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("FromAgents")
                        .HasColumnType("text");

                    b.Property<bool>("HasHttpOpen")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("IpAddressId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsMainPortal")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("TargetId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("IpAddressId");

                    b.HasIndex("TargetId");

                    b.ToTable("Subdomains");
                });

            modelBuilder.Entity("ReconNess.Entities.SubdomainLabel", b =>
                {
                    b.Property<Guid>("SubdomainId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("LabelId")
                        .HasColumnType("uuid");

                    b.HasKey("SubdomainId", "LabelId");

                    b.HasIndex("LabelId");

                    b.ToTable("SubdomainLabel");
                });

            modelBuilder.Entity("ReconNess.Entities.SubdomainNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<Guid?>("SubdomainId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("SubdomainId");

                    b.ToTable("SubdomainNote");
                });

            modelBuilder.Entity("ReconNess.Entities.Target", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BugBountyProgram")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("InScope")
                        .HasColumnType("text");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OutOfScope")
                        .HasColumnType("text");

                    b.Property<string>("RootDomain")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Targets");
                });

            modelBuilder.Entity("ReconNess.Entities.TargetNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<Guid?>("TargetId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("TargetId");

                    b.ToTable("TargetNote");
                });

            modelBuilder.Entity("ReconNess.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ReconNess.Entities.AgentCategory", b =>
                {
                    b.HasOne("ReconNess.Entities.Agent", "Agent")
                        .WithMany("AgentCategories")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReconNess.Entities.Category", "Category")
                        .WithMany("AgentCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReconNess.Entities.Service", b =>
                {
                    b.HasOne("ReconNess.Entities.Subdomain", "Subdomain")
                        .WithMany("Services")
                        .HasForeignKey("SubdomainId");
                });

            modelBuilder.Entity("ReconNess.Entities.Subdomain", b =>
                {
                    b.HasOne("ReconNess.Entities.IpAddress", "IpAddress")
                        .WithMany("SubDomains")
                        .HasForeignKey("IpAddressId");

                    b.HasOne("ReconNess.Entities.Target", "Target")
                        .WithMany("Subdomains")
                        .HasForeignKey("TargetId");
                });

            modelBuilder.Entity("ReconNess.Entities.SubdomainLabel", b =>
                {
                    b.HasOne("ReconNess.Entities.Label", "Label")
                        .WithMany("Subdomains")
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReconNess.Entities.Subdomain", "Subdomain")
                        .WithMany("Labels")
                        .HasForeignKey("SubdomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReconNess.Entities.SubdomainNote", b =>
                {
                    b.HasOne("ReconNess.Entities.Subdomain", "Subdomain")
                        .WithMany("Notes")
                        .HasForeignKey("SubdomainId");
                });

            modelBuilder.Entity("ReconNess.Entities.TargetNote", b =>
                {
                    b.HasOne("ReconNess.Entities.Target", "Target")
                        .WithMany("Notes")
                        .HasForeignKey("TargetId");
                });
#pragma warning restore 612, 618
        }
    }
}
