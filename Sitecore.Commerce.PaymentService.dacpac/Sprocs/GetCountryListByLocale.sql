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

CREATE PROCEDURE [dbo].[GETCOUNTRYLISTBYLOCALE]
    @nvc_Locale                    [nvarchar](15)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode                           INT;
    DECLARE @i_Error                                INT;
    SET @i_ReturnCode = 0;
    
    -- SELECT COUNTRIES OR REGIONS
    SELECT C.[TWOLETTERISOCODE]
          ,CT.[LOCALE]
          ,CT.[SHORTNAME]
          ,CT.[LONGNAME]
    FROM [dbo].[COUNTRYORREGION] AS C WITH(NOLOCK)
    LEFT OUTER JOIN [dbo].[COUNTRYORREGIONTRANSLATION] AS CT WITH(NOLOCK)
    ON CT.[COUNTRYCODE] = C.[TWOLETTERISOCODE]
    WHERE CT.LOCALE = @nvc_Locale;

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
