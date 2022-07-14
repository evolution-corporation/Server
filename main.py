import asyncio
from app import app
from models import create_tables

if __name__ == '__main__':
    create_tables()
    app.run(debug=True, host='192.168.1.146')
