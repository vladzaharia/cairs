using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem{
    public class ReportInput {

        public Constants.ReportType reportType {
            get; set; }

        public int monthChosen {
            get; set; }

        public int startYear {
            get; set; }

        public int endYear {
            get; set; }

        public DateTime sTime {
            get; set; }

        public DateTime eTime {
            get; set; }

        public List<Constants.DataType> dataTypes {
            get; set; }

        public List<Constants.StratifyOption> stratifyOptions {
            get; set; }
    }
}