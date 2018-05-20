using System;
using System.Collections.Generic;
using System.Text;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    public class Table : DataTable
    {
        public Table(TableRow[] rows) : base(rows) { }

        public List<T> CreateSet<T>()
        {            
            throw new NotImplementedException("TODO: Make this available like what is in Specflow.Assist.");
        }
        public T CreateInstance<T>()
        {
            throw new NotImplementedException("TODO: Make this available like what is in Specflow.Assist.");
        }
        public void CompareToInstance<T>(T instance)
        {
            throw new NotImplementedException("TODO: Make this available like what is in Specflow.Assist.");
        }
        public void CompareToSet<T>(IEnumerable<T> set, bool sequentialEquality = false)
        {
            throw new NotImplementedException("TODO: Make this available like what is in Specflow.Assist.");
        }
    }
}
