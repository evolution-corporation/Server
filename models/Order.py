import os
from datetime import datetime
import enum
import hashlib
from uuid import uuid4
from .BaseModal import *
from .User import User
from .Product import Product
from .Errors import *


class OrderStatus(enum.Enum):
    NOT_PAID = 0
    PAID = 1
    CANCEL = 2


class Order(BaseModal):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True, default=uuid4())
    user = ForeignKeyField(User, backref='order', column_name='uid')
    amount = IntegerField(column_name='Amount')
    status = EnumField_(enum=OrderStatus, column_name='Status',
                        default=OrderStatus.NOT_PAID)
    dateTimeCreate = DateTimeField(
        column_name='DateTimeCreate', default=datetime.now())
    product = ForeignKeyField(
        Product, backref='order', column_name='ProductId')
    optionalData = JSONField(column_name='OptionalData', null=True)

    def serialization(self):
        data = {'id': self.id, 'status': self.status.name}
        return data

    def editStatusPayment(self, status, amount=None):
        self.status = status
        if self.product.id == os.environ['ID_REGISTRATION_PLANTS']:
            print('Меняем статус дереве, или удаляем')
        self.save()

    def generateToken(self):
        description = self.product.description
        amount = self.amount
        terminal_key = os.environ['TERMINAL_KEY']
        password = os.environ['TERMINAL_PASSWORD']
        token = f'{amount}{description}{self.id}{password}{terminal_key}'.encode(
            'utf-8')
        return hashlib.sha256(token).hexdigest()

    @classmethod
    def createOrder(cls, user, product, **kwargs):
        try:
            if str(product.id) == os.environ['ID_REGISTRATION_PLANTS']:
                if kwargs.get('amount', None) is None or kwargs.get('plant_id', None) is None:
                    raise NotFoundFieldForPaidRegistration()
                optional_data = {'plants_id': kwargs['plant_id']}
                order = cls.create(
                    user=user, product=product, amount=kwargs['amount'], optionalData=optional_data)
            else:
                order = cls.create(user=user, product=product)
            order.save()
            return order
        except:
            pass

    class Meta:
        table_name = 'order'
