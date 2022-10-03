# dotnet-6-crud-api

.NET 6.0 - CRUD API Example

Documentation at https://jasonwatmore.com/post/2022/03/15/net-6-crud-api-example-and-tutorial

## Роли пользователей

- NO_REGISTRATION - Информации об пользователе нет в системе
- USER - стандартный набор возможностей
- ADMIN - максимальный набор возможностей

## API методы

- Обработка информация об пользователях
    - /users
    - /authentication
    - /user.image
    - /nickname
    - /user.image
    - /subnotification

- Обработка информации об медитациях
    - /meditation
    - /meditation.audio
    - /meditation.image
    - /dmd

- Обработка платежей
    - /payment
- Проверка работоспособности сервера
    - /api/204

### Уникальные типы данных

#### Перечисления

##### UserCategory(string)

- "NULL"
- "BLOGGER"
- "COMMUNITY"
- "ORGANIZATION"
- "EDITOR"
- "WRITER"
- "GARDENER"
- "FLOWER_MAN"
- "PHOTOGRAPHER"

##### UserGender(string)

- "MALE"
- "FEMALE"
- "OTHER"

##### UserRole(string)

- "USER"
- "ADMIN"

##### TypeMeditation(String):

- "relaxation"
- "breathingPractices"
- "directionalVisualizations"
- "dancePsychotechnics"
- "basic"
- "set"

##### CountDayMeditation(string):

- "2-3days"
- "4-5days"
- "6-7days"

##### TimeMeditation(string):

- "lessThan15minutes"
- "moreThan15AndLessThan60Minutes"
- "moreThan60Minutes"

#### Интерфейсы

#### Subscription

| Название    | Тип    |
|-------------|--------|
| Headers     | string |
| Description | string |
| PayloadText | string |

#### MeditationPreferences

| Назавние           | Тип                |
|--------------------|--------------------|
| TypeMeditation     | TypeMeditation     |
| CountDayMeditaiton | CountDayMeditation |
| TimeMeditation     | TimeMeditation     |

### Параметры методов:
Если метод принимает параметры в теле, то ожидается, что они будут передаваться в виде json.

#### /users

- **GET**. Не требует авторизации. Не требует права администратора.
  > Возвращает информацию обо всех пользователях или конкретном

| Параметр                 | Обязательно | Описание                                                                                          |
|--------------------------|:-----------:|:--------------------------------------------------------------------------------------------------|
| <user_id: string>       |     нет     | Вернуть информацию по конкретному пользователю, если user_id не задан вернет список пользователей |
| is_minimum_data: boolean |     нет     | Если true вернет краткую информацию об пользователе                                               |

- **POST**. Требует авторизации. Не требует права администратора. Authorization-Token должен принадлежать пользователю
  > Регистрирует пользователя в системе, если он не найден. Для этого необходимо передать в заголовке "Authorization"
  токен авторизации Firebase Auth. Данные принимаются в виде json

*Заголовок*

| Название      |     Тип      | Обязательно | Описание                       |
|---------------|:------------:|:-----------:|:-------------------------------|
| Authorization |    string    |     да      | Токен авторизации пользователя |

*Тело*:

| Название    |     Тип      | Обязательно | Описание                                          |
|-------------|:------------:|:-----------:|:--------------------------------------------------|
| Nickname    |    string    |     да      | Уникальное имя пользователя                       |
| Birthday    |   ISO-8601   |     да      | Дата рождения пользователя                        |
| Status      |    string    |     нет     | Текстовое сообщение в профиле пользователя        |
| Gender      |  UserGender  |     нет     | Пол пользователя                                  |
| Category    | UserCategory |     нет     | Категория пользователя                            |
| Image       |    base64    |     нет     | Изображение пользователя                          |
| DisplayName |    string    |     нет     | Имя пользователя которое будет везде отображаться |
| ExpoToken   |    string    |     нет     | Токен для отправки push-уведомлений через Expo    |

- **PUT**. Требует авторизации. Требует права администратора.
  > Обновляет данные пользователя на приведенные в теле. Данные принимаются в виде json

*Параметры*

