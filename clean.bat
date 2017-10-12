@echo off
for /d /r . %%d in (bin,obj,.vs,packages) do @if exist "%%d" rd /s/q "%%d"