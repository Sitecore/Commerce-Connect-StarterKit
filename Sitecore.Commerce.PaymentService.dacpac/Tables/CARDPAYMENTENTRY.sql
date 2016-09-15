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

-- Create table CARDPAYMENTENTRY
CREATE TABLE [dbo].[CARDPAYMENTENTRY](
    [ALLOWVOICEAUTHORIZATION] bit NOT NULL,
    [CARDTYPES] nvarchar(255) NOT NULL,
    [DEFAULTCARDHOLDERNAME] nvarchar(255),
    [DEFAULTCITY] nvarchar(255),
    [DEFAULTCOUNTRYCODE] nvarchar(15),
    [DEFAULTPOSTALCODE] nvarchar(15),
    [DEFAULTSTATEORPROVINCE] nvarchar(255),
    [DEFAULTSTREET1] nvarchar(255),
    [DEFAULTSTREET2] nvarchar(255),
    [ENTRYDATA] nvarchar(max) NOT NULL,
    [ENTRYID] uniqueidentifier NOT NULL,
    [ENTRYLOCALE] nvarchar(15) NOT NULL,
    [ENTRYUTCTIME] datetime NOT NULL,
    [HOSTPAGEORIGIN] nvarchar(255) NOT NULL,
    [INDUSTRYTYPE] nvarchar(255) NOT NULL,
    [RECORDID] bigint NOT NULL IDENTITY(1,1),
    [SERVICEACCOUNTID] nvarchar(255) NOT NULL,
	[SHOWSAMEASSHIPPINGADDRESS] bit NOT NULL,
    [SUPPORTCARDSWIPE] bit NOT NULL,
	[SUPPORTCARDTOKENIZATION] bit NOT NULL,
    [TRANSACTIONTYPE] nvarchar(255) NOT NULL,
    [USED] bit NOT NULL,
CONSTRAINT [PK_CARDPAYMENTENTRY] PRIMARY KEY CLUSTERED
(
    [RECORDID]
),
CONSTRAINT [FK_CARDPAYMENTENTRY_DEFAULTCOUNTRYCODE] FOREIGN KEY ([DEFAULTCOUNTRYCODE]) REFERENCES [dbo].[COUNTRYORREGION]
(
    [TWOLETTERISOCODE]
),
CONSTRAINT [IX_CARDPAYMENTENTRY_ENTRYID] UNIQUE NONCLUSTERED
(
   [ENTRYID]
)
) ON [PRIMARY]
