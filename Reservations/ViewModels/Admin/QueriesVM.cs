using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reservations.Models;
using Reservations.Models.Queries;

namespace Reservations.ViewModels.Admin
{
    public class QueriesVM
    {
        public IEnumerable<QueryOneVM> QueriesDataQuery1 { get; set; }
        public IEnumerable<QueryTwoVM> QueriesDataQuery2 { get; set; }
        public IEnumerable<QueryThreeVM> QueriesDataQuery3 { get; set; }
        

    }
}