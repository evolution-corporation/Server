import asyncio

from app import psql_db
from .User import User
# from .Product import Product
# from .Order import Order
from .Meditation import Meditation, UserListenMeditation, MeditationAudioLength
from .Translates import Translate
from .BaseModal import BaseModal


def create_tables():
    tables = [User, Meditation,
              Translate, UserListenMeditation, MeditationAudioLength]
    with psql_db:
        psql_db.create_tables(tables)
