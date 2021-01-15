﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieStorm.Data;

namespace MovieStorm.Migrations
{
    [DbContext(typeof(StormContext))]
    partial class StormContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MovieStorm.Data.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("genre")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("varchar(1024)")
                        .HasMaxLength(1024);

                    b.Property<string>("path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("preview")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("released")
                        .HasColumnType("datetime");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.Property<int>("views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("user_id");

                    b.ToTable("Movie");
                });

            modelBuilder.Entity("MovieStorm.Data.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("movie_id")
                        .HasColumnType("int");

                    b.Property<int>("rating")
                        .HasColumnType("int");

                    b.Property<int>("user_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("movie_id");

                    b.HasIndex("user_id");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("MovieStorm.Data.Subtitle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<int>("movie_id")
                        .HasColumnType("int");

                    b.Property<string>("path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("movie_id");

                    b.ToTable("Subtitle");
                });

            modelBuilder.Entity("MovieStorm.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("admin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("logo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("token")
                        .HasColumnType("text");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("varchar(80)")
                        .HasMaxLength(80);

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("MovieStorm.Data.Movie", b =>
                {
                    b.HasOne("MovieStorm.Data.User", "User")
                        .WithMany("Movies")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieStorm.Data.Review", b =>
                {
                    b.HasOne("MovieStorm.Data.Movie", "Movie")
                        .WithMany("Reviews")
                        .HasForeignKey("movie_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MovieStorm.Data.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MovieStorm.Data.Subtitle", b =>
                {
                    b.HasOne("MovieStorm.Data.Movie", "Movie")
                        .WithMany("Subtitles")
                        .HasForeignKey("movie_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
