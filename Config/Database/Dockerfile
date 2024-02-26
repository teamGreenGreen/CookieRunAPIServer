FROM mysql:8.2

ENV MYSQL_ROOT_PASSWORD=1234

COPY ./dump.sql /docker-entrypoint-initdb.d

EXPOSE 3306

CMD ["mysqld","--character-set-server=utf8", "--collation-server=utf8_general_ci"]
