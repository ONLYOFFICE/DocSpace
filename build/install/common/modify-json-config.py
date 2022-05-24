from jsonpath_ng import jsonpath, parse
import json
import sys

filePath = None
saveFilePath = None
jsonValue = None

def openJsonFile(filePath):
    try:
        with open(filePath, 'r') as f:
            return json.load(f)
    except FileNotFoundError as e:
        return False
    except IOError as e:
        return False

def parseJsonValue(jsonValue):
    data = jsonValue.split("=")
    data[0] = "$." + data[0].strip()
    data[1] = data[1].replace(" ", "")
    
    return data

def updateJsonFile(jsonData, jsonUpdateValue):
    jsonpath_expr = parse(jsonUpdateValue[0])
    jsonpath_expr.find(jsonData)
    jsonpath_expr.update(jsonData, jsonUpdateValue[1])
    
    return jsonData

def writeJsonFile(jsonFile, jsonData, indent=4):
    with open(jsonFile, 'w') as f:
        f.write(json.dumps(jsonData, ensure_ascii=False, indent=indent))
    
    return 1

filePath = sys.argv[1]
saveFilePath = filePath
jsonValue = sys.argv[2]

jsonData = openJsonFile(filePath)
jsonUpdateValue = parseJsonValue(jsonValue)
updateJsonFile(jsonData, jsonUpdateValue)
writeJsonFile(filePath, jsonData)
