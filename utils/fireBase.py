from flask import request
from functools import wraps
from firebase_admin import auth, credentials, initialize_app

cred = credentials.Certificate('adminConfig.fb.json')
firebase = initialize_app(cred)


def check_token(function):
    @wraps(function)
    def wrap(*args, **kwargs):
        try:
            if not request.headers.get('authorization'):
                return {'message': 'No token provider'}, 401
            userUid = 'OJqASpMNp1aVc7WiqeZdXEOVftP2'  # auth.verify_id_token(request.headers['authorization'])
            request.userUid = userUid
            print(request.userUid)
        except auth.InvalidIdTokenError:
            return {'message': 'Invalid token provided'}, 401
        else:
            return function(user_id=userUid, *args, **kwargs)
    return wrap
