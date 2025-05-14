using Microsoft.EntityFrameworkCore;
using Project.Server.Models;
using System.Reflection.Metadata;
namespace Project.Server.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }

        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<UserGroupChat> UserGroups { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserFriend>()
        .HasKey(uf => new { uf.UserId, uf.FriendId });

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.Friend)
                .WithMany() 
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict); 

            
            modelBuilder.Entity<UserGroupChat>()
                .HasKey(ugc => new { ugc.UserId, ugc.GroupChatId });

            modelBuilder.Entity<UserGroupChat>()
                .HasOne(ugc => ugc.User)
                .WithMany(u => u.GroupChats)
                .HasForeignKey(ugc => ugc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserGroupChat>()
                .HasOne(ugc => ugc.GroupChat)
                .WithMany(gc => gc.Users)
                .HasForeignKey(ugc => ugc.GroupChatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserConversation>()
                .HasKey(uc => new { uc.UserId, uc.ConversationId }); 

            modelBuilder.Entity<UserConversation>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<UserConversation>()
                .HasOne(uc => uc.Conversation)
                .WithMany(c => c.Users)
                .HasForeignKey(uc => uc.ConversationId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany() 
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.GroupChat)
                .WithMany(gc => gc.Messages)
                .HasForeignKey(m => m.GroupChatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.FriendRequestsSent)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.FriendRequestsReceived)
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasMany(m => m.Attachments)
                .WithOne()
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attachment>()
                .HasOne<Message>()
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .Navigation(u => u.Friends)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(u => u.Conversations)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(u => u.GroupChats)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(u => u.FriendRequestsSent)
                .AutoInclude();

            modelBuilder.Entity<User>()
                .Navigation(u => u.FriendRequestsReceived)
                .AutoInclude();

        }


    }
}
