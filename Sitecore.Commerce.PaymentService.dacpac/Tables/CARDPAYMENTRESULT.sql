----------------------------------------------------------------------------------------------------------------------
-- Copyright 2016 Sitecore Corporation A/S 
-- Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file  
-- except in compliance with the License. You may obtain a copy of the License at 
--       http://www.apache.org/licenses/LICENSE-2.0 
--
-- Unless required by applicable law or agreed to in writing, software distributed under the  
-- License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,  
-- either express or implied. See the License for the specific language governing permissions  
-- and limitations under the License. 
----------------------------------------------------------------------------------------------------------------------

-- Create table CARDPAYMENTRESULT
-- with a foreigh to CARDPAYMENTENTRY on [ENTRYID] (1..1 TO 0..1)
CREATE TABLE [dbo].[CARDPAYMENTRESULT](
    [ENTRYID] uniqueidentifier NOT NULL,
    [RECORDID] bigint NOT NULL IDENTITY(1,1),
    [RETRIEVED] bit NOT NULL,
    [RESULTACCESSCODE] uniqueidentifier NOT NULL,
    [RESULTDATA] nvarchar(max) NOT NULL,
	[SERVICEACCOUNTID] nvarchar(255) NOT NULL,
CONSTRAINT [PK_CARDPAYMENTRESULT] PRIMARY KEY CLUSTERED
(
    [RECORDID]
),
CONSTRAINT [FK_CARDPAYMENTRESULT_ENTRYID] FOREIGN KEY ([ENTRYID]) REFERENCES [dbo].[CARDPAYMENTENTRY]
(
    [ENTRYID]
) ON DELETE CASCADE,
CONSTRAINT [IX_CARDPAYMENTRESULT_RESULTACCESSCODE] UNIQUE NONCLUSTERED
(
   [RESULTACCESSCODE]
),
CONSTRAINT [IX_CARDPAYMENTRESULT_ENTRYID] UNIQUE NONCLUSTERED
(
   [ENTRYID]
)
) ON [PRIMARY]
