using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.CodeSort.Test
{
    public class Class
    {
        private readonly int _age;
        private readonly string _dogName;

        public Class()
        {
            
        }

        private Class(string dogName)
        {
            _dogName = dogName;
        }

        public Class(string fullName, int age)
        {
            FullName = fullName;
            _age = age;
        }

        public void Bark() { }
        public void Run() { }
        private void Analyze() { }

        internal string zipCode;
        public string FullName { get; set; }
        public string _fullName { get; set; }
        private string LasName { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public int Age { get; set; }
        private string dogName;
        private readonly decimal _income;
    }
}
