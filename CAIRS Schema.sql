/*
 * ER/Studio Data Architect 9.5 SQL Code Generation
 * Project :      Sasquatch.dm1
 *
 * Date Created : Friday, April 05, 2013 03:57:27
 * Target DBMS : Microsoft SQL Server 2008
 */

/* 
 * TABLE: AuditLog 
 */

CREATE TABLE AuditLog(
    RequestID    bigint      NOT NULL,
    UserId       int         NOT NULL,
    AuditType    tinyint     NOT NULL,
    AuditDate    datetime    NOT NULL,
    CONSTRAINT PK16 PRIMARY KEY NONCLUSTERED (RequestID, UserId, AuditType, AuditDate)
)
go



IF OBJECT_ID('AuditLog') IS NOT NULL
    PRINT '<<< CREATED TABLE AuditLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE AuditLog >>>'
go

/* 
 * TABLE: Keyword 
 */

CREATE TABLE Keyword(
    KeywordID       int              IDENTITY(1,1),
    KeywordValue    nvarchar(128)    NOT NULL,
    Active          bit              NOT NULL,
    CONSTRAINT PK2 PRIMARY KEY NONCLUSTERED (KeywordID),
    CONSTRAINT UNIQUE1 UNIQUE (KeywordValue)
)
go



IF OBJECT_ID('Keyword') IS NOT NULL
    PRINT '<<< CREATED TABLE Keyword >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Keyword >>>'
go

/* 
 * TABLE: KeywordQuestion 
 */

CREATE TABLE KeywordQuestion(
    KeywordID             int       NOT NULL,
    QuestionResponseID    bigint    NOT NULL,
    RequestID             bigint    NOT NULL,
    CONSTRAINT PK3 PRIMARY KEY CLUSTERED (KeywordID, QuestionResponseID, RequestID)
)
go



IF OBJECT_ID('KeywordQuestion') IS NOT NULL
    PRINT '<<< CREATED TABLE KeywordQuestion >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE KeywordQuestion >>>'
go

/* 
 * TABLE: QuestionResponse 
 */

CREATE TABLE QuestionResponse(
    QuestionResponseID    bigint            NOT NULL,
    RequestID             bigint            NOT NULL,
    Question              nvarchar(1024)    NULL,
    Response              nvarchar(max)     NULL,
    TimeSpent             smallint          NULL,
    SpecialNotes          nvarchar(1024)    NULL,
    Severity              tinyint           NULL,
    Consequence           tinyint           NULL,
    QuestionTypeID        int               NULL,
    TumourGroupID         int               NULL,
    CONSTRAINT PK4 PRIMARY KEY NONCLUSTERED (QuestionResponseID, RequestID)
)
go



IF OBJECT_ID('QuestionResponse') IS NOT NULL
    PRINT '<<< CREATED TABLE QuestionResponse >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE QuestionResponse >>>'
go

/* 
 * TABLE: QuestionType 
 */

CREATE TABLE QuestionType(
    QuestionTypeID    int             IDENTITY(1,1),
    Value             nvarchar(64)    NOT NULL,
    Code              nvarchar(10)    NOT NULL,
    Active            bit             NOT NULL,
    CONSTRAINT PK10 PRIMARY KEY NONCLUSTERED (QuestionTypeID)
)
go



IF OBJECT_ID('QuestionType') IS NOT NULL
    PRINT '<<< CREATED TABLE QuestionType >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE QuestionType >>>'
go

/* 
 * TABLE: Reference 
 */

CREATE TABLE Reference(
    ReferenceID           bigint            NOT NULL,
    QuestionResponseID    bigint            NOT NULL,
    RequestID             bigint            NOT NULL,
    ReferenceType         tinyint           NOT NULL,
    ReferenceString       nvarchar(1024)    NOT NULL,
    CONSTRAINT PK5 PRIMARY KEY NONCLUSTERED (ReferenceID, QuestionResponseID, RequestID)
)
go



