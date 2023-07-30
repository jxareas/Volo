﻿using IssueTracking.Domain.Issues;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace IssueTracking.EntityFrameworkCore
{
    public static class IssueTrackingDbContextModelCreatingExtensions
    {
        public static void ConfigureIssueTracking(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

         builder.Entity<Issue>(b =>
            {
                b.ToTable(IssueTrackingConsts.DbTablePrefix + "Issues", IssueTrackingConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasMany(x => x.Comments).WithOne(x => x.Issue).HasForeignKey(x => x.IssueId).IsRequired();
            });

            builder.Entity<IssueLabel>(b =>
            {
                b.HasKey(e => new { e.LabelId, e.IssueId }).IsClustered(false);
                b.ToTable(IssueTrackingConsts.DbTablePrefix + "IssueLabels", IssueTrackingConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<Comment>(b =>
            {
                b.ToTable(IssueTrackingConsts.DbTablePrefix + "Comments", IssueTrackingConsts.DbSchema);
                b.ConfigureByConvention();
            });
        }
    }
}