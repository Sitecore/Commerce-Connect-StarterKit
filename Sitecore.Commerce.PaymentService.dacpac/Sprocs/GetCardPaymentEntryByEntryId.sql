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

CREATE PROCEDURE [dbo].[GETCARDPAYMENTENTRYBYENTRYID]
    @id_EntryId                        [uniqueidentifier]
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode                           INT;
    DECLARE @i_Error                                INT;
    SET @i_ReturnCode = 0;

    -- SELECT CARDPAYMENTENTRY
    SELECT [ALLOWVOICEAUTHORIZATION]
      ,[CARDTYPES]
      ,[DEFAULTCARDHOLDERNAME]
      ,[DEFAULTCITY]
      ,[DEFAULTCOUNTRYCODE]
      ,[DEFAULTPOSTALCODE]
      ,[DEFAULTSTATEORPROVINCE]
      ,[DEFAULTSTREET1]
      ,[DEFAULTSTREET2]
      ,[ENTRYDATA]
      ,[ENTRYID]
      ,[ENTRYLOCALE]
      ,[ENTRYUTCTIME]
      ,[HOSTPAGEORIGIN]
      ,[INDUSTRYTYPE]
      ,[RECORDID]
      ,[SERVICEACCOUNTID]
      ,[SHOWSAMEASSHIPPINGADDRESS]
      ,[SUPPORTCARDSWIPE]
      ,[SUPPORTCARDTOKENIZATION]
      ,[TRANSACTIONTYPE]
      ,[USED]
    FROM [dbo].[CARDPAYMENTENTRY] WITH(NOLOCK)
    WHERE [ENTRYID] = @id_EntryId;

    SELECT @i_Error = @@ERROR;
    IF @i_Error <> 0
    BEGIN
        SET @i_ReturnCode = @i_Error;
        GOTO exit_label;
    END;

exit_label:
    RETURN @i_ReturnCode;
END;
GO
