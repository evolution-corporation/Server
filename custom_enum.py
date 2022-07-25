from enum import Enum, auto as auto_enum


class CustomEnum(Enum):
    @classmethod
    @property
    def items(cls):
        items = []
        for name, member in cls.__members__.items():
            items.append(member)
        return tuple(items)