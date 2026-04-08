using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Riverbed.Pricing.Paint.Reports
{
    public partial class CustomerProjectQuoteVI : DevExpress.XtraReports.UI.XtraReport
    {
        public CustomerProjectQuoteVI()
        {
            InitializeComponent();
        }

        public CustomerProjectQuoteVI(Guid customerProjectId)
        {
            InitializeComponent();
            this.Parameters["CustomerProjectGuid"].Value = customerProjectId;            
        }
    }
}
