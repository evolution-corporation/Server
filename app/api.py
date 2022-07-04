from datetime import datetime
import json
import os

from flask import request, redirect, url_for, send_file
from peewee import DoesNotExist
from app import app
from utils.fireBase import check_token
from utils.Errors import *
from models.User import User
from models.Errors import *

def createJSONAnswer(user_id=None, code=200, **kwargs):
    return json.dumps({'uid': user_id, 'timeReturn': datetime.now().isoformat(), **kwargs}), code


def createJSONReject(message: str, code=500, **kwargs):
    return createJSONAnswer(status='Error', message=message, code=code, **kwargs)


@app.route('/index')
@app.route('/')
def index():
    return 'Hello World'


@app.route('/image/<model>/<uuid>', methods=['GET'])
def showImage(model, uuid):
    return send_file(f'{os.getcwd()}/uploaded/image/{model}/{uuid}.jpg')


@app.route('/api/auth', methods=['POST'])
@check_token
def authUser(user_id):
    result = User.getUserDataByUID(uid=user_id)
    return createJSONAnswer(user_id=user_id, exists=bool(result), user=result)


@app.route('/api/nickName/<nickname>', methods=['GET'])
def nickName(nickname):
    generate_free_nick_name = request.args.get('generate_free_nickName', type=bool)
    return User.checkUniqueNickName(nickname, generate_free_nick_name)


@app.route('/api/user/create', methods=['POST'])
@check_token
def userCreate(user_id):
    try:
        request_data = request.get_json()
        user = User.createAccount(uid=user_id, **request_data)
        return createJSONAnswer(user_id=user_id, status='AccountCreate', result=user.getData(get_plants=True))
    except NickNameNoUnique:
        return createJSONReject(user_id=user_id, message=str(NickNameNoUnique()))
    except TheUserExists:
        return createJSONReject(message=str(TheUserExists(uid=user_id)), user_id=user_id)
    except:
        return createJSONReject(user_id=user_id, message=str(UnknownError()))


@app.route('/api/user/update', methods=['POST', 'PUT'])
@check_token
def userUpdate(user_id):
    try:
        user = User.get_by_id(user_id)
        if request.files.get('image', False):
            user.updateData(image=request.files['image'])
            return createJSONAnswer(user_id=user_id, status='User image suspect update', result=user.getData(get_plants=False))
        else:
            request_data = request.get_json()
            user.updateData(**request_data)
            return createJSONAnswer(user_id=user_id, status='User data suspect update', result=user.getData(get_plants=False))
    except DoesNotExist:
        return createJSONReject(message=str(TheUserNoExists(uid=user_id)), user_id=user_id)
    except FileTypeError:
        return createJSONReject(message=str(FileTypeError(desired_type=FileType.IMG)), user_id=user_id)
    except:
        return createJSONReject(user_id=user_id, message=str(UnknownError()))


@app.route('/api/user/get/<user_id>', methods=['GET'])
def get_user(user_id):
    try:
        get_plants = int(request.args.get('get_plants', 0)) == 1
        is_minimum_data = int(request.args.get('is_minimum_data', 0)) == 1
        user = User.get_by_id(user_id, is_minimum_data=get_plants, get_plants=is_minimum_data)
        return createJSONAnswer(user_id=user_id, result=user.getData(get_plants=True))
    except DoesNotExist:
        return createJSONReject(message=str(TheUserNoExists(uid=user_id)), user_id=user_id)
    except:
        return createJSONReject(user_id=user_id, message=str(UnknownError()))


@app.route('/api/user', methods=['PUT'])
@check_token
def update_user():
    pass
