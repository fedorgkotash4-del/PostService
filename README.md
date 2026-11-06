# PostService — практичні роботи 36 і 37

Відкрийте PostService.code-workspace у VS Code.
Запуск у терміналі: dotnet run --launch-profile http
Адреса: http://localhost:5036/
Зупинка: Ctrl+C. Не запускайте другу копію сервера на тому самому порту.
Потрібен .NET SDK 10.

## Перевірка ПР-37
Встановіть REST Client у VS Code. Відкрийте test.rest і виконуйте запити
через Send Request у порядку зверху вниз. Після запуску є записи з Id 1 і 2.
GET /postings — усі записи; GET /postings/1 — один запис.
POST /postings — створення (201 і заголовок Location).
PUT /postings/{id} — оновлення (200).
DELETE /postings/{id} — видалення (204).
Для відсутнього Id GET, PUT та DELETE повертають 404.
DeliveryType у JSON: 0 — Department, 1 — Courier, 2 — ExpressCourier.
Id і CreatedAt нового запису задає сервер. PUT зберігає Id та CreatedAt.
Дані зберігаються у List<Posting> і скидаються після перезапуску.
Кореневий маршрут із ПР-36 збережено.

Перед збиранням у Windows проєкт автоматично зупиняє свою попередню копію з тієї самої папки bin, щоб вона не блокувала DLL. Це стосується також запуску через F5. Після збирання сервер потрібно запустити знову.

