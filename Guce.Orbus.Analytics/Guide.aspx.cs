using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Guce.Orbus.Analytics
{
    public partial class Guide : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.ClearHeaders();
                Response.ClearContent();
                Response.Redirect("GUIDE.docx");
            }
        }
    }
}