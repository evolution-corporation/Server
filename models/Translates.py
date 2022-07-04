import os
from datetime import date
import enum
import re
from uuid import uuid4
from functools import wraps
from flask import request


from .BaseModal import *
from .Errors import *
from utils.filesTools import uploadImg
from utils.mapTools import CheckingCoordinate
from playhouse.postgres_ext import JSONField


class Language(enum.Enum):
    AB = enum.auto()  # "ab"
    AF = enum.auto()  # "af"
    AK = enum.auto()  # "ak"
    SQ = enum.auto()  # "sq"
    AM = enum.auto()  # "am"
    AR = enum.auto()  # "ar"
    AN = enum.auto()  # "an"
    HY = enum.auto()  # "hy"
    AS = enum.auto()  # "as"
    AV = enum.auto()  # "av"
    AE = enum.auto()  # "ae"
    AY = enum.auto()  # "ay"
    AZ = enum.auto()  # "az"
    BA = enum.auto()  # "ba"
    BM = enum.auto()  # "bm"
    EU = enum.auto()  # "eu"
    BE = enum.auto()  # "be"
    BN = enum.auto()  # "bn"
    BH = enum.auto()  # "bh"
    BI = enum.auto()  # "bi"
    BO = enum.auto()  # "bo"
    BS = enum.auto()  # "bs"
    BR = enum.auto()  # "br"
    BG = enum.auto()  # "bg"
    MY = enum.auto()  # "my"
    CA = enum.auto()  # "ca"
    CS = enum.auto()  # "cs"
    CH = enum.auto()  # "ch"
    CE = enum.auto()  # "ce"
    ZH = enum.auto()  # "zh"
    CU = enum.auto()  # "cu"
    CV = enum.auto()  # "cv"
    KW = enum.auto()  # "kw"
    CO = enum.auto()  # "co"
    CR = enum.auto()  # "cr"
    CY = enum.auto()  # "cy"
    DA = enum.auto()  # "da"
    DE = enum.auto()  # "de"
    DV = enum.auto()  # "dv"
    NL = enum.auto()  # "nl"
    DZ = enum.auto()  # "dz"
    EL = enum.auto()  # "el"
    EN = enum.auto()  # "en"
    EO = enum.auto()  # "eo"
    ET = enum.auto()  # "et"
    EE = enum.auto()  # "ee"
    FO = enum.auto()  # "fo"
    FA = enum.auto()  # "fa"
    FJ = enum.auto()  # "fj"
    FI = enum.auto()  # "fi"
    FR = enum.auto()  # "fr"
    FY = enum.auto()  # "fy"
    FF = enum.auto()  # "ff"
    KA = enum.auto()  # "ka"
    GD = enum.auto()  # "gd"
    GA = enum.auto()  # "ga"
    GL = enum.auto()  # "gl"
    GV = enum.auto()  # "gv"
    GN = enum.auto()  # "gn"
    GU = enum.auto()  # "gu"
    HT = enum.auto()  # "ht"
    HA = enum.auto()  # "ha"
    HE = enum.auto()  # "he"
    HZ = enum.auto()  # "hz"
    HI = enum.auto()  # "hi"
    HO = enum.auto()  # "ho"
    HR = enum.auto()  # "hr"
    HU = enum.auto()  # "hu"
    IG = enum.auto()  # "ig"
    IS = enum.auto()  # "is"
    IO = enum.auto()  # "io"
    II = enum.auto()  # "ii"
    IU = enum.auto()  # "iu"
    IE = enum.auto()  # "ie"
    IA = enum.auto()  # "ia"
    ID = enum.auto()  # "id"
    IK = enum.auto()  # "ik"
    IT = enum.auto()  # "it"
    JV = enum.auto()  # "jv"
    JA = enum.auto()  # "ja"
    KL = enum.auto()  # "kl"
    KN = enum.auto()  # "kn"
    KS = enum.auto()  # "ks"
    KR = enum.auto()  # "kr"
    KK = enum.auto()  # "kk"
    KM = enum.auto()  # "km"
    KI = enum.auto()  # "ki"
    RW = enum.auto()  # "rw"
    KY = enum.auto()  # "ky"
    KV = enum.auto()  # "kv"
    KG = enum.auto()  # "kg"
    KO = enum.auto()  # "ko"
    KJ = enum.auto()  # "kj"
    KU = enum.auto()  # "ku"
    LO = enum.auto()  # "lo"
    LA = enum.auto()  # "la"
    LV = enum.auto()  # "lv"
    LI = enum.auto()  # "li"
    LN = enum.auto()  # "ln"
    LT = enum.auto()  # "lt"
    LB = enum.auto()  # "lb"
    LU = enum.auto()  # "lu"
    LG = enum.auto()  # "lg"
    MK = enum.auto()  # "mk"
    MH = enum.auto()  # "mh"
    ML = enum.auto()  # "ml"
    MI = enum.auto()  # "mi"
    MR = enum.auto()  # "mr"
    MS = enum.auto()  # "ms"
    MG = enum.auto()  # "mg"
    MT = enum.auto()  # "mt"
    MN = enum.auto()  # "mn"
    NA = enum.auto()  # "na"
    NV = enum.auto()  # "nv"
    NR = enum.auto()  # "nr"
    ND = enum.auto()  # "nd"
    NG = enum.auto()  # "ng"
    NE = enum.auto()  # "ne"
    NN = enum.auto()  # "nn"
    NB = enum.auto()  # "nb"
    NO = enum.auto()  # "no"
    NY = enum.auto()  # "ny"
    OC = enum.auto()  # "oc"
    OJ = enum.auto()  # "oj"
    OR = enum.auto()  # "or"
    OM = enum.auto()  # "om"
    OS = enum.auto()  # "os"
    PA = enum.auto()  # "pa"
    PI = enum.auto()  # "pi"
    PL = enum.auto()  # "pl"
    PT = enum.auto()  # "pt"
    PS = enum.auto()  # "ps"
    QU = enum.auto()  # "qu"
    RM = enum.auto()  # "rm"
    RO = enum.auto()  # "ro"
    RN = enum.auto()  # "rn"
    RU = enum.auto()  # "ru"
    SG = enum.auto()  # "sg"
    SA = enum.auto()  # "sa"
    SI = enum.auto()  # "si"
    SK = enum.auto()  # "sk"
    SL = enum.auto()  # "sl"
    SE = enum.auto()  # "se"
    SM = enum.auto()  # "sm"
    SN = enum.auto()  # "sn"
    SD = enum.auto()  # "sd"
    SO = enum.auto()  # "so"
    ST = enum.auto()  # "st"
    ES = enum.auto()  # "es"
    SC = enum.auto()  # "sc"
    SR = enum.auto()  # "sr"
    SS = enum.auto()  # "ss"
    SU = enum.auto()  # "su"
    SW = enum.auto()  # "sw"
    SV = enum.auto()  # "sv"
    TY = enum.auto()  # "ty"
    TA = enum.auto()  # "ta"
    TT = enum.auto()  # "tt"
    TE = enum.auto()  # "te"
    TG = enum.auto()  # "tg"
    TL = enum.auto()  # "tl"
    TH = enum.auto()  # "th"
    TI = enum.auto()  # "ti"
    TO = enum.auto()  # "to"
    TN = enum.auto()  # "tn"
    TS = enum.auto()  # "ts"
    TK = enum.auto()  # "tk"
    TR = enum.auto()  # "tr"
    TW = enum.auto()  # "tw"
    UG = enum.auto()  # "ug"
    UK = enum.auto()  # "uk"
    UR = enum.auto()  # "ur"
    ZU = enum.auto()  # "zu"
    UZ = enum.auto()  # "uz"
    VE = enum.auto()  # "ve"
    VI = enum.auto()  # "vi"
    VO = enum.auto()  # "vo"
    WO = enum.auto()  # "wo"
    XH = enum.auto()  # "xh"
    WA = enum.auto()  # "wa"
    YI = enum.auto()  # "yi"
    YO = enum.auto()  # "yo"
    ZA = enum.auto()  # "za"


