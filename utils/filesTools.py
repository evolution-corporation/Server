import os
import base64
from uuid import uuid4
from werkzeug.datastructures import FileStorage
from .Errors import *


def uploadImg(image, sub_directory_name, old_name_file=None):
    if old_name_file is not None:
        os.remove(f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{old_name_file}.jpg')
    image_id = uuid4()
    if isinstance(image, FileStorage):
        image: FileStorage = image
        if image.mimetype.find('image/') == -1:
            raise FileTypeError(desired_type=FileType.IMG)
        print(image.content_type)
        image.save(f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{image_id}.jpg')
        image.close()
    elif type(image) is str and \
            image.index('data:image/') != -1 and image.index('base64'):
        with open(f'{os.getcwd()}/uploaded/image/{sub_directory_name}/{image_id}.jpg', 'wb') as imageFile:
            imageFile.write(base64.b64decode(image.split('base64,')[1]))
    else:
        raise FileTypeError(desired_type=FileType.IMG)
    return image_id
