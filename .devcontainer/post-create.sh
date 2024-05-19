printf "Start of Script File\n" > /workspaces/BigLog.txt

printf "BEFORE CD\n" >> /workspaces/BigLog.txt
cd /usr/share

mv dotnet dotnet-8.0.101
mkdir dotnet

printf "BEFORE curl command\n" >> /workspaces/BigLog.txt
curl -Lo dotnet.tar.gz https://download.visualstudio.microsoft.com/download/pr/85bcc525-4e9c-471e-9c1d-96259aa1a315/930833ef34f66fe9ee2643b0ba21621a/dotnet-sdk-8.0.201-linux-x64.tar.gz

printf "BEFORE tar command\n" >> /workspaces/BigLog.txt
tar -C dotnet -xf dotnet.tar.gz

printf "BEFORE remove file\n" >> /workspaces/BigLog.txt
rm dotnet.tar.gz

printf "End of Script File\n" >> /workspaces/BigLog.txt

