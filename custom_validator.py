from datetime import date
from functools import wraps
from typing import re, Optional

from Error import ValueNotCorrect, DateBirthdayNotCorrect


def validate_nickname(function):
    @wraps(function)
    def wrapped(nickname: Optional[str] = None, *args, **kwargs):
        """
        Проверяет, валидность веденного уникального имени пользователя.
        :params nickname: Введенное уникальное имя пользователя
        """
        if nickname is not None and not (0 < len(nickname) < 16 and re.sub(r'^[a-z\d\._]*$', nickname, '') == ''):
            raise ValueNotCorrect('nickname', {'isNull': False, 'min_len': 1, 'max_len': 16,
                                               'allowed_characters': '"a-z", "0-9",".", "_"'})
        return function(nickname=nickname, *args, **kwargs)
    return wrapped


def validate_birthday(function):
    @wraps(function)
    def wrapped(birthday: Optional[date] = None, *args, **kwargs):
        """
        Проверяет, валидность веденной даты рождения пользователя.
        :params nickname: Введенная дата рождения пользователя.
        """
        if birthday is not None and birthday > date.today():
            raise DateBirthdayNotCorrect(birthday)
        return function(birthday=birthday, *args, **kwargs)
    return wrapped
