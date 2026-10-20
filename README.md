# Practical Task: SQL Injection — Palestinian Olive Oil API

**Student:** Izzaldeen · **Stack:** ASP.NET Core (.NET 10) + EF Core 10 + SQL Server Express (local)

## How to run

```bash
dotnet ef database update      # creates OliveOilDb with 5 fictional products
dotnet run --launch-profile http
```

Swagger: `http://localhost:5149/swagger` · Automated comparison: `powershell -File run-tests.ps1`

| Endpoint | Part | Implementation |
| --- | --- | --- |
| `GET /api/OliveOilProducts/search?term=` | 2 | LINQ `Contains` (EF generates parameters) |
| `GET /api/OliveOilProducts/search-vulnerable?term=` | 3 | `FromSqlRaw` + string concatenation (**intentionally vulnerable**) |
| `GET /api/OliveOilProducts/search-secure?term=` | 4 | `FromSqlInterpolated` (parameterized) |

Sample data (fictional): Nabali Extra Virgin (Nablus), Souri Premium (Jenin), Rumi Classic (Tulkarm), Nabali Cold Pressed (Salfit), Souri Organic (Ramallah).

## 1. What is SQL Injection?

SQL Injection is a vulnerability where user input is concatenated into a SQL statement, so the database cannot tell the developer's SQL code apart from the user's data. An attacker sends input containing SQL syntax (for example a `'` to close the string, then `OR`, `UNION`, or `--`) and changes the meaning of the query: reading data they should not see, bypassing filters or logins, or modifying data.

## 2. Test with normal input

Input `Nabali` → both endpoints return **2 rows** (Id 1 Nabali Extra Virgin, Id 4 Nabali Cold Pressed).
Query executed by the vulnerable endpoint:

```sql
SELECT Id, Name, Region, PricePerLiter, QuantityInStock FROM OliveOilProducts WHERE Name LIKE '%Nabali%'
```

## 3. Query alteration with crafted input (local only)

Actual SQL captured from the EF Core command log:

| Input | SQL sent by the vulnerable endpoint | Result |
| --- | --- | --- |
| `' OR 1=1 --` | `... WHERE Name LIKE '%' OR 1=1 --%'` | **5 rows**: every product, the filter is bypassed |
| `xyz' OR Region = 'Jenin' --` | `... WHERE Name LIKE '%xyz' OR Region = 'Jenin' --%'` | **1 row** (Souri Premium): attacker wrote their own condition |
| `' UNION SELECT object_id, name, 'sys.tables', 0, 0 FROM sys.tables --` | `... LIKE '%' UNION SELECT object_id, name, 'sys.tables', 0, 0 FROM sys.tables --%'` | **7 rows**: the 5 products plus the database's table names (`__EFMigrationsHistory`, `OliveOilProducts`) |

## 4. Why this happens

The code builds the query with `"... LIKE '%" + term + "%'"`. The input becomes part of the SQL **text** before it reaches SQL Server:

- `'` closes the string literal early.
- `OR 1=1` / `UNION SELECT ...` adds new SQL logic chosen by the attacker.
- `--` comments out the rest of the original query (`%'`), so the statement stays valid.

SQL Server receives one string and parses all of it as code. It has no way to know which part came from the user. The third test shows the impact goes beyond this table: an attacker can read the database's metadata, then other tables.

## 5. Secure implementation and comparison

```csharp
var pattern = "%" + term + "%";
db.OliveOilProducts.FromSqlInterpolated($"SELECT ... FROM OliveOilProducts WHERE Name LIKE {pattern}");
```

Query actually sent for **every** input (from the log): `... WHERE Name LIKE @p0`, with the input passed separately as parameter `@p0`.

| Input | Vulnerable | Secure |
| --- | --- | --- |
| `Nabali` | 2 rows | 2 rows |
| `' OR 1=1 --` | 5 rows (all) | 0 rows |
| `xyz' OR Region = 'Jenin' --` | 1 row (Jenin) | 0 rows |
| `' UNION SELECT ... FROM sys.tables --` | 7 rows (incl. table names) | 0 rows |

Normal input behaves the same in both. Malicious input changes the vulnerable query, but the secure one just searches for a product literally named `' OR 1=1 --`, finds none, and returns an empty list.

## 6. Why the parameterized version is safer

- **Code and data travel separately.** The SQL text is fixed (`LIKE @p0`) and compiled before the value is attached, so the value can never become SQL syntax.
- **No escaping by hand.** Quotes, `--` and keywords inside the value are just characters.
- `FromSqlInterpolated` (and `FromSql` in EF Core 7+) converts each `{value}` into a `DbParameter` automatically. `FromSqlRaw` with a string you built yourself does **not**. That one-word difference is the whole vulnerability.
- Note: `%` and `_` typed by the user still act as LIKE wildcards (that is pattern matching, not injection). Escape them if exact matching is required.

**Rule:** never concatenate user input into SQL. Use LINQ, or parameterized raw SQL when raw SQL is needed.
