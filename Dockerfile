FROM buildpack-deps:jessie-scm

#wrk 
RUN apt-get update \
    && apt-get install -y build-essential libssl-dev git \
    && git clone https://github.com/wg/wrk.git \
    && cd wrk \
    && make \
    && cp wrk /usr/local/bin

# Install .NET CLI dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libc6 \
        libcurl3 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu52 \
        liblttng-ust0 \
        libssl1.0.0 \
        libstdc++6 \
        libunwind8 \
        libuuid1 \
        zlib1g \
    && rm -rf /var/lib/apt/lists/*


RUN curl -SL https://dot.net/v1/dotnet-install.sh --output dotnet-install.sh \
    && chmod +x ./dotnet-install.sh

ENV DOTNET_INSTALL_DIR /usr/share/dotnet
RUN ./dotnet-install.sh -v 1.1.5
RUN ./dotnet-install.sh -v 2.0.3
RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# # Install .NET Core SDK
# ENV DOTNET_SDK_VERSION 1.1.5
# ENV DOTNET_SDK_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-dev-debian-x64.$DOTNET_SDK_VERSION.tar.gz
# RUN curl -SL $DOTNET_SDK_DOWNLOAD_URL --output dotnet.tar.gz \
#     && mkdir -p /usr/share/dotnet \
#     && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
#     && rm dotnet.tar.gz \
#     && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

#     # Install .NET Core SDK
# ENV DOTNET_SDK_VERSION 2.0.3
# ENV DOTNET_SDK_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-x64.tar.gz
# ENV DOTNET_SDK_DOWNLOAD_SHA 0f1006a5844f860242abb42c8d930f50dc5116ff5a3282b3cdae01ed9ac75dbfc6c74dc3d201358832082d7b6d0aa29b55043afa0493f61892e748c1cfbd0afa

# RUN curl -SL $DOTNET_SDK_DOWNLOAD_URL --output dotnet.tar.gz \
#     && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
#     && rm dotnet.tar.gz \
#     && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Trigger the population of the local package cache
ENV NUGET_XMLDOC_MODE skip
RUN mkdir warmup \
    && cd warmup \
    && dotnet new classlib --language F# \
    && cd .. \
    && rm -rf warmup \
    && rm -rf /tmp/NuGetScratch

# snapshot versions found via http://download.mono-project.com/repo/debian/dists/wheezy/snapshots/4.8.0.524/snapshots/
ENV MONO_VERSION 4.6.2.16
#Install mono 
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF  \
    && echo "deb http://download.mono-project.com/repo/debian wheezy-libjpeg62-compat main" | tee -a /etc/apt/sources.list.d/mono-xamarin.list \
    && echo "deb http://download.mono-project.com/repo/debian wheezy/snapshots/$MONO_VERSION  main" | tee -a /etc/apt/sources.list.d/mono-xamarin.list \
    && apt-get update \
    && apt-get install -y mono-devel ca-certificates-mono fsharp mono-vbnc nuget referenceassemblies-pcl mono-complete \
    && rm -rf /var/lib/apt/lists/*

#Other miscellaneous dependencies
RUN apt-get update && apt-get install -y lsof 

COPY . /usr/src/app/source
WORKDIR /usr/src/app/source
EXPOSE 8083

RUN dotnet --info && mono --version

CMD "./build.sh"