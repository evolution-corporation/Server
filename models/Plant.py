import os
import random
from datetime import date
import enum
from uuid import uuid4
from .BaseModal import *
from .User import User
from .Errors import *
from utils.filesTools import uploadImg
from utils.mapTools import CheckingCoordinate


class PlantCategory(enum.Enum):
    TREE = 0
    SHRUB = 1
    FLOWER = 2


class PlantStatus(enum.Enum):
    RESERVED = 0
    PLANTED = 1
    AWAITING_PAYMENT = 2
    AWAITING_LOADING_DATA = 3


def generate_random_coordinate(metre=1):
    return metre * pow(random.randint(1, 2), -1) * random.random() * 0.111


class Plant(BaseModal):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True, default=uuid4())
    name = CharField(column_name='Name')
    message = CharField(column_name='Message', null=True)
    dateOfPlanting = DateField(
        column_name='DateOfPlanting', default=date.today())
    user = ForeignKeyField(User, backref='plant', column_name='uid')
    imageId = UUIDField(column_name='ImageId', null=True)
    category = EnumField_(enum=PlantCategory, column_name='Category')
    subUsers = ManyToManyField(User, backref='plant')
    coordinate = GeoJSONField(column_name='Coordinate', unique=True)
    status = EnumField_(enum=PlantStatus, column_name='Status',
                        default=PlantStatus.PLANTED)
    country = CharField(column_name='Country', null=True)
    amount = IntegerField(column_name='Amount', null=True)

    def serialization(self, is_minimum_data=False):
        plant_data = {
            'id': self.id,
            'name': self.name,
            'coordinate': self.coordinate,
            'category': self.category.name,
            'country': self.country,
            'dateOfPlanting': self.dateOfPlanting
        }
        if not is_minimum_data:
            plant_data['message'] = self.message
            plant_data['imageId'] = self.imageId
            plant_data['user'] = self.user.serialization(
                is_minimum_data=True, get_plants=True)
        return plant_data

    def updateData(self, amount=None, image=None):
        if amount is not None:
            self.amount = amount
            if self.imageId is not None:
                self.status = PlantStatus.PLANTED
            else:
                self.status = PlantStatus.AWAITING_LOADING_DATA
        elif image is not None:
            image_id = uploadImg(
                image=image, sub_directory_name='plant', old_name_file=self.imageId)
            self.imageId = image_id
            if self.status != PlantStatus.AWAITING_PAYMENT:
                self.status = PlantStatus.PLANTED
        self.save()

    @classmethod
    def plantingPlant(cls, name, user, coordinate, category, image=None, amount=None, message=None, sub_users=[]):
        CheckingCoordinate(coordinate=coordinate)
        if name is None or category is None:
            raise RequiredFields()
        if not isinstance(category, PlantCategory):
            category = PlantCategory[category]
        check_coordinate = Plant.get_or_none(Plant.coordinate == coordinate)
        if check_coordinate is not None:
            while check_coordinate is not None:
                coordinate['longitude'] += generate_random_coordinate()
                coordinate['latitude'] += generate_random_coordinate()
                check_coordinate = Plant.get_or_none(
                    Plant.coordinate == coordinate)
        image_id = uploadImg(
            image=image, sub_directory_name='plant') if image is not None else None
        status = PlantStatus.PLANTED if image_id is not None else PlantStatus.AWAITING_LOADING_DATA
        status = status if amount is None else PlantStatus.AWAITING_PAYMENT
        plant = Plant.create(name=name, user=user, coordinate=coordinate, category=category,
                             message=message, imageId=image_id, status=status)
        plant.save()
        return plant

    @classmethod
    def getByUser(cls, user):
        plants = cls.select().where(Plant.user == user and Plant.status ==
                                    PlantStatus.PLANTED).order_by(Plant.dateOfPlanting)
        return plants, plants.count

    class Meta:
        table_name = 'plant'
