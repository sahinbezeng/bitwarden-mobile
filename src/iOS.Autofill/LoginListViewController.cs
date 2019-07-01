using System;
using Bit.iOS.Autofill.Models;
using Foundation;
using UIKit;
using Bit.iOS.Core.Controllers;
using Bit.App.Resources;
using Bit.iOS.Core.Views;
using Bit.iOS.Autofill.Utilities;

namespace Bit.iOS.Autofill
{
    public partial class LoginListViewController : ExtendedUITableViewController
    {
        public LoginListViewController(IntPtr handle)
            : base(handle)
        { }

        public Context Context { get; set; }
        public CredentialProviderViewController CPViewController { get; set; }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavItem.Title = AppResources.Items;
            CancelBarButton.Title = AppResources.Cancel;

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 44;
            TableView.Source = new TableSource(this);
            await ((TableSource)TableView.Source).LoadItemsAsync();
        }

        partial void CancelBarButton_Activated(UIBarButtonItem sender)
        {
            CPViewController.CompleteRequest();
        }

        partial void AddBarButton_Activated(UIBarButtonItem sender)
        {
            PerformSegue("loginAddSegue", this);
        }

        partial void SearchBarButton_Activated(UIBarButtonItem sender)
        {
            PerformSegue("loginSearchFromListSegue", this);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if(segue.DestinationViewController is UINavigationController navController)
            {
                if(navController.TopViewController is LoginAddViewController addLoginController)
                {
                    addLoginController.Context = Context;
                    addLoginController.LoginListController = this;
                }
                if(navController.TopViewController is LoginSearchViewController searchLoginController)
                {
                    searchLoginController.Context = Context;
                    searchLoginController.CPViewController = CPViewController;
                }
            }
        }

        public void DismissModal()
        {
            DismissViewController(true, async () =>
            {
                await ((TableSource)TableView.Source).LoadItemsAsync();
                TableView.ReloadData();
            });
        }

        public class TableSource : ExtensionTableSource
        {
            private Context _context;
            private LoginListViewController _controller;

            public TableSource(LoginListViewController controller)
                : base(controller.Context, controller)
            {
                _context = controller.Context;
                _controller = controller;
            }

            public async override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                await AutofillHelpers.TableRowSelectedAsync(tableView, indexPath, this,
                    _controller.CPViewController, _controller, "loginAddSegue");
            }
        }
    }
}
