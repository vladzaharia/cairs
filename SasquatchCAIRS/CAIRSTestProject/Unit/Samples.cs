using System;
using System.Collections.Generic;
using System.Linq;
using SasquatchCAIRS;

namespace CAIRSTestProject {
    public static class Samples {

        public static IQueryable<Request> getSampleRequests() {
            Request request0 = new Request {
                TimeOpened = new DateTime(2013, 02, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 02, 01, 10, 05, 0),
                RequestID = 0,
                RequestorTypeID = 0,
                RegionID = 0,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            Request request1 = new Request {
                TimeOpened = new DateTime(2013, 02, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 02, 01, 10, 05, 0),
                RequestID = 1,
                RequestorTypeID = 1,
                RegionID = 1,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            Request request2 = new Request {
                TimeOpened = new DateTime(2013, 03, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 03, 01, 10, 05, 0),
                RequestID = 2,
                RequestorTypeID = 2,
                RegionID = null,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            Request request3 = new Request {
                TimeOpened = new DateTime(2013, 04, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 04, 01, 10, 05, 0),
                RequestID = 3,
                RequestorTypeID = null,
                RegionID = 0,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            Request request4 = new Request {
                TimeOpened = new DateTime(2013, 05, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 05, 01, 10, 05, 0),
                RequestID = 4,
                RequestorTypeID = 0,
                RegionID = 1,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            Request request5 = new Request {
                TimeOpened = new DateTime(2013, 02, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 02, 01, 10, 05, 0),
                RequestID = 5,
                RequestorTypeID = 1,
                RegionID = null,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            Request request6 = new Request {
                TimeOpened = new DateTime(2013, 03, 01, 10, 0, 0),
                TimeClosed = new DateTime(2013, 03, 01, 10, 05, 0),
                RequestID = 6,
                RequestorTypeID = 2,
                RegionID = 0,
                RequestStatus =
                    (byte)
                    SasquatchCAIRS.Models.Constants.RequestStatus.Completed
            };

            List<Request> sampleRequests = new List<Request> {
                request0,
                request1,
                request2,
                request3,
                request4,
                request5,
                request6
            };

            return sampleRequests.AsQueryable();
        }

        public static IQueryable<QuestionResponse> getSampleQs() {
            QuestionResponse qR0 = new QuestionResponse {
                QuestionResponseID = 0,
                RequestID = 0,
                TimeSpent = 0,
                TumourGroupID = 0
            };

            QuestionResponse qR1 = new QuestionResponse {
                QuestionResponseID = 1,
                RequestID = 1,
                TimeSpent = 10,
                TumourGroupID = 1
            };

            QuestionResponse qR2 = new QuestionResponse {
                QuestionResponseID = 2,
                RequestID = 2,
                TimeSpent = 20,
                TumourGroupID = null
            };

            QuestionResponse qR3 = new QuestionResponse {
                QuestionResponseID = 3,
                RequestID = 3,
                TimeSpent = 30,
                TumourGroupID = 0
            };

            QuestionResponse qR4 = new QuestionResponse {
                QuestionResponseID = 4,
                RequestID = 4,
                TimeSpent = 40,
                TumourGroupID = 1
            };

            QuestionResponse qR5 = new QuestionResponse {
                QuestionResponseID = 5,
                RequestID = 5,
                TimeSpent = 50,
                TumourGroupID = null
            };

            QuestionResponse qR6 = new QuestionResponse {
                QuestionResponseID = 6,
                RequestID = 6,
                TimeSpent = 60,
                TumourGroupID = 0
            };

            QuestionResponse qR7 = new QuestionResponse {
                QuestionResponseID = 7,
                RequestID = 0,
                TimeSpent = 70,
                TumourGroupID = 1
            };

            QuestionResponse qR8 = new QuestionResponse {
                QuestionResponseID = 8,
                RequestID = 1,
                TimeSpent = 80,
                TumourGroupID = null
            };

            QuestionResponse qR9 = new QuestionResponse {
                QuestionResponseID = 9,
                RequestID = 2,
                TimeSpent = 90,
                TumourGroupID = 0
            };

            List<QuestionResponse> sampleQs = new List<QuestionResponse> {
                qR0,
                qR1,
                qR2,
                qR3,
                qR4,
                qR5,
                qR6,
                qR7,
                qR8,
                qR9
            };

            return sampleQs.AsQueryable();
        } 

        public static IQueryable<Region> getSampleRegions() {
            Region region0 = new Region {
                RegionID = 0,
                Code = "BC",
                Value = "British Columbia",
                Active = true
            };

            Region region1 = new Region {
                RegionID = 1,
                Code = "ON",
                Value = "Ontraio",
                Active = false
            };

            List<Region> sampleRegions = new List<Region> {
                region0,
                region1
            };
            return sampleRegions.AsQueryable();
        }

        public static IQueryable<RequestorType> getSampleCallerTypes() {
            RequestorType requestor0 = new RequestorType {
                RequestorTypeID = 0,
                Code = "ADMIN",
                Value = "Administrator",
                Active = true
            };

            RequestorType requestor1 = new RequestorType {
                RequestorTypeID = 1,
                Code = "DRUG CO",
                Value = "Drug Company",
                Active = false
            };

            RequestorType requestor2 = new RequestorType {
                RequestorTypeID = 2,
                Code = "FAMILY",
                Value = "Family Members",
                Active = false
            };

            List<RequestorType> sampleCallers = new List<RequestorType> {
                requestor0,
                requestor1,
                requestor2
            };

            return sampleCallers.AsQueryable();
        }

        public static IQueryable<TumourGroup> getSampleTumourGroups() {
            TumourGroup tg0 = new TumourGroup {
                TumourGroupID = 0,
                Code = "BR",
                Value = "Breast",
                Active = true
            };

            TumourGroup tg1 = new TumourGroup {
                TumourGroupID = 1,
                Code = "CNS",
                Value = "Central Nervous System",
                Active = false
            };

            List<TumourGroup> sampleTumourGroups = new List<TumourGroup> {
                tg0, tg1
            };

            return sampleTumourGroups.AsQueryable();
        } 
    }
}
