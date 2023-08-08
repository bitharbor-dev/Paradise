#!/bin/bash

echo "Checking MS SQL Login."

for i in {1..10};
	do

	/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -S localhost -b -i /Scripts/SQL/CheckLogin.sql

		if [ $? -eq 0 ]
		then
			echo "MS SQL Login already exists."
			break
		else
			echo "MS SQL Login not found. Creating..."
		fi

	/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -S localhost -b -i /Scripts/SQL/CreateLogin.sql

		if [ $? -eq 0 ]
		then
			echo "MS SQL Login created."
			break
		else
			echo "..."
			sleep 1
		fi

	echo "Failed to create MS SQL Login. Performing new attempt."

	done

if [ $? -ne 0 ]
then
	echo "Failed to create MS SQL Login. Check the configuration and restart the Docker service."
fi