import os
from datetime import datetime, date
import enum
import random
from typing import Union
from uuid import uuid4
from calendar import monthrange
from .BaseModal import *
from .Errors import *
from .User import User
from utils.filesTools import uploadImg, uploadAudio, FileStorage, getLengthAudio
from utils.mapTools import CheckingCoordinate
from .Translates import Language, Translate


class TypeMeditation(enum.Enum):
    relaxation = enum.auto()
    breathingPractices = enum.auto()
    directionalVisualizations = enum.auto()
    dancePsychotechnics = enum.auto()
    DMD = enum.auto()

# class TypeMeditation(enum.Enum):
#     relaxation = enum.auto()
#     breathingPractices = enum.auto()
#     directionalVisualizations = enum.auto()
#     dancePsychotechnics = enum.auto()
#     DMD = enum.auto()

#     @classmethod
#     class Field(Field):
#         field_type = 'TypeMeditation'

#         def __init__(self, *args, **kwargs):
#             super().__init__(*args, **kwargs)

#         def db_value(self, value):
#             return value.name

#         def python_value(self, value):
#             return TypeMeditation(value)


class Meditation(BaseModal):

    name = ForeignKeyField(Translate, backref='meditation',
                           column_name='Name', lazy_load=False, on_delete='CASCADE')
    description = ForeignKeyField(
        Translate, backref='meditation', column_name='Description', lazy_load=False, on_delete='CASCADE')
    typeMeditation = EnumField(
        name_enum='TypeMeditation', enum=TypeMeditation, column_name='TypeMeditation')
    imageId = UUIDField(column_name='ImageId')
    audioId = UUIDField(column_name='AudioId')
    listenAllTime = IntegerField(column_name='Listening All Time', default=0)
    uploadDateTime = DateTimeField(
        column_name='Upload dateTime', default=datetime.now())

    def serialization(self, isMinimal: bool = False, language: Language = Language.RU):
        return {
            'name': Translate.get_by_id(self.name).getTranslate(language),
            'description': Translate.get_by_id(
                self.description).getTranslate(language),
            'image': self.imageId,
            'lengthAudio': self.lengthAudio.get().length
        }

    def play(self, user: User):
        UserListenMeditation.create(user=user, meditation=self)

    @classmethod
    def create(cls, name: dict, description: dict, image: FileStorage, typeMeditation: str, audio: dict):
        typeMeditation = TypeMeditation[typeMeditation]
        nameId = Translate.uploadTranslate(name)
        descriptionI = Translate.uploadTranslate(description)
        imageId = uploadImg(image, 'meditation')
        audioId = None
        for language in audio:
            audioId = uploadAudio(audio.get(language),
                                  language, audio_id=audioId)

        meditation = super().create(name=nameId,
                                    description=descriptionI,
                                    typeMeditation=typeMeditation,
                                    imageId=imageId,
                                    audioId=audioId)
        for language in audio:
            MeditationAudioLength.create(
                language=Language[language.upper()], meditation=meditation)
        return meditation

    @classmethod
    def getPopular(cls, timePeriod: Union['toDay', 'allTime'] = 'toDay'):
        popularMeditation = None
        if timePeriod == 'toDay':
            popularMeditation = UserListenMeditation.getPopularToDay()
        if timePeriod == 'allTime' or popularMeditation == None:
            popularMeditation = cls.select().order_by(Meditation.listenAllTime).limit(1)
            popularMeditation = popularMeditation[0] if len(
                popularMeditation) else None
        else:
            popularMeditation = cls.select().order_by(
                Meditation.uploadDateTime).limit(1)[0]
        return popularMeditation

    @classmethod
    def getById(cls, id: str, returnError: bool = False):
        try:
            return cls.get_by_id(id)
        except DoesNotExist:
            if returnError:
                raise DoesNotExist()
            else:
                return None

    @classmethod
    def getByParams(cls, countDay: int, timeMeditation: int, typesMeditation: list[TypeMeditation], user: User, language: Language = Language.RU):
        historyWeek = UserListenMeditation.getListingLastWeek(user=user)
        if len(historyWeek) <= countDay:
            duration = (MeditationAudioLength.length <= 18 * 60)
            if timeMeditation > 15 and timeMeditation < 60:
                duration = ((MeditationAudioLength.length <= 15 * 60)
                            & (MeditationAudioLength.length >= 60 * 60))
            elif timeMeditation >= 60:
                duration = (MeditationAudioLength.length >= 60 * 60)
            if timeMeditation < 60 and typesMeditation.count(TypeMeditation.DMD) == 1:
                typesMeditation.remove(TypeMeditation.DMD)
            return cls.__getMeditationFromList(Meditation.select().where(Meditation.typeMeditation.in_(
                typesMeditation) & (Meditation.typeMeditation.not_in(historyWeek))).join(MeditationAudioLength).where((MeditationAudioLength.language == language) & duration))
        else:
            return None

    @classmethod
    def getIsNotListenedUser(cls, user: User, language: Language = Language.RU):
        history = UserListenMeditation.getListing(user=user)
        return cls.__getMeditationFromList(Meditation.select().where((Meditation.typeMeditation.not_in(history))).join(
            MeditationAudioLength).where((MeditationAudioLength.language == language)))

    @classmethod
    def __getMeditationFromList(cls, meditations):
        if meditations.count() > 1:
            return meditations[random.randint(0, meditations.count() - 1)]
        elif meditations.count() == 1:
            return meditations[0]
        else:
            return None

    class Meta:
        table_name = 'meditation'


