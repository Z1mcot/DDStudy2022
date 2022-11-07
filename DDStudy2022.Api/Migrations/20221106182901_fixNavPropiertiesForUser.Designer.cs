﻿// <auto-generated />
using System;
using DDStudy2022.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DDStudy2022.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221106182901_fixNavPropiertiesForUser")]
    partial class fixNavPropiertiesForUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc.2.22472.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Attachment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Attachments");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Post", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostComment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("PostId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("PostComment", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long?>("AvatarId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.UserSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("RefreshToken")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Avatar", b =>
                {
                    b.HasBaseType("DDStudy2022.DAL.Entities.Attachment");

                    b.ToTable("Avatars", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostImage", b =>
                {
                    b.HasBaseType("DDStudy2022.DAL.Entities.Attachment");

                    b.Property<long>("PostId")
                        .HasColumnType("bigint");

                    b.HasIndex("PostId");

                    b.ToTable("PostImage", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Attachment", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Post", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", null)
                        .WithMany("Posts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostComment", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.User", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Avatar", "Avatar")
                        .WithOne("User")
                        .HasForeignKey("DDStudy2022.DAL.Entities.User", "AvatarId");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.UserSession", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Avatar", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("DDStudy2022.DAL.Entities.Avatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostImage", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("DDStudy2022.DAL.Entities.PostImage", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.Post", "Post")
                        .WithMany("Content")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Content");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.User", b =>
                {
                    b.Navigation("Posts");

                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Avatar", b =>
                {
                    b.Navigation("User")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
