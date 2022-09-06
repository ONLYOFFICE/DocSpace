import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";

import { PluginsModule } from "./plugins/plugins.module";

import { Plugin } from "./entities/plugin.entity";

@Module({
  imports: [
    TypeOrmModule.forRoot({
      type: "mysql",
      host: "localhost",
      port: 3306,
      username: "root",
      password: "root",
      database: "onlyoffice",
      entities: [Plugin],
      synchronize: true,
    }),
    PluginsModule,
  ],
})
export class AppModule {}