class UserListenMeditation(BaseModal):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True, default=uuid4())
    user = ForeignKeyField(
        User, backref='listenMeditation', column_name='uid', on_delete='CASCADE')
    meditation = ForeignKeyField(
        Meditation, backref='userListenMeditation', column_name='MeditationId', on_delete='CASCADE')
    dateTimeListen = DateTimeField(
        column_name="DateTimeListen", default=datetime.now())

    @classmethod
    def create(cls, user: User, meditation: Meditation):
        lastUserListingMeditation = cls.get(
            UserListenMeditation.user == user, UserListenMeditation.meditation == meditation)
        delta = datetime.now() - lastUserListingMeditation.dateTimeListen
        if delta.total_seconds() > 12 * 3600:
            super().create(user=user, meditation=meditation)

    @classmethod
    def getPopularToDay(cls) -> Union[Meditation, None]:
        toDay = datetime.today().date()
        toDayList = cls.select().where(
            UserListenMeditation.dateTimeListen >= toDay)
        mustPopular = None
        countListen = {}
        if len(toDayList):
            for listenInfo in toDayList:
                countListen[listenInfo.meditation] += 1
                if mustPopular == None:
                    mustPopular = listenInfo.meditation
                else:
                    mustPopular = listenInfo if countListen[listenInfo] > countListen[mustPopular] else mustPopular
            return Meditation.get_by_id(mustPopular)
        else:
            return None

    @classmethod
    def getListingLastWeek(cls, user: User) -> list[str]:
        return cls.getListing(user=user, time='week')

    @classmethod
    def getListing(cls, user: User, time: Union['week', 'all'] = 'all') -> list[Meditation]:
        try:
            history = cls.select(UserListenMeditation.meditation).where(
                (UserListenMeditation.user == user))
            if time == 'week':
                toDay = datetime.today().date()
                dayOfTheWeek = toDay.weekday()
                startWeek = datetime.today().date()
                if toDay.day - dayOfTheWeek < 0:
                    startWeek.replace(day=monthrange(
                        year=toDay.year, month=toDay.month - 1)[1] + toDay.day - dayOfTheWeek)
                else:
                    startWeek.replace(day=toDay.day - dayOfTheWeek)
                history = history.where((UserListenMeditation.dateTimeListen >=
                                         startWeek) & (UserListenMeditation.user == user))
            return list(map(lambda row: row.meditation, history))
        except DoesNotExist:
            return []

    class Meta:
        table_name = 'userListenMeditation'


class MeditationAudioLength(BaseModal):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True, default=uuid4())
    meditation = ForeignKeyField(
        Meditation, backref='lengthAudio', column_name='MeditationId', on_delete='CASCADE')
    language = EnumField(
        name_enum='language', enum=Language, column_name='LanguageAudio')
    length = IntegerField(column_name='LengthInSeconds')

    @classmethod
    def create(cls, language: Language, meditation: Meditation):
        super().create(meditation=meditation, language=language,
                       length=getLengthAudio(meditation.audioId, language.name), id=uuid4())

    class Meta:
        table_name = 'meditationAudioLength'
