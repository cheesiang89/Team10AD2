﻿
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Team10AD_WCF;

public class Service : IService
{

    /////////Test
    public Test TestM()
    {
        WCFCatalogue c = GetCatalogue("C001");
        WCFCatalogue c2 = GetCatalogue("C002");
        return Test.Make(c.ItemCode,new WCFCatalogue[] {c,c2 });
    }


    /////////////Catalogue
    public WCFCatalogue GetCatalogue(string itemCode)
    {
        Catalogue c = Data.GetCatalogue(itemCode);
        return WCFCatalogue.Make(c.ItemCode, c.Description, c.Location, c.BalanceQuantity, c.UnitOfMeasure);
    }

    public WCFCatalogue[] ListCatalogues()
    {
        List<WCFCatalogue> l = new List<WCFCatalogue>();
        foreach(Catalogue c in Data.ListCatalogues()){
            WCFCatalogue w = WCFCatalogue.Make(c.ItemCode, c.Description, c.Location, c.BalanceQuantity, c.UnitOfMeasure);
            l.Add(w);
        }
        return l.ToArray<WCFCatalogue>();
    }


    /////////////Disbursement
    public WCFDisbursementDetail[] ListDisbursementDetail(string disbursementID)
    {
        List<WCFDisbursementDetail> l = new List<WCFDisbursementDetail>();

        foreach (DisbursementDetail c in Data.GetDisbursementDetails(Convert.ToInt32(disbursementID)))
        {
            WCFDisbursementDetail w = WCFDisbursementDetail.Make(disbursementID, c.Remarks, c.ItemCode, c.QuantityRequested.ToString(), c.QuantityCollected.ToString(), c.Catalogue.Description, c.Catalogue.UnitOfMeasure);
            l.Add(w);
        }

        return l.ToArray<WCFDisbursementDetail>();
    }

    public WCFDisbursement[] ListDisbursements(String status)
    {
        List<WCFDisbursement> l = new List<WCFDisbursement>();

        foreach(Disbursement d in Data.ListDisbursements(status))
        {
            WCFDisbursement w = WCFDisbursement.Make(d.DisbursementID.ToString(), d.CollectionDate.HasValue ? d.CollectionDate.Value.ToString("dd-MMM-yyyy") : null, d.CollectionPoint.PointName, d.Department.DepartmentName, d.Status, d.StoreStaffID.ToString());
            l.Add(w);
        }

        return l.ToArray<WCFDisbursement>();
    }



    /////////////Retrieval & RetrievalDetail
    public WCFRetrievalDetail[] ListRetrievalDetails(string retrievalID) {
        List<WCFRetrievalDetail> l = new List<WCFRetrievalDetail>();
        foreach (RetrievalDetail c in Data.ListRetrievalDetails(Int32.Parse(retrievalID)))
        {
            WCFRetrievalDetail w = WCFRetrievalDetail.Make(Int32.Parse(retrievalID), c.ItemCode, c.RequestedQuantity, c.RetrievedQuantity/*, c.QuantityAfter*/);
            l.Add(w);
        }
        return l.ToArray<WCFRetrievalDetail>();

    }

    public WCFRetrieval[] ListUncollectedRetrievals()
    {
        List<WCFRetrieval> l = new List<WCFRetrieval>();
        foreach (Retrieval c in Data.ListUncollectedRetrievals())
        {
            WCFRetrieval w = WCFRetrieval.Make(c.RetrievalID, c.RetrievalDate.HasValue? c.RetrievalDate.Value.ToString("dd-MMM-yyyy"):null/*, ListRetrievalDetails(c.RetrievalID.ToString())*/);
            l.Add(w);
        }
        return l.ToArray<WCFRetrieval>();
    }

    public WCFRetrieval[] ListCollectedRetrievals()
    {
        List<WCFRetrieval> l = new List<WCFRetrieval>();
        foreach (Retrieval c in Data.ListCollectedRetrievals())
        {
            WCFRetrieval w = WCFRetrieval.Make(c.RetrievalID, c.RetrievalDate.HasValue ? c.RetrievalDate.Value.ToString("dd-MMM-yyyy") : null/*, ListRetrievalDetails(c.RetrievalID.ToString())*/);
            l.Add(w);
        }
        return l.ToArray<WCFRetrieval>();
    }


