IF NOT EXISTS (SELECT * FROM [master].sys.sql_logins WHERE [name] = '$(LOGIN)')
THROW 51000, 'The record does not exist.', 1;

GO