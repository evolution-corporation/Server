from typing import Optional, Type
from datetime import datetime
from uuid import uuid4

from playhouse.postgres_ext import Field, UUIDField

from custom_enum import CustomEnum
from app import db_wrapper, psql_db


class EnumField(Field):
    def __init__(self, name_enum: str, enum: Type[CustomEnum], default: Optional[CustomEnum] = None, *args, **kwargs):
        self.field_type = name_enum
        self.enum = enum
        if default:
            self.default = default
        super().__init__(*args, **kwargs)

    def db_value(self, value):
        if value is None:
            return None
        return value.name

    def python_value(self, value):
        if value is None:
            return None
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


class BaseModal(db_wrapper.Model):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True)

    @property
    def serialization(self) -> dict:
        """
        Сериализация данных указанных в self.__dir__
        """
        fields_name = dir(self)
        json_model = {}
        for field_name in fields_name:
            value = getattr(self, field_name)
            if isinstance(value, datetime):
                serialization_value = value.isoformat()
            elif isinstance(value, CustomEnum):
                serialization_value = value.name
            else:
                serialization_value = value
            json_model.update({field_name: serialization_value})
        return json_model

    @classmethod
    def create(cls, *arg, **kwargs):
        if kwargs.get('id', False):
            return super(db_wrapper.Model, cls).create(*arg, **kwargs)
        else:

            return super(db_wrapper.Model, cls).create(id=uuid4(), *arg, **kwargs)

    class Meta:
        database = psql_db
