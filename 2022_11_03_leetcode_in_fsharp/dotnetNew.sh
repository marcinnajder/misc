#!/bin/bash
if ["$1" == ""]
then
        echo "Podaj nazwe projektu jako argument np. ./dotnetnew.sh SuperProject"
else

mkdir $1
cd $1
dotnet new console -n $1 -lang "F#"
dotnet new sln
dotnet new mstest -n $1.Tests -lang "F#"
dotnet sln $1.sln add $1/$1.fsproj
dotnet sln $1.sln add $1.Tests/$1.Tests.fsproj
dotnet add $1.Tests/$1.Tests.fsproj reference $1/$1.fsproj
dotnet new gitignore
dotnet build
cd ..

fi

