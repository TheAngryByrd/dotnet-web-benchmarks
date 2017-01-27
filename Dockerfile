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

# Install .NET Core SDK
ENV DOTNET_SDK_VERSION 1.0.0-preview2-003156
ENV DOTNET_SDK_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/$DOTNET_SDK_VERSION/dotnet-dev-debian-x64.$DOTNET_SDK_VERSION.tar.gz

RUN curl -SL $DOTNET_SDK_DOWNLOAD_URL --output dotnet.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Install .NET Core SDK
ENV DOTNET_SDK_VERSION 1.0.0-preview2-1-003177
ENV DOTNET_SDK_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/preview/Binaries/$DOTNET_SDK_VERSION/dotnet-dev-debian-x64.$DOTNET_SDK_VERSION.tar.gz

RUN curl -SL $DOTNET_SDK_DOWNLOAD_URL --output dotnet.tar.gz \
    && tar --overwrite-dir -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz 
    
# Trigger the population of the local package cache
ENV NUGET_XMLDOC_MODE skip
RUN mkdir warmup \
    && cd warmup \
    && dotnet new \
    && cd .. \
    && rm -rf warmup \
    && rm -rf /tmp/NuGetScratch

# Override Mono with source version
# Install LLVM libs

ENV MONO_PREFIX=/mono

RUN git clone https://github.com/mono/llvm.git && \
    cd llvm && \
    ./configure --prefix=$MONO_PREFIX --enable-optimized --enable-targets="x86 x86_64" && \
    make && make install && \
    cd ..

# Build Mono with linked LLVM JIT backend
RUN curl -O https://download.mono-project.com/sources/mono/mono-4.6.2.16.tar.bz2 && \
    tar -xjvf mono-4.6.2.16.tar.bz2

RUN apt-get update && apt-get install -y autoconf libtool automake build-essential gettext cmake pkg-config

ENV DYLD_FALLBACK_LIBRARY_PATH="$MONO_PREFIX/lib:$DYLD_LIBRARY_FALLBACK_PATH"
ENV LD_LIBRARY_PATH="$MONO_PREFIX/lib:$LD_LIBRARY_PATH"
ENV C_INCLUDE_PATH="$MONO_PREFIX/include:$GNOME_PREFIX/include"
ENV ACLOCAL_PATH="$MONO_PREFIX/share/aclocal"
ENV PKG_CONFIG_PATH="$MONO_PREFIX/lib/pkgconfig:$GNOME_PREFIX/lib/pkgconfig"
ENV PATH"=$MONO_PREFIX/bin:$PATH"

RUN cd mono-4.6.2 && \
    ./autogen.sh --prefix=$MONO_PREFIX --enable-llvm=yes && \
    make && make install

# Install fsharp
RUN cd / && \
    curl -O https://codeload.github.com/fsharp/fsharp/tar.gz/4.0.1.20 && \
    tar -xzvf 4.0.1.20

ENV MONO_USE_LLVM="1"
ENV MONO_ENV_OPTIONS="--llvm $MONO_ENV_OPTIONS"

RUN cert-sync /etc/ssl/certs/ca-certificates.crt

RUN cd fsharp-4.0.1.20 && \
     ./autogen.sh --prefix $MONO_PREFIX && \
     make && \
     make install

RUN apt-get update && apt-get install -y lsof 
COPY . /usr/src/app/source
WORKDIR /usr/src/app/source
EXPOSE 8083

CMD "./build.sh"