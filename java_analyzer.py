import javalang
import sys


# the java file data file data geting here

def readfile(files):
    a=open(file=files,mode='r')
    file_text=a.read()
    java_analyzer(file_text)
    a.close

#the code for the java file i need 

def java_analyzer(a):
    tree = javalang.parse.parse(a)
    print("Classes")
    print(tree.types[0].name)
    print('\n'*2,"Methods")
    for i in range(len(tree.types[0].body)):
        try:
            print(tree.types[0].body[i].name)
        except :
            continue
    print('\n'*2,"Fields")
    for j in range(len(tree.types[0].body)):
        try:
            print(tree.types[0].body[j].declarators[0].name)
        except:
            continue
#print(len(sys.argv))

#input for the file and validating 
if len(sys.argv)>2:
    for i in sys.argv[2:]:
        if '.java' in i[-5:]: 
            print("the java file here!")
            readfile(i)
        else:
            print(" not the java file there!!!!!!!!!")
            break
else:
    readfile(sys.argv[1])
