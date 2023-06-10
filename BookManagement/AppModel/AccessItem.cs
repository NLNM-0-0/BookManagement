using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class AccessItem : ICloneable
    {
        public int Index { get; set; }
        public CHUCNANG Access { get; set; }
        public bool IsAllowed { get; set; }
        public bool IsEnabled { get; set; }
        public AccessItem(int index, CHUCNANG access, bool isAllowed, bool isEnabled)
        {
            Index = index;
            Access = access;
            IsAllowed = isAllowed;
            IsEnabled = isEnabled;
        }

        public object Clone()
        {
            return new AccessItem
            (
                Index,
                Access,
                IsAllowed,
                IsEnabled
            );
        }
    }
}