IF OBJECT_ID('Reference') IS NOT NULL
    PRINT '<<< CREATED TABLE Reference >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Reference >>>'
go

/* 
 * TABLE: Region 
 */

CREATE TABLE Region(
    RegionID    int             NOT NULL,
    Value       nvarchar(64)    NOT NULL,
    Code        nvarchar(10)    NOT NULL,
    Active      bit             NOT NULL,
    CONSTRAINT PK7 PRIMARY KEY NONCLUSTERED (RegionID),
    CONSTRAINT UNIQUE2 UNIQUE (Value),
    CONSTRAINT UNIQUE3 UNIQUE (Code)
)
go



IF OBJECT_ID('Region') IS NOT NULL
    PRINT '<<< CREATED TABLE Region >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Region >>>'
go

/* 
 * TABLE: Request 
 */

CREATE TABLE Request(
    RequestID            bigint          IDENTITY(10000,1),
    ParentRequestID      bigint          NULL,
    RequestorFName       nvarchar(128)    NULL,
    RequestorLName       nvarchar(128)    NULL,
    RequestorPhone       nvarchar(128)    NULL,
    RequestorPhoneExt    nvarchar(128)    NULL,
    RequestorEmail       nvarchar(128)    NULL,
    PatientFName         nvarchar(128)    NULL,
    PatientLName         nvarchar(128)    NULL,
    PatientGender        tinyint         NULL,
    PatientAge           tinyint         NULL,
    PatientAgencyID      nvarchar(128)    NULL,
    RequestStatus        tinyint         NOT NULL,
    TimeOpened           datetime        NOT NULL,
    TimeClosed           datetime        NULL,
    RegionID             int             NULL,
    RequestorTypeID      int             NULL,
    CONSTRAINT PK1 PRIMARY KEY CLUSTERED (RequestID)
)
go



IF OBJECT_ID('Request') IS NOT NULL
    PRINT '<<< CREATED TABLE Request >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Request >>>'
go

/* 
 * TABLE: RequestLock 
 */

CREATE TABLE RequestLock(
    RequestID    bigint      NOT NULL,
    UserId       int         NOT NULL,
    StartTime    datetime    NOT NULL,
    CONSTRAINT PK17 PRIMARY KEY CLUSTERED (RequestID, UserId)
)
go



IF OBJECT_ID('RequestLock') IS NOT NULL
    PRINT '<<< CREATED TABLE RequestLock >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE RequestLock >>>'
go

/* 
 * TABLE: RequestorType 
 */

CREATE TABLE RequestorType(
    RequestorTypeID    int             NOT NULL,
    Value              nvarchar(64)    NOT NULL,
    Code               nvarchar(10)    NOT NULL,
    Active             bit             NOT NULL,
    CONSTRAINT PK9 PRIMARY KEY NONCLUSTERED (RequestorTypeID),
    CONSTRAINT UNIQUE4 UNIQUE (Value),
    CONSTRAINT UNIQUE5 UNIQUE (Code)
)
go



IF OBJECT_ID('RequestorType') IS NOT NULL
    PRINT '<<< CREATED TABLE RequestorType >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE RequestorType >>>'
go

/* 
 * TABLE: TumourGroup 
 */

CREATE TABLE TumourGroup(
    TumourGroupID    int             NOT NULL,
    Value            nvarchar(64)    NOT NULL,
    Code             nvarchar(10)    NOT NULL,
    Active           bit             NOT NULL,
    CONSTRAINT PK8 PRIMARY KEY CLUSTERED (TumourGroupID),
    CONSTRAINT UNIQUE6 UNIQUE (Value),
    CONSTRAINT UNIQUE7 UNIQUE (Code)
)
go



IF OBJECT_ID('TumourGroup') IS NOT NULL
    PRINT '<<< CREATED TABLE TumourGroup >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE TumourGroup >>>'
go

/* 
 * TABLE: UserGroup 
 */

