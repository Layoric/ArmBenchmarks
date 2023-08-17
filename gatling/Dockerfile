FROM amazoncorretto:8-al2-jdk

# working directory for gatling
WORKDIR /opt

ARG MAVEN_VERSION=3.9.4
ARG USER_HOME_DIR="/root"
ARG BASE_URL=https://repo1.maven.org/maven2/org/apache/maven/apache-maven/$MAVEN_VERSION

RUN yum -y update && yum install -y wget tar gzip

RUN mkdir -p /usr/share/maven /usr/share/maven/ref \
 && curl -fsSL -o /tmp/apache-maven.tar.gz ${BASE_URL}/apache-maven-${MAVEN_VERSION}-bin.tar.gz \
 && tar -xzf /tmp/apache-maven.tar.gz -C /usr/share/maven --strip-components=1 \
 && rm -f /tmp/apache-maven.tar.gz \
 && ln -s /usr/share/maven/bin/mvn /usr/bin/mvn

ENV MAVEN_HOME /usr/share/maven
ENV MAVEN_CONFIG "$USER_HOME_DIR/.m2"

# create directory for gatling install
RUN mkdir -p gatling

COPY . /opt/gatling

# set directories below to be mountable from host
VOLUME ["/opt/gatling/conf","/opt/gatling/results","/opt/gatling/user-files"]

WORKDIR /opt/gatling

RUN mvn compile

# set environment variables
# ENV PATH /opt/gatling/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
# ENV GATLING_HOME /opt/gatling