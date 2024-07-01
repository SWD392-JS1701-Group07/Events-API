    using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Events.Models.Models;

public partial class EventsDbContext : DbContext
{
    public EventsDbContext()
    {
    }

    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Collaborator> Collaborators { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventSchedule> EventSchedules { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sponsor> Sponsors { get; set; }

    public virtual DbSet<Sponsorship> Sponsorships { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.StudentId).HasMaxLength(8);
            entity.Property(e => e.Username).HasMaxLength(30);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");

            entity.HasOne(d => d.Subject).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Account_Subject");
        });

        modelBuilder.Entity<Collaborator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_JoinEvent");

            entity.Property(e => e.Task).HasMaxLength(100);

            entity.HasOne(d => d.Account).WithMany(p => p.Collaborators)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JoinEvent_Account");

            entity.HasOne(d => d.Event).WithMany(p => p.Collaborators)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JoinEvent_Event");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.Property(e => e.EndSellDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.StartSellDate).HasColumnType("datetime");

            entity.HasOne(d => d.Owner).WithMany(p => p.Events)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Account");
        });

        modelBuilder.Entity<EventSchedule>(entity =>
        {
            entity.ToTable("EventSchedule");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Place).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.EventSchedules)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventSchedule_Event");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.VnPayResponseCode).HasMaxLength(10);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customers");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Sponsor>(entity =>
        {
            entity.ToTable("Sponsor");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<Sponsorship>(entity =>
        {
            entity.ToTable("Sponsorship");

            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Event).WithMany(p => p.Sponsorships)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sponsorship_Event");

            entity.HasOne(d => d.Sponsor).WithMany(p => p.Sponsorships)
                .HasForeignKey(d => d.SponsorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sponsorship_Sponsor");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OrdersId).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Qrcode).HasColumnName("QRCode");

            entity.HasOne(d => d.Event).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Event");

            entity.HasOne(d => d.Orders).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.OrdersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Orders");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
