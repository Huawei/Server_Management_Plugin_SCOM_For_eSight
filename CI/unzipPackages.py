#coding=utf-8
import sys
import os
import tarfile

def unziptargz(inputfile, dirs):
    t = tarfile.open(inputfile)
    t.extractall(path = dirs)
    t.close
          
                
                
if __name__ == "__main__":
	workspace = sys.argv[1]
	sslpath = os.path.join(workspace, "third_party\\Src\\OpenSSL\\openssl-1.0.2n.tar.gz")
	libpath = os.path.join(workspace, "third_party\\Src\\OpenSSL")
	unziptargz(sslpath, libpath)
    #unziptargz ("D:\\jenkins\\workspace\\scomForESight_Compile\\third_party\\Src\\OpenSSL\\openssl-1.0.2n.tar.gz", "D:\\jenkins\\workspace\\scomForESight_Compile\\third_party\\Src\\OpenSSL")
    