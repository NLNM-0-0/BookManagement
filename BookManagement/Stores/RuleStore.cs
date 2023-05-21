using BookManagement.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class RuleStore
    {
        private GenericDataRepository<THAMSO> ruleRepo = new GenericDataRepository<THAMSO>();
        public static RuleStore instance;

        private ObservableCollection<THAMSO> currentRules;
        public ObservableCollection<THAMSO> CurrentRules
        {
            get { return currentRules; }
            set
            {
                currentRules = value;
            }
        }
        public int getValue(String ruleName)
        {
            THAMSO rule = currentRules.Single(p=>p.TenThamSo == ruleName);
            return rule.GiaTri;
        }
        
        public RuleStore()
        {
            currentRules = new ObservableCollection<THAMSO>();
        }
    }
}