def toJSON(data: dict) -> str:
    json = '{'
    for key, translate in data:
        if isinstance(key, Language):
            json += f'{key.name.lower()}:{translate},'
    return json[:-1] + '}'


def toDict(data: str) -> dict:
    translates = {}
    for key, translate in json.loads(data):
        if isinstance(key, str):
            translates[Language[key.upper()]] = translate
    return translates


class Translate(BaseModal):
    id = UUIDField(column_name='Id', index=True,
                   primary_key=True, default=uuid4())

    translate = JSONField(column_name='Translate')

    def getTranslate(self, language: Language = Language.RU) -> str:
        nameLanguage = language.name.lower()
        if self.translate.get(nameLanguage) != None:
            return self.translate[nameLanguage]
        else:
            return self.getTranslate()

    def addTranslate(self, language: Language, text: str):
        self.translate[language.name.lower()] = text
        self.save()

    @classmethod
    def uploadTranslate(cls, translate: dict):
        return Translate.create(translate=translate, id=uuid4())

    class Meta:
        table_name = 'translate'


def getLanguageRequest(function):
    @wraps(function)
    def wrapped(*args, **kwargs):
        language = None
        try:
            headerLanguage = request.headers.get('Accept-Language')
            if headerLanguage.find(';') != -1:
                headerLanguage = headerLanguage.split(';')
                print(headerLanguage)
                for lang in headerLanguage:
                    lang = re.search(r'[a-z][a-z]', lang).group(0)
                    if not lang:
                        continue
                    else:
                        headerLanguage = lang
                        break
            language = Language(headerLanguage)
        except:
            language = Language.RU
        return function(language=language, *args, **kwargs)
    return wrapped