    public string UpdateRetrievalDetails(WCFRetrievalDetail[] b)
    {
        List<RetrievalDetail> l = new List<RetrievalDetail>();
        foreach(WCFRetrievalDetail c in b)
        {
            RetrievalDetail r = new RetrievalDetail();
            r.RetrievalID = c.RetrievalID;
            r.ItemCode = c.ItemCode;
            r.RequestedQuantity = c.RequestedQuantity;
            r.RetrievedQuantity = c.RetrievedQuantity;
            l.Add(r);
        }
        return Data.UpdateRetrievalDetails(l);
    }


    ///////////////Receive & ReceiveDetail 
    public WCFReceived[] ReceivedList()
    {
        List<WCFReceived> l = new List<WCFReceived>();
        foreach (GoodsReceivedRecord g in Data.ReceivedList())
        {
            WCFReceived w = WCFReceived.Make(g.GoodReceiveID, g.ReceivedDate, g.POID, g.StoreStaffID);
            l.Add(w);
        }
        return l.ToArray<WCFReceived>();
    }

    public WCFReceivedDetail[] ListReceivedDetail(string receivedID)
    {
        List<WCFReceivedDetail> l = new List<WCFReceivedDetail>();
        foreach (GoodsReceivedRecordDetail g in Data.ReceivedDetailList(Int32.Parse(receivedID)))
        {
            WCFReceivedDetail w = WCFReceivedDetail.Make(g.ItemCode, g.Catalogue.Description,  g.ReceivedQuantity, g.Catalogue.Supplier.SupplierName);
            l.Add(w);
        }
        return l.ToArray<WCFReceivedDetail>();
    }

    public WCFPendingReceive[] PendingReceiveList()
    {
        List<WCFPendingReceive> l = new List<WCFPendingReceive>();
        foreach (PurchaseOrder g in Data.PendingReceive())
        {
            WCFPendingReceive w = WCFPendingReceive.Make(g.POID, g.CreationDate, g.SupplierCode, g.StoreStaffID);
            l.Add(w);
        }
        return l.ToArray<WCFPendingReceive>();
    }

    public WCFPendingReceiveDetail[] PendingReceiveDetailList(string poID)
    {
        List<WCFPendingReceiveDetail> l = new List<WCFPendingReceiveDetail>();
        foreach (PurchaseOrderDetail g in Data.PendingReceiveDetail(Int32.Parse(poID)))
        {
            WCFPendingReceiveDetail w = WCFPendingReceiveDetail.Make(g.ItemCode, g.POID, g.Quantity, g.UnitPrice);
            l.Add(w);
        }
        return l.ToArray<WCFPendingReceiveDetail>();
    }

    public void UpdateWCFRetrieval(WCFRetrieval b)
    {
        throw new NotImplementedException();
    }

    public WCFDepartment[] ListDepartment()
    {
        throw new NotImplementedException();
    }

    public WCFPendingReceiveDetail[] PendingReceiveDetailList()
    {
        throw new NotImplementedException();
    }

    ///////Approver & Rep
    public WCFApprover[] ListApprover(string depCode)
    {
        List<WCFApprover> l = new List<WCFApprover>();
        foreach (Employee c in Data.A_EmployeeList(depCode))
        {
            WCFApprover w = WCFApprover.Make(c.EmployeeID, c.Name, c.DepartmentCode);
            l.Add(w);
        }
        return l.ToArray<WCFApprover>();
    }

    public void UpdateApprover(WCFApprover a) {
        Employee e = new Employee
        {
            EmployeeID = a.EmployeeID,
            Name=a.Name,
            DepartmentCode=a.DepartmentCode,
        };
        Data.UpdateApprover(e);
    }

    public WCFApprover GetApprover(string depCode) {
        Employee i = Data.GetApprover(depCode);
        WCFApprover a = WCFApprover.Make(i.EmployeeID, i.Name, i.DepartmentCode);
        return a;
    }

