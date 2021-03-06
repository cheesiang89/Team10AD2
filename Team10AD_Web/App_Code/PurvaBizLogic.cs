﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team10AD_Web.Model;
using Team10AD_Web.DTO;
namespace Team10AD_Web
{
    /// <summary>
    /// Summary description for BusinessLogic
    /// </summary>
    public static class PurvaBizLogic
    {


        public static Supplier GetSupplier(string supplierCode)
        {
            using (Team10ADModel tm = new Team10ADModel())
            {
                List<Supplier> result = tm.Suppliers.Where(x => x.SupplierCode == supplierCode).ToList();
                return result[0];
            }

        }

        public static List<Supplier> ShowSuppliers()
        {
            using (Team10ADModel tm = new Team10ADModel())
            {
                return tm.Suppliers.OrderBy(x => x.SupplierName).ToList();
            }
        }
        public static string GetDescriptionFromItemCode(string query)
        {
            string description = "";
            try
            {
                using (Team10ADModel tm = new Team10ADModel())
                {
                    description = tm.Catalogues.Where(x => x.ItemCode == query).Select(x => x.Description).First();
                    return description;
                }
            }
            catch (Exception)
            {

                return description;
            }
        }
        public static List<Catalogue> GetLowStockByStatus(string status)
        {
            using (Team10ADModel tm = new Team10ADModel())
            {
                return tm.Catalogues.Where(x => x.ShortfallStatus == status).Select(x => x).ToList();
            }

        }
        public static Catalogue GetItemByCode(string itemCode)
        {
            using (Team10ADModel tm = new Team10ADModel())
            {
                return tm.Catalogues.Where(x => x.ItemCode == itemCode).Select(x => x).First();
            }

        }
       
        public static bool SavePOInfo(List<POIntermediate> poList, int storeStaffID)
        {
            bool result = false;
            //string test = "";
            HashSet<string> supSet = new HashSet<string>();
            //Save supplier names in HashSet
            foreach (var item in poList)
            {
                supSet.Add(item.SupplierName);
            }
            //Create different PO objects depending on suppliers
            int count = supSet.Count;
            //Iterate through hashset to createPO- Convert to Array and iterate Array 
            string[] stringArray = supSet.ToArray();


            foreach (string supName in stringArray)
            {
                //TEST
                //    test += supName;
                //Create a PO for each unique supplier

                using (Team10ADModel m = new Team10ADModel())
                {
                    List<PurchaseOrderDetail> poDetailList = new List<PurchaseOrderDetail>();
                    PurchaseOrder po = new PurchaseOrder();
                    PurchaseOrderDetail pd;
                    int? minOrderQty;
                    //CreationDate
                    po.CreationDate = DateTime.Now;
                    //StoreStaffID 
                    po.StoreStaffID = storeStaffID;
                    //Get from POIntermediate: SupplierCode, PODetails: ItemCode,Quantity, UnitPrice, Status
                    foreach (var poIntermediate in poList)
                    {
                        if (supName == poIntermediate.SupplierName && poIntermediate.Quantity!=0)
                        {
                            pd= new PurchaseOrderDetail();
                            po.SupplierCode = poIntermediate.SupplierName;
                            pd.ItemCode = poIntermediate.ItemCode;
                            pd.Quantity = poIntermediate.Quantity;
                            pd.UnitPrice = m.SupplierDetails
                                .Where(x => x.ItemCode == pd.ItemCode)
                                .Select(x => x.Price).First();
                            minOrderQty = m.Catalogues
                                .Where(x => x.ItemCode == pd.ItemCode)
                                .Select(x => x.MinimumOrderQuantity).First();
                            pd.Status = "Unreceived";
                            //Only add if order qty >= Min order qty
                            if (pd.Quantity>=minOrderQty)
                            {
                                poDetailList.Add(pd);
                            }
                           
                        }

                    }
                    po.PurchaseOrderDetails = poDetailList;
                    //Status
                    po.Status = "Unreceived";
                    //Only save changes if PO is not empty
                    if (poDetailList.Count != 0)
                    {
                        m.PurchaseOrders.Add(po);
                        foreach (PurchaseOrderDetail item in poDetailList)
                        {
                            Catalogue c = m.Catalogues.Where(x => x.ItemCode == item.ItemCode).Select(x => x).First();
                           
                            //Add pending delivery qty
                            int? qty = m.Catalogues.Where(x => x.ItemCode == item.ItemCode).Select(x => x.PendingDeliveryQuantity).First();
                            int? balanceQty = m.Catalogues.Where(x => x.ItemCode == item.ItemCode).Select(x => x.BalanceQuantity).First();
                            int? reorderQty = m.Catalogues.Where(x => x.ItemCode == item.ItemCode).Select(x => x.ReorderLevel).First();
                            qty += item.Quantity.GetValueOrDefault();
                            c.PendingDeliveryQuantity = qty;
                            if (qty>=(reorderQty-balanceQty))
                            {
                                //Set shortfall to false
                                c.ShortfallStatus = "False";
                            }
                        }

                        m.SaveChanges();
                        result = true;
                    }
                   
                }
                //return test;
               
            }
            return result;
        }

        public static int? GetMinOrderQty(string itemCode)
        {
            int? minOrderQty = 0;
            using (Team10ADModel m = new Team10ADModel())
            {
               minOrderQty  = m.Catalogues.Where(x => x.ItemCode == itemCode).Select(x => x.MinimumOrderQuantity).First();
            }
            return minOrderQty;
        }
    }
}
