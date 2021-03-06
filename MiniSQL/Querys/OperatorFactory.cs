﻿using MiniSQL.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSQL.Querys
{
    public class OperatorFactory
    {
        private static OperatorFactory operatorFactory;

        private OperatorFactory() 
        { 
        
        }

        public static OperatorFactory GetOperatorFactory() 
        {
            if (operatorFactory == null) operatorFactory = new OperatorFactory();
            return operatorFactory;
        }

        public Operator GetOperator(string operatorKey) 
        {
            Operator op = null;
            switch (operatorKey) 
            {
                case OperatorKeys.EqualKey:
                    op = Operator.equal;
                    break;
                case OperatorKeys.LessKey:
                    op = Operator.less;
                    break;
                case OperatorKeys.HigherKey:
                    op = Operator.higher;
                    break;                      
            }
            return op;
        }


    }
}
