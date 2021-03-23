FROM node:12
WORKDIR /usr/src/app

COPY package.json ./
COPY yarn.lock ./

RUN yarn install

COPY . .

RUN yarn build

EXPOSE 5008

CMD [ "yarn", "build:start" ]
