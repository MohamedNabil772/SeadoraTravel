SELECT 'Categories' as tbl, count(*) from "Categories"
UNION ALL
SELECT 'Destinations' as tbl, count(*) from "Destinations"
UNION ALL
SELECT 'Suppliers' as tbl, count(*) from "Suppliers"
UNION ALL
SELECT 'PaymentAgreements' as tbl, count(*) from "PaymentAgreements"
UNION ALL
SELECT 'Tours' as tbl, count(*) from "Tours";
