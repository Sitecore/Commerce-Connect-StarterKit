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

CREATE PROCEDURE [dbo].[PURGECARDPAYMENTHISTORY]
    @i_EntryLifeInMinutes    [int]
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode                           INT;
    DECLARE @i_TransactionIsOurs                    INT;
    DECLARE @i_Error                                INT;
    DECLARE @dt_CutoffUtcDateTime                   DATETIME;
    DECLARE @i_BatchIndex                           INT;

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

    -- Calculate cutoff UTC date time
    SET @dt_CutoffUtcDateTime = DATEADD(minute, @i_EntryLifeInMinutes * -1, GETUTCDATE());

    -- Delete the expired CARDPAYMENTENTRY
    -- IT will cascade delete the expired CARDPAYMENTRESULT
    -- Delete in batches to avoid overload on CPU. 
    SET @i_BatchIndex = 1;
    WHILE EXISTS (SELECT 1 FROM [dbo].[CARDPAYMENTENTRY] WHERE ENTRYUTCTIME < @dt_CutoffUtcDateTime)
    BEGIN
        DELETE TOP(10000) FROM [dbo].[CARDPAYMENTENTRY]
        WHERE ENTRYUTCTIME < @dt_CutoffUtcDateTime;

        IF (@i_BatchIndex >= 1000)
            BREAK;

        SET @i_BatchIndex = @i_BatchIndex + 1;
        WAITFOR DELAY '00:00:05';
    END;

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
