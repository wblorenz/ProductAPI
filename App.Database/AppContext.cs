using App.Model.Entities;
using App.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Database
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options)
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ValidateErrors();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ValidateErrors();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ValidateErrors()
        {
            var changedEntities = ChangeTracker
            .Entries()
            .Where(_ => _.State == EntityState.Added ||
                        _.State == EntityState.Modified);

            var errors = new List<ValidationResult>(); // all errors are here
            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null);
                Validator.TryValidateObject(
                    e.Entity, vc, errors, validateAllProperties: true);
            }
            if (errors.Any())
            {
                throw new DomainValidationException(errors);
            }
        }
    }
}
