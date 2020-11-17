//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class BILL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BILL()
        {
            this.BILLINFOes = new HashSet<BILLINFO>();
        }
    
        public int BillID { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public Nullable<int> ToTalPrice { get; set; }
        public Nullable<int> CustomID { get; set; }
        public Nullable<int> EmployID { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<int> ShippingCost { get; set; }
        public Nullable<int> Sale { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> Payment { get; set; }
        public string VoucherID { get; set; }
    
        public virtual CUSTOMER CUSTOMER { get; set; }
        public virtual EMPLOYEE EMPLOYEE { get; set; }
        public virtual VOUCHER VOUCHER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BILLINFO> BILLINFOes { get; set; }
    }
}
