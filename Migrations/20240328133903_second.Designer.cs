﻿// <auto-generated />
using System;
using FunnyLanguage_WPF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FunnyLanguage_WPF.Migrations
{
    [DbContext(typeof(VideoContext))]
    [Migration("20240328133903_second")]
    partial class second
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("FunnyLanguage_WPF.Models.Language", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("VideoId")
                        .HasColumnType("INTEGER");

                    b.HasKey("LanguageId");

                    b.HasIndex("VideoId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.Video", b =>
                {
                    b.Property<int>("VideoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CaptionFolderPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("VideoId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.Word", b =>
                {
                    b.Property<int>("WordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SecondLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SuccessRate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TranslatedText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WordlistId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WordId");

                    b.HasIndex("WordlistId");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.WordList", b =>
                {
                    b.Property<int>("WordListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("VideoId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WordListId");

                    b.HasIndex("VideoId");

                    b.ToTable("WordLists");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.Language", b =>
                {
                    b.HasOne("FunnyLanguage_WPF.Models.Video", null)
                        .WithMany("Languages")
                        .HasForeignKey("VideoId");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.Word", b =>
                {
                    b.HasOne("FunnyLanguage_WPF.Models.WordList", null)
                        .WithMany("Words")
                        .HasForeignKey("WordlistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.WordList", b =>
                {
                    b.HasOne("FunnyLanguage_WPF.Models.Video", "Video")
                        .WithMany()
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Video");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.Video", b =>
                {
                    b.Navigation("Languages");
                });

            modelBuilder.Entity("FunnyLanguage_WPF.Models.WordList", b =>
                {
                    b.Navigation("Words");
                });
#pragma warning restore 612, 618
        }
    }
}
