from datetime import datetime
import json
import os

from flask import request, redirect, url_for, send_file
from app import app



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
