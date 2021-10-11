from logging import exception
import os
from github import Repository
from github import Github
from cryptography.fernet import Fernet
import github

def encrpt_token(messg):
    key=Fernet.generate_key()
    fernet = Fernet(key)
    enmesseg=fernet.encrypt(messg.encode())
    return enmesseg,key

def decrpt_tokne(messg):
    key=messg[1]
    messgs=messg[0][2:-1].encode()
    fernet = Fernet(key[1:-1])
    n=fernet.decrypt(messgs).decode()
    return n,key,messg[2]

def storedata(token,name):
    file=open('.temp','a')
    data=encrpt_token(token)
    file.writelines([str(data[0]),',',str(data[1]),',',name])
    file.close
    
def get_token():
    print("pls enter the personal token:",end='')
    PC_token=input()

    if len(PC_token)==40:
        print("personal access token:",PC_token)
        print("token cheking...")
        
        try:
            token=Github(PC_token)
            print("login successfully with :",token.get_user().login)
            storedata(PC_token,token.get_user().login)
        except Exception as e :
            print('\n',type(e).__name__,'\n')
            print(RuntimeError(e))

    elif len(PC_token)<=40 or len(PC_token)>=40:
        print("pls check the personal access token")
        get_token()

    elif len(PC_token)==0:
        print('pls enter the token')
        get_token()

    return PC_token

def check_store():
    if os.path.isfile('.temp'):
        data=open('.temp','r')
        data=data.readlines()
        ndata=[]
        for i in data:
            ndata.append(decrpt_tokne(i.split(',')))
        print(ndata)
    return ndata

def create_repo_secret():
    Repository.Repository.create_secret(self,secret_name='hello',unencrypted_value='hello')

#create_repo_secret()
try:
    check_store()
except :
    get_token()
#token_access = Github("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")
#print(token_access.get_user().login)
