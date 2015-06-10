C#从Sharepoint上获取List数据：

添加引用：

using Microsoft.SharePoint.Client;
using System.Data;

源码：
 public static void getDataFromSPList(string siteURL, string listName)
        {
            ClientContext clientContext = new ClientContext(siteURL);
            Web web = clientContext.Web;
            ListCollection collList = web.Lists;

            clientContext.Load(collList);
            clientContext.ExecuteQuery();   //Sharepoint的远程交互只有在ExecuteQuery之后才被执行的。

            foreach (Microsoft.SharePoint.Client.List list in collList)
            {
                Console.WriteLine("Title:{0}",list.Title);
            }

            //CamlQuery
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "";

            Microsoft.SharePoint.Client.List planlist = collList.GetByTitle(listName);
            ListItemCollection collListItem = planlist.GetItems(camlQuery);
            clientContext.Load(collListItem,
                items=>items.Include(
                    item=>item.Id,
                    item=>item.DisplayName,
                    item=>item.HasUniqueRoleAssignments)
                );
            clientContext.ExecuteQuery();

            foreach (ListItem olistItem in collListItem)
            {
                Console.WriteLine("ID: {0} \nDisplay name: {1} \nUnique role assignments: {2}",
                    olistItem.Id, olistItem.DisplayName, olistItem.HasUniqueRoleAssignments);
            }
            
            
            clientContext.Load(planlist);
            clientContext.ExecuteQuery();
            Console.WriteLine(planlist.Title.ToString());
            Console.WriteLine(planlist.ItemCount.ToString());
            Console.ReadKey();
        }
        


//Get List Infomation
        private void GetList()
        {
            try
            {
                WebServices1.Lists listService=new GetListTest.WebServices1.Lists();
                listService.Credentials=System.Net.CredentialCache.DefaultCredentials;
                XmlNode ndLists = listService.GetList("Test");//参数列表名，String类型。
                Console.Write(ndLists.OuterXml);
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);                
            }
            
        }
        
        private void GetListItem()
        {
                WebServices1.Lists listService=new GetListTest.WebServices1.Lists();
                listService.Credentials=System.Net.CredentialCache.DefaultCredentials;
                XmlDocument xmlDoc=new System.Xml.XmlDocument();
                XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element,"Query","");
                XmlNode ndViewFields=xmlDoc.CreateNode(XmlNodeType.Element,"ViewFields","");
                XmlNode ndQueryOptions=xmlDoc.CreateNode(XmlNodeType.Element,"QueryOptions","");
                ndQueryOptions.InnerXml="";//Query 设置
                ndViewFields.InnerXml="";//视图 设置
                ndQuery.InnerXml="";//Caml 设置
                
                try
                {
                    XmlNode ndListItems=listService.GetListItems("Test",null,ndQuery,ndViewFields,"1",ndQueryOptions,null);//获取列表内容
                    Console.Write(ndListItems.OuterXml);//输出获取的xml内容
                }
                catch(System.Web.Services.Protocols.SoapException ex)
                {
                    
                }
            
        }
        
        private void UpdateItem()
        {
                WebServices1.Lists listService=new GetListTest.WebServices1.Lists();
                listService.Credentials=System.Net.CredentialCache.DefaultCredentials;
                string strBatch="<Method ID='1' Cmd='Update'>"+ //cmd 参数，update为更新，还有new，delete
                                "<Field Name='ID'>1</Field>"+ //Name属性为字段名称，里面为字段值
                                "<Field Name='Title'>这个已经被修改</Field>"；
                
                XmlDocument xmlDoc=new System.Xml.XmlDocument();
                XmlElement elBatch=xmlDoc.CreateElement("Batch");
                elBatch.InnerXml = strBatch;
                XmlNode ndReturn = listService.UpdateListItems("Test",elBatch); //第一个参数是列表名
                Console.Write("操作成功");            
        }