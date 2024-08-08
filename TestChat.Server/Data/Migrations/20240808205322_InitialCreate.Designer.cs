﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestChat.Server.Data;

#nullable disable

namespace TestChat.Server.Data.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20240808205322_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.6.24327.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestChat.Server.Data.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RecipientName")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("SenderName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("TestChat.Server.Data.Entities.SentimentAnalysis", b =>
                {
                    b.Property<Guid>("MessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("NegativeSentiment")
                        .HasColumnType("float");

                    b.Property<double>("NeutralSentiment")
                        .HasColumnType("float");

                    b.Property<double>("PositiveSentiment")
                        .HasColumnType("float");

                    b.HasKey("MessageId");

                    b.ToTable("SentimentAnalyses");
                });

            modelBuilder.Entity("TestChat.Server.Data.Entities.SentimentAnalysis", b =>
                {
                    b.HasOne("TestChat.Server.Data.Entities.Message", "Message")
                        .WithOne("SentimentAnalysis")
                        .HasForeignKey("TestChat.Server.Data.Entities.SentimentAnalysis", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("TestChat.Server.Data.Entities.Message", b =>
                {
                    b.Navigation("SentimentAnalysis");
                });
#pragma warning restore 612, 618
        }
    }
}
