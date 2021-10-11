from logging import exception
import os
from github import Repository
from github import Github
from cryptography.fernet import Fernet
import github

s=Github('naik984907@gmail.com',"Hackerghost@1")
print(s.get_user().login)
