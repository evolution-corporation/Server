from datetime import date
import enum
from uuid import uuid4
from .BaseModal import *
from .User import User
from .Errors import *
from utils.filesTools import uploadImg
from utils.fireBase import editUserDataFB


class Plant(BaseModal):
    id = UUIDField(column_name='Id', index=True, primary_key=True, default=uuid4())
    name = CharField(column_name='Name')
    message = CharField(column_name='Message')
    dateOfPlanting = DateField(column_name='DateOfPlanting', default=date.today())
    user = ForeignKeyField(User, backref='plant', column_name='uid')
    imageId = UUIDField(column_name='ImageId', null=True)
    subUser = ManyToManyField(User, backref='plant')
    coordinate = JSONField(column_name='Coordinate')

    class Meta:
        table_name = 'plant'
