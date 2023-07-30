﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IssueTracking.Localization;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace IssueTracking.Blazor.Menus
{
    public class IssueTrackingMenuContributor : IMenuContributor
    {
        private readonly IConfiguration _configuration;

        public IssueTrackingMenuContributor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
            else if (context.Menu.Name == StandardMenus.User)
            {
                await ConfigureUserMenuAsync(context);
            }
        }

        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<IssueTrackingResource>();

           var issueTrackingMenu = new ApplicationMenuItem("IssueTrackingMenu", l["Menu:IssueTracking"], icon: "fas fa-tasks");
            var issuesMenu = new ApplicationMenuItem("IssuesMenu", l["Menu:Issues"], url: "/issues");
            issueTrackingMenu.AddItem(issuesMenu);


            context.Menu.AddItem(issueTrackingMenu);

            return Task.CompletedTask;
        }

        private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
        {
            var accountStringLocalizer = context.GetLocalizer<AccountResource>();
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            var identityServerUrl = _configuration["AuthServer:Authority"] ?? "";

            if (currentUser.IsAuthenticated)
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    "Account.Manage",
                    accountStringLocalizer["ManageYourProfile"],
                    $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage",
                    icon: "fa fa-cog",
                    order: 1000,
                    null,
                    "_blank"));
            }

            return Task.CompletedTask;
        }
    }
}
