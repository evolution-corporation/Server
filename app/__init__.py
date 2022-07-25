import datetime
import os
from flask import Flask
from playhouse.postgres_ext import PostgresqlExtDatabase
from playhouse.flask_utils import FlaskDB
from functools import wraps

from dotenv import load_dotenv
import Error


def api_answer(function):
    @wraps(function)
    def wrapped(request_uid=None, *args, **kwargs):
        answer = dict()
        code = 200
        if request_uid is not None:
            answer.update({'request_id': request_uid})
        try:
            answer.update(function(request_uid=request_uid, *args, **kwargs))
        except (Error.InvalidIdTokenError, Error.AccessDenied, Error.DoesNotExist, Error.FileTypeError) as error:
            if isinstance(error, Error.InvalidIdTokenError):
                code = 401
            elif isinstance(error, Error.AccessDenied):
                code = 403
            elif isinstance(error, Error.DoesNotExist):
                code = 404
            elif isinstance(error, Error.FileTypeError):
                code = 415
            answer.update({'message': str(error)})
        answer.update({'timeReturn': datetime.datetime.now().isoformat()})
        return answer, code
    return wrapped


load_dotenv(".env")

app = Flask(__name__)
app.config.from_object(__name__)
app.config['MAX_CONTENT_LENGTH'] = 250 * 1024 * 1024
psql_db = PostgresqlExtDatabase(
    database=os.environ['DB_NAME'],
    user=os.environ['DB_USER'],
    password=os.environ['DB_PASSWORD'],
    host=os.environ['DB_HOST'],
    port=os.environ['DB_PORT']
)
db_wrapper = FlaskDB(app, database=psql_db)


from app import routes
from app import api
from app import apiEcstasys
