﻿using MiniSQL.Classes;
using MiniSQL.Constants;
using MiniSQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSQL.Querys
{
    public class Select : DataManipulationQuery
    {
        private List<string> selectedColumnNames;
        public bool selectedAllColumns;
        private List<int> selectedRowsIndexInTable;
             
        public Select(IDatabaseContainer container) : base(container)
        {
            this.selectedColumnNames = new List<string>();
            this.selectedRowsIndexInTable = new List<int>();
        }

        public override void ExecuteParticularQueryAction(ITable table)
        {
            IEnumerator<Row> rowEnumerator = table.GetRowEnumerator();
            int i = 0;
            string result = "";
            while (rowEnumerator.MoveNext()) 
            {
                if (this.whereClause.IsSelected(rowEnumerator.Current)) 
                {
                    this.AddAfectedRow(rowEnumerator.Current);
                    result = result + "\n" + this.GenerateStringResult(rowEnumerator.Current);
                    this.selectedRowsIndexInTable.Add(i);
                }
                i = i + 1;
            }
            this.SetResult(this.GenerateHeader() + result);
        }

        private string GenerateStringResult(Row row) 
        {
            string stringfiedRow = "{";
            IEnumerator<string> selectedColumnsEnumerator = this.selectedColumnNames.GetEnumerator();
            if (selectedColumnsEnumerator.MoveNext())
            {
                stringfiedRow = stringfiedRow + "'" + row.GetCell(selectedColumnsEnumerator.Current).data + "'";
                while (selectedColumnsEnumerator.MoveNext())stringfiedRow = stringfiedRow + ", '" + row.GetCell(selectedColumnsEnumerator.Current).data + "'";          
            }
            return stringfiedRow + "}";        
        }

        private string GenerateHeader() 
        {
            string header = "[";
            IEnumerator<string> columnNameEnumerator = this.selectedColumnNames.GetEnumerator();
            if (columnNameEnumerator.MoveNext())
            {
                header = header + "'" + columnNameEnumerator.Current + "'";
                while (columnNameEnumerator.MoveNext()) header = header + ", '" + columnNameEnumerator.Current + "'";
            }
            return header + "]";        
        }

        public void AddSelectedColumnName(string columnName)
        {
            if (!columnName.Equals("*")) this.selectedColumnNames.Add(columnName);
            else this.selectedAllColumns = true;                             
        }

        protected override void ValidateParameters(ITable table)
        {
            if (this.selectedAllColumns) this.SelectAllColumnsOfATable(table);
            else
            {
                IEnumerator<string> enumerator = selectedColumnNames.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!table.ExistColumn(enumerator.Current)) this.SaveTheError(QuerysStringResultConstants.SelectedColumnDoenstExistError(enumerator.Current));
                }               
            }
        }

        private void SelectAllColumnsOfATable(ITable table) 
        {
            IEnumerator<Column> columnEnumerator = table.GetColumnEnumerator();
            while (columnEnumerator.MoveNext()) this.selectedColumnNames.Add(columnEnumerator.Current.columnName);                            
        }
 
        public IEnumerator<int> GetSelectedRowsIndex()
        {
            return this.selectedRowsIndexInTable.GetEnumerator();
        }

        public override string GetNeededExecutePrivilege()
        {
            return SystemeConstants.SelectPrivilegeName;
        }
    }
}
