from datetime import datetime
import json
import os

from flask import request, redirect, url_for, send_file
from peewee import DoesNotExist
from app import app
from utils.fireBase import authorizationWithFireBase
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


@app.route('/download/<app_name>')
def downloadApplication(app_name):
    if app_name == 'plants':
        return redirect('https://expo.dev/accounts/nikitabs/projects/plants/builds/de1547a6-bdc5-4caf-b57e-47bc046a60ff')
    if app_name == 'ecstasys':
        return send_file(f'{os.getcwd()}/static/app-debug.apk')
        return redirect('https://expo.dev/accounts/nikitabs/projects/ecstasys/builds/8eb142fa-2f7d-42c7-b26d-acf38c3a8ab8')
    return '', 404


@app.route('/image/<model>/<uuid>', methods=['GET'])
def showImage(model, uuid):
    return send_file(f'{os.getcwd()}/uploaded/image/{model}/{uuid}.jpg')


@app.route('/audio/<language>/<uuid>', methods=['GET'])
def playAudio(language, uuid):
    return send_file(f'{os.getcwd()}/uploaded/audio/{language}/{uuid}.mp3')
#
#
# @app.route('/api/auth', methods=['POST'])
# @authorizationWithFireBase
# def authUser(user_id):
#     result = User.getUserDataByUID(uid=user_id)
#     return createJSONAnswer(user_id=user_id, exists=bool(result), user=result)
#
#
# @app.route('/api/nickName/<nickname>', methods=['GET'])
# def nickName(nickname):
#     generate_free_nick_name = request.args.get('generate_free_nickName', type=bool)
#     return User.checkUniqueNickname(nickname, generate_free_nick_name)
#
#
# @app.route('/api/users', methods=['POST', 'PUT', 'DELETE'])
# @authorizationWithFireBase
# def userCreate(user_id):
#     try:
#         request_data = request.get_json()
#         user = User.createAccount(uid=user_id, **request_data)
#         return createJSONAnswer(user_id=user_id, status='AccountCreate', result=user.getData(get_plants=True))
#     except NicknameNoUnique:
#         return createJSONReject(user_id=user_id, message=str(NicknameNoUnique()))
#     except TheUserExists:
#         return createJSONReject(message=str(TheUserExists(uid=user_id)), user_id=user_id)
#     except:
#         return createJSONReject(user_id=user_id, message=str(UnknownError()))
#
#
# @app.route('/api/user/update', methods=['POST', 'PUT'])
# @authorizationWithFireBase
# def userUpdate(user_id):
#     try:
#         user = User.get_by_id(user_id)
#         if request.files.get('image', False):
#             user.updateData(image=request.files['image'])
#             return createJSONAnswer(user_id=user_id, status='User image suspect update', result=user.getData(get_plants=False))
#         else:
#             request_data = request.get_json()
#             user.updateData(**request_data)
#             return createJSONAnswer(user_id=user_id, status='User data suspect update', result=user.getData(get_plants=False))
#     except DoesNotExist:
#         return createJSONReject(message=str(TheUserNoExists(uid=user_id)), user_id=user_id)
#     except FileTypeError:
#         return createJSONReject(message=str(FileTypeError(desired_type=FileType.IMG)), user_id=user_id)
#     except:
#         return createJSONReject(user_id=user_id, message=str(UnknownError()))
#
#
# @app.route('/api/user/get/<user_id>', methods=['GET'])
# def get_user(user_id):
#     try:
#         get_plants = int(request.args.get('get_plants', 0)) == 1
#         is_minimum_data = int(request.args.get('is_minimum_data', 0)) == 1
#         user = User.get_by_id(user_id, is_minimum_data=get_plants, get_plants=is_minimum_data)
#         return createJSONAnswer(user_id=user_id, result=user.getData(get_plants=True))
#     except DoesNotExist:
#         return createJSONReject(message=str(TheUserNoExists(uid=user_id)), user_id=user_id)
#     except:
#         return createJSONReject(user_id=user_id, message=str(UnknownError()))
