from enum import Enum


class FileType(Enum):
    IMG = 'image'
    AUDIO = 'audio'


class FileTypeError(Exception):
    def __init__(self, desired_type=FileType.IMG):
        self.desired_type = desired_type.name

    def __str__(self):
        return f'The file has the wrong type. Expected file type {self.desired_type}'


class CoordinatesIncorrect(Exception):
    def __str__(self):
        return 'The coordinates are entered incorrectly'


class NotMeditationId(Exception):
    def __str__(self):
        return 'Audio recording ID not entered'
