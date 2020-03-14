﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSQL.Constants
{
    public class RequestAndRegexConstants
    {
        public const string selectQueryIdentificator = "SELECT";
        public const string insertQueryIdentificator = "INSERT";
        public const string dropTableQueryIdentificator = "DROP TABLE";
        public const string deleteQueryIdentificator = "DELETE";
        public const string updateQueryIdentificator = "UPDATE";
        public const string createTableQueryIdentificator = "CREATE TABLE";
        public static string queryTagName = "query";
        public static string databaseTagName = "database";
        public static string tableTagName = "table";
        public static string selectedColumnTagName = "selectedColumn";
        public static string toEvaluateColumnTagName = "toEvalColumn";
        public static string valueTagName = "value";
        public static string updatedColumnTagName = "updatedColumn";
        public static string updatedValueTagName = "toValue";
        public static string operatorTagName = "operator";
        public static string evalValueTagName = "evalValue";
        public static string columnTagName = "column";
        public static string columnTypeTagName = "columnType";

        public static string queryGroup = "?<" + queryTagName +">";
        public static string databaseGroup = "?<" + databaseTagName + ">";
        public static string tableGroup = "?<" +tableTagName + ">";
        public static string selectedColumnGroup = "?<" + selectedColumnTagName + ">";
        public static string toEvaluateColumnGroup = "?<" + toEvaluateColumnTagName + ">";
        public static string valueGroup = "?<" + valueTagName + ">";
        public static string updatedColumnGroup = "?<" + updatedColumnTagName + ">";
        public static string updatedvalueGroup = "?<" + updatedValueTagName + ">";
        public static string operatorGroup = "?<" + operatorTagName + ">";
        public static string evalValueGroup = "?<" + evalValueTagName + ">";
        public static string columnGroup = "?<" + columnTagName + ">";
        public static string columnTypeGroup = "?<" + columnTypeTagName + ">";

        public static string columnsTypes = TypesKeyConstants.IntTypeKey + "|" + TypesKeyConstants.DoubleTypeKey + "|" + TypesKeyConstants.StringTypeKey;
        public static string operators = "[" + OperatorKeys.EqualKey + OperatorKeys.HigherKey + OperatorKeys.LessKey + "]";
        public static string NAINCG = "[^\\* ,<=>\\(\\)]";
        public static string wherePatern = "WHERE (" + toEvaluateColumnGroup + NAINCG + "+)(" + operatorGroup + operators + ")(" + evalValueGroup + NAINCG + "+)";
        public static string fromPattern = "FROM (" + databaseGroup + NAINCG + "+)\\.("+ tableGroup + NAINCG + "+)(?: " + wherePatern + ")";
        public static string selectPattern = "^(" + queryGroup + selectQueryIdentificator + ") (?:(" + selectedColumnGroup  + "\\*)|(" + selectedColumnGroup + NAINCG + "+)(?:,(" + selectedColumnGroup + NAINCG + "+))*) " + fromPattern + "?;$";
        //public static string insertPattern = "^(" + queryGroup + insertQueryIdentificator + ") INTO (" + databaseGroup + NAINCG + "+)\\.(" + tableGroup + NAINCG + "+)(?:\\((" + selectedColumnGroup + NAINCG + "+)(?:,(" + selectedColumnGroup + NAINCG + "+))*\\))? VALUES\\((" + valueGroup + NAINCG + "+)(?:,(" + valueGroup + NAINCG + "+))*\\);$";
        public static string insertPattern = "^(" + queryGroup + insertQueryIdentificator + ") INTO (" + databaseGroup + NAINCG + "+)\\.(" + tableGroup + NAINCG + "+) VALUES\\((" + valueGroup + NAINCG + "+)(?:,(" + valueGroup + NAINCG + "+))*\\);$";
        public static string dropPattern = "^(" + queryGroup + dropTableQueryIdentificator + ") (" + databaseGroup + NAINCG + "+)\\.(" + tableGroup + NAINCG + "+);$";
        public static string deletePattern = "^(" + queryGroup + deleteQueryIdentificator + ") " + fromPattern + "?;$";
        public static string updatePattern = "^(" + queryGroup + updateQueryIdentificator + ") (" + databaseGroup + NAINCG + "+)\\.(" + tableGroup + NAINCG + "+) SET (" + updatedColumnGroup + NAINCG + "+)=(" + updatedvalueGroup + NAINCG + "+)(?:, (" + updatedColumnGroup + NAINCG + "+)=(" + updatedvalueGroup + NAINCG + "+))* " + wherePatern + ";$";
        public static string createPattern = "^(" + queryGroup + createTableQueryIdentificator + ") (" + databaseGroup + NAINCG + "+)\\.(" + tableGroup + NAINCG + "+) \\((" + columnGroup + NAINCG + "+) (" + columnTypeGroup + columnsTypes + ")(?:, (" + columnGroup + NAINCG + "+) (" + columnTypeGroup + columnsTypes + "))*\\);$";


    }
}
