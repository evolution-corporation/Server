from datetime import datetime


def createJSONAnswer(user_id=None, code=200, **kwargs):
    return {'uid': user_id, 'timeReturn': datetime.now().isoformat(), **kwargs}, code


def createJSONReject(message: str, code=500, **kwargs):
    return createJSONAnswer(status='Error', message=message, code=code, **kwargs)
