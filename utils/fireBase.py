import os
from flask import request
from functools import wraps
from firebase_admin import auth, credentials, initialize_app


cred = credentials.Certificate('adminConfig.fb.json')
firebase = initialize_app(cred)


def authorizationWithFireBase(function):
    @wraps(function)
    def wrapped(*args, **kwargs):
        user = None
        try:
            if not request.headers.get('authorization'):
                return {'message': 'No token provider'}, 401
            user = auth.verify_id_token(request.headers['authorization'])
            return function(request_uid=user, *args, **kwargs)
        except auth.InvalidIdTokenError:
            return {'message': 'Invalid token provided'}, 401
    return wrapped


def authorizationRequestWithFireBaseAuthorization(stronger=False, *args_decorator, **kwargs_decorator):
    def inner_decorator(function):
        @wraps(function)
        def wrapped(*args, **kwargs):
            try:
                token = request.headers.get('authorization', None)
                user = None
                if stronger and token is None:
                    return {'message': 'No token provider'}, 401
                if request.headers.get('authorization'):
                    user = auth.verify_id_token(request.headers['authorization'])
                return function(request_uid=user, *args, **kwargs)
            except auth.InvalidIdTokenError:
                if stronger:
                    return {'message': 'Invalid token provided'}, 401
                else:
                    return function(request_uid=None, *args, **kwargs)

        return wrapped
    return inner_decorator


def editUserDataFB(uid, image_uuid=None, display_name=None):
    params = {}
    if image_uuid is not None:
        params['photo_url'] = f'{os.environ["HOME_URL"]}/image/user/{image_uuid}'
    if display_name is not None:
        params['display_name'] = display_name
    user = auth.update_user(uid, **params)
