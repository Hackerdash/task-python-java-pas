import os
os.system('wget "https://www.designite-tools.com/static/download/DJE/DesigniteJava.jar"')
os.system('java -jar DesigniteJava.jar -ci -repo $GITHUB_REPOSITORY -pat ${{ secrets.PAT }} ')
os.system('curl -X PUT -H "Authorization: Token ${{ secrets.QSCORED_API_KEY }}" -H "repository-link:https://github.com/" + GITHUB_REPOSITORY -H "username: " -H "Content-Type: mulitpart/form-data" --url "https://qscored.com/api/upload/file.xml?is_open_access=off&version=$GITHUB_RUN_NUMBER&project_name=" -F "file=@Designite_output/DesigniteAnalysis.xml" ')