    public WCFRequisition[] CheckRequisition(string  employeeID) {
        List<WCFRequisition> l = new List<WCFRequisition>();
        foreach (Requisition c in Data.checkRequisition(Int32.Parse(employeeID)))
        {
            WCFRequisition w = WCFRequisition.Make(c.RequisitionID, c.RequisitionDate.HasValue? c.RequisitionDate.Value.ToString("dd-MM-yyyy"):null, c.ApprovalDate.HasValue? c.ApprovalDate.Value.ToString("dd-MM-yyyy"):null,c.RequestorID,c.Status,c.Remarks,c.ApproverID);
            l.Add(w);
        }
        return l.ToArray<WCFRequisition>();
    }



    public WCFRep[] ListRep(string depCode)
    {
        List<WCFRep> l = new List<WCFRep>();
        foreach (Employee c in Data.R_EmployeeList(depCode))
        {
            WCFRep w = WCFRep.Make(c.EmployeeID, c.Name, c.DepartmentCode);
            l.Add(w);
        }
        return l.ToArray<WCFRep>();
    }

    public void UpdateRep(WCFRep r)
    {
        Employee e = new Employee
        {
            EmployeeID = r.EmployeeID,
            Name = r.Name,
            DepartmentCode = r.DepartmentCode,
        };
        Data.UpdateRep(e);
    }


////////ReceivingGoods
    public WCFReceivingGoodData[] RecievingGoods(string poid)
    {

        List<WCFReceivingGoodData> goodslist = new List<WCFReceivingGoodData>();

        Dictionary<PurchaseOrderDetail, PurchaseOrder> dictionary = Data.RecievingGoodsList(poid);
        List<PurchaseOrderDetail> polist = new List<PurchaseOrderDetail>();

        foreach (KeyValuePair<PurchaseOrderDetail, PurchaseOrder> detail in dictionary)
        {
            polist.Add(detail.Key);
        }

        PurchaseOrder po = dictionary[polist[0]];

        foreach (PurchaseOrderDetail d in polist)
        {
            WCFReceivingGoodData good = WCFReceivingGoodData.Make(po.CreationDate.HasValue ? po.CreationDate.Value.ToString("dd-MMM-yyyy") : null, d.POID, d.ItemCode, d.Quantity, d.Catalogue.Description, po.Supplier.SupplierName);
            goodslist.Add(good);
        }


        return goodslist.ToArray<WCFReceivingGoodData>();
    }

    public WCFPurchaseOrderList[] PurchaseOrderList()
    {
        List<WCFPurchaseOrderList> wcfpolist = new List<WCFPurchaseOrderList>();

        List<PurchaseOrder> polist = Data.PurchaseOrderList();
        foreach (PurchaseOrder po in polist)
        {
            WCFPurchaseOrderList wcfpo = WCFPurchaseOrderList.Make(po.CreationDate.HasValue ? po.CreationDate.Value.ToString("dd-MMM-yyyy") : null, po.POID, po.Supplier.SupplierName);
            wcfpolist.Add(wcfpo);
        }

        return wcfpolist.ToArray<WCFPurchaseOrderList>();
    }


    ////////////LogIn
    public WCFEmployeeLogIn[] ListEmployeeLogIn() {
        List<WCFEmployeeLogIn> l = new List<WCFEmployeeLogIn>();
        foreach (Employee c in Data.EmployeeLogIn())
        {
            WCFEmployeeLogIn w = WCFEmployeeLogIn.Make(c.EmployeeID, c.Name, c.Phone);
            l.Add(w);
        }
        return l.ToArray<WCFEmployeeLogIn>();
    }

    public WCFClerkLogIn[] ListClerkLogIn()
    {
        List<WCFClerkLogIn> l = new List<WCFClerkLogIn>();
        foreach (StoreStaff c in Data.ClerkLogIn())
        {
            WCFClerkLogIn w = WCFClerkLogIn.Make(c.StoreStaffID, c.Name, c.PhoneNumber);
            l.Add(w);
        }
        return l.ToArray<WCFClerkLogIn>();
    }
}