| Название           | Обязательно | Описание                                                |
|--------------------|:-----------:|:--------------------------------------------------------|
| <user_id: string> |     да      | id пользователя информацию которого необходимо обновить |

*Тело*:

| Параметр    |     Тип      | Описание                                          |
|-------------|:------------:|:--------------------------------------------------|
| Nickname    |    string    | Уникальное имя пользователя                       |
| Role        |   UserRole   | Роль пользователя                                 |
| Birthday    |   ISO-8601   | Дата рождения пользователя                        |
| Status      |    string    | Текстовое сообщение в профиле пользователя        |
| Gender      |  UserGender  | Пол пользователя                                  |
| Category    | UserCategory | Категория пользователя                            |
| Image       |    base64    | Изображение пользователя                          |
| DisplayName |    string    | Имя пользователя которое будет везде отображаться |
| ExpoToken   |    string    | Токен для отправки push уведомлений через Expo    |

- **PATCH** Требует авторизации. Не требует права администратора.
  > Обновляет данные пользователя в системе, если он найден. Для этого необходимо передать в заголовке "Authorization"
  токен авторизации Firebase Auth.

*Заголовок*

| Название      |     Тип      | Обязательно | Описание                       |
|---------------|:------------:|:-----------:|:-------------------------------|
| Authorization |    string    |     да      | Токен авторизации пользователя |

*Тело*:

| Название    |     Тип      | Обязательно | Описание                                          |
|-------------|:------------:|:-----------:|:--------------------------------------------------|
| NickName    |    string    |     да      | Уникальное имя пользователя                       |
| Birthday    |   ISO-8601   |     да      | Дата рождения пользователя                        |
| Status      |    string    |     нет     | Текстовое сообщение в профиле пользователя        |
| Gender      |  UserGender  |     нет     | Пол пользователя                                  |
| Category    | UserCategory |     нет     | Категория пользователя                            |
| Image       |    base64    |     нет     | Изображение пользователя                          |
| DisplayName |    string    |     нет     | Имя пользователя которое будет везде отображаться |

#### /authentication

- **GET**. Требует авторизации. Не требует права администратора.
  > Возвращает данные пользователя на основании токена

>
*Заголовок*

| Название      |     Тип      | Обязательно | Описание                       |
|---------------|:------------:|:-----------:|:-------------------------------|
| Authorization |    string    |     да      | Токен авторизации пользователя |

### /user.image

- **GET**. Не требует авторизации. Не требует права администратора.

> Возвращает ссылку на изображение запрошенного пользователя

*Заголовок*

| Название       | Обязательно | Описание                                                            |
|----------------|:-----------:|:--------------------------------------------------------------------|
| Authorization  |     да      | Firebase токен пользователя информацию которого необходимо обновить |

### /subnotification

- **PUT**.

> Подписывает пользователя на уведомления.

*Заголовок*

| Название      | Обязательно | Тип    |
|---------------|-------------|--------|
| Authorization | Да          | string |

*Параметры*

| Название  | Обязательно | Тип    |
|-----------|-------------|--------|
| expoToken | Да          | string |
| frequency | Да          | int    |

### /meditation

- **GET**. Не требует авторизации. Не требует права администратора.
  > Возвращает информацию обо всех, конкретных или по параметрам медитациях. Если запрос имеет одновременно
  preferences=true и
  popularToDay=true, то вернёт список медитаций по параметрам.

*Заголовок*

| Название        |  Тип   | Обязательно | Описание                                                                                   |
|-----------------|:------:|:-----------:|:-------------------------------------------------------------------------------------------|
| Authorization   | string |     нет     | Токен авторизации пользователя                                                             |
| Accept-Language | string |     да      | Дву символьное обозначения естественного языка на котором вернуть информацию об медитациях |

*Параметры*

