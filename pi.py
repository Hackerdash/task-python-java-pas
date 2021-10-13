import os
import sys
os.system('wget "https://www.designite-tools.com/static/download/DJE/DesigniteJava.jar"')
def javacomplie(PAT,qscore):
    os.system('java -jar DesigniteJava.jar -ci -repo $GITHUB_REPOSITORY -pat  '+PAT)
    os.system('''curl -X PUT -H "Authorization: Token {}" -H "repository-link:https://github.com/" + GITHUB_REPOSITORY -H "username: " -H "Content-Type: mulitpart/form-data" --url "https://qscored.com/api/upload/file.xml?is_open_access=off&version=$GITHUB_RUN_NUMBER&project_name=" -F "file=@Designite_output/DesigniteAnalysis.xml" '''.format(qscore))
javacomplie(sys.argv[1],sys.argv[2])




