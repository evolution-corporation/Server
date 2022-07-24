from enum import Enum
from typing import Union
from peewee import *
from playhouse.postgres_ext import *
from app import db_wrapper, psql_db
from shapely import wkb
from uuid import uuid4


class EnumField(Field):
    def __init__(self, name_enum: str, enum: Enum, *args, **kwargs):
        self.field_type = name_enum
        self.enum = enum
        super().__init__(*args, **kwargs)

    def db_value(self, value):
        return value.name

    def python_value(self, value):
        return self.enum[value]


# class GeoJSONField(Field):
#     field_type = 'geography(POINT, 4326)'
#
#     def __init__(self, *args, **kwargs):
#         super().__init__(*args, **kwargs)
#
#     def db_value(self, value):
#         return f'SRID=4326;POINT({value["longitude"]} {value["latitude"]})'
#
#     def python_value(self, value):
#         coordinate = wkb.loads(value, hex=True).xy
#         return {"longitude": coordinate[0][0], "latitude": coordinate[1][0]}


class EnumField_(IntegerField):
    def __init__(self, enum, *args, **kwargs):
        super(IntegerField, self).__init__(*args, **kwargs)
        self.enum = enum

    def db_value(self, value: Enum):
        return value.value

    def python_value(self, value):
        return self.enum(value)


class BaseModal(db_wrapper.Model):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True)

    @classmethod
    def create(cls, *arg, **kwargs):
        if kwargs.get('id', False):
            return super(db_wrapper.Model, cls).create(*arg, **kwargs)
        else:

            return super(db_wrapper.Model, cls).create(id=uuid4(), *arg, **kwargs)

    class Meta:
        database = psql_db
