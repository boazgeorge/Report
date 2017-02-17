using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Report
{
    class RDLDocument
    {
        public string fileName;
        XmlDocument doc;
        public ArrayList errors = new ArrayList();
        public RDLDocument(string fileName)
        {
            this.fileName = fileName;
        }

        private void initializeXmlDoc()
        {
            string readText = File.ReadAllText(fileName);
            doc = new XmlDocument();
            doc.LoadXml(readText);
        }

       public void validate()
        {
            if (doc == null) initializeXmlDoc();
            try {
                validateDescription();//validate description
            }
            catch(NullReferenceException ex)
            {
                errors.Add("Missing parts in description section");
            }
            try
            {
                validateAuthor();//validate author
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in author section");
            }
            try {
                validateDataSource();//validate data source
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in datasource section");
            }
            try {
                validateWidthSum();//validate width sum
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in header section");
            }
            try {
                validateFirstTablixRow();//validate first tablix row
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in first tablix row section");
            }
            try {
                validatePageHeader();//validate page header
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in page header section");
            }
            try {
                validatePageFooter();//validate footer
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in page footer section");
            }
            try {
                validatePage();//validate page
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in page section");
            }
            try {
                validateTextBoxes();//validate text boxes names
            }
            catch (NullReferenceException ex)
            {
                errors.Add("Missing parts in text box section");
            }
        }

        private void validateDescription()
        {
            XmlNode descriptionNode = doc.GetElementsByTagName("Description")[0];
            if (descriptionNode == null)
            {
                errors.Add("No description found for report");
            }
        }

        private void validateAuthor()
        {
            XmlNode authorNode = doc.GetElementsByTagName("Author")[0];
            if (authorNode == null)
            {
                errors.Add("No author mentioned");
                return;
            }
            string author = authorNode.InnerText;

            if(string.Compare(author, "ARGO Data Resource Corp.", false)!=0)
            {
                errors.Add("Incorrect author");
            }
           
        }

        private void validateDataSource()
        {
            XmlNode dataSourceNode = doc.GetElementsByTagName("DataSources")[0];
            if (dataSourceNode == null)
            {
                errors.Add("There is no data source");
                return;
            }
            bool correct = false;
            foreach(XmlNode x in dataSourceNode.ChildNodes)
            {
                if (x.Name == "DataSource")
                {
                    foreach(XmlAttribute y in x.Attributes)
                    {
                        if(y.Value=="ArgoOlap" || y.Value == "ArgoSql")
                        {
                            correct = true;
                        }
                    }
                }
            }

            if (!correct)
                errors.Add("No ArgoOlap or ArgoSql datasource");
        }

        private void validateWidthSum()
        {
            XmlNode tablixNode=findTablixNode(doc);

            double totalWidth = 0;
            foreach(XmlNode x in tablixNode.ChildNodes)
            {
                string value = x.InnerText;
                Console.WriteLine(value);
                totalWidth += Double.Parse(value.Substring(0, value.Length - 2));
                
            }
            if (totalWidth > 10.25)
            {
                Console.WriteLine("WIDTH GREATER FUCH " + totalWidth);
                errors.Add("Column width sum exceeds maximum value");
            }
        }


        private void validateFirstTablixRow()
        {
            XmlNode tablixRowsNode = findTablixRowsNode(doc);
            XmlNode firstTablixRow = tablixRowsNode.FirstChild;
            foreach(XmlNode d in firstTablixRow.ChildNodes)
            {
                if (d.Name == "Height")
                {
                    if (d.InnerText != "0.5in")
                    {
                        errors.Add("First row column height is not 0.5in");
                    }
                }
            }

          
        }
       
        private void validatePageHeader()
        {
            XmlNode pageHeaderNode = findPageHeader(doc);
            foreach(XmlNode d in pageHeaderNode.ChildNodes)
            {
                if (d.Name == "Height")
                {
                    if (d.InnerText != "0.625in")
                    {
                        errors.Add("Incorrect headaer height");
                    }
                }

                if (d.Name == "PrintOnFirstPage")
                {
                    if (d.InnerText != "true")
                    {
                        errors.Add("Incorrect headaer print on first page value");
                    }
                }

                if (d.Name == "PrintOnLastPage")
                {
                    if (d.InnerText != "true")
                    {
                        errors.Add("Incorrect headaer print on last page value");
                    }
                }
            }
        }


        private void validatePageFooter()
        {
            XmlNode pageHeaderNode = findPageFooter(doc);
            foreach (XmlNode d in pageHeaderNode.ChildNodes)
            {
                if (d.Name == "Height")
                {
                    if (d.InnerText != "0.625in")
                    {
                        errors.Add("Incorrect footer height");
                    }
                }

            }
        }
        private void validatePage()
        {
            XmlNode pageNode = findPageNode(doc);
            foreach(XmlNode d in pageNode)
            {
                if(d.Name== "PageHeight")
                {
                    if (d.InnerText != "8.5in")
                    {
                        errors.Add("Page height invalid");
                    }
                }
                if (d.Name == "PageWidth")
                {
                    if (d.InnerText != "11in")
                    {
                        errors.Add("Page width invalid");
                    }
                }
                if (d.Name == "InteractiveHeight")
                {
                    if (d.InnerText != "0in")
                    {
                        errors.Add("Page interactive height invalid");
                    }
                }
                if (d.Name == "InteractiveWidth")
                {
                    if (d.InnerText != "11in")
                    {
                        errors.Add("Page interactive height invalid");
                    }
                }
                if (d.Name == "LeftMargin")
                {
                    if (d.InnerText != "0.25in")
                    {
                        errors.Add("Page left margin invalid");
                    }
                }
                if (d.Name == "TopMargin")
                {
                    if (d.InnerText != "0.5in")
                    {
                        errors.Add("Page top margin invalid");
                    }
                }
                if (d.Name == "BottomMargin")
                {
                    if (d.InnerText != "0.5in")
                    {
                        errors.Add("Page bottom margin invalid");
                    }
                }
            }
        }

        private void validateTextBoxes()
        {
            findTextBoxNodes(doc);
        }

        /**
            find the tablixcolumns node in doc
        **/
        private XmlNode findTablixNode(XmlNode d)
        {
            if (d.Name == "TablixColumns")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findTablixNode(x);
                if (t != null) return t;
            }
            return null;
        }


        /**
            find the tablixrows node in doc
        **/
        private XmlNode findTablixRowsNode(XmlNode d)
        {
            if (d.Name == "TablixRows")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findTablixRowsNode(x);
                if (t != null) return t;
            }
            return null;
        }
        /**
            find the page header 
        **/
        private XmlNode findPageHeader(XmlNode d)
        {
            if (d.Name == "PageHeader")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findPageHeader(x);
                if (t != null) return t;
            }
            return null;
        }

        /**
            find the page footer
        **/
        private XmlNode findPageFooter(XmlNode d)
        {
            if (d.Name == "PageFooter")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findPageFooter(x);
                if (t != null) return t;
            }
            return null;
        }

        private XmlNode findPageNode(XmlNode d)
        {
            if (d.Name == "Page")
                return d;
            foreach (XmlNode x in d.ChildNodes)
            {
                XmlNode t = findPageNode(x);
                if (t != null) return t;
            }
            return null;
        }


        /**
        find all text boxes and check their names
        **/
        private void findTextBoxNodes(XmlNode d)
        {
            if (d.Name == "Textbox")
            {
                string finalValue = "";
                findValueOfTextBoxNodes(d, ref finalValue);

                //process the content value by removing starting and ending spaces. 
                while(finalValue.Length>0 && finalValue[0]==' ')
                {
                    finalValue=finalValue.Remove(0, 1);
                }
                while(finalValue.Length > 0 && finalValue[finalValue.Length-1]==' ')
                {
                    finalValue = finalValue.Remove(finalValue.Length - 1, 1);
                }
                //remove middle spaces
                finalValue=finalValue.Replace(' ', '_');
                if (d.Attributes.Count>0 )
                {
                    if (d.Attributes[0].Value != ("tb_" + finalValue))
                {
                    Console.WriteLine("tb_"+finalValue);

                    errors.Add(d.Attributes[0].Value + " is not a valid name");
                }
                }

                
            }
            else
            {
                foreach(XmlNode x in d.ChildNodes)
                {
                    findTextBoxNodes(x);
                }
            }
        }

        /**
        find values inside textboxes
        **/
        private void findValueOfTextBoxNodes(XmlNode d,ref string finalValue)
        {
          foreach(XmlNode x in d.ChildNodes)
            {
                if (x.Name == "Value")
                {
                    finalValue = x.InnerText;
                }
                else
                {
                    findValueOfTextBoxNodes(x, ref finalValue);
                }
            }
          
        }
        /**
        find the prompt and validate 
        **/
        private void findPrompt(XmlNode d)
        {
            if (d.Name == "Prompt")
            {
                string promptValue = d.InnerText;
                promptValue=promptValue.Replace(" ","") ;
                string character=promptValue.Substring(Math.Max(0, promptValue.Length - 1));
                if (character != ":")
                    errors.Add("Invalid end of prompt");
                
            }
            else
            {
                foreach (XmlNode x in d.ChildNodes)
                {
                    findPrompt(x);
                }
            }
        }



        /***
        ========================================================================================================
        Fix Coding goes here onwards
        ========================================================================================================
        ***/
        private void fixDescription()
        {
            XmlNode descriptionNode = doc.GetElementsByTagName("Description")[0];
            if (descriptionNode == null)
            {
                XmlElement element = doc.CreateElement("Description");
                element.InnerText = "Here is the description";
                doc.DocumentElement.AppendChild(element);
            }
        }


        private void fixAuthor()
        {
            XmlNode authorNode = doc.GetElementsByTagName("Author")[0];
            if (authorNode == null)
            {
                XmlElement element = doc.CreateElement("Author");
                element.InnerText = "ARGO Data Resource Corp.";
                doc.DocumentElement.AppendChild(element);
                return;
            }
            string author = authorNode.InnerText;

            if (string.Compare(author, "ARGO Data Resource Corp.", false) != 0)
            {
                authorNode.InnerText = "ARGO Data Resource Corp.";
            }

        }

        private void fixDataSource()
        {
            Console.WriteLine("Came to here mans");
            XmlNode dataSourceNode = doc.GetElementsByTagName("DataSources")[0];
            if (dataSourceNode == null)
            {

                XmlElement element = doc.CreateElement("DataSources");
                element.InnerXml = "    <DataSource Name=\"ArgoSql\">< DataSourceReference > ArgoSql </ DataSourceReference >< rd:SecurityType > None </ rd:SecurityType >< rd:DataSourceID > 1f426c32 - 1665 - 4391 - a16d - ccab8293b26e </ rd:DataSourceID ></ DataSource >";
                doc.DocumentElement.AppendChild(element);
                

                return;
            }
            bool correct = false;
            XmlNode dataSource = null;
            foreach (XmlNode x in dataSourceNode.ChildNodes)
            {
                if (x.Name == "DataSource")
                {
                    dataSource = x;
                    foreach (XmlAttribute y in x.Attributes)
                    {
                        if (y.Value == "ArgoOlap" || y.Value == "ArgoSql")
                        {
                            correct = true;
                        }
                    }
                }
            }

            if (!correct && dataSource!=null)
            {
                XmlAttribute xKey = doc.CreateAttribute("name");
                xKey.Value = "ArgoOlap";
                dataSource.Attributes.Append(xKey);

            }

        }
        private void fixWidthSum()
        {
            XmlNode tablixNode = findTablixNode(doc);
            int count = 0;
            double totalWidth = 0;
            foreach (XmlNode x in tablixNode.ChildNodes)
            {
                string value = x.InnerText;
                totalWidth += Double.Parse(value.Substring(0, value.Length - 2));
                count += 1;
            }
            if (totalWidth > 10.25)
            {
                double decrement = (10.25) / (1+count);
                foreach (XmlNode x in tablixNode.ChildNodes)
                {

                    string value = x.InnerText;
                    x.InnerXml= "<Width>" + decrement+ "in</Width>";
                    
                }
            }
        }
        private void fixFirstTablixRow()
        {
            XmlNode tablixRowsNode = findTablixRowsNode(doc);
            XmlNode firstTablixRow = tablixRowsNode.FirstChild;
            foreach (XmlNode d in firstTablixRow.ChildNodes)
            {
                if (d.Name == "Height")
                {
                    if (d.InnerText != "0.5in")
                    {
                        d.InnerText = "0.5in";
                    }
                }
            }


        }

        private void fixPageHeader()
        {
            XmlNode pageHeaderNode = findPageHeader(doc);
            foreach (XmlNode d in pageHeaderNode.ChildNodes)
            {
                if (d.Name == "Height")
                {
                    if (d.InnerText != "0.625in")
                    {
                        d.InnerText = "0.625in";
                    }
                }

                if (d.Name == "PrintOnFirstPage")
                {
                    if (d.InnerText != "true")
                    {
                        d.InnerText = "true";
                    }
                }

                if (d.Name == "PrintOnLastPage")
                {
                    if (d.InnerText != "true")
                    {
                        d.InnerText = "true";
                    }
                }
            }
        }


        private void fixPageFooter()
        {
            XmlNode pageHeaderNode = findPageFooter(doc);
            foreach (XmlNode d in pageHeaderNode.ChildNodes)
            {
                if (d.Name == "Height")
                {
                    if (d.InnerText != "0.625in")
                    {
                        d.InnerText = "0.625in";
                    }
                }

            }
        }
        private void fixPage()
        {
            XmlNode pageNode = findPageNode(doc);
            foreach (XmlNode d in pageNode)
            {
                if (d.Name == "PageHeight")
                {
                    if (d.InnerText != "8.5in")
                    {
                        d.InnerText = "8.5in";
                    }
                }
                if (d.Name == "PageWidth")
                {
                    if (d.InnerText != "11in")
                    {
                        d.InnerText = "11in";
                    }
                }
                if (d.Name == "InteractiveHeight")
                {
                    if (d.InnerText != "0in")
                    {
                        d.InnerText = "0in";
                    }
                }
                if (d.Name == "InteractiveWidth")
                {
                    if (d.InnerText != "11in")
                    {
                        d.InnerText = "11in";
                    }
                }
                if (d.Name == "LeftMargin")
                {
                    if (d.InnerText != "0.25in")
                    {
                        d.InnerText = "0.25in";
                    }
                }
                if (d.Name == "TopMargin")
                {
                    if (d.InnerText != "0.5in")
                    {
                        d.InnerText = "0.5in";
                    }
                }
                if (d.Name == "BottomMargin")
                {
                    if (d.InnerText != "0.5in")
                    {
                        d.InnerText = "0.5in";
                    }
                }
            }
        }


       
        public void fixRDL()
        {
            fixDescription();
            fixAuthor();
            fixDataSource();
            fixWidthSum();
            fixFirstTablixRow();
            fixPageHeader();
            fixPageFooter();
            fixPage();
            //fixTextBoxNodes(doc);
            doc.Save(fileName);
            errors.Clear();
        }

    }
}
