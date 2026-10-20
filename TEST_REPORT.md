# Test Report: Vulnerable vs Secure Search

**Date:** 2026-10-20 · **Environment:** local SQL Server Express (`OliveOilDb`), fictional data, 5 products
**Malicious input used:** `' OR 1=1 --` · SQL captured from the EF Core command log (`EnableSensitiveDataLogging`, Development only)

## Results

| Test | Endpoint | Input | Rows returned |
| --- | --- | --- | --- |
| 1 | search-vulnerable | `Nabali` | 2 (Nabali Extra Virgin, Nabali Cold Pressed) |
| 2 | search-secure | `Nabali` | 2 (same) |
| 3 | search-vulnerable | `' OR 1=1 --` | **5: the whole table** |
| 4 | search-secure | `' OR 1=1 --` | **0** |

## SQL trace

**Test 3: vulnerable (`FromSqlRaw` + concatenation)**

```
Parameters=[]
SELECT Id, Name, Region, PricePerLiter, QuantityInStock FROM OliveOilProducts
WHERE Name LIKE '%' OR 1=1 --%'
```

- `Parameters=[]`: no parameters, the input went inside the SQL text.
- `'` closed the string, so the LIKE became `LIKE '%'`.
- `OR 1=1` is always true, so every row matches.
- `--` commented out the leftover `%'`, so the query stayed valid.

**Test 4: secure (`FromSqlInterpolated`)**

```
Parameters=[p0='%' OR 1=1 --%']
SELECT Id, Name, Region, PricePerLiter, QuantityInStock FROM OliveOilProducts
WHERE Name LIKE @p0
```

- The SQL text is fixed: `LIKE @p0`. The input is not in it.
- The whole input travels separately as the value of `@p0`.
- SQL Server searched for a name containing the literal text `' OR 1=1 --`. No product has that name, so 0 rows.

## Conclusion

Normal input gives the same result on both endpoints. The crafted input changed the logic of the vulnerable query, because the input became SQL code. On the secure endpoint the same input stayed data, because the query and the value are sent separately.
