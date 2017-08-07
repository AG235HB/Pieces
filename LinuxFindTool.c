#include <stdio.h>
#include <stdlib.h>
#include <dirent.h>
#include <limits.h>
#include <string.h>
#include <sys/stat.h>
 
int walk(const char*, int);

char* findfile;
char dirname;
 
int main(int argc, char* argv[])
{
    if(argc != 3)
    {
    fprintf(stderr, "Usage: %s DIR FILE\n", argv[0]);
    exit(1);
    }
	findfile = argv[2];

    exit(walk(argv[1], 1));
}
 
char buf[PATH_MAX];

 
int walk(const char* dirname, int do_dirs)
{
    DIR* dp;
    struct stat st;
    struct dirent* dirp;
    int retval = 0;
 
    size_t len = strlen(dirname);
 
    if((dp = opendir(dirname)) == NULL)
    {
    perror(dirname);
    return 1;
    }
 
    strncpy(buf, dirname, len);
 
    while((dirp = readdir(dp)) != NULL)
    {


    if(strcmp(dirp->d_name, ".") == 0 || strcmp(dirp->d_name, "..") == 0)
        continue;

    buf[len] = '/';
    buf[len+1] = '\0';
 
    strcat(buf, dirp->d_name);

dirname = buf;
if(strcmp(dirp->d_name, findfile)==0)
	printf("%s\n", dirname);
    
    if(lstat(buf, &st) == -1)
    {
        perror(dirp->d_name);
        retval = 1;
        break;
    }

    if(S_ISDIR(st.st_mode))
    {
//printf("ENTER\n");
        if(do_dirs)
        {
        }
                
        if(walk(buf, do_dirs) == 1)
        {
        retval = 1;
        break;
        }
        
    }
    buf[len] = '\0';
    }
 
    closedir(dp);
//printf("EXIT\n");
    return retval;
}
