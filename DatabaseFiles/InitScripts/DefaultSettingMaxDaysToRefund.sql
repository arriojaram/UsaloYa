
DECLARE @config XML = 
N'<PairSettingsDto>
  <Key>maxDaysToRefund</Key>
  <Value>0</Value>
</PairSettingsDto>';

-- 1️ Si PaymentsJson es NULL → inicializarlo con la nueva configuración
UPDATE Company
SET PaymentsJson = 
N'<PairSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                xmlns:xsd="http://www.w3.org/2001/XMLSchema">' +
CAST(@config AS NVARCHAR(MAX)) +
N'</PairSettings>'
WHERE PaymentsJson IS NULL;

-- 2️ Si PaymentsJson ya existe pero no tiene maxDaysToRefund → agregarlo
UPDATE Company
SET PaymentsJson.modify('
    insert sql:variable("@config") 
    as last into (/PairSettings)[1]
')
WHERE PaymentsJson IS NOT NULL
  AND PaymentsJson.exist('/PairSettings/PairSettingsDto[Key="maxDaysToRefund"]') = 0;