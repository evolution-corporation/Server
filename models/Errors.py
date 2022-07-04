class NicknameNoUnique(Exception):
    def __init__(self, *args):
        super().__init__()
        if args:
            self.nickName = [0]
        else:
            self.nickName = None

    def __str__(self):
        if self.nickName:
            return f'Nick name no unique, {self.nickName}'
        else:
            return 'Nick name not exist'


class IdNotSpecified(Exception):
    def __str__(self):
        return 'Id not specified'


class TheUserExists(Exception):
    def __init__(self, uid=None):
        super().__init__()
        self.uid = uid

    def __str__(self):
        if self.uid:
            return f'The user exists with id {self.uid}. Check JWT or method'
        else:
            return 'The user exists. Check JWT or method'


class TheUserNoExists(Exception):
    def __init__(self, uid=None):
        super().__init__()
        self.uid = uid

    def __str__(self):
        if self.uid:
            return f'The user not exists with id {self.uid}.'
        else:
            return 'The user not exists.'


class AccessDenied(Exception):
    def __str__(self):
        return "Access is allowed only to the administration or the owner of the object."


class UnknownError(Exception):
    def __str__(self):
        return 'Unknown Error'


class NotFoundFieldForPaidRegistration(Exception):
    def __str__(self):
        return 'The necessary fields for the paid registration of the plant were not found'


class TheAmountIsTooSmall(Exception):
    def __str__(self):
        return 'The payment amount is too small'


class RequiredFields(Exception):
    def __str__(self):
        return 'Required fields are not filled in'