CREATE TABLE UserGroup(
    GroupID    int             NOT NULL,
    Value      nvarchar(64)    NOT NULL,
    Code       nvarchar(10)    NOT NULL,
    Active     bit             NOT NULL,
    CONSTRAINT PK15 PRIMARY KEY CLUSTERED (GroupID),
    CONSTRAINT UNIQUE8 UNIQUE (Value),
    CONSTRAINT UNIQUE9 UNIQUE (Code)
)
go



IF OBJECT_ID('UserGroup') IS NOT NULL
    PRINT '<<< CREATED TABLE UserGroup >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserGroup >>>'
go

/* 
 * TABLE: UserGroups 
 */

CREATE TABLE UserGroups(
    GroupID    int    NOT NULL,
    UserId     int    NOT NULL,
    CONSTRAINT PK14 PRIMARY KEY NONCLUSTERED (GroupID, UserId)
)
go



IF OBJECT_ID('UserGroups') IS NOT NULL
    PRINT '<<< CREATED TABLE UserGroups >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserGroups >>>'
go

/* 
 * TABLE: UserProfile 
 */

CREATE TABLE UserProfile(
    UserId          int              IDENTITY(1,1),
    UserName        nvarchar(56)     NOT NULL,
    UserFullName    nvarchar(max)    NULL,
    UserEmail       nvarchar(max)    NULL,
    UserStatus      bit              NOT NULL,
    CONSTRAINT PK11 PRIMARY KEY CLUSTERED (UserId)
)
go



IF OBJECT_ID('UserProfile') IS NOT NULL
    PRINT '<<< CREATED TABLE UserProfile >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserProfile >>>'
go

/* 
 * TABLE: webpages_Membership 
 */

CREATE TABLE webpages_Membership(
    UserId          int             NOT NULL,
    CreateDate      datetime        NULL,
    ConfirmationToken 
                    nvarchar(128)   NULL,
    IsConfirmed     bit             NULL,
    LastPasswordFailureDate
                    datetime        NULL,
    PasswordFailuresSinceLastSuccess
                    int             NOT NULL,
    Password        nvarchar(128)   NOT NULL,
    PasswordChangedDate
                    datetime        NULL,
    PasswordSalt    nvarchar(128)   NOT NULL,
    PasswordVerificationToken
                    nvarchar(128)   NULL,
    PasswordVerificationTokenExpirationDate
                    datetime        NULL,
    CONSTRAINT PK__webpages__1788CC4C37A5467C PRIMARY KEY CLUSTERED (UserId)
)
go



IF OBJECT_ID('webpages_Membership') IS NOT NULL
    PRINT '<<< CREATED TABLE webpages_Membership >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE webpages_Membership >>>'
go

/* 
 * TABLE: webpages_OAuthMembership 
 */

CREATE TABLE webpages_OAuthMembership(
    Provider        nvarchar(30)    NOT NULL,
    ProviderUserId  nvarchar(100)   NOT NULL,
    UserId          int             NOT NULL,
    CONSTRAINT PK__webpages__F53FC0ED33D4B598 PRIMARY KEY CLUSTERED (Provider, ProviderUserId)
)
go



IF OBJECT_ID('webpages_OAuthMembership') IS NOT NULL
    PRINT '<<< CREATED TABLE webpages_OAuthMembership >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE webpages_OAuthMembership >>>'
go


/* 
 * TABLE: webpages_Roles 
 */

CREATE TABLE webpages_Roles(
    RoleId          int             IDENTITY(1,1),
    RoleName        nvarchar(256)   NOT NULL,
    CONSTRAINT PK__webpages__8AFACE1A3D5E1FD2 PRIMARY KEY CLUSTERED (RoleId),
    CONSTRAINT UQ__webpages__8A2B6160403A8C7D UNIQUE (RoleName)
)
go



IF OBJECT_ID('webpages_Roles') IS NOT NULL
    PRINT '<<< CREATED TABLE webpages_Roles >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE webpages_Roles >>>'
go

/* 
 * TABLE: webpages_UsersInRoles 
 */

