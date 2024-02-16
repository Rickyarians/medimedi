using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Confluent.Kafka;
using CollectionServices.BusinessObject.Kafka;
using Microsoft.EntityFrameworkCore;

namespace CRUDServices.BusinessObjects.Code;

public class BOGeneral
{
   

    public static string GenerateXml(XmlSerializer xml, object? @object)
    {
        string strXml;
        using (var sww = new StringWriter())
        {
            using (var writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented })
            {
                xml.Serialize(writer, @object);
                strXml = sww.ToString();
            }
        }

        return strXml.Replace("utf-16", "utf-8");
    }

    public static string EncodeBase64(string value)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(valueBytes);
    }

    public static string DecodeBase64(string value)
    {
        try
        {
            var valueBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
        catch 
        {
            return "";
        }
    }



   

   








}