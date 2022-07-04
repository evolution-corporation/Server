import os
from flask import Flask
from playhouse.postgres_ext import PostgresqlExtDatabase
from playhouse.flask_utils import FlaskDB

from dotenv import load_dotenv

load_dotenv(".env")

app = Flask(__name__)
app.config.from_object(__name__)
app.config['MAX_CONTENT_LENGTH'] = 250 * 1024 * 1024
psql_db = PostgresqlExtDatabase(
    database=os.environ['DB_NAME'],
    user=os.environ['DB_USER'],
    password=os.environ['DB_PASSWORD'],
    host=os.environ['DB_HOST']
)
db_wrapper = FlaskDB(app, database=psql_db)

from app import routes
from app import api
from app import apiEcstasys
