﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Team10AD_Web.App_Code;
namespace Team10AD_Web.Service
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";

        [WebGet()]
        [OperationContract]
        public string Greeting()
        {
            // Add your operation implementation here
            return "Hello world";
        }

        // Add more operations here and mark them with [OperationContract]
        [OperationContract]
        [WebInvoke (Method ="POST")]
        public string GetJSON(List<CartData> cart)
        {
            string x="";
            // Add your operation implementation here
            foreach (var item in cart)
            {
                x += item.ItemCode;
            }

            return x;
        }
    }
}
