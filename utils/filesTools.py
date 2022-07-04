import os
import requests
import base64
from uuid import uuid4
from werkzeug.datastructures import FileStorage
from mutagen.mp3 import MP3
from .Errors import *


def uploadImg(image, sub_directory_name, old_name_file=None):
    if os.listdir(
            f'{os.getcwd()}/uploaded/image').count(sub_directory_name) == 0:
        os.mkdir(f'{os.getcwd()}/uploaded/image/{sub_directory_name}')
    if old_name_file is not None:
        os.remove(
            f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{old_name_file}.jpg')
    image_id = uuid4()
    if isinstance(image, FileStorage):
        image: FileStorage = image
        if image.mimetype.find('image/') == -1:
            raise FileTypeError(desired_type=FileType.IMG)
        image.save(
            f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{image_id}.jpg')
        image.close()
    elif type(image) is str:
        if image.find('http') != -1 and image.find('://') != -1:
            imageRequest = requests.get(image)
            with open(f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{image_id}.jpg', 'wb') as imageFile:
                imageFile.write(imageRequest.content)
        elif image.find('data:image/') != -1 and image.find('base64') != -1:
            with open(f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{image_id}.jpg', 'wb') as imageFile:
                imageFile.write(base64.b64decode(image.split('base64,')[1]))
    else:
        raise FileTypeError(desired_type=FileType.IMG)
    return image_id


def uploadAudio(audio, language: str, audio_id=None):
    if audio_id == None:
        audio_id = uuid4()
    if os.listdir(
            f'{os.getcwd()}/uploaded/audio').count(language) == 0:
        os.mkdir(f'{os.getcwd()}/uploaded/audio/{language.lower()}')
    if isinstance(audio, FileStorage):
        audio: FileStorage = audio
        if audio.mimetype.find('audio/') == -1:
            raise FileTypeError(desired_type=FileType.AUDIO)
        audio.save(
            f'{os.getcwd()}/uploaded/audio/{language.lower()}/{audio_id}{audio.filename[audio.filename.rindex("."):]}')
        audio.close()
    else:
        raise FileTypeError(desired_type=FileType.AUDIO)
    return audio_id


def getLengthAudio(audio_id: str, language: str):
    path = f'{os.getcwd()}/uploaded/audio/{language.lower()}/{audio_id}.mp3'
    if os.path.isfile(path):
        audio = MP3(
            f'{os.getcwd()}/uploaded/audio/{language.lower()}/{audio_id}.mp3')
    else:
        audio = MP3(
            f'{os.getcwd()}/uploaded/audio/ru/{audio_id}.mp3')
    return round(audio.info.length)
