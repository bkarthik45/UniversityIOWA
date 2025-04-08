using System;
using IOWABacked.Models;
using Microsoft.EntityFrameworkCore;

namespace IOWABacked.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reimbursement> Reimbursements { get; set; }
    }
}