CREATE TABLE webpages_UsersInRoles(
    UserId          int             NOT NULL,
    RoleId          int             NOT NULL,
    CONSTRAINT PK__webpages__AF2760AD440B1D61 PRIMARY KEY CLUSTERED (UserId, RoleId)
)
go



IF OBJECT_ID('webpages_UsersInRoles') IS NOT NULL
    PRINT '<<< CREATED TABLE webpages_UsersInRoles >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE webpages_UsersInRoles >>>'
go

/* 
 * TABLE: AuditLog 
 */

ALTER TABLE AuditLog ADD CONSTRAINT RefUserProfile53 
    FOREIGN KEY (UserId)
    REFERENCES UserProfile(UserId)
go

ALTER TABLE AuditLog ADD CONSTRAINT RefRequest16 
    FOREIGN KEY (RequestID)
    REFERENCES Request(RequestID)
go


/* 
 * TABLE: KeywordQuestion 
 */

ALTER TABLE KeywordQuestion ADD CONSTRAINT RefKeyword52 
    FOREIGN KEY (KeywordID)
    REFERENCES Keyword(KeywordID)
go

ALTER TABLE KeywordQuestion ADD CONSTRAINT RefQuestionResponse51 
    FOREIGN KEY (QuestionResponseID, RequestID)
    REFERENCES QuestionResponse(QuestionResponseID, RequestID)
go


/* 
 * TABLE: QuestionResponse 
 */

ALTER TABLE QuestionResponse ADD CONSTRAINT RefRequest4 
    FOREIGN KEY (RequestID)
    REFERENCES Request(RequestID)
go

ALTER TABLE QuestionResponse ADD CONSTRAINT RefQuestionType8 
    FOREIGN KEY (QuestionTypeID)
    REFERENCES QuestionType(QuestionTypeID)
go

ALTER TABLE QuestionResponse ADD CONSTRAINT RefTumourGroup45 
    FOREIGN KEY (TumourGroupID)
    REFERENCES TumourGroup(TumourGroupID)
go


/* 
 * TABLE: Reference 
 */

ALTER TABLE Reference ADD CONSTRAINT RefQuestionResponse21 
    FOREIGN KEY (QuestionResponseID, RequestID)
    REFERENCES QuestionResponse(QuestionResponseID, RequestID)
go


/* 
 * TABLE: Request 
 */

ALTER TABLE Request ADD CONSTRAINT RefRegion10 
    FOREIGN KEY (RegionID)
    REFERENCES Region(RegionID)
go

ALTER TABLE Request ADD CONSTRAINT RefRequestorType11 
    FOREIGN KEY (RequestorTypeID)
    REFERENCES RequestorType(RequestorTypeID)
go

ALTER TABLE Request ADD CONSTRAINT RefRequest46 
    FOREIGN KEY (ParentRequestID)
    REFERENCES Request(RequestID)
go


/* 
 * TABLE: RequestLock 
 */

ALTER TABLE RequestLock ADD CONSTRAINT RefUserProfile54 
    FOREIGN KEY (UserId)
    REFERENCES UserProfile(UserId)
go

ALTER TABLE RequestLock ADD CONSTRAINT RefRequest44 
    FOREIGN KEY (RequestID)
    REFERENCES Request(RequestID)
go


/* 
 * TABLE: UserGroups 
 */

ALTER TABLE UserGroups ADD CONSTRAINT RefUserProfile55 
    FOREIGN KEY (UserId)
    REFERENCES UserProfile(UserId)
go

ALTER TABLE UserGroups ADD CONSTRAINT RefUserGroup41 
    FOREIGN KEY (GroupID)
    REFERENCES UserGroup(GroupID)
go

/*
 * TABLE: webpages_UsersInRoles
 */
 
ALTER TABLE webpages_Roles ADD CONSTRAINT fk_RoleId
    FOREIGN KEY (RoleId)
    REFERENCES webpages_Roles(RoleId)
go
