import os
import random
from datetime import date, datetime
import enum
from functools import wraps

from .BaseModal import *
from .Errors import *
from utils.filesTools import uploadImg
from utils.fireBase import editUserDataFB


class UserRole(enum.Enum):
    NO_REGISTRATION = 0
    USER = 1
    ADMIN = 2


class UserCategory(enum.Enum):
    NULL = None
    BLOGGER = 1
    COMMUNITY = 2
    ORGANIZATION = 3
    EDITOR = 4
    WRITER = 5
    GARDENER = 6
    FLOWER_MAN = 7
    PHOTOGRAPHER = 8


class UserGender(enum.Enum):
    MALE = 0
    FEMALE = 1
    OTHER = 2


class StatusAuthentication(enum.Enum):
    NONE = 0
    NO_AUTHORIZED = 1
    AUTHORIZED = 2


class User(BaseModal):
    uid = CharField(column_name='UID', primary_key=True)
    nickName = CharField(max_length=16, column_name='NickName', index=True)
    birthday = DateField(column_name='Birthday', null=True)
    imageId = UUIDField(column_name='ImageId', null=True)
    displayName = CharField(
        max_length=32, column_name='DisplayName', null=True)
    status = CharField(max_length=128, column_name='Status', null=True)
    role = EnumField_(enum=UserRole, column_name='Role',
                      default=UserRole.NO_REGISTRATION)
    gender = EnumField_(enum=UserGender, column_name='Gender',
                        default=UserGender.OTHER)
    category = EnumField_(enum=UserCategory, column_name='Category', null=True)
    registration = DateTimeField(
        column_name='DateTimeRegistration', default=datetime.now())

    def updateData(self, **kwargs):
        if kwargs.get('image', False):
            self.imageId = uploadImg(
                kwargs['image'], sub_directory_name='user', old_name_file=self.imageId)
        if kwargs.get('display_name', False):
            self.displayName = kwargs['display_name']
        if kwargs.get('image', False) or kwargs.get('display_name', False):
            editUserDataFB(self.uid, self.imageId, self.displayName)
        if kwargs.get('nickname', False):
            if kwargs['nickname'] != self.nickName:
                if not User.checkUniqueNickname(kwargs['nickname']):
                    raise NicknameNoUnique(kwargs['nickname'])
                self.nickName = kwargs['nickname']
        if kwargs.get('birthday', False):
            self.birthday = date.fromisoformat(kwargs['birthday'])
        if kwargs.get('status', False) or kwargs['status'] is None:
            self.status = None if kwargs['status'] == '' else kwargs['status']
        if kwargs.get('gender', False):
            self.gender = UserGender(kwargs['gender'])
        if kwargs.get('category', False):
            self.category = UserCategory(kwargs['category'])
        self.save()

    def editRole(self, role: UserRole, admin):
        if admin.role == UserRole.ADMIN:
            self.role = role
            self.save()

    def serialization(self, is_minimum_data=False):
        user_data = {
            'uid': self.uid,
            'nickName': self.nickName,
            'role': self.role.name,
            'image': self.imageId,
            'displayName': self.displayName
        }
        if not is_minimum_data:
            user_data['status'] = self.status
            user_data['category'] = self.category.name if self.category else None
            user_data['gender'] = self.gender.name
            user_data['birthday'] = self.birthday.isoformat()
        return user_data

    def isAdmin(self) -> bool:
        return self.role == UserRole.ADMIN

    @classmethod
    def createAccount(cls, uid: str, **kwargs):
        try:
            cls.get_by_id(uid)
            raise TheUserExists(uid)
        except DoesNotExist:
            params = {}
            if not kwargs.get('nickname', False):
                raise NicknameNoUnique()
            params['nickName'] = kwargs['nickname']
            if not cls.checkUniqueNickname(params['nickName']):
                raise NicknameNoUnique(params['nickName'])
            if kwargs.get('birthday', False):
                if kwargs['birthday'].find('T') != -1:
                    kwargs['birthday'] = kwargs['birthday'][:kwargs['birthday'].find(
                        'T')]
                params['birthday'] = date.fromisoformat(kwargs['birthday'])
                params['role'] = UserRole.USER
            if kwargs.get('status', False):
                params['status'] = kwargs['status']
            if kwargs.get('gender', False):
                params['gender'] = UserGender(kwargs['gender'])
            if kwargs.get('category', False):
                params['category'] = UserCategory(kwargs['category'])
            if kwargs.get('image', False):
                params['imageId'] = uploadImg(
                    kwargs['image'], sub_directory_name='user')
            if kwargs.get('displayName', False):
                params['displayName'] = kwargs['displayName']
            user = cls.create(uid=uid, **params)
            user.save()
            return user

    @classmethod
    def checkUniqueNickname(cls, nickname):
        try:
            if nickname == 'nikita123a':
                return False
            result = User.get(User.nickName == nickname)
            return not result
        except DoesNotExist:
            return True

    @classmethod
    def generateUniqueNickname(cls, nickname=''):
        list_generate_nickname = []
        bad_words = []
        random_part_words = []
        with open(os.path.join(os.path.normcase(os.getcwd()), os.environ['PATH_JSON_NICKNAME'])) as file:
            nickname_word_scheme = json.load(file)
            bad_words.extend(nickname_word_scheme.get('bad_words', []))
            random_part_words.extend(
                nickname_word_scheme.get('random_part_words', []))

        if len(random_part_words) == 0:
            return []
        while len(list_generate_nickname) < 5:
            random_part = random.choice(random_part_words)
            random_nickname = f'{nickname}_{random_part}'
            if cls.checkUniqueNickname(random_nickname):
                list_generate_nickname.append(f'{nickname}_{random_part}')
            random_part_words.remove(random_part)
        return list_generate_nickname

    @classmethod
    def searchByNicknameOrDisplayName(cls, nickname, strong=False, limit=20):
        users = []
        if strong:
            users = User.select() \
                .where(User.nickName.startswith(nickname)).limit(limit).order_by(User.nickName)
            return users
        else:
            users_search_part = {}
            number_of_users_found = 0
            queries = [
                User.select()
                .where(User.nickName.startswith(nickname[:2])),
                User.select()
                .where(User.nickName.startswith(nickname[:3])),
                User.select()
                .where(User.nickName.endswith(nickname[-3:]))
            ]
            queries.extend([User.select().where(User.nickName.contains(nickname[index:index + 3])) for index in
                            range(1, len(nickname) - 3)])
            for queried in queries:
                number_of_users_found += len(queried)
                for user in queried:
                    if users_search_part.get(user, False):
                        users_search_part[user] += 1
                    else:
                        users_search_part[user] = 1
            for user, score in users_search_part.items():
                if score / number_of_users_found <= 0.3:
                    users_search_part.pop(user, None)
                else:
                    if len(users) == 0:
                        users.append(user)
                    else:
                        for index in range(len(users)):
                            if user.nickName >= users[index].nickName:
                                users.insert(index, user)
                                break
            users.reverse()
            return users[:limit]

    @classmethod
    def getById(cls, uid):
        try:
            return User.get_by_id(uid)
        except DoesNotExist:
            return None

    class Meta:
        table_name = 'user'


def onlyAdministrator(function):
    @wraps(function)
    def wrapped(request_uid, *args, **kwargs):
        try:
            if User.get_by_id(request_uid).isAdmin():
                return function(request_uid=request_uid, *args, **kwargs)
            return {'message': 'Not enough rights to perform actions'}, 403
        except DoesNotExist:
            return {'message': 'No user to verify rights found'}, 401
    return wrapped
