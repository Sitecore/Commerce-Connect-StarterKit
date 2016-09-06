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

CREATE PROCEDURE [dbo].[GETCARDPAYMENTRESULTBYRESULTACCESSCODE]
    @nvc_ServiceAccountId   [nvarchar](255),
    @id_ResultAccessCode    [uniqueidentifier]
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode                           INT;
    DECLARE @i_RowCount                             INT;
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

    -- UPDATE CARDPAYMENTRESULT, SET RETRIEVED = 1
    UPDATE [dbo].[CARDPAYMENTRESULT]
       SET [RETRIEVED] = 1
    WHERE [RESULTACCESSCODE] = @id_ResultAccessCode
      AND [SERVICEACCOUNTID] = @nvc_ServiceAccountId
      AND [RETRIEVED] = 0;

    SELECT @i_Error = @@ERROR, @i_Rowcount = @@ROWCOUNT;

    IF @i_Error <> 0
    BEGIN
        SET @i_ReturnCode = @i_Error;
        GOTO exit_label;
    END;

    IF @i_RowCount = 0
    BEGIN;
       SET @i_ReturnCode = 100001; -- Not found
       GOTO exit_label;
    END;

    -- SELECT CARDPAYMENTRESULT
    SELECT [ENTRYID]
          ,[RECORDID]
          ,[RETRIEVED]
          ,[RESULTACCESSCODE]
          ,[RESULTDATA]
    FROM [dbo].[CARDPAYMENTRESULT] WITH(NOLOCK)
    WHERE [RESULTACCESSCODE] = @id_ResultAccessCode
      AND [SERVICEACCOUNTID] = @nvc_ServiceAccountId;

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
