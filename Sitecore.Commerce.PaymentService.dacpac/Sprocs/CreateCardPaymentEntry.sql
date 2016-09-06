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

CREATE PROCEDURE [dbo].[CREATECARDPAYMENTENTRY]
    @b_AllowVoiceAuthorization        [bit],
    @nvc_CardTypes                    [nvarchar](255),
    @nvc_DefaultCardHolderName        [nvarchar](255),
    @nvc_DefaultCity                  [nvarchar](255),
    @nvc_DefaultCountryCode           [nvarchar](15),
    @nvc_DefaultPostalCode            [nvarchar](15),
    @nvc_DefaultStateOrProvince       [nvarchar](255),
    @nvc_DefaultStreet1               [nvarchar](255),
    @nvc_DefaultStreet2               [nvarchar](255),
    @nvc_EntryData                    [nvarchar](max),
    @id_EntryId                       [uniqueidentifier],
    @nvc_EntryLocale                  [nvarchar](15),
    @dt_EntryUtcTime                  [datetime],
    @nvc_HostPageOrigin               [nvarchar](255),
    @nvc_IndustryType                 [nvarchar](255),
    @nvc_ServiceAccountId             [nvarchar](255),
    @b_ShowSameAsShippingAddress      [bit],
    @b_SupportCardSwipe               [bit],
    @b_SupportCardTokenization        [bit],
    @nvc_TransactionType              [nvarchar](255),
    @b_Used                           [bit]
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode                           INT;
    DECLARE @i_TransactionIsOurs                    INT;
    DECLARE @i_Error                                INT;

    -- initializes the return code and assume the transaction is not ours by default
    SET @i_ReturnCode = 0;
    SET @i_TransactionIsOurs = 0;

    IF @@TRANCOUNT = 0
    BEGIN
        BEGIN TRANSACTION;

        SELECT @i_Error = @@ERROR;
        IF @i_Error <> 0
        BEGIN
            SET @i_ReturnCode = @i_Error;
            GOTO exit_label;
        END;

        SET @i_TransactionIsOurs = 1;
    END;

    -- INSERT INTO THE CARDPAYMENTENTRY
    IF (@nvc_DefaultCountryCode = '')
        SET @nvc_DefaultCountryCode = NULL;

    INSERT INTO [dbo].[CARDPAYMENTENTRY]
           ([ALLOWVOICEAUTHORIZATION]
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
           ,[SERVICEACCOUNTID]
           ,[SHOWSAMEASSHIPPINGADDRESS]
           ,[SUPPORTCARDSWIPE]
           ,[SUPPORTCARDTOKENIZATION]
           ,[TRANSACTIONTYPE]
           ,[USED])
     VALUES
           (@b_AllowVoiceAuthorization
           ,@nvc_CardTypes
           ,@nvc_DefaultCardHolderName
           ,@nvc_DefaultCity
           ,@nvc_DefaultCountryCode
           ,@nvc_DefaultPostalCode
           ,@nvc_DefaultStateOrProvince
           ,@nvc_DefaultStreet1
           ,@nvc_DefaultStreet2
           ,@nvc_EntryData
           ,@id_EntryId
           ,@nvc_EntryLocale
           ,@dt_EntryUtcTime
           ,@nvc_HostPageOrigin
           ,@nvc_IndustryType
           ,@nvc_ServiceAccountId
           ,@b_ShowSameAsShippingAddress
           ,@b_SupportCardSwipe
           ,@b_SupportCardTokenization
           ,@nvc_TransactionType
           ,@b_Used);

    SELECT @i_Error = @@ERROR;
    IF @i_Error <> 0
    BEGIN
        SET @i_ReturnCode = @i_Error;
        GOTO exit_label;
    END;
    
    IF @i_TransactionIsOurs = 1
    BEGIN
        COMMIT TRANSACTION;

        SET @i_Error = @@ERROR;
        IF @i_Error <> 0
        BEGIN
            SET @i_ReturnCode = @i_Error;
            GOTO exit_label;
        END;

        SET @i_TransactionIsOurs = 0;
    END;

exit_label:

    IF @i_TransactionIsOurs = 1
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    RETURN @i_ReturnCode;
END;
GO
