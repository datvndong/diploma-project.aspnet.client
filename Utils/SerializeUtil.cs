using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CentralizedDataSystem.Utils {
    public class SerializeUtil {
        public static string SerializeAnObject(object AnObject) {
            XmlSerializer Xml_Serializer = new XmlSerializer(AnObject.GetType());
            StringWriter Writer = new StringWriter();

            Xml_Serializer.Serialize(Writer, AnObject);
            return Writer.ToString();
        }

        public static object DeSerializeAnObject(string XmlOfAnObject, Type ObjectType) {
            StringReader StrReader = new StringReader(XmlOfAnObject);
            XmlSerializer Xml_Serializer = new XmlSerializer(ObjectType);
            XmlTextReader XmlReader = new XmlTextReader(StrReader);

            try {
                object AnObject = Xml_Serializer.Deserialize(XmlReader);
                return AnObject;
            } finally {
                XmlReader.Close();
                StrReader.Close();
            }
        }
    }
}