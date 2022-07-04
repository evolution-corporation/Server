from .Errors import *


def CheckingCoordinate(coordinate):
    longitude = coordinate.get('longitude', None)
    latitude = coordinate.get('latitude', None)
    if longitude is None or latitude is None:
        raise CoordinatesIncorrect()
    if abs(longitude) > 180 or abs(latitude) > 90:
        raise CoordinatesIncorrect()
    return True