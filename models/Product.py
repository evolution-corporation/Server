from uuid import uuid4
from .BaseModal import *
from .Errors import *


class Product(BaseModal):
    id = UUIDField(column_name='Id', index=True, primary_key=True, default=uuid4())
    price = IntegerField(column_name='Amount', null=True)
    title = CharField(column_name='Title')
    description = CharField(column_name='Description')

    class Meta:
        table_name = 'product'