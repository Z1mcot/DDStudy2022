﻿// <auto-generated />
using System;
using DDStudy2022.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DDStudy2022.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc.2.22472.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

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

            modelBuilder.Entity("DDStudy2022.DAL.Entities.CommentLike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("CommentLike", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsModified")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsShown")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("PublishDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsModified")
                        .HasColumnType("boolean");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("PublishDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("PostComments");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostLike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("PostLike", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Stories", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsShown")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("PublishDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NameTag")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("NameTag")
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

                    b.Property<string>("IPAddress")
                        .HasColumnType("text");

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

            modelBuilder.Entity("DDStudy2022.DAL.Entities.UserSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<Guid>("SubscriberId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("SubscriptionDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Avatar", b =>
                {
                    b.HasBaseType("DDStudy2022.DAL.Entities.Attachment");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.HasIndex("OwnerId")
                        .IsUnique();

                    b.ToTable("Avatars", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostAttachment", b =>
                {
                    b.HasBaseType("DDStudy2022.DAL.Entities.Attachment");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.ToTable("PostAttachment", (string)null);
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.StoriesAttachment", b =>
                {
                    b.HasBaseType("DDStudy2022.DAL.Entities.Attachment");

                    b.Property<Guid>("StoriesId")
                        .HasColumnType("uuid");

                    b.HasIndex("StoriesId")
                        .IsUnique();

                    b.ToTable("StoriesAttachment", (string)null);
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

            modelBuilder.Entity("DDStudy2022.DAL.Entities.CommentLike", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.PostComment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.User", "User")
                        .WithMany("LikedComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Post", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostComment", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.Post", null)
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostLike", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Post", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.User", "User")
                        .WithMany("LikedPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Stories", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "Author")
                        .WithMany("Stories")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.UserSession", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "User")
                        .WithMany("UserSessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.UserSubscription", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.User", "Author")
                        .WithMany("Subscribers")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.User", "Subscriber")
                        .WithMany("Subscriptions")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Subscriber");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Avatar", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("DDStudy2022.DAL.Entities.Avatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.User", "Owner")
                        .WithOne("Avatar")
                        .HasForeignKey("DDStudy2022.DAL.Entities.Avatar", "OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostAttachment", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("DDStudy2022.DAL.Entities.PostAttachment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.Post", "Post")
                        .WithMany("Content")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.StoriesAttachment", b =>
                {
                    b.HasOne("DDStudy2022.DAL.Entities.Attachment", null)
                        .WithOne()
                        .HasForeignKey("DDStudy2022.DAL.Entities.StoriesAttachment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DDStudy2022.DAL.Entities.Stories", "Stories")
                        .WithOne("Content")
                        .HasForeignKey("DDStudy2022.DAL.Entities.StoriesAttachment", "StoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stories");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Content");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.PostComment", b =>
                {
                    b.Navigation("Likes");
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.Stories", b =>
                {
                    b.Navigation("Content")
                        .IsRequired();
                });

            modelBuilder.Entity("DDStudy2022.DAL.Entities.User", b =>
                {
                    b.Navigation("Avatar");

                    b.Navigation("LikedComments");

                    b.Navigation("LikedPosts");

                    b.Navigation("Posts");

                    b.Navigation("Stories");

                    b.Navigation("Subscribers");

                    b.Navigation("Subscriptions");

                    b.Navigation("UserSessions");
                });
#pragma warning restore 612, 618
        }
    }
}
