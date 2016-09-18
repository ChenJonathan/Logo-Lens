import csv

def getTicker(name):
    name = name.lower().replace(" ", "-")
    if name in rawData:
        return rawData[name][0]

    # If the name is not in rawData, check if the name is a substring of a key
    for key in rawData:
        if name in key:
            return rawData[key][0]

with open('companylist.csv', 'rb') as csvfile:
    csvreader = csv.reader(csvfile)
    rawData = dict()
    for row in csvreader:
        rawData[row[1].lower().replace(" ", "-")] = [row[0]] + row[2:-1]