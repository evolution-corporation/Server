import json
import os
import random
import enum
import re
from datetime import datetime, date
from typing import  TypeVar, Optional

from functools import wraps
from peewee import ModelSelect, CharField, DateField, UUIDField, DateTimeField, DoesNotExist
from werkzeug.datastructures import FileStorage

from custom_enum import CustomEnum, auto_enum
from Error import UserDoesNotExist, UserDoesExist, ValueNotCorrect
from custom_validator import validate_nickname, validate_birthday
from .BaseModal import BaseModal, EnumField
from .Errors import *
from utils.filesTools import uploadImg


class UserRole(CustomEnum):
    USER = auto_enum()
    ADMIN = auto_enum()


class UserCategory(CustomEnum):
    BLOGGER = auto_enum()
    COMMUNITY = auto_enum()
    ORGANIZATION = auto_enum()
    EDITOR = auto_enum()
    WRITER = auto_enum()
    GARDENER = auto_enum()
    FLOWER_MAN = auto_enum()
    PHOTOGRAPHER = auto_enum()


class UserGender(CustomEnum):
    MALE = auto_enum()
    FEMALE = auto_enum()
    OTHER = auto_enum()


class StatusAuthentication(enum.Enum):
    NONE = 0
    NO_AUTHORIZED = 1
    AUTHORIZED = 2


Self = TypeVar("Self", bound="User")


class UserList:
    """
    Список с моделями пользователей
    """

    def __init__(self, users: tuple[Self]):
        self.__list = users
        self.__index = 0

    def to_serialization(self) -> list[dict]:
        """
        Сериализация списка пользователей
        """
        users_list: list[dict] = []
        for user in self.__list:
            users_list.append(user.serialization)
        return users_list

    def __iter__(self):
        return self

    def __next__(self):
        if self.__index == len(self.__list) - 1:
            raise StopIteration
        self.__index = self.__index + 1
        return self.__data[self.__index]