| Параметр                                  | Обязательно | Описание                                                                                        |
|-------------------------------------------|:-----------:|:------------------------------------------------------------------------------------------------|
| <meditation_id: string>                  |     нет     | Вернуть информацию по конкретной медитации, если meditation_id не задан вернет список медитаций |
| is_minimum_data: boolean                  |     нет     | Если true вернет краткую информацию об медитации                                                |
| preferences:  MeditationPreferences       |     нет     | Вернет информацию об медитацию которая подходит под параметры, или случайную                    |
| getIsNotListened: boolean                 |     нет     | Если true то вернет исключит из рекомендаций медитации которые были уже просушены. *            |
| Так же необходимо передать Authorization* |             |                                                                                                 |
| popularToDay: boolean                     |     нет     | Вернет информацию об медитацию которая в текущие сутки является самой прослушиваемой            |

- **POST**. Требует авторизации.
  > Добавляет информацию об медитации. Данные принимаются в виде form-data. Чтобы загрузить аудиозаписи на разных
  языках, необходимо каждый чтобы поле в котором будет находиться аудио запись имело шаблон названия audio_language, где
  language - дву символьное обозначения естественного языка

*Заголовок*

| Название      |     Тип      | Обязательно | Описание                       |
|---------------|:------------:|:-----------:|:-------------------------------|
| Authorization |    string    |     да      | Токен авторизации пользователя |

В данном случаем данные будут загружены и создаться новая медитация

*Тело*:

| Название       |      Тип       | Обязательно | Описание                                       |
|----------------|:--------------:|:-----------:|:-----------------------------------------------|
| Name           |     string     |     да      | Название медитации                             |
| Description    |     string     |     да      | Описание медитации                             |
| Image          |     base64     |     да      | изображение медитации                          |
| TypeMeditation | TypeMeditation |     да      | Тип медитации                                  |
| IsSubscribe    |    boolean     |     нет     | Чтобы прослушать медитацию необходима подписка |

- **PATCH** Требует авторизации.
  > Обновляет данные об медитации в системе, если он найден.

*Заголовок*

| Название      |     Тип      | Обязательно | Описание                       |
|---------------|:------------:|:-----------:|:-------------------------------|
| Authorization |    string    |     да      | Токен авторизации пользователя |

*Тело*:

| Название       |      Тип       | Обязательно | Описание                                       |
|----------------|:--------------:|:-----------:|:-----------------------------------------------|
| Name           |     string     |     да      | Название медитации                             |
| Description    |     string     |     да      | Описание медитации                             |
| Image          |     base64     |     да      | изображение медитации                          |
| TypeMeditation | TypeMeditation |     да      | Тип медитации                                  |
| IsSubscribe    |    boolean     |     нет     | Чтобы прослушать медитацию необходима подписка |

### /meditation.audio

- **GET**. Не требует авторизации. Не требует права администратора.
  > Возвращает ссылку на аудио запись конкретной медитации на конкретном языке.

*Заголовок*

| Название        |  Тип   | Обязательно | Описание                                                                                   |
|-----------------|:------:|:-----------:|:-------------------------------------------------------------------------------------------|
| Authorization   | string |     да      | Токен авторизации пользователя                                                             |

*Параметры*

| Параметр                 | Обязательно | Описание                                                                                            |
|--------------------------|:-----------:|:----------------------------------------------------------------------------------------------------|
| <meditation_id: string> |     Да      | Вернуть информацию по конкретной медитации, если meditation_id не задан вернет список пользователей |

### /meditation.image

- **Get**. Не требует авторизации. Не требует права администратора.
  > Возвращает ссылку на фотографию конкретной медитации

*Заголовок*

| Название        |  Тип   | Обязательно | Описание                                                                                   |
|-----------------|:------:|:-----------:|:-------------------------------------------------------------------------------------------|
| Authorization   | string |     да      | Токен авторизации пользователя                                                             |


*Параметры*

| Название | Тип    | Обязательно | Описание                                                |
|----------|--------|-------------|---------------------------------------------------------|
| id       | int    | Да          | Id медитации для который необходимо получить фотографию |

### /payment

- **Get**. Требует авторизации.

> Возвращает уникальный идентификатор для оплаты.

*Заголовок*

| Название      | Тип    | Обязательно | Описание                       |
|---------------|--------|-------------|--------------------------------|
| Authorization | string | Да          | Токен авторизации пользователя |

### /api/204

- **Get**

> При обращении всегда возвращает 204 response

