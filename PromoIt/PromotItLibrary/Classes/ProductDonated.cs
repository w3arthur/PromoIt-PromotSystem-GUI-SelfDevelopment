﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using PromotItLibrary.Models;
using PromotItLibrary.Patterns.DataTables;
using PromotItLibrary.Interfaces;
using PromotItLibrary.Enums;
using PromotItLibrary.Patterns.Actions.Actions_Fuction_State;
using PromotItLibrary.Patterns.Actions.Actions_MySql_State;
using PromotItLibrary.Patterns.Actions.Actions_Queue_State;
using PromotItLibrary.Patterns.LinkedLists.LinkedList_Function_State;
using PromotItLibrary.Patterns.LinkedLists.LinkedLists_MySql_State;
using PromotItLibrary.Patterns.LinkedLists.Queue_State;
using PromotItLibrary.Interfaces.Actions;
using PromotItLibrary.Interfaces.DataTables;
using PromotItLibrary.Interfaces.LinkedList;
using PromotItLibrary.Interfaces.Users;

namespace PromotItLibrary.Classes
{
    public class ProductDonated : IProductDonated, IActionsProduct_ProductDonated, ILinkedListProduct_ProductDonated, IDataTabletProduct_ProductDonated
    {
        private readonly MySQL _mySQL = Configuration.MySQL;
        private readonly HTTPClient _httpClient = Configuration.HTTPClient;

        private readonly Modes _mode;
        private readonly IActionsProduct_ProductDonated actionsProduct;
        private readonly ILinkedListProduct_ProductDonated linkedListProduct;
        private readonly IDataTabletProduct_ProductDonated dataTabletProduct;

        public IProductInCampaign ProductInCampaign { get; set; }
        public IUsers ActivistUser { get; set; }
        public string Quantity { get; set; }
        public string Shipped { get; set; }
        public string Id { get; set; }

        public ProductDonated()
        {
            //Actions State
            //LinkdeList States
            if ((_mode ?? Configuration.Mode) == Modes.Queue)
            {
                actionsProduct = new ActionsProduct_Queue(this, _httpClient);
                linkedListProduct = new LinkedListProduct_Queue(this,  _httpClient);
            }
            else if ((_mode ?? Configuration.Mode) == Modes.Functions)
            {
                actionsProduct = new ActionsProduct_Function(this, _httpClient);
                linkedListProduct = new LinkedListProduct_Function(this,  _httpClient);
            }
            if ((_mode ?? Configuration.DatabaseMode) == Modes.MySQL)
            {
                actionsProduct = new ActionsProduct_MySql(this, _mySQL);
                linkedListProduct = new LinkedListProduct_MySql(this, _mySQL);
            }

            //DataTable States ?
            dataTabletProduct = new DataTabletProduct(this);
        }


        //Actions
        public async Task SetTwitterMessagTweet_SetBuyAnItemAsync()
        {
            try
            {
                await Twitter.SetTwitterMessage_SetBuyAnItemAsync($"Product: {this.ProductInCampaign.Name}, Quantity {this.Quantity}" +
                    $"\nOrdered by Social Activist: @{this.ActivistUser.UserName}" +
                    $"\nFrom Business: {this.ProductInCampaign.BusinessUser.UserName}");
                Loggings.ReportLog($"Trying To Set a tweet for buying an item, Activist UserName ({this.ActivistUser.UserName}) CampaignName ({this.ProductInCampaign.Name}) BuisnessUserName ({this.ProductInCampaign.BusinessUser.UserName})" +
                    $"\nProductId ({this.ProductInCampaign.Id}) Quantity ({this.Quantity})");
            }
            catch
            {  //Twitter exeption
                Loggings.ErrorLog($"Some error to set a tweet for buying an item, Activist UserName ({this.ActivistUser.UserName}) CampaignName ({this.ProductInCampaign.Name}) BuisnessUserName ({this.ProductInCampaign.BusinessUser.UserName})" +
                    $"\nProductId ({this.ProductInCampaign.Id}) Quantity ({this.Quantity})");
            }
        }

        public async Task<bool> SetBuyAnItemAsync(Modes mode = null) =>
            await actionsProduct.SetBuyAnItemAsync(mode);
        public async Task<bool> SetProductShippingAsync(Modes mode = null) =>
            await actionsProduct.SetProductShippingAsync(mode);

        // LinkedList
        public async Task<List<ProductDonated>> GetDonatedProductForShipping_ListAsync(Modes mode = null) =>
             await linkedListProduct.GetDonatedProductForShipping_ListAsync(mode);

        //DataTable
        public async Task<DataTable> GetDonatedProductForShipping_DataTableAsync() =>
             await dataTabletProduct.GetDonatedProductForShipping_DataTableAsync();

    }
}
