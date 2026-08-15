using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HJMSurrenSystem.MES.MiCheckBOMInventoryProxy
{
    internal static class MiCheckBOMInventoryContract
    {
        public const string Namespace = "http://machineintegration.ws.atlmes.com/";
    }

    [GeneratedCode("wsdl", "4.8")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [WebServiceBinding(Name = "MiCheckBOMInventoryServiceBinding", Namespace = MiCheckBOMInventoryContract.Namespace)]
    public class MiCheckBOMInventoryServiceService : SoapHttpClientProtocol
    {
        [SoapDocumentMethod("", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlElement("miCheckBOMInventoryResponse", Namespace = MiCheckBOMInventoryContract.Namespace)]
        public miCheckBOMInventoryResponse miCheckBOMInventory(
            [XmlElement("miCheckBOMInventory", Namespace = MiCheckBOMInventoryContract.Namespace)] miCheckBOMInventory request)
        {
            object[] results = Invoke("miCheckBOMInventory", new object[] { request });
            return (miCheckBOMInventoryResponse)results[0];
        }
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = MiCheckBOMInventoryContract.Namespace)]
    public class miCheckBOMInventory
    {
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public CheckBOMInventoryRequest CheckBOMInventoryRequest;
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = MiCheckBOMInventoryContract.Namespace)]
    public class CheckBOMInventoryRequest
    {
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string site;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string operation;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string operationRevision;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string resource;

        [XmlElement("parameterArray", Form = XmlSchemaForm.Qualified)]
        public CheckBOMInventoryParameter[] parameterArray;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string user;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string activity;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string sfc;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string modeCheckOperation;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string modeProcessSFC;
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = MiCheckBOMInventoryContract.Namespace)]
    public class CheckBOMInventoryParameter
    {
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string usage;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string category;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string dataField;
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = MiCheckBOMInventoryContract.Namespace)]
    public class miCheckBOMInventoryResponse
    {
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public CheckBOMInventoryResponse @return;
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = MiCheckBOMInventoryContract.Namespace)]
    public class CheckBOMInventoryResponse
    {
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public int code;

        [XmlIgnore]
        public bool codeSpecified;

        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public string message;
    }
}
