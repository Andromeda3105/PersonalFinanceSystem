#!/bin/bash
# Скрипт автоматичного розгортання Personal Finance System (Production)

echo "Починаємо збірку проєкту..."
dotnet publish ../../src/PersonalFinanceSystem.API -c Release -o ./publish_output

echo "Зупиняємо сервіс бекенду..."
sudo systemctl stop finance-api

echo "Копіюємо нові файли на сервер..."
sudo cp -r ./publish_output/* /var/www/finance-api/

echo "Оновлюємо базу даних (EF Core міграції)..."
# Разомковується, коли налаштовано рядок підключення на сервері
# dotnet ef database update --project ../../src/PersonalFinanceSystem.API

echo "Запускаємо сервіс бекенду..."
sudo systemctl start finance-api

echo "Розгортання успішно завершено!"