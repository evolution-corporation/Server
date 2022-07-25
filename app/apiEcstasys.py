import os
from datetime import datetime
import json

from flask import request, send_file

from app import app as rest_api
from models.User import User, onlyAdministrator

from models.Meditation import Meditation, TypeMeditation
from models.Translates import Language, getLanguageRequest

from utils.Errors import *
from utils.fireBase import authorizationRequestWithFireBaseAuthorization
from utils.response import *


def createJSONAnswer(user_id=None, code=200, **kwargs):
    return {'uid': user_id, 'timeReturn': datetime.now().isoformat(), **kwargs}, code


def createJSONReject(message: str, code=500, **kwargs):
    return createJSONAnswer(status='Error', message=message, code=code, **kwargs)


@rest_api.route('/api/meditation', methods=['POST'])
@authorizationRequestWithFireBaseAuthorization(stronger=True)
@onlyAdministrator
def meditationPost(request_uid: str):
    # try:
    name = json.loads(request.form.get('name'))
    description = json.loads(request.form.get('description'))
    typeMeditation = request.form.get('type')
    image = request.files.get('image')
    audio = {}
    if request.form.get('audio') == None:
        audio = {}
        for key in request.files:
            file = request.files.get(key)
            if file.mimetype.find('audio/') == -1:
                continue
            audio[key[-2:]] = file
    else:
        audio['ru'] = request.files.get('audio')
    if name and description and typeMeditation and image and len(audio.values()) > 0:
        Meditation.create(
            name, description, image, typeMeditation, audio)
    return 'ok', 200
    # except:
    #     return 'err', 400


@rest_api.route('/api/meditation/<meditation_id>', methods=['GET'])
@rest_api.route('/api/meditation', methods=['GET'])
@authorizationRequestWithFireBaseAuthorization(stronger=False)
@getLanguageRequest
def getMeditation(language, request_uid=None, meditation_id=None):
    if meditation_id:
        isMinimalInformation = True if request.args.get(
            'isMinimal', 'False').lower() == 'true' else False
        meditation = Meditation.get_by_id(meditation_id)
        return createJSONAnswer(user_id=request_uid, result=meditation.serialization(
            isMinimal=isMinimalInformation, language=language))
    else:
        user = User.get_by_id(request_uid)
        result: dict[str, Meditation] = {}
        arg = request.args
        if request.args.get('params', None) != None and not user is None:
            params = json.loads(request.args['params'])
            if user == None:
                pass
            result['recommend'] = Meditation.getByParams(countDay=params['countDay'], timeMeditation=params['time'], typesMeditation=list(
                map(lambda typeMeditation: TypeMeditation[typeMeditation], params['type'])), user=user, language=language)
        if request.args.get('getIsNotListened', 'False').lower() == 'true' and request.args.get('params', None) == None and not user is None:
            result['recommend'] = Meditation.getIsNotListenedUser(
                user=user, language=language)

        if request.args.get(
                'popularToDay', 'False').lower() == 'true':
            result['popularToDay'] = Meditation.getPopular()
        for meditations in result.items():
            if meditations[1] is not None:
                result[meditations[0]] = meditations[1].serialization(
                    isMinimal=True, language=language)

        return createJSONAnswer(user_id=request_uid, result=result)


@rest_api.route('/api/meditation.play/<meditation_id>', methods=['GET'])
@authorizationRequestWithFireBaseAuthorization(stronger=False)
@getLanguageRequest
def playMeditation(language, meditation_id=None, request_uid=None):
    try:
        if meditation_id == None:
            raise NotMeditationId()
        meditation = Meditation.get_by_id(meditation_id)
        if request_uid != None:
            user = User.get_by_id(request_uid)
            meditation.play(user)
        return send_file(f'{os.getcwd()}/uploaded/audio/{language.name.lower()}/{meditation.audioId}.mp3')
    except NotMeditationId:
        return createJSONReject(user_id=request_uid, code=404)
    except DoesNotExist:
        return createJSONReject(user_id=request_uid, code=404)
