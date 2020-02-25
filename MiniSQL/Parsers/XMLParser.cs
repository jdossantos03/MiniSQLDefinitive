﻿using MiniSQL.Classes;
using MiniSQL.Constants;
using MiniSQL.DataTypes;
using MiniSQL.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MiniSQL.Parsers
{
    class XMLParser : AbstractParser
    {
        private string[] xmlDeclaration;

        public XMLParser() 
        {
            this.xmlDeclaration = new string[] { "1.0", null, null };
        }

        public override void DeleteDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void DeleteTable(string databaseName, string tableName)
        {
            throw new NotImplementedException();
        }

        public override bool ExistDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override bool ExistTable(string databaseName, string tableName)
        {
            throw new NotImplementedException();
        }

        public override Database LoadDatabase(string databaseName)
        {
            Database database = new Database(databaseName, null, null);
            XmlDocument databaseProperties = new XmlDocument();
            databaseProperties.Load(this.GetUbicationManager().GetDatabasePropertiesFilePath(databaseName) + ".xml");
            XmlNodeList tableNodes = databaseProperties.GetElementsByTagName(XMLTagsConstants.DatabasePropertiesTableElementTag_WR);
            IEnumerator tableNodesEnumerator = tableNodes.GetEnumerator();
            while (tableNodesEnumerator.MoveNext())
            {
                database.AddTable(this.LoadTable(database.databaseName, ((XmlNode)tableNodesEnumerator.Current).InnerText));
            }
            return database;
        }

        public override Table LoadTable(string databaseName, string tableName)
        {
            Table table = this.LoadTableStructure(databaseName, tableName);
            this.LoadTableData(databaseName, table);
            return table;            
        }

        private Table LoadTableStructure(string databaseName, string tableName)
        {
            Table table = new Table(tableName);
            XmlDocument tableStructDocument = new XmlDocument();
            tableStructDocument.Load(this.GetUbicationManager().GetTableStructureFilePath(databaseName, tableName) + ".xml");
            IEnumerator enumerator = tableStructDocument.GetElementsByTagName(XMLTagsConstants.TableStructureColumnElementTag_WR).GetEnumerator();
            XmlNode xmlNode;
            while (enumerator.MoveNext())
            {
                //https://www.w3schools.com/xml/xpath_syntax.asp ostia tu que guapo
                xmlNode = (XmlNode)enumerator.Current;
                table.AddColumn(new Column(xmlNode.SelectSingleNode(XMLTagsConstants.TableStructureColumnNameTag_WR).InnerText, DataTypesFactory.GetDataTypesFactory().GetDataType(xmlNode.SelectSingleNode(XMLTagsConstants.TableStructureColumnDataTypeTag_WR).InnerText)));
            }
            return table;
        }

        private void LoadTableData(string databaseName, Table table)
        {
            XmlDocument tableDataDocument = new XmlDocument();
            tableDataDocument.Load(this.GetUbicationManager().GetTableDataFilePath(databaseName, table.tableName) + ".xml");
            IEnumerator rowEnumerator = tableDataDocument.GetElementsByTagName(XMLTagsConstants.TableDataRowElementTag_WR).GetEnumerator();
            IEnumerator cellEnumerator;
            XmlNode xmlCellNode;
            Row row;
            while (rowEnumerator.MoveNext())
            {
                row = table.CreateRowDefinition();
                cellEnumerator = ((XmlNode)rowEnumerator.Current).SelectNodes(XMLTagsConstants.TableDataCellElementTag_WR).GetEnumerator();
                while (cellEnumerator.MoveNext())
                {
                    xmlCellNode = (XmlNode)cellEnumerator.Current;
                    row.GetCell(xmlCellNode.Attributes.GetNamedItem(XMLTagsConstants.TableDataCellColumnNameAtributeTag_WR).InnerText).data = xmlCellNode.InnerText;
                }
                table.AddRow(row);
            }
        }

        public override void SaveDatabase(Database database)
        {
            XmlDocument databaseProperties = new XmlDocument();
            XmlElement databaseElement = databaseProperties.CreateElement(XMLTagsConstants.DatabasePropertiesRootElementTag_WR);
            IUbicationManager ubicationManager = this.GetUbicationManager();
            string databasePath = ubicationManager.GetDatabaseFilePath(database.databaseName);
            if (!Directory.Exists(databasePath)) Directory.CreateDirectory(databasePath);
            IEnumerator<KeyValuePair<string, Table>> enumerator = database.ReadTables().GetEnumerator();
            while (enumerator.MoveNext()) 
            {
                databaseElement.AppendChild(this.CreateSimpleNode(databaseProperties, XMLTagsConstants.DatabasePropertiesTableElementTag_WR, enumerator.Current.Value.tableName));
                this.SaveTable(database, enumerator.Current.Value);
            }
            databaseProperties.AppendChild(databaseElement);
            databaseProperties.InsertBefore(databaseProperties.CreateXmlDeclaration(this.xmlDeclaration[0], this.xmlDeclaration[1], this.xmlDeclaration[2]), databaseElement);
            databaseProperties.Save(ubicationManager.GetDatabasePropertiesFilePath(database.databaseName) + ".xml");
        }

        public override void SaveTable(Database database, Table table)
        {
            this.SaveTableStruct(table).Save(this.GetUbicationManager().GetTableStructureFilePath(database.databaseName, table.tableName) + ".xml");
            this.SaveTableData(table).Save(this.GetUbicationManager().GetTableDataFilePath(database.databaseName, table.tableName) + ".xml");
        }

        private XmlDocument SaveTableStruct(Table table) 
        {
            XmlDocument tableStructureXML = new XmlDocument();            
            XmlElement tableElement = tableStructureXML.CreateElement(XMLTagsConstants.TableStructureRootElementTag_WR);
            IEnumerator<Column> enumerator = table.GetColumnList().GetEnumerator();
            XmlElement column;
            while (enumerator.MoveNext())
            {
                column = tableStructureXML.CreateElement(XMLTagsConstants.TableStructureColumnElementTag_WR);
                column.AppendChild(this.CreateSimpleNode(tableStructureXML, XMLTagsConstants.TableStructureColumnNameTag_WR, enumerator.Current.columnName));
                column.AppendChild(this.CreateSimpleNode(tableStructureXML, XMLTagsConstants.TableStructureColumnDataTypeTag_WR, enumerator.Current.dataType.GetSimpleTextValue()));
                tableElement.AppendChild(column);
            }
            tableStructureXML.AppendChild(tableElement);
            tableStructureXML.InsertBefore(tableStructureXML.CreateXmlDeclaration(this.xmlDeclaration[0], this.xmlDeclaration[1], this.xmlDeclaration[2]), tableElement);
            return tableStructureXML;
        }

        private XmlDocument SaveTableData(Table table) 
        {
            XmlDocument tableData = new XmlDocument();
            XmlElement tableElement = tableData.CreateElement(XMLTagsConstants.TableDataRootElementTag_WR);
            XmlElement rowElement;
            XmlElement cellElement;
            IEnumerator<Row> rowEnumerator = table.GetRowEnumerator();
            IEnumerator<Cell> cellEnumerator;
            while (rowEnumerator.MoveNext()) 
            {
                rowElement = tableData.CreateElement(XMLTagsConstants.TableDataRowElementTag_WR);
                cellEnumerator = rowEnumerator.Current.GetCellEnumerator();
                while (cellEnumerator.MoveNext()) 
                {
                    cellElement = this.CreateSimpleNode(tableData, XMLTagsConstants.TableDataCellElementTag_WR, cellEnumerator.Current.column.dataType.ParseToSaveData(cellEnumerator.Current.data));
                    cellElement.SetAttribute(XMLTagsConstants.TableStructureColumnNameTag_WR, cellEnumerator.Current.column.columnName);
                    rowElement.AppendChild(cellElement);
                }
                tableElement.AppendChild(rowElement);
            }
            tableData.AppendChild(tableElement);
            tableData.InsertBefore(tableData.CreateXmlDeclaration(this.xmlDeclaration[0], this.xmlDeclaration[1], this.xmlDeclaration[2]), tableElement);
            return tableData;
        }

        private XmlElement CreateSimpleNode(XmlDocument xmlDocument, string nodeName, string nodeText) 
        {
            XmlElement element = xmlDocument.CreateElement(nodeName);
            XmlText textNode = xmlDocument.CreateTextNode(nodeText);
            element.AppendChild(textNode);
            return element;
        }

    }
}
//Delimitator version