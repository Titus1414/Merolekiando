using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Merolekiando.Models
{
    public partial class MerolikandoDBContext : DbContext
    {
        public MerolikandoDBContext()
        {
        }

        public MerolikandoDBContext(DbContextOptions<MerolikandoDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ConversationCount> ConversationCounts { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Folower> Folowers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Municipality> Municipalities { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<ProdImage> ProdImages { get; set; }
        public virtual DbSet<ProdMunicipality> ProdMunicipalities { get; set; }
        public virtual DbSet<ProdProvince> ProdProvinces { get; set; }
        public virtual DbSet<ProdView> ProdViews { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserVerification> UserVerifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=64.20.48.40;Database=MerolikandoDB;User Id=Merole;Password=Mero321?; MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AllowAll).HasColumnName("allowAll");

                entity.Property(e => e.CurrentColor).HasColumnName("currentColor");

                entity.Property(e => e.Description)
                    .HasMaxLength(350)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.SubscriptionImage)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("subscriptionImage");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Image)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Time).HasColumnName("time");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chat");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConnFrom)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ConnId)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ConnTo)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Key)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("key");

                entity.Property(e => e.Link).HasColumnName("link");

                entity.Property(e => e.Message)
                    .IsUnicode(false)
                    .HasColumnName("message");

                entity.Property(e => e.RecieverId).HasColumnName("recieverId");

                entity.Property(e => e.SenderId).HasColumnName("senderId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<ConversationCount>(entity =>
            {
                entity.ToTable("ConversationCount");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ConversationCounts)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_ConversationCount_Product");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("Favorite");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_Favorite_Product");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Favorite_Users");
            });

            modelBuilder.Entity<Folower>(entity =>
            {
                entity.HasOne(d => d.FuserNavigation)
                    .WithMany(p => p.Folowers)
                    .HasForeignKey(d => d.Fuser)
                    .HasConstraintName("FK_Folowers_Users");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConnId)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastMessage)
                    .IsUnicode(false)
                    .HasColumnName("lastMessage");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.Read).HasColumnName("read");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.HasOne(d => d.ToNavigation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.To)
                    .HasConstraintName("FK_Messages_Users");
            });

            modelBuilder.Entity<Municipality>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.HasOne(d => d.Prv)
                    .WithMany(p => p.Municipalities)
                    .HasForeignKey(d => d.PrvId)
                    .HasConstraintName("FK_Municipalities_Province");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Uid).HasColumnName("UId");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_Notifications_Product");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_Notifications_Users");
            });

            modelBuilder.Entity<ProdImage>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Image)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.PId).HasColumnName("pId");

                entity.HasOne(d => d.PIdNavigation)
                    .WithMany(p => p.ProdImages)
                    .HasForeignKey(d => d.PId)
                    .HasConstraintName("FK_ProdImages_Product");
            });

            modelBuilder.Entity<ProdMunicipality>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.Mnc)
                    .WithMany(p => p.ProdMunicipalities)
                    .HasForeignKey(d => d.MncId)
                    .HasConstraintName("FK_ProdMunicipalities_Municipalities");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ProdMunicipalities)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_ProdMunicipalities_Product");
            });

            modelBuilder.Entity<ProdProvince>(entity =>
            {
                entity.ToTable("ProdProvince");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ProdProvinces)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_ProdProvince_Product");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.ProdProvinces)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_ProdProvince_Province");
            });

            modelBuilder.Entity<ProdView>(entity =>
            {
                entity.ToTable("ProdView");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PId).HasColumnName("pId");

                entity.HasOne(d => d.PIdNavigation)
                    .WithMany(p => p.ProdViews)
                    .HasForeignKey(d => d.PId)
                    .HasConstraintName("FK_ProdView_Product");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ProdViews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ProdView_Users");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.ChildCategory).HasColumnName("childCategory");

                entity.Property(e => e.Condition)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("condition");

                entity.Property(e => e.CreatedDate).HasColumnName("createdDate");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.FireOnPrice).HasColumnName("fireOnPrice");

                entity.Property(e => e.IsDelivering).HasColumnName("isDelivering");

                entity.Property(e => e.IsPickup).HasColumnName("isPickup");

                entity.Property(e => e.IsPromoted).HasColumnName("isPromoted");

                entity.Property(e => e.IsReported).HasColumnName("isReported");

                entity.Property(e => e.IsSold).HasColumnName("isSold");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.SellerId).HasColumnName("sellerId");

                entity.Property(e => e.SubCategoryId).HasColumnName("subCategoryId");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Time).HasColumnName("time");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Compliment)
                    .HasMaxLength(350)
                    .IsUnicode(false)
                    .HasColumnName("compliment");

                entity.Property(e => e.Rating1)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("rating");

                entity.Property(e => e.UidTo).HasColumnName("UIdTo");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.UidToNavigation)
                    .WithMany(p => p.RatingUidToNavigations)
                    .HasForeignKey(d => d.UidTo)
                    .HasConstraintName("FK_Rating_Users1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RatingUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Rating_Users");
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.SubCategories)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("FK_SubCategories_Category");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Description)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.IdImage)
                    .HasMaxLength(550)
                    .IsUnicode(false)
                    .HasColumnName("idImage");

                entity.Property(e => e.Image)
                    .HasMaxLength(550)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.IsVerified).HasColumnName("isVerified");

                entity.Property(e => e.LoginType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("loginType");

                entity.Property(e => e.MemberSince).HasColumnName("memberSince");

                entity.Property(e => e.MunicipalityId).HasColumnName("municipalityId");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("number");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.ProvinceId).HasColumnName("provinceId");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 1)");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Subscriptions).HasColumnName("subscriptions");

                entity.Property(e => e.UniqueId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("uniqueId");

                entity.Property(e => e.VerificationSent).HasColumnName("verificationSent");

                entity.HasOne(d => d.Municipality)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.MunicipalityId)
                    .HasConstraintName("FK_Users_Municipalities");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Users_Province");
            });

            modelBuilder.Entity<UserVerification>(entity =>
            {
                entity.ToTable("UserVerification");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Bimage)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("BImage");

                entity.Property(e => e.Fimage)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("FImage");

                entity.Property(e => e.Message)
                    .HasMaxLength(550)
                    .IsUnicode(false)
                    .HasColumnName("message");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserVerifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserVerification_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
