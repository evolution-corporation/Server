import asyncio

from app import psql_db
from .User import User
from .Product import Product
from .Order import Order
from .Plant import Plant
from .Meditation import Meditation, UserListenMeditation, MeditationAudioLength
from .Translates import Translate
from .BaseModal import BaseModal


async def create_tables():
    # createCustomType()
    tables = [User, Product, Order, Plant, Meditation,
              Translate, UserListenMeditation, MeditationAudioLength]
    with psql_db:
        psql_db.create_tables(tables)


def createCustomType():
    print(psql_db.execute_sql(sql))
    with psql_db:
        psql_db.execute_sql("CREATE TYPE Language as ENUM('ab','af','ak','sq','am','ar','an','hy','as','av','ae','ay','az','ba','bm','eu','be','bn','bh','bi','bo','bs','br','bg','my','ca','cs','ch','ce','zh','cu','cv','kw','co','cr','cy','da','de','dv','nl','dz','el','en','eo','et','ee','fo','fa','fj','fi','fr','fy','ff','ka','gd','ga','gl','gv','gn','gu','ht','ha','he','hz','hi','ho','hr','hu','ig','is','io','ii','iu','ie','ia','id','ik','it','jv','ja','kl','kn','ks','kr','kk','km','ki','rw','ky','kv','kg','ko','kj','ku','lo','la','lv','li','ln','lt','lb','lu','lg','mk','mh','ml','mi','mr','ms','mg','mt','mn','na','nv','nr','nd','ng','ne','nn','nb','no','ny','oc','oj','or','om','os','pa','pi','pl','pt','ps','qu','rm','ro','rn','ru','sg','sa','si','sk','sl','se','sm','sn','sd','so','st','es','sc','sr','ss','su','sw','sv','ty','ta','tt','te','tg','tl','th','ti','to','tn','ts','tk','tr','tw','ug','uk','ur','zu','uz','ve','vi','vo','wo','xh','wa','yi','yo','za');")
        psql_db.execute_sql(
            "CREATE TYPE TypeMeditation as ENUM('relaxation','breathingPractices','directionalVisualizations','dancePsychotechnics','DMD');")