class User(BaseModal):
    id = CharField(column_name='id', primary_key=True)
    nickname = CharField(max_length=16, column_name='NickName', index=True)
    birthday = DateField(column_name='Birthday', null=True)
    imageId = UUIDField(column_name='ImageId', null=True)
    name = CharField(
        max_length=32, column_name='DisplayName', null=True)
    status = CharField(max_length=128, column_name='Status', null=True)
    role = EnumField(enum=UserRole, name_enum='UserRole', column_name='Role', default=UserRole.USER)
    gender = EnumField(enum=UserGender, name_enum='UserGender', column_name='Gender', default=UserGender.OTHER)
    category = EnumField(enum=UserCategory, name_enum='UserCategory', column_name='Category', null=True)
    registration = DateTimeField(
        column_name='DateTimeRegistration', default=datetime.now())

    def full_update(self, nickname: Optional[str] = None, birthday: Optional[date] = None, status: Optional[str] = None,
                    gender: Optional[UserGender] = None, category: Optional[UserCategory] = None,
                    name: Optional[str] = None, image: Optional[FileStorage] = None, role: Optional[UserRole] = None):
        """
        Обновляет все данные об пользователе. Имеет минимальную проверку.
        Если какие-то данные не являются валидными, то они не будут обновлены.
        :param nickname: Уникальное имя пользователя.
        :param birthday: Дата рождения пользователя.
        :param status: Отображаемое сообщение пользователем.
        :param gender: Пол выбранный пользователем.
        :param category: Категория профиля выбранная пользователем.
        :param name: Имя отображаемое пользователем.
        :param image: Изображения профиля пользователя.
        :param role: Роль пользователя в системе
        """
        if nickname is not None and User.checkUniqueNickname(nickname):
            self.nickname = nickname
        if birthday is not None and birthday < date.today():
            self.birthday = birthday
        if image is not None:
            self.imageId = uploadImg(image, sub_directory_name='user', old_name_file=self.imageId)
        if status is not None:
            self.status = status
        if gender is not None:
            self.gender = gender
        if category is not None:
            self.category = category
        if name is not None:
            self.name = name
        if role is not None:
            self.role = role
        self.save()

    @validate_nickname
    def update_nickname(self, nickname: str):
        """
        Обновляет уникальное имя пользователя.
        :param nickname: Новый уникальное имя пользователя.
        """
        if not (0 < len(nickname) < 16 and re.sub(r'^[a-z\d\._]*$', nickname, '') == ''):
            raise ValueNotCorrect('nickname', {'isNull': False, 'min_len': 1, 'max_len': 16,
                                               'allowed_characters': '"a-z", "0-9",".", "_"'})
        if User.check_unique_nickname(nickname):
            self.nickname = nickname
            self.save()
        else:
            raise UserDoesNotExist(nickname=nickname)

    def update_image(self, image: FileStorage):
        """
        Обновляет изображение пользователя.
        :param image: Новое изображение пользователя.
        """
        self.imageId = uploadImg(image, sub_directory_name='user', old_name_file=self.imageId)
        self.save()

    def update_status(self, status: str):
        """
        Обновляет отображаемое на странице пользователя сообщение.
        :param status: Новое сообщение пользователя.
        """
        self.status = status
        self.save()

    def update_gender(self, gender: UserGender):
        """
        Обновляет отображаемый пол пользователя.
        :param gender: Новый пол пользователя.
        """
        self.gender = gender
        self.save()

    def update_category(self, category: UserCategory):
        """
        Обновляет отображаемую категория профиля.
        :param category: Новая категория профиля.
        """
        self.category = category
        self.save()

    def update_name(self, name: str):
        """
        Обновляет имя пользователя.
        :param name: Новое имя пользователя.
        """
        self.name = name
        self.save()

    @property
    def is_admin(self) -> bool:
        return self.role == UserRole.ADMIN

    def __str__(self):
        return self.id

    @classmethod
    @validate_nickname
    @validate_birthday
    def create(cls, uid: str, nickname: str, birthday: date, status: Optional[str] = None,
               gender: Optional[UserGender] = None, category: Optional[UserCategory] = None,
               name: Optional[str] = None, image: Optional[FileStorage] = None) -> Self:
        """
        Добавляет данные об пользователе в систему. Если не найден id и nickname.
        :param uid: id пользователя.
        :param nickname: Уникальное имя пользователя.
        :param birthday: Дата рождения пользователя.
        :param status: Отображаемое сообщение пользователем.
        :param gender: Пол выбранный пользователем.
        :param category: Категория профиля выбранная пользователем.
        :param name: Имя отображаемое пользователем.
        :param image: Изображения профиля пользователя.
        """

        if User.get_or_none(User.id == uid) is not None:
            raise UserDoesExist(uid)
        if User.get_or_none(User.nickname == nickname) is not None:
            raise UserDoesExist(nickname=nickname)
        user = User(id=uid, nickname=nickname, birthday=birthday, gender=gender, category=category, status=status,
                    name=name)
        user.save()
        if image is not None:
            user.imageId = uploadImg(image, sub_directory_name='user')
        user.save()
        return user

    @classmethod
    @validate_nickname
    def check_unique_nickname(cls, nickname: str) -> bool:
        """
        Проверяет, является ли уникальным nickname.
        :param nickname: проверяемы никнейм.
        """
        try:
            result = User.get(User.nickName == nickname)
            return not result
        except DoesNotExist:
            return True

    @classmethod
    @validate_nickname
    def generate_unique_nickname(cls, nickname='') -> list[str]:
        """
        Генерирует варианты уникальных nickname`ов на основе предоставленного.
        :param nickname: базовый nickname.
        """
        list_generate_nickname = []
        bad_words = []
        random_part_words = []
        with open(os.path.join(os.path.normcase(os.getcwd()), os.environ['PATH_JSON_NICKNAME'])) as file:
            nickname_word_scheme = json.load(file)
            bad_words.extend(nickname_word_scheme.get('bad_words', []))
            random_part_words.extend(
                nickname_word_scheme.get('random_part_words', []))

        if len(random_part_words) == 0:
            return []
        while len(list_generate_nickname) < 5:
            random_part = random.choice(random_part_words)
            random_nickname = f'{nickname}_{random_part}'
            if User.check_unique_nickname(random_nickname):
                list_generate_nickname.append(f'{nickname}_{random_part}')
            random_part_words.remove(random_part)
        return list_generate_nickname

    @classmethod
    def search_by_nickname(cls, nickname, strong=False, limit=20) -> UserList:
        """
        Поиск пользователей по nickname.
        :param nickname: Вариант nickname искомого пользователя.
        :param strong: Если True, то будет использоваться строгое совпадение.
        :param limit: Кол-во возвращаемы пользователей.
        """
        users = []
        if strong:
            users = User.select() \
                .where(User.nickName.startswith(nickname)).limit(limit).order_by(User.nickName)
            return
        else:
            users_search_part = {}
            number_of_users_found = 0
            queries = [
                User.select()
                .where(User.nickName.startswith(nickname[:2])),
                User.select()
                .where(User.nickName.startswith(nickname[:3])),
                User.select()
                .where(User.nickName.endswith(nickname[-3:]))
            ]
            queries.extend([User.select().where(User.nickName.contains(nickname[index:index + 3])) for index in
                            range(1, len(nickname) - 3)])
            for queried in queries:
                number_of_users_found += len(queried)
                for user in queried:
                    if users_search_part.get(user, False):
                        users_search_part[user] += 1
                    else:
                        users_search_part[user] = 1
            for user, score in users_search_part.items():
                if score / number_of_users_found <= 0.3:
                    users_search_part.pop(user, None)
                else:
                    if len(users) == 0:
                        users.append(user)
                    else:
                        for index in range(len(users)):
                            if user.nickName >= users[index].nickName:
                                users.insert(index, user)
                                break
            users.reverse()
            return users[:limit]

    @classmethod
    def get_one(cls, uid=None, nickname=None) -> Self:
        """
        Получить пользователя по uid, nickname.
        Необходимо передать uid или nickname
        :param uid: id пользователя в системе.
        :param nickname: nickname пользователя в системе.
        """
        user: ModelSelect[User] = User.select()
        if uid is None and nickname is None:
            raise TypeError('It is necessary to pass the uid or nickname')
        try:
            if uid is not None:
                user = user.where(User.id == uid)
            if nickname is not None:
                user = user.where(User.nickname == nickname)
            return user.get()
        except DoesNotExist:
            if uid is not None:
                raise UserDoesNotExist(uid)
            elif nickname is not None:
                raise UserDoesNotExist(nickname)

    @classmethod
    def get_list(cls, page: int = 1, birthday: date | None = None,
                 role: tuple[UserRole] = UserRole.items,
                 gender: tuple[UserGender] = UserGender.items,
                 category: tuple[UserCategory | None] = UserCategory.items + (None,)) -> UserList:
        """
        Получить список пользователей. Отсортированных по nickname.
        Фильтры (birthday, role, gender, category), если ни один из параметров не указан,
        то возвращаются все пользователи.
        :param page: Номер страницы с записями(на одной странице 20 записей). 0 - вернуть все записи.
        :param birthday: Дата больше или равна дате рождения пользователей, которых необходимо вернуть.
        :param role: Список ролей пользователей в системе, которые будут возвращены.
        :param gender: Список полов пользователей в системе, которые будут возвращены.
        :param category: Список категорий пользователей в системе, которые будут возвращены.
        """
        users: ModelSelect[User] = User.select().order_by(User.nickname) \
            .where((User.role << role) & (User.gender << gender))
        if birthday is not None:
            users = users.where((birthday >= User.birthday))
        if None in category:
            index_category_none = category.index(None)
            users = users.where((User.category << category[:index_category_none] + category[index_category_none + 1:]) |
                                User.category.is_null())
        else:
            users = users.where(User.category << category)
        if page != 0:
            users = users.paginate(page, 20)
        return UserList(tuple(user for user in users))

    @staticmethod
    def __dir__() -> list[str]:
        return ['id', 'nickname', 'role', 'name', 'status', 'category', 'gender', 'birthday']

    class Meta:
        table_name = 'user'


def only_administrator(function):
    @wraps(function)
    def wrapped(request_uid, *args, **kwargs):
        """
        Проверяет, является ли пользователь, который отправил запрос администратором.
        :params request_uid: id пользователя
        """
        if not User.get_one(request_uid).is_admin:
            raise AccessDenied()
        return function(request_uid=request_uid, *args, **kwargs)

    return wrapped
