using System;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    class UserController
	{
        public static void CheckDoc()
        {
            try
            {
                XDocument xDoc = XDocument.Load("users.xml");
            }
            catch(FileNotFoundException)
            {
                XDocument xDoc = new XDocument(new XElement("users", ""));
                xDoc.Save("users.xml");
            }
        }

        public static void CreateUser(long UserId)
        {
            CheckDoc();
            XDocument xDoc = XDocument.Load("users.xml");
            XElement xRoot = xDoc.Element("users");
            XElement newUser = new XElement("user",
                new XAttribute("id", UserId),
                new XElement("university", ""), 
                new XElement("faculty", ""),
                new XElement("course", ""),
                new XElement("group", ""));
			xRoot.Add(newUser);
            xDoc.Save("users.xml");
        }

        public static bool CheckUser(long UserId) //Проверка существования пользователя
        {
            //Делаю через System.Xml
            CheckDoc();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("users.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("user[@id='" + UserId + "']");
            if (node != null)
            {
                return true;
            }
            return false;
        }

        public static void RecreateUser(long UserId)
        {

            //Делаю через System.Xml
            CheckDoc();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("users.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("user[@id='" + UserId + "']");

            xRoot.RemoveChild(node);
            xDoc.Save("users.xml");

            CreateUser(UserId);
        }
		
		public static string GetUserInfo(long UserId, string type)
		{
			//Делаю через System.Xml
			CheckDoc();
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load("users.xml");
			XmlElement xRoot = xDoc.DocumentElement;
			XmlNode node = xRoot.SelectSingleNode("user[@id='" + UserId + "']");

			foreach (XmlNode find in node.ChildNodes)
			{
				if (find.Name == type)
				{
					return find.InnerText;
				}
			}
			return "";
		}

		public static void EditUser(long UserId, string type, string param)
        {
            //Делаю через System.Xml
            CheckDoc();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("users.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("user[@id='" + UserId + "']");

            foreach (XmlNode find in node.ChildNodes)
            {
                if (find.Name == type)
                {
                    node.RemoveChild(find);
                    XmlElement el = xDoc.CreateElement(type);
                    XmlText text = xDoc.CreateTextNode(param);
                    node.AppendChild(el);
                    el.AppendChild(text);
                    xDoc.Save("users.xml");
                    break;
                }
            }
        }
        public static string CheckUserElements(long UserId, string type) 
        {
            //Делаю через System.Xml
            CheckDoc();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("users.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("user[@id='" + UserId + "']");

            string ans ="";
            foreach (XmlNode find in node.ChildNodes) //Запутался
            {
                if (find.Name == type)
                {
                        ans = find.InnerText;
                }
            }
            return ans;
        }
    }
}
