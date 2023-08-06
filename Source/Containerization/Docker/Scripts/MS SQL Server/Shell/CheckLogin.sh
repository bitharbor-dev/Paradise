#!/bin/bash

/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -S localhost -b -i /Scripts/SQL/CheckLogin.sql

exit $?