﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
	// consists of all the constants that are used in the projects
	public class SD
	{
		//defined roles
		public const string Role_Customer = "Customer";
        public const string Role_Admin= "Admin";
        public const string Role_Company= "Company";
        public const string Role_Employee= "Employee";

		//order status
		public const string StatusPending = "Pending";
		public const string StatusApproved= "Approved";
		public const string StatusInProgress= "Processing";
		public const string StatusShipped= "Shipped";
		public const string StatusCancelled= "Cancelled";
		public const string StatusRefunded= "Refunded";

		//payment status
		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayment = "ApprovedDelayedPayment";
		public const string PaymentStatusRejected = "Rejected";

		//user session
		public const string SessionCart = "SessionShoppingCart";
	}
}
