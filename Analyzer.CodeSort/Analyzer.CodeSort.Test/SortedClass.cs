using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.CodeSort.Test
{
    public class Class
    {

        internal string zipCode;
        private readonly int _age;
        private readonly string _dogName;
        private readonly decimal _income;
        private string dogName;

        public Class()
        {

        }

        public Class(string fullName, int age)
        {
            FullName = fullName;
            _age = age;
        }

        private Class(string dogName)
        {
            _dogName = dogName;
        }
        public string _fullName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string StreetAddress { get; set; }
        private string LasName { get; set; }

        public void Bark() { }
        public void Run() { }
        private void Analyze() { }
    }
}
