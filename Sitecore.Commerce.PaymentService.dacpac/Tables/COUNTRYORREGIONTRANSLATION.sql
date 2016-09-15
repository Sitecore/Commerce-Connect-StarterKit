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

-- Create table COUNTRYORREGIONTRANSLATION
CREATE TABLE [dbo].[COUNTRYORREGIONTRANSLATION](
    [COUNTRYCODE] nvarchar(15) NOT NULL,
    [LOCALE] nvarchar(15) NOT NULL,
    [LONGNAME] nvarchar(255) NOT NULL,
    [RECORDID] bigint NOT NULL IDENTITY(1,1),
    [SHORTNAME] nvarchar(255) NOT NULL,
CONSTRAINT [PK_COUNTRYORREGIONTRANSLATION] PRIMARY KEY CLUSTERED
(
    [RECORDID]
),
CONSTRAINT [FK_COUNTRYORREGIONTRANSLATION_COUNTRYCODE] FOREIGN KEY ([COUNTRYCODE]) REFERENCES [dbo].[COUNTRYORREGION]
(
    [TWOLETTERISOCODE]
),
CONSTRAINT [IX_COUNTRYORREGIONTRANSLATION_TWOLETTERISOCODE] UNIQUE NONCLUSTERED
(
   [COUNTRYCODE], [LOCALE]
)
) ON [PRIMARY]
