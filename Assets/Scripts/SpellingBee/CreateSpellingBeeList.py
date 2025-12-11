import numpy as np

def main():
    f = open("../../StreamingAssets/signsList.txt", "r")
    signs = f.readlines()
    signs = [sign.strip() for sign in signs]
    f.close()

    spellingBeeList = [sign for sign in signs if (len(sign) >= 4) and (len(sign) <= 7)]
    print(len(spellingBeeList))
    f = open("../../StreamingAssets/spellingBeeList.txt", "w")
    for sign in spellingBeeList:
        f.write(sign + "\n")

def LetterCounts():
    f = open("../../StreamingAssets/signsList.txt", "r")
    signs = f.readlines()
    signs = [sign.strip() for sign in signs]
    f.close()
    alphabet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z']
    spellingBeeList = [sign for sign in signs if (len(sign) >= 4) and (len(sign) <= 7)]
    counts = []
    for letter in alphabet:
        count = 0
        for sign in spellingBeeList:
            if letter in sign:
                count += 1
        print(letter + " : " + str(count))
        counts.append(count)
    counts.sort()
    print(counts[len(counts)//2])

    
LetterCounts()