using System;
using System.Collections.Generic;
using System.Linq;
using SasquatchCAIRS;

namespace CAIRSTestProject {
    public class Samples {

        public static IQueryable<Request> getSampleRequests() {
            Request request1 = new Request();
            request1.TimeOpened = new DateTime(2013, 03, 01);
            Request request2 = new Request();
            request2.TimeOpened = new DateTime(2012, 04, 01);
            Request request3 = new Request();
            request3.TimeOpened = new DateTime(2013, 04, 05);
            List<Request> sampleRequests = new List<Request>();
            sampleRequests.Add(request1);
            sampleRequests.Add(request2);
            sampleRequests.Add(request3);

            return sampleRequests.AsQueryable();
        }
    }
}
