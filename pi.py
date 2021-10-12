import os
os.system('wget "https://www.designite-tools.com/static/download/DJE/DesigniteJava.jar"')
def javacomplie(PAT):
    os.system('java -jar DesigniteJava.jar -ci -repo $GITHUB_REPOSITORY -pat  '+PAT)
