FROM mcr.microsoft.com/dotnet/sdk:6.0
RUN apt-get update
# Install dependencies for Projector
RUN apt-get install sudo python3 python3-pip python3-cryptography python3-netifaces less libxext6 libxrender1 libxtst6 libfreetype6 libxi6 -y
# Add dev user
RUN useradd -rm -d /home/dev -s /bin/bash -g root -G sudo -u 1001 -p "$(openssl passwd -6 devpassword)" dev
# Copy login script
COPY start.sh /usr/local/bin/start.sh
RUN chown dev /usr/local/bin/start.sh
RUN chmod +x /usr/local/bin/start.sh
SHELL [ "/bin/bash", "-c" ]
USER dev
WORKDIR /home/dev
# Update pip
RUN python3 -m pip install -U pip
# Install Projector for dev user
RUN pip3 install projector-installer --user
RUN source ~/.profile
# Install Rider 2021.3.3 using Projector
RUN echo "y" | ~/.local/bin/projector autoinstall --config-name Rider --ide-name "Rider 2021.3.3" --port 9999
RUN ~/.local/bin/projector install-certificate 
EXPOSE 9999
COPY . /home/dev/source
CMD ["/bin/bash", "/usr/local/bin/start.sh"]