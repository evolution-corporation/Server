from typing import Optional

from firebase_admin.auth import InvalidIdTokenError
from peewee import DoesNotExist
from datetime import date


class FileTypeError(Exception):
    def __init__(self, desired_type='image'):
        self.desired_type = desired_type

    def __str__(self):
        return f'The file has the wrong type. Expected file type {self.desired_type}'


class AccessDenied(Exception):
    def __str__(self):
        return 'Access is allowed only to the administration or the owner of the object.'


class UserDoesNotExist(DoesNotExist):
    def __init__(self, uid=None, *args, **kwargs):
        self.uid = uid
        self.userParameters = kwargs

    def __str__(self):
        if self.uid is not None:
            return f'The user with this id was not found'
        return 'No user meets these criteria'


class UserDoesExist(ValueError):
    def __init__(self, uid=None, *args, **kwargs):
        self.uid = uid
        self.userParameters = kwargs

    def __str__(self):
        if self.uid is not None:
            return f'The user with this id was not found'
        return 'No user meets these criteria'


class DateBirthdayNotCorrect(ValueError):
    def __init__(self, birthday: Optional[date] = None):
        self.birthday = birthday

    def __str__(self):
        return f'The date of birth cannot be greater than or equal to the current date.'


class ValueNotCorrect(ValueError):
    def __init__(self, name: str, parameters: Optional[dict] = {'isNull': False}):
        self.name = name
        self.parameters = parameters

    def __str__(self):
        return f'The {self.name} value must have the following parameters: {self.parameters}'